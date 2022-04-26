using Diabeticare.Views;
using Xamarin.Forms;

namespace Diabeticare
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            /*
             * Registering routes that are not visible in the navigation bar.
             * Note: Pages are not loaded if you do not do this
            */

            // Default page
            Routing.RegisterRoute("DefaultPage", typeof(DefaultPage));

            // Default statistics page
            Routing.RegisterRoute("StatisticsTabPage", typeof(StatisticsTabPage));

            //General pages
            Routing.RegisterRoute("InfoPage", typeof(InfoPage));
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
            Routing.RegisterRoute("AboutPage", typeof(AboutPage));

            // BGL pages
            Routing.RegisterRoute("EditBglPage", typeof(EditBglPage));
            Routing.RegisterRoute("AddBglDataPage", typeof(AddBglDataPage));
            Routing.RegisterRoute("BglEntryPage", typeof(BglEntryPage));

            // Sleep pages
            Routing.RegisterRoute("AddSlpDataPage", typeof(AddSlpDataPage));
            Routing.RegisterRoute("EditSlpPage", typeof(EditSlpPage));
            Routing.RegisterRoute("SlpEntryPage", typeof(SlpEntryPage));

            // Carbohydrate pages
            Routing.RegisterRoute("AddCarbDataPage", typeof(AddCarbDataPage));
            Routing.RegisterRoute("CarbEntryPage", typeof(CarbEntryPage));
        }

    }
}
