using Diabeticare.Views;
using System;
using Xamarin.Forms;
using System.IO;
using Xamarin.Forms.Xaml;
using Diabeticare.Services;

namespace Diabeticare
{
    public partial class App : Application
    {
        static SlpDatabase slpDatabase;
        static BglDatabase bglDatabase;

        // Create the database connection as a singleton.
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
