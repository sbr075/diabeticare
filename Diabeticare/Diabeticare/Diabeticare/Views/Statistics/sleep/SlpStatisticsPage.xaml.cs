using System;
using System.Collections.Generic;
using SkiaSharp;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.Models;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlpStatisticsPage : ContentPage
    {
        List<ChartEntry> ChartEntries = new List<ChartEntry>();
        Random random = new Random();
        readonly int recommendedSleepMin = 7;
        readonly int recommendedSleepMax = 9;
        public SlpStatisticsPage()
        {
            InitializeComponent();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GenDayChart(null, null);
        }

        // Generate sleep chart for current day
        private async void GenDayChart(object sender, EventArgs e)
        {
            LabelMid.Text = "Sleep Schedule - Today";
            ChartDescription.Text = "Your sleep schedule today.";
            ChartEntries.Clear(); // Empty list before adding new data
            
            // Get list of sleep entries from database
            List<SleepModel> SlpEntries = (List<SleepModel>)await App.Sdatabase.GetSlpEntriesAsync();

            TimeSpan sleepTime = new TimeSpan(0, 0, 0);
            for (int i = SlpEntries.Count - 1; i >= 0; i--)
            {
                // Remove sleep entries that are not registered today
                if (SlpEntries[i].SleepEnd < DateTime.Today)
                {
                    SlpEntries.RemoveAt(i);
                    continue;
                }
                
                // Sum up todays sleep entries
                sleepTime += SlpEntries[i].SleepEnd.Subtract(SlpEntries[i].SleepStart);
            }

            // Exit if there are no entries for today
            if (SlpEntries.Count == 0) return;

            // Set graph color depending on the amount of sleep
            string graphColor;
            if ((float)sleepTime.TotalHours < recommendedSleepMin) graphColor = "#ff1447";
            else if ((float)sleepTime.TotalHours > recommendedSleepMax) graphColor = "#FFDA00";
            else graphColor = "#00e400";

            // Create chart entry
            ChartEntries.Add(new ChartEntry((float)sleepTime.TotalHours)
            {
                Color = SKColor.Parse(graphColor),
                //ValueLabelColor = SKColor.Parse(graphColor),
                Label = $"Today",
                ValueLabel = $"{Math.Floor((float)sleepTime.TotalHours)}H {(float)sleepTime.Minutes}Min"
            });

            // Create sleep point chart
            SlpChart.Chart = new PointChart() { Entries = ChartEntries, LabelTextSize=30, PointAreaAlpha = 200, PointSize = 20, MaxValue = 9, LabelOrientation = Orientation.Horizontal, ValueLabelOrientation = Orientation.Horizontal};
        }

        // Generate sleep chart for the last 7 days
        private void GenWeekChart(object sender, EventArgs e)
        {
            LabelMid.Text = "Sleep Schedule - 7 Days";
            ChartDescription.Text = "Your sleep schedule the last 7 days.";
            int GoBackDays = 6;
            string LabelFormat = "MMMM dd";

            // Create chart entries for the last 7 days
            CreateChartEntries(GoBackDays, ChartEntries, LabelFormat);

            // Create sleep point chart
            SlpChart.Chart = new PointChart() { Entries = ChartEntries, LabelTextSize = 30, PointAreaAlpha = 200, PointSize = 20, MaxValue = 9, LabelOrientation = Orientation.Horizontal, ValueLabelOrientation = Orientation.Horizontal };
        }

        // Generate sleep chart for the last 30 days
        private void GenMonthChart(object sender, EventArgs e)
        {
            LabelMid.Text = "Sleep Schedule - 30 Days";
            ChartDescription.Text = "Your sleep schedule the last 30 days.";
            int GoBackDays = 29;
            string LabelFormat = "MM/dd";

            // Create chart entries for the last 30 days
            CreateChartEntries(GoBackDays, ChartEntries, LabelFormat);

            // Create sleep point chart
            SlpChart.Chart = new PointChart() { Entries = ChartEntries, LabelTextSize = 30, PointAreaAlpha = 200, PointSize = 10, MaxValue = 9, LabelOrientation = Orientation.Vertical, ValueLabelOrientation = Orientation.Horizontal };
        }

        // GoTo EditSlpPage
        async void ViewSLPEntries(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new SlpEntriesPage());
        }

        // GoTo AddSlpDataPage
        private async void CreateSLPEntry(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new AddSlpDataPage());
        }

        // Creates chart entries for sleep entries the last N days
        private async void CreateChartEntries(int days, List<ChartEntry> chartEntries, string labelFormat)
        {
            // Empty chart list before adding new data
            chartEntries.Clear();

            // Get list of entries from database
            List<SleepModel> slpEntries = (List<SleepModel>)await App.Sdatabase.GetSlpEntriesAsync();

            // Set date to N days ago
            DateTime nDaysAgo = DateTime.Today.AddDays(-days);

            // Iterate through N days
            for (DateTime day = nDaysAgo; day.Date <= DateTime.Now; day = day.AddDays(1))
            {
                TimeSpan sleepTime = new TimeSpan(0, 0, 0);
                for (int i = 0; i < slpEntries.Count; i++)
                {
                    if(slpEntries[i].SleepEnd.Date == day.Date)
                    {
                        // Sum up current day's sleep entries
                        sleepTime += slpEntries[i].SleepEnd.Subtract(slpEntries[i].SleepStart);
                    }
                }
                
                // Skip to next iteration/day if there are no sleep entries to avoid creating empty chart entries
                if (sleepTime == TimeSpan.Zero) continue;

                // Change datetime format
                string dateTime;
                if (day.Date == DateTime.Today) dateTime = "Today";
                else dateTime = day.Date.ToString(labelFormat);

                // Set graph color depending on the amount of sleep
                string graphColor;
                if ((float)sleepTime.TotalHours < recommendedSleepMin) graphColor = "#ff1447";
                else if ((float)sleepTime.TotalHours > recommendedSleepMax) graphColor = "#FFDA00";
                else graphColor = "#00e400";

                // Create chart entry
                chartEntries.Add(new ChartEntry((float)sleepTime.TotalHours)
                {
                    Color = SKColor.Parse(graphColor),
                    Label = dateTime,
                    //ValueLabel = $"{Math.Floor((float)sleepTime.TotalHours)}H {(float)sleepTime.Minutes}Min"
                    ValueLabel = $"{Math.Round((float)sleepTime.TotalHours, 1)}H"
                });

            }
        }
    }
}