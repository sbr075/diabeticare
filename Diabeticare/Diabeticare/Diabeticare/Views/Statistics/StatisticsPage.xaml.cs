using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatisticsPage : ContentPage
    {
        public StatisticsPage()
        {
            InitializeComponent();
        }

        // Generate BGL graph for current day
        private void GenDayGraph(object sender, EventArgs e)
        {
            LabelMid.Text = "BGL graph today";
        }

        // Generate BGL graph for last 7 days
        private void GenWeekGraph(object sender, EventArgs e)
        {
            LabelMid.Text = "BGL graph last 7 days";
        }

        // Generate BGL graph for last 30 days
        private void GenMonthGraph(object sender, EventArgs e)
        {
            LabelMid.Text = "BGL graph last 30 days";
        }

        // GoTo EditBglPage
        async void Edit_BGL_Data(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EditBglPage));
        }

        // GoTo AddBglDataPage
        private async void Create_BGL_Data(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddBglDataPage));
        }
    }
}