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
            userDatabase.CreateTableAsync<User>().Wait();
        }

        public async Task<IEnumerable<User>> GetUserEntriesAsync()
        {
            // Fetches all user entries where AutoLogIn is enabled
            var userEntries = await userDatabase.Table<User>().Where(userEntry => userEntry.AutoLogIn == true).ToListAsync();
            return userEntries;
        }

        public async Task<User> GetUserEntryAsync(string username)
        {
            var user = await userDatabase.Table<User>().FirstOrDefaultAsync(userEntry => userEntry.Username == username);
            return user;
        }

        public async Task AddUserEntryAsync(string username, string email)
        {
            var userEntry = new User
            {
                Username = username,
                Email = email,
            };
            await userDatabase.InsertAsync(userEntry);
        }

        public Task<int> UpdateUserEntryAsync(User userEntry, string password, bool autoLogIn)
        {
            if (userEntry.ID == 0)
                return null;

            userEntry.Password = password;
            userEntry.AutoLogIn = autoLogIn;

            return userDatabase.UpdateAsync(userEntry);
        }

        public Task<int> UpdateUserTokenAsync(User userEntry, string token)
        {
            if (userEntry.ID == 0)
                return null;

            userEntry.Token = token;

            return userDatabase.UpdateAsync(userEntry);
        }

        public async Task DeleteUserEntryAsync(int id)
        {
            await userDatabase.DeleteAsync<User>(id);
        }
    }
}
