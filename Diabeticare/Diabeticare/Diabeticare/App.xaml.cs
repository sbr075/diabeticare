using System;
using Xamarin.Forms;
using System.IO;
using Diabeticare.Services;
using Diabeticare.Models;
using Diabeticare.Views;

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
                if (bglDatabase == null)
                {
                    bglDatabase = new BglDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bglEntries.db3"));
                }
                return bglDatabase;
            }
        }
        
        static CarbDatabase carbDatabase;
        public static CarbDatabase Cdatabase
        {
            get
            {
                if (carbDatabase == null)
                {
                    carbDatabase = new CarbDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "carbEntries.db3"));
                }
                return carbDatabase;
            }
        }

        static MoodDatabase moodDatabase;
        public static MoodDatabase Mdatabase
        {
            get
            {
                if (moodDatabase == null)
                {
                    moodDatabase = new MoodDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "moodEntries.db3"));
                }
                return moodDatabase;
            }
        }

        static ExerciseDatabase exerciseDatabase;
        public static ExerciseDatabase Edatabase
        {
            get
            {
                if (exerciseDatabase == null)
                {
                    exerciseDatabase = new ExerciseDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "exerciseEntries.db3"));
                }
                return exerciseDatabase;
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
            // LoginShell only has the register/login tabs such that user cannot navigate to other parts of the application
            // without loggin on
            MainPage = new LoginShell(); // Starts application with login shell


            // Set address application will communicate with
            server_addr = "10.0.2.2:5000";
        }

        protected override void OnStart()
        {
        }

        protected override async void OnSleep()
        {
            // Logs out the user if they put down the application and has not enabled AutoLogIn
            if (App.user.AutoLogIn == false)
                await App.Udatabase.UpdateUserEntryAsync(App.user, "", false);

            App.user = null;
        }

        protected override void OnResume()
        {
        }
    }
}
