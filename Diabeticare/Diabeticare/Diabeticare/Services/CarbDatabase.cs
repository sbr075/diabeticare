using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Diabeticare.Models;
using System.Threading.Tasks;

namespace Diabeticare.Services
{
    public class CarbDatabase
    {

        readonly SQLiteAsyncConnection carbDatabase;

        public CarbDatabase(string dbPath)
        {
            carbDatabase = new SQLiteAsyncConnection(dbPath);
            carbDatabase.CreateTableAsync<CarbohydrateModel>().Wait();
        }

        public async Task<IEnumerable<CarbohydrateModel>> GetCarbEntriesAsync()
        {
            // Fetches all entries where entry's registered UserID matches current logged in UserID
            var carbEntries = await carbDatabase.Table<CarbohydrateModel>().Where(ent => ent.UserID == App.user.ID).ToListAsync();
            return carbEntries;
        }

        public async Task<CarbohydrateModel> GetCarbEntryAsync(int id)
        {
            // Fetches single entry where entry's ID matches specified ID
            var carb = await carbDatabase.Table<CarbohydrateModel>().FirstOrDefaultAsync(carbEntry => carbEntry.ID == id);
            return carb;
        }

        public async Task AddCarbEntryAsync(float carbohydrates, DateTime dateOfInput, string foodName, int server_id)
        {
            // Creates a new object and adds to database
            var carb = new CarbohydrateModel
            {
                UserID = App.user.ID,
                ServerID = server_id,
                Carbohydrates = carbohydrates,
                DateOfInput = dateOfInput,
                FoodName = foodName
            };
            await carbDatabase.InsertAsync(carb);
        }

        public Task<int> UpdateCarbEntryAsync(CarbohydrateModel carbEntry, float newValue, DateTime newDate, string foodName, int server_id)
        {
            // Checks if ID is valid
            if (carbEntry.ID == 0)
                return null;

            // Push update to local database
            carbEntry.Carbohydrates = newValue;
            carbEntry.DateOfInput = newDate;
            carbEntry.FoodName = foodName;
            return carbDatabase.UpdateAsync(carbEntry);
        }

        public async Task DeleteCarbEntryAsync(int id)
        {
            await carbDatabase.DeleteAsync<CarbohydrateModel>(id);
        }

        public async Task DeleteUserCarbEntriesAsync()
        {
            try
            {
                // Get all entries related to user
                var carbEntries = await carbDatabase.Table<CarbohydrateModel>().Where(ent => ent.UserID == App.user.ID).ToListAsync();
                foreach (var carbEntry in carbEntries)
                {
                    await carbDatabase.DeleteAsync(carbEntry);
                }
            }
            catch
            {
                // Returns incase no entries exists for user
                return;
            }
        }
    }
}
