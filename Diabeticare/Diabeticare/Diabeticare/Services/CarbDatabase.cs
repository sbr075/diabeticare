using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using Diabeticare.Models;
namespace Diabeticare.Services
{
    public class CarbDatabase
    {

        readonly SQLiteAsyncConnection carbDatabase;

        public CarbDatabase(string dbPath)
        {
            carbDatabase = new SQLiteAsyncConnection(dbPath);
            carbDatabase.CreateTableAsync<CarbohydrateModel>().Wait();
        }

    }
}
