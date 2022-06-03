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
            // Fetches user object which matches given username
            var user = await userDatabase.Table<UserModel>().FirstOrDefaultAsync(userEntry => userEntry.Username == username);
            return user;
        }

        public async Task AddUserEntryAsync(string username, string email)
        {
            // Add new user object to database
            var userEntry = new UserModel
            {
                Username = username,
                Email = email,
            };
            await userDatabase.InsertAsync(userEntry);
        }

        public Task<int> UpdateUserEntryAsync(UserModel userEntry, string password, bool autoLogIn)
        {
            // Checks for valid ID
            if (userEntry.ID == 0)
                return null;

            // Password and AutoLogIn can be empty and false if user did not select RememberMe option
            // This keeps the password out of the device for security reasons
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
