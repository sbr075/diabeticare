using System;
using System.Collections.Generic;
using Diabeticare.Models;
using System.Threading.Tasks;
using SQLite;

namespace Diabeticare.Services
{
    public class BglDatabase
    {
        
        readonly SQLiteAsyncConnection bglDatabase;

        public BglDatabase(string dbPath)
        {
            bglDatabase = new SQLiteAsyncConnection(dbPath);
            bglDatabase.CreateTableAsync<BglModel>().Wait();
        }

        public async Task<IEnumerable<BglModel>> GetBglEntriesAsync()
        {
            var bglEntries = await bglDatabase.Table<BglModel>().ToListAsync();

            // Compare entries, and sync server database
            return bglEntries;
        }

        public async Task<BglModel> GetBglEntryAsync(int id)
        {
            var bgl = await bglDatabase.Table<BglModel>().FirstOrDefaultAsync(bglEntry => bglEntry.ID == id);
            return bgl;
        }

        public async Task AddBglEntryAsync(float bglMeasurement, DateTime timeOfMeasurment, int server_id)
        {
            var bgl = new BglModel
            {
                ServerID = server_id,
                BGLmeasurement = bglMeasurement,
                TimeOfMeasurment = timeOfMeasurment
            };

            await bglDatabase.InsertAsync(bgl);
        }

        public Task<int> UpdateBglEntryAsync(Bgl bglEntry, float newValue, DateTime newTime, int server_id)
        {
            if (bglEntry.ID == 0)
                return null;

            // Push update to local
            bglEntry.ServerID = server_id;
            bglEntry.BGLmeasurement = newValue;
            bglEntry.TimeOfMeasurment = newTime;
            return bglDatabase.UpdateAsync(bglEntry);
        }

        public async Task DeleteBglEntryAsync(int id)
        {
            await bglDatabase.DeleteAsync<BglModel>(id);
        }


    }
}
