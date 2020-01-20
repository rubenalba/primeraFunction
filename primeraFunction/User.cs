using Microsoft.WindowsAzure.Storage.Table;

namespace primeraFunction
{
    public class User : TableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Nombre { get; set; }
        public string Rol { get; set; }
    }
}