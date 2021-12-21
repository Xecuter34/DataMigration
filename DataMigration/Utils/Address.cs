using DataMigration.DB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataMigration.Utils
{
    class Address
    {
        public static async Task<Guid> AddAddressAsync()
        {
            Guid addressId = Guid.NewGuid();
            using DatabaseContext dbContext = new DatabaseContext();
            await dbContext.AddAsync(new DB.Models.Address
            {
                Id = addressId,
                AddressLine_1 = "DEFAUT",
                AddressLine_2 = "DEFAUT",
                City = "DEFAULT",
                State = "DEFAULT",
                Country = "GB",
                PostalCode = "DEFAULT",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();
            return addressId;
        }
    }
}
