using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
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
            slpDatabase.CreateTableAsync<SleepModel>().Wait();
        }

        public async Task<IEnumerable<SleepModel>> GetSlpEntriesAsync()
        {
            // Returns all sleep entries
            var slpEntries = await slpDatabase.Table<SleepModel>().ToListAsync();
            return slpEntries;
        }

        public async Task<SleepModel> GetSlpEntryAsync(int id)
        {
            var slp = await slpDatabase.Table<SleepModel>().FirstOrDefaultAsync(slpEntry => slpEntry.ID == id);
            return slp;
        }

        public async Task AddSlpEntryAsync(DateTime sleepStart, DateTime sleepEnd, DateTime createdAt)
        {
            var slpEntry = new SleepModel
            {
                SleepStart = sleepStart,
                SleepEnd = sleepEnd,
                CreatedAt = createdAt
            };
            await slpDatabase.InsertAsync(slpEntry);
        }


        public Task<int> UpdateSlpEntryAsync(SleepModel slpEntry, DateTime newSleepStart, DateTime newSleepEnd)
        {
            if (slpEntry.ID == 0)
                return null;

            slpEntry.SleepStart = newSleepStart;
            slpEntry.SleepEnd = newSleepEnd;
            return slpDatabase.UpdateAsync(slpEntry);
        }

        public async Task DeleteSlpEntryAsync(int id)
        {
            await slpDatabase.DeleteAsync<SleepModel>(id);
        }


    }
}
