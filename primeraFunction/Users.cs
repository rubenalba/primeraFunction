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

        /*[FunctionName("UserDelete")]
        public static async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = null)]  HttpRequest req,
            [Table("Users", partitionKey: "default", rowKey:"{code}", Connection = "ConnectionString")] CloudTable Users,
            [Table("Users", Connection = "ConnectionString")] CloudTable userTable)
        {
            if (Users == null)
            {
                return new NotFoundResult();
            }

            var operation = TableOperation.Delete(Users);
            await userTable.ExecuteAsync(operation);
            return new OkResult();
        }*/

    }
}