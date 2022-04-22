using System;
using Xamarin.Forms;
using System.IO;
using Diabeticare.Services;
using Diabeticare.Models;

namespace Diabeticare
{
    public partial class App : Application
    {
        public static User user;

        static ApiServices apiservices;
        static UserDatabase userDatabase;
        static SlpDatabase slpDatabase;
        static BglDatabase bglDatabase;

        // Create the database connection as a singleton.
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

        // Create the database connection as a singleton.
        public static BglDatabase Bdatabase
        {
            get
            {
                if (bglDatabase == null)
                {
                    bglDatabase = new BglDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bglEntries.db3"));
                }
                return bglDatabase;
            }
        }

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
            MainPage = new AppShell();
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
