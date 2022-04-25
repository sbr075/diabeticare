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

        public async Task AddSlpEntryAsync(DateTime sleepStart, DateTime sleepEnd, int server_id)
        {
            var slpEntry = new SleepModel
            {
                ServerID = server_id,
                SleepStart = sleepStart,
                SleepEnd = sleepEnd,
            };
            await slpDatabase.InsertAsync(slpEntry);
        }


        public Task<int> UpdateSlpEntryAsync(Sleep slpEntry, DateTime newSleepStart, DateTime newSleepEnd, int server_id)
        {
            if (slpEntry.ID == 0)
                return null;

            slpEntry.ServerID = server_id;
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
