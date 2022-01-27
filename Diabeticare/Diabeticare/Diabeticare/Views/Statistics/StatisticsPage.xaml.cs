using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StatisticsPage : ContentPage
    {

        List<ChartEntry> entries = new List<ChartEntry>();
        Random random = new Random();

        public StatisticsPage()
        {
            InitializeComponent();
        }

        // Generate BGL chart for current day
        private void GenDayChart(object sender, EventArgs e)
        {
            /* Get BGL data from database later. Using dummy data for now. */

            LabelMid.Text = "BGL chart today";
            
            // Empty list before adding new data
            entries.Clear();

            int bgl_measurements = 5;

            for (int i = 0; i < bgl_measurements; i++)
            {
                float rand_bgl_value = random.Next(100, 141);
                // Generate new chart entry and add it to entries list
                entries.Add(new ChartEntry(rand_bgl_value)
                {
                    Color = SKColor.Parse("#ff1447"),
                    Label = $"{i}",
                    ValueLabel = $"{rand_bgl_value}"
                });

            }
            
            // Create BGL chart
            //BglChart.Chart = new LineChart { Entries = entries, LabelTextSize = 30, LabelOrientation = Orientation.Horizontal, ValueLabelOrientation = Orientation.Horizontal };
            BglChart.Chart = new BarChart { Entries = entries, LabelTextSize = 30, LabelOrientation = Orientation.Horizontal, ValueLabelOrientation = Orientation.Horizontal };
        }

        // Generate BGL chart for last 7 days
        private void GenWeekChart(object sender, EventArgs e)
        {
            LabelMid.Text = "BGL chart last 7 days";
        }

        // Generate BGL chart for last 30 days
        private void GenMonthChart(object sender, EventArgs e)
        {
            LabelMid.Text = "BGL chart last 30 days";
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