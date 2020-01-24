using Microsoft.WindowsAzure.Storage.Table;

namespace primeraFunction
{
    public class User : TableEntity
    {
        public string Nombre { get; set; }
        public string Rol { get; set; }
    }
}