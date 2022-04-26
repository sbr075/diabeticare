using System;
using Xamarin.Forms;
using System.IO;
using Diabeticare.Services;
using Diabeticare.Models;

namespace Diabeticare
{
    public partial class App : Application
    {
        public static UserModel user;
        public static string server_addr;

        // Create the database connection as a singleton.
        static UserDatabase userDatabase;
        public static UserDatabase Udatabase
        {
            get
            {
                if (userDatabase == null)
                {
                    userDatabase = new UserDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "userEntries.db3"));
                }
                return userDatabase;
            }
        }

        static SlpDatabase slpDatabase;
        public static SlpDatabase Sdatabase
        {
            get
            {
                if (slpDatabase == null)
                {
                    slpDatabase = new SlpDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sleepEntries.db3"));
                }
                return slpDatabase;
            }
        }

        static BglDatabase bglDatabase;
        public static BglDatabase Bdatabase
        {
            get
            {
                if (carbDatabase == null)
                {
                    carbDatabase = new CarbDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bglEntries.db3"));
                }
                return carbDatabase;
            }
        }

        public static CarbDatabase Cdatabase
        {
            get
            {
                if (carbDatabase == null)
                {
                    carbDatabase = new CarbDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bglEntries.db3"));
                }
                return carbDatabase;
            }
        }

        public static CarbDatabase Cdatabase
        {
            get
            {
                if (bglDatabase == null)
                {
                    bglDatabase = new BglDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "carbEntries.db3"));
                }
                return bglDatabase;
            }
        }

        static ApiServices apiservices;
        public static ApiServices apiServices
        {
            get
            {
                if (apiservices == null)
                {
                    apiservices = new ApiServices();
                }
                return apiservices;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new LoginShell();
            server_addr = "10.0.2.2:5000";
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
