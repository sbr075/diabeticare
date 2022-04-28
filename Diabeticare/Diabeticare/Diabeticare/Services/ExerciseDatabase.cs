﻿using System;
using System.Collections.Generic;
using Diabeticare.Models;
using System.Threading.Tasks;
using SQLite;

namespace Diabeticare.Services
{
    public class ExerciseDatabase
    {
        readonly SQLiteAsyncConnection exerciseDatabase;

        public ExerciseDatabase(string dbPath)
        {
            exerciseDatabase = new SQLiteAsyncConnection(dbPath);
            exerciseDatabase.CreateTableAsync<ExerciseModel>().Wait();
        }

        public async Task<IEnumerable<ExerciseModel>> GetExerciseEntriesAsync(string name)
        {
            var exerciseEntries = await exerciseDatabase.Table<ExerciseModel>().Where(ent => ent.UserID == App.user.ID && ent.Name == name).ToListAsync();
            return exerciseEntries;
        }

        public async Task<ExerciseModel> GetExerciseEntryAsync(int id)
        {
            var exercise = await exerciseDatabase.Table<ExerciseModel>().FirstOrDefaultAsync(exerciseEntry => exerciseEntry.ID == id);
            return exercise;
        }

        public async Task AddExerciseEntryAsync(string name, DateTime start, DateTime end, int server_id)
        {
            var exercise = new ExerciseModel
            {
                UserID = App.user.ID,
                ServerID = server_id,
                Name = name,
                ExerciseStart = start,
                ExerciseEnd = end
            };

            await exerciseDatabase.InsertAsync(exercise);
        }

        public Task<int> UpdateExerciseEntryAsync(ExerciseModel exerciseEntry, DateTime start, DateTime end, int server_id)
        {
            if (exerciseEntry.ID == 0)
                return null;

            exerciseEntry.ServerID = server_id;
            exerciseEntry.Name = exerciseEntry.Name;
            exerciseEntry.ExerciseStart = start;
            exerciseEntry.ExerciseEnd = end;
            return exerciseDatabase.UpdateAsync(exerciseEntry);
        }

        public async Task DeleteExerciseEntryAsync(int id)
        {
            await exerciseDatabase.DeleteAsync<ExerciseModel>(id);
        }

        public async Task DeleteUserExerciseEntriesAsync()
        {
            try
            {
                // Get all entries related to user
                var exerciseEntries = await exerciseDatabase.Table<ExerciseModel>().Where(ent => ent.UserID == App.user.ID).ToListAsync();
                foreach (var exerciseEntry in exerciseEntries)
                {
                    await exerciseDatabase.DeleteAsync(exerciseEntry);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
