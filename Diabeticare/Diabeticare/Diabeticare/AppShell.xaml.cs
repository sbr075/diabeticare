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
            Routing.RegisterRoute("BglStatisticsPage", typeof(BglStatisticsPage));
            Routing.RegisterRoute("BglEntriesPage", typeof(BglEntriesPage));
            Routing.RegisterRoute("BglEntryPage", typeof(BglEntryPage));
            Routing.RegisterRoute("AddBglDataPage", typeof(AddBglDataPage));
            Routing.RegisterRoute("EditBglPage", typeof(EditBglPage));

            // Sleep pages
            Routing.RegisterRoute("SlpStatisticsPage", typeof(SlpStatisticsPage));
            Routing.RegisterRoute("SlpEntriesPage", typeof(SlpEntriesPage));
            Routing.RegisterRoute("SlpEntryPage", typeof(SlpEntryPage));
            Routing.RegisterRoute("AddSlpDataPage", typeof(AddSlpDataPage));
            Routing.RegisterRoute("EditSlpPage", typeof(EditSlpPage));

            // Carbohydrate pages
            Routing.RegisterRoute("CarbStatisticsPage", typeof(CarbStatisticsPage));
            Routing.RegisterRoute("CarbEntriesPage", typeof(CarbEntriesPage));
            Routing.RegisterRoute("CarbEntryPage", typeof(CarbEntryPage));
            Routing.RegisterRoute("AddCarbDataPage", typeof(AddCarbDataPage));
            Routing.RegisterRoute("EditCarbPage", typeof(EditCarbPage));

            // Exercise pages
            Routing.RegisterRoute("ExerciseStatisticsPage", typeof(ExerciseStatisticsPage));
            Routing.RegisterRoute("ExerciseEntriesPage", typeof(ExerciseEntriesPage));
            Routing.RegisterRoute("ExerciseEntryPage", typeof(ExerciseEntryPage));
            Routing.RegisterRoute("AddExericseDataPage", typeof(AddExerciseDataPage));
            Routing.RegisterRoute("EditExericsePage", typeof(EditExercisePage));
        }
    }
}
