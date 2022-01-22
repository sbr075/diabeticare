using Diabeticare.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            /*
             * Registering routes that are not visible in the navigation bar.
             * Note: Pages are not loaded if you do not do this
            */
            Routing.RegisterRoute("EditBglPage", typeof(EditBglPage));
            Routing.RegisterRoute("AddBglDataPage", typeof(AddBglDataPage));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
