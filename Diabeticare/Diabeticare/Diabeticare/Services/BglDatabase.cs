using System;
using System.Collections.Generic;
using System.Text;
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
            var bglEntries = await bglDatabase.Table<Bgl>().ToListAsync();
            return bglEntries;
        }

        public async Task<Bgl> GetBglEntryAsync(int id)
        {
            var bgl = await bglDatabase.Table<Bgl>().FirstOrDefaultAsync(bglEntry => bglEntry.ID == id);
            return bgl;
        }

        public async Task AddBglEntryAsync(float bglMeasurement, DateTime createdAt, TimeSpan bglTime)
        {
            var bgl = new Bgl
            {
                BGLmeasurement = bglMeasurement,
                CreatedAt = createdAt,
                BGLtime = bglTime
            };

            await bglDatabase.InsertAsync(bgl);
        }

        public Task<int> UpdateBglEntryAsync(Bgl bglEntry, float newValue, TimeSpan newTime)
        {
            if (bglEntry.ID == 0)
                return null;

            bglEntry.BGLmeasurement = newValue;
            bglEntry.BGLtime = newTime;
            return bglDatabase.UpdateAsync(bglEntry);
        }

        public async Task DeleteBglEntryAsync(int id)
        {
            await bglDatabase.DeleteAsync<Bgl>(id);
        }


    }
}
