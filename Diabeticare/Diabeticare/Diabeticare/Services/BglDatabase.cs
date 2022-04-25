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
            bglDatabase.CreateTableAsync<BglModel>().Wait();
        }

        public async Task<IEnumerable<BglModel>> GetBglEntriesAsync()
        {
            var bglEntries = await bglDatabase.Table<BglModel>().ToListAsync();
            return bglEntries;
        }

        public async Task<BglModel> GetBglEntryAsync(int id)
        {
            var bgl = await bglDatabase.Table<BglModel>().FirstOrDefaultAsync(bglEntry => bglEntry.ID == id);
            return bgl;
        }

        public async Task AddBglEntryAsync(float bglMeasurement, DateTime createdAt, TimeSpan bglTime)
        {
            var bgl = new BglModel
            {
                BGLmeasurement = bglMeasurement,
                CreatedAt = createdAt,
                BGLtime = bglTime
            };

            await bglDatabase.InsertAsync(bgl);
        }

        public Task<int> UpdateBglEntryAsync(BglModel bglEntry, float newValue, TimeSpan newTime)
        {
            if (bglEntry.ID == 0)
                return null;

            bglEntry.BGLmeasurement = newValue;
            bglEntry.BGLtime = newTime;
            return bglDatabase.UpdateAsync(bglEntry);
        }

        public async Task DeleteBglEntryAsync(int id)
        {
            await bglDatabase.DeleteAsync<BglModel>(id);
        }


    }
}
