using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.WebJobs.Host;
using System.Net;
using System;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.IO;

namespace primeraFunction
{
    public static class Users
    {
        [FunctionName("Users")]
        public static async Task<List<string>> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var usersList = new List<string>();

            for (int i = 0; i < 50; i++)
            {
                usersList.Add($"User{i}");
            }
            return usersList;
        }

        [FunctionName("GetAllUser")]
        public static async Task<IActionResult> GetAll(
              [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
              [Table("Users", Connection = "ConnectionString")] CloudTable Users,
              ILogger log)
        {
            log.LogInformation("Obteniendo Todos los usuarios");
            var tableQuery = new TableQuery<UserTableEntity>();
            var querySegment = await Users.ExecuteQuerySegmentedAsync(tableQuery, null);

            return new OkObjectResult(querySegment.Select(Map.ToUser));
        }

        [FunctionName("UserInsert")]
        public static async Task<IActionResult> UserInsert(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Users")]HttpRequest req,
           [Table("Users", Connection = "ConnectionString")] IAsyncCollector<UserTableEntity> userTable,
           ILogger log)  /*Conexion a la tabla (envio)*/
        {
            log.LogInformation("Insert user");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<User>(requestBody);

            var user = new User()
            {
                Nombre = "Bort",
                Rol = "Desconocido",
                PartitionKey = "Spain",
                RowKey = "2",
                Timestamp = DateTime.UtcNow
            };
            await userTable.AddAsync(user.ToTableEntity());

            return new OkObjectResult(user);
        }

        [FunctionName("Table_DeleteTodo")]
        public static async Task<IActionResult> DeleteUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Users/{id}")]HttpRequest req,
        [Table("Users", Connection = "ConnectionString")] CloudTable userTable,
        ILogger log, string id)
        {
            var deleteOperation = TableOperation.Delete(
                new TableEntity() { PartitionKey = "Spain", RowKey = id, ETag = "*" });
            try
            {
                var deleteResult = await userTable.ExecuteAsync(deleteOperation);
            }
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == 404)
            {
                return new NotFoundResult();
            }
            return new OkResult();
        }
    }

    /*[FunctionName("Table_UpdateUSer")]
    public static async Task<IActionResult> UpdateTodo(
    [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo2/{id}")]HttpRequest req,
    [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
    TraceWriter log, string id)
{

    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
    var updated = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);
    var findOperation = TableOperation.Retrieve<TodoTableEntity>("TODO", id);
    var findResult = await todoTable.ExecuteAsync(findOperation);
    if (findResult.Result == null)
    {
        return new NotFoundResult();
    }
    var existingRow = (TodoTableEntity)findResult.Result;

    existingRow.IsCompleted = updated.IsCompleted;
    if (!string.IsNullOrEmpty(updated.TaskDescription))
    {
        existingRow.TaskDescription = updated.TaskDescription;
    }

    var replaceOperation = TableOperation.Replace(existingRow);
    await todoTable.ExecuteAsync(replaceOperation);

    return new OkObjectResult(existingRow.ToTodo());
}*/

}