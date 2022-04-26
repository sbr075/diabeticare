using System;
using System.Collections.Generic;
using SkiaSharp;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.Models;
using System.Linq;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarbStatisticsPage : ContentPage
    {
        List<ChartEntry> ChartEntries = new List<ChartEntry>();

        //readonly int recommendedCarbMin = 7;
        //readonly int recommendedCarbpMax = 9;

        public CarbStatisticsPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            GenDayChart(null, null);
        }

        // Generate carbohydrate chart for current day
        private async void GenDayChart(object sender, EventArgs e)
        {
            LabelMid.Text = "Average carbohydrates - Today";
            ChartDescription.Text = "Average carbohydrates today.";
            ChartEntries.Clear(); // Empty list before adding new data

            // Get list of carbohydrate entries from database from today
            var carbEntries = await App.Cdatabase.GetCarbEntriesAsync();
            carbEntries = carbEntries.Where(ent => ent.DateOfInput.Day == DateTime.Now.Day).OrderByDescending(ent => ent.DateOfInput.Day);

            if (carbEntries.Count() == 0)
                return;


            // Get total carbohydrates for today
            float totalCarbs = carbEntries.Select(ent => ent.Carbohydrates).Sum();

            // Set graph color depending on the amount of carbohydrates
            string graphColor = "#00e400";

            // Create chart entry
            ChartEntries.Add(new ChartEntry(totalCarbs)
            {
                Color = SKColor.Parse(graphColor),
                Label = "Today",
                ValueLabel = $"{Math.Round(totalCarbs, 1)} Carbohydrates"
            });

            // Create carbohydrate point chart
            CarbChart.Chart = new PointChart() { Entries = ChartEntries, LabelTextSize = 30, PointAreaAlpha = 200, PointSize = 20, MaxValue = 9, LabelOrientation = Orientation.Horizontal, ValueLabelOrientation = Orientation.Horizontal };
        }

        // Generate carbohydrate chart for the last 7 days
        private void GenWeekChart(object sender, EventArgs e)
        {
            LabelMid.Text = "Average carbohydrates - 7 Days";
            ChartDescription.Text = "Average carbohydrates the last 7 days.";
            int GoBackDays = 6;
            string LabelFormat = "MMMM dd";

            // Create chart entries for the last 7 days
            CreateChartEntries(GoBackDays, ChartEntries, LabelFormat);

            // Create carbohydrate point chart
            CarbChart.Chart = new PointChart() { Entries = ChartEntries, LabelTextSize = 30, PointAreaAlpha = 200, PointSize = 20, MaxValue = 9, LabelOrientation = Orientation.Horizontal, ValueLabelOrientation = Orientation.Horizontal };
        }

        // Generate carbohydrate chart for the last 30 days
        private void GenMonthChart(object sender, EventArgs e)
        {
            LabelMid.Text = "Average carbohydrates - 30 Days";
            ChartDescription.Text = "Average carbohydrates the last 30 days.";
            int GoBackDays = 29;
            string LabelFormat = "MM/dd";

            // Create chart entries for the last 30 days
            CreateChartEntries(GoBackDays, ChartEntries, LabelFormat);

            // Create carbohydrate point chart
            CarbChart.Chart = new PointChart() { Entries = ChartEntries, LabelTextSize = 30, PointAreaAlpha = 200, PointSize = 10, MaxValue = 9, LabelOrientation = Orientation.Vertical, ValueLabelOrientation = Orientation.Horizontal };
        }

        // GoTo EditSlpPage
        async void ViewCarbEntries(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new CarbEntriesPage());
        }

        // GoTo AddSlpDataPage
        private async void CreateCarbEntry(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new AddCarbDataPage());
        }

        // Creates chart entries for carbohydrate entries the last N days
        private async void CreateChartEntries(int days, List<ChartEntry> chartEntries, string labelFormat)
        {
            // Empty chart list before adding new data
            chartEntries.Clear();

            // Get list of entries from database
            var carbEntries = await App.Cdatabase.GetCarbEntriesAsync();

            carbEntries = carbEntries.Where(ent => ent.DateOfInput <= DateTime.Now && ent.DateOfInput >= DateTime.Now.AddDays(-days));
            if (carbEntries.Count() == 0)
                return;

            var distinctDays = carbEntries.Select(ent => ent.DateOfInput.Day).Distinct().OrderBy(ent => ent);
            foreach (var day in distinctDays)
            {
                var dayCarbs = carbEntries.Where(ent => ent.DateOfInput.Day == day);
                var totalCarbs = dayCarbs.Select(ent => ent.Carbohydrates).Sum();

                // Change datetime format
                string dateTime;
                if (day == DateTime.Now.Day) 
                    dateTime = "Today";
                else
                    dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day).ToString(labelFormat);

                // Set graph color depending on the amount of carbohydrate
                string graphColor = "#00e400";

                // Create chart entry
                chartEntries.Add(new ChartEntry(totalCarbs)
                {
                    Color = SKColor.Parse(graphColor),
                    Label = dateTime,
                    ValueLabel = $"{Math.Round(totalCarbs, 1)} Carbohydrates"
                });
            }
        }
    }
}