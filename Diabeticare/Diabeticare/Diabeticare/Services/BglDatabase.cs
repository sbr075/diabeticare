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
            bglDatabase.CreateTableAsync<Bgl>().Wait();
        }

        public async Task<IEnumerable<Bgl>> GetBglEntriesAsync()
        {
            // Fetch from server

            // Fetch from local
            var bglEntries = await bglDatabase.Table<Bgl>().ToListAsync();

            // Compare entries, and sync server database
            return bglEntries;
        }

        public async Task<Bgl> GetBglEntryAsync(int id)
        {
            var bgl = await bglDatabase.Table<Bgl>().FirstOrDefaultAsync(bglEntry => bglEntry.ID == id);
            return bgl;
        }

        public async Task AddBglEntryAsync(float bglMeasurement, DateTime timeOfMeasurment, int server_id)
        {
            // Push to local
            var bgl = new Bgl
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
            // Delete from server

            // Delete from local
            await bglDatabase.DeleteAsync<Bgl>(id);
        }


    }
}
