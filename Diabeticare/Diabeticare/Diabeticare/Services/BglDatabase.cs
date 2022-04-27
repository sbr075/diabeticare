﻿using System;
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
            var bglEntries = await bglDatabase.Table<BglModel>().Where(ent => ent.UserID == App.user.ID).ToListAsync();
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
                UserID = App.user.ID,
                ServerID = server_id,
                BGLmeasurement = bglMeasurement,
                TimeOfMeasurment = timeOfMeasurment
            };

            await bglDatabase.InsertAsync(bgl);
        }

        public Task<int> UpdateBglEntryAsync(BglModel bglEntry, float newValue, DateTime newTime, int server_id)
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

        public async Task DeleteUserBglEntriesAsync()
        {
            try
            {
                // Get all entries related to user
                var bglEntries = await bglDatabase.Table<BglModel>().Where(ent => ent.UserID == App.user.ID).ToListAsync();
                foreach (var bglEntry in bglEntries)
                {
                    await bglDatabase.DeleteAsync(bglEntry);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
