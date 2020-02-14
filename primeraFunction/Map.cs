using System;
using System.Collections.Generic;
using System.Text;

namespace primeraFunction
{
    public static class Map
    {
        public static UserTableEntity ToTableEntity(this User user)
        {
            return new UserTableEntity()
            {
                Nombre = user.Nombre,
                Rol = user.Rol,
                PartitionKey = "Spain",
                RowKey = user.RowKey,
                Timestamp = user.Timestamp
            };
        }

        public static User ToUser(this UserTableEntity user)
        {
            return new User()
            {
                Nombre = user.Nombre,
                PartitionKey = "Spain",
                Rol = user.Rol,
                RowKey = user.RowKey,
                Timestamp = user.Timestamp
            };
        }
            
    }
}
