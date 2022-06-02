using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Diabeticare.Models;

namespace Diabeticare.Services
{
    public class UserDatabase
    {
        readonly SQLiteAsyncConnection userDatabase;

        public UserDatabase(string dbPath)
        {
            userDatabase = new SQLiteAsyncConnection(dbPath);
            userDatabase.CreateTableAsync<UserModel>().Wait();
        }

        public async Task<IEnumerable<UserModel>> GetUserEntriesAsync()
        {
            // Fetches all user entries where AutoLogIn is enabled
            var userEntries = await userDatabase.Table<UserModel>().Where(userEntry => userEntry.AutoLogIn == true).ToListAsync();
            return userEntries;
        }

        public async Task<UserModel> GetUserEntryAsync(string username)
        {
            var user = await userDatabase.Table<UserModel>().FirstOrDefaultAsync(userEntry => userEntry.Username == username);
            return user;
        }

        public async Task AddUserEntryAsync(string username, string email)
        {
            var userEntry = new UserModel
            {
                Username = username,
                Email = email,
            };
            await userDatabase.InsertAsync(userEntry);
        }

        public Task<int> UpdateUserEntryAsync(UserModel userEntry, string password, bool autoLogIn)
        {
            if (userEntry.ID == 0)
                return null;

            userEntry.Password = password;
            userEntry.AutoLogIn = autoLogIn;

            return userDatabase.UpdateAsync(userEntry);
        }

        public Task<int> UpdateUserTokenAsync(UserModel userEntry, string token)
        {
            if (userEntry.ID == 0)
                return null;

            userEntry.Token = token;

            return userDatabase.UpdateAsync(userEntry);
        }

        public async Task DeleteUserEntryAsync()
        {
            await userDatabase.DeleteAsync<UserModel>(App.user.ID);
        }
    }
}
