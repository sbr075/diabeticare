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
            // Fetches all entries where entry's registered UserID matches current logged in UserID
            var slpEntries = await slpDatabase.Table<SleepModel>().Where(ent => ent.UserID == App.user.ID).ToListAsync();
            return slpEntries;
        }

        public async Task<SleepModel> GetSlpEntryAsync(int id)
        {
            // Fetches single entry where entry's ID matches specified ID
            var slp = await slpDatabase.Table<SleepModel>().FirstOrDefaultAsync(slpEntry => slpEntry.ID == id);
            return slp;
        }

        public async Task AddSlpEntryAsync(DateTime sleepStart, DateTime sleepEnd, int server_id)
        {
            // Creates a new object and adds to database
            var slpEntry = new SleepModel
            {
                UserID = App.user.ID,
                ServerID = server_id,
                SleepStart = sleepStart,
                SleepEnd = sleepEnd,
            };
            await slpDatabase.InsertAsync(slpEntry);
        }


        public Task<int> UpdateSlpEntryAsync(SleepModel slpEntry, DateTime newSleepStart, DateTime newSleepEnd, int server_id)
        {
            // Checks if ID is valid
            if (slpEntry.ID == 0)
                return null;

            // Push update to local database
            slpEntry.ServerID = server_id;
            slpEntry.SleepStart = newSleepStart;
            slpEntry.SleepEnd = newSleepEnd;
            return slpDatabase.UpdateAsync(slpEntry);
        }

        public async Task DeleteSlpEntryAsync(int id)
        {
            await slpDatabase.DeleteAsync<SleepModel>(id);
        }

        public async Task DeleteUserSlpEntriesAsync()
        {
            try
            {
                // Get all entries related to user
                var slpEntries = await slpDatabase.Table<SleepModel>().Where(ent => ent.UserID == App.user.ID).ToListAsync();
                foreach (var slpEntry in slpEntries)
                {
                    await slpDatabase.DeleteAsync(slpEntry);
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
