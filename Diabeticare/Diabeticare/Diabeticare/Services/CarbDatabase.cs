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
            var carbEntries = await carbDatabase.Table<CarbohydrateModel>().ToListAsync();
            return carbEntries;
        }

        public async Task<CarbohydrateModel> GetCarbEntryAsync(int id)
        {
            var carb = await carbDatabase.Table<CarbohydrateModel>().FirstOrDefaultAsync(carbEntry => carbEntry.ID == id);
            return carb;
        }

        public async Task AddCarbEntryAsync(float carbohydrates, DateTime dateOfInput, string foodName, int server_id)
        {
            var carb = new CarbohydrateModel
            {
                ServerID = server_id,
                Carbohydrates = carbohydrates,
                DateOfInput = dateOfInput,
                FoodName = foodName
            };
            await carbDatabase.InsertAsync(carb);
        }

        public Task<int> UpdateCarbEntryAsync(CarbohydrateModel carbEntry, float newValue, DateTime newDate, string foodName, int server_id)
        {
            if (carbEntry.ID == 0)
                return null;

            carbEntry.Carbohydrates = newValue;
            carbEntry.DateOfInput = newDate;
            carbEntry.FoodName = foodName;
            return carbDatabase.UpdateAsync(carbEntry);
        }

        public async Task DeleteCarbEntryAsync(int id)
        {
            await carbDatabase.DeleteAsync<CarbohydrateModel>(id);
        }
    }
}
