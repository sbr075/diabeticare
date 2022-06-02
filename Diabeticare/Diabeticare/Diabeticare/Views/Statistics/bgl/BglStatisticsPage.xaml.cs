using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.Models;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BglStatisticsPage : ContentPage
    {

        List<ChartEntry> ChartEntries = new List<ChartEntry>();

        readonly int BglLowDangerLimit = 4;
        readonly int BglHighSoftLimit = 10;
        //readonly int BglHighDangerZone = 12?;
        public BglStatisticsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            GenDayChart(null, null);
        }

        // Generate BGL chart for today
        private async void GenDayChart(object sender, EventArgs e)
        {
            LabelMid.Text = "BGL Chart - Today";
            ChartDescription.Text = "Your BGL measurements for today.";
            ChartEntries.Clear(); // Empty list before adding new data

            // Max number of entries displayed in the chart
            int maxDayChartEntries = 10;

            // Get list of entries from database
            List<BglModel> BglEntries = (List<BglModel>)await App.Bdatabase.GetBglEntriesAsync();

            // Iterating through entries and removing those that were not registered today
            // Note: Iterating backwards to avoid index shifting issues when removing elements
            for (int i = BglEntries.Count - 1; i >= 0; i--)
                if (BglEntries[i].TimeOfMeasurment.Day != DateTime.Now.Day)
                    BglEntries.RemoveAt(i);

            // Limiting max number of entries in the chart.
            while (BglEntries.Count > maxDayChartEntries)
                BglEntries.RemoveAt(0);

            // Sort entries based on BGLtime
            BglEntries = BglEntries.OrderBy(bglEntry => bglEntry.TimeOfMeasurment.TimeOfDay).ToList();

            // Create chart entries for each BGL entry
            for (int i = 0; i < BglEntries.Count; i++)
            {
                // Change timespan format to only show hours:minutes
                string timeStamp = BglEntries[i].TimeOfMeasurment.TimeOfDay.ToString(@"hh\:mm");

                // Generate new chart entry and add it to ChartEntries list
                ChartEntries.Add(new ChartEntry(BglEntries[i].BGLmeasurement)
                {
                    Color = SKColor.Parse("#ff1447"),
                    Label = $"{timeStamp}",
                    ValueLabel = $"{BglEntries[i].BGLmeasurement}"
                });
            }
            
            // Create BGL line chart
            BglChart.Chart = new LineChart { Entries = ChartEntries, LabelTextSize = 30, LabelOrientation = Orientation.Horizontal, ValueLabelOrientation = Orientation.Horizontal};
        }

        // Generate BGL chart for last 7 days
        private void GenWeekChart(object sender, EventArgs e)
        {
            LabelMid.Text = "BGL Chart - 7 Days";
            ChartDescription.Text = "Your average BGL the last 7 days.";
            int GoBackDays = 6;
            string LabelFormat = "MMMM dd";

            // Create chart entries for the last 7 days
            CreateChartEntries(GoBackDays, ChartEntries, LabelFormat);
            
            // Create BGL point chart
            BglChart.Chart = new PointChart { Entries = ChartEntries, LabelTextSize = 30, PointAreaAlpha = 200, PointSize = 20, LabelOrientation = Orientation.Horizontal, ValueLabelOrientation = Orientation.Horizontal };
        }


        // Generate BGL chart for last 30 days
        private void GenMonthChart(object sender, EventArgs e)
        {
            LabelMid.Text = "BGL Chart - 30 Days";
            ChartDescription.Text = "Your average BGL the last 30 days.";
            int GoBackDays = 29;
            string LabelFormat = "MM/dd";

            // Create chart entries for the last 30 days
            CreateChartEntries(GoBackDays, ChartEntries, LabelFormat);

            // Create BGL point chart
            BglChart.Chart = new PointChart { Entries = ChartEntries, LabelTextSize = 30, PointAreaAlpha = 200, PointSize = 10, LabelOrientation = Orientation.Vertical, ValueLabelOrientation = Orientation.Horizontal };
        }

        // GoTo ViewBglDataPage
        async void ViewBGLEntries(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new BglEntriesPage());
        }

        // GoTo AddBglDataPage
        private async void CreateBGLEntry(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new AddBglDataPage());
        }

        // Creates chart entries for BGL measurements the last N days
        private async void CreateChartEntries(int days, List<ChartEntry> chartEntries, string labelFormat)
        {
            // Empty chart list before adding new data
            chartEntries.Clear();

            // Get list of entries from database
            var bglEntries = await App.Bdatabase.GetBglEntriesAsync();

            bglEntries = bglEntries.Where(ent => ent.TimeOfMeasurment <= DateTime.Now && ent.TimeOfMeasurment >= DateTime.Now.AddDays(-days));
            if (bglEntries.Count() == 0)
                return;

            var distinctDays = bglEntries.Select(ent => ent.TimeOfMeasurment.Day).Distinct().OrderBy(ent => ent);
            foreach (var day in distinctDays)
            {
                var dayBgl = bglEntries.Where(ent => ent.TimeOfMeasurment.Day == day);
                var bglAverage = bglEntries.Select(ent => ent.BGLmeasurement).Average();

                // Change datetime format
                string dateTime;
                if (day == DateTime.Now.Day)
                    dateTime = "Today";
                else
                    dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day).ToString(labelFormat);

                // Set graph color based on BGL value
                string graphColor;
                if (bglAverage < BglLowDangerLimit) graphColor = "#ff1447";
                else if (bglAverage > BglHighSoftLimit) graphColor = "#FFDA00";
                else graphColor = "#00e400";

                // Generate new chart entry
                chartEntries.Add(new ChartEntry(bglAverage)
                {
                    Color = SKColor.Parse(graphColor),
                    Label = $"{dateTime}",
                    ValueLabel = $"{Math.Round(bglAverage, 1)}"
                });
            }
        }
    }
}