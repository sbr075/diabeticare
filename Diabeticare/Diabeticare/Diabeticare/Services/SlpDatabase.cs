using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Diabeticare.Models;

namespace Diabeticare.Services
{
    public class SlpDatabase
    {
        readonly SQLiteAsyncConnection slpDatabase;

        public SlpDatabase(string dbPath)
        {
            slpDatabase = new SQLiteAsyncConnection(dbPath);
            slpDatabase.CreateTableAsync<Sleep>().Wait();
        }

        public async Task<IEnumerable<Sleep>> GetSlpEntriesAsync()
        {
            // Returns all sleep entries
            var slpEntries = await slpDatabase.Table<Sleep>().ToListAsync();
            return slpEntries;
        }

        public async Task<Sleep> GetSlpEntryAsync(int id)
        {
            var slp = await slpDatabase.Table<Sleep>().FirstOrDefaultAsync(slpEntry => slpEntry.ID == id);
            return slp;
        }

        public async Task AddSlpEntryAsync(DateTime sleepStart, DateTime sleepEnd, DateTime createdAt)
        {
            var slpEntry = new Sleep
            {
                SleepStart = sleepStart,
                SleepEnd = sleepEnd,
                CreatedAt = createdAt
            };
            await slpDatabase.InsertAsync(slpEntry);
        }


        public Task<int> UpdateSlpEntryAsync(Sleep slpEntry, DateTime newSleepStart, DateTime newSleepEnd)
        {
            if (slpEntry.ID == 0)
                return null;

            slpEntry.SleepStart = newSleepStart;
            slpEntry.SleepEnd = newSleepEnd;
            return slpDatabase.UpdateAsync(slpEntry);
        }

        public async Task DeleteSlpEntryAsync(int id)
        {
            await slpDatabase.DeleteAsync<Sleep>(id);
        }
    }
}
