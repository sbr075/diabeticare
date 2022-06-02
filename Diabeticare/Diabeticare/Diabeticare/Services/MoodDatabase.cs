using System;
using System.Collections.Generic;
using Diabeticare.Models;
using System.Threading.Tasks;
using SQLite;

namespace Diabeticare.Services
{
    public class MoodDatabase
    {
        readonly SQLiteAsyncConnection moodDatabase;

        public MoodDatabase(string dbPath)
        {
            moodDatabase = new SQLiteAsyncConnection(dbPath);
            moodDatabase.CreateTableAsync<MoodModel>().Wait();
        }

        public async Task<IEnumerable<MoodModel>> GetMoodEntriesAsync()
        {
            var bglEntries = await moodDatabase.Table<MoodModel>().Where(ent => ent.UserID == App.user.ID).ToListAsync();
            return bglEntries;
        }

        public async Task<MoodModel> GetMoodEntryAsync(int id)
        {
            var bgl = await moodDatabase.Table<MoodModel>().FirstOrDefaultAsync(bglEntry => bglEntry.ID == id);
            return bgl;
        }

        public async Task AddMoodEntryAsync(int moodValue, DateTime date, int server_id)
        {
            var mood = new MoodModel
            {
                UserID = App.user.ID,
                ServerID = server_id,
                MoodValue = moodValue,
                Date = date
            };

            await moodDatabase.InsertAsync(mood);
        }

        public Task<int> UpdateMoodEntryAsync(MoodModel moodEntry, int newValue, DateTime newTime, int server_id)
        {
            if (moodEntry.ID == 0)
                return null;

            // Push update to local
            moodEntry.ServerID = server_id;
            moodEntry.MoodValue = newValue;
            moodEntry.Date = newTime;
            return moodDatabase.UpdateAsync(moodEntry);
        }

        public async Task DeleteMoodEntryAsync(int id)
        {
            await moodDatabase.DeleteAsync<MoodModel>(id);
        }

        public async Task DeleteUserMoodEntriesAsync()
        {
            try
            {
                // Get all entries related to user
                var moodEntries = await moodDatabase.Table<MoodModel>().Where(ent => ent.UserID == App.user.ID).ToListAsync();
                foreach (var moodEntry in moodEntries)
                {
                    await moodDatabase.DeleteAsync(moodEntry);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
