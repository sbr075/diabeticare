using System;
using System.Collections.Generic;
using SkiaSharp;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.Models;
using System.Linq;
using Diabeticare.ViewModels;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlpStatisticsPage : ContentPage
    {
        List<ChartEntry> ChartEntries = new List<ChartEntry>();
        
        readonly int recommendedSleepMin = 7;
        readonly int recommendedSleepMax = 9;
        public SlpStatisticsPage()
        {
            InitializeComponent();
            BindingContext = new SlpViewModel();
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

            // Exit if there are no entries for today
            if (SlpEntries.Count == 0) return;

            TimeSpan sleepTime = new TimeSpan(0, 0, 0);
            for (int i = SlpEntries.Count - 1; i >= 0; i--)
            {
                var slpEntry = SlpEntries[i];
                // Remove sleep entries that are not registered today
                if (slpEntry.SleepEnd < DateTime.Today)
                {
                    SlpEntries.RemoveAt(i);
                    continue;
                }

                // If Sleep start and sleep end is not on the same day
                if (slpEntry.SleepStart.Day != slpEntry.SleepEnd.Day)
                {
                    // If current day is looking at sleep start days
                    if (slpEntry.SleepStart.Day == DateTime.Today.Day)
                    {
                        DateTime endOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, slpEntry.SleepStart.Day, 23, 59, 59);
                        sleepTime += endOfDay - slpEntry.SleepStart;
                    }
                    // Current day is looking at sleep end days
                    else
                    {
                        DateTime startOfDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, slpEntry.SleepEnd.Day);
                        sleepTime += slpEntry.SleepEnd - startOfDay;
                    }
                }
                else
                {
                    sleepTime += (slpEntry.SleepEnd - slpEntry.SleepStart);
                }
            }

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
                Label = "Today",
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
        private async void CreateChartEntries(int daysBack, List<ChartEntry> chartEntries, string labelFormat)
        {
            // Empty chart list before adding new data
            chartEntries.Clear();

            // Get list of entries from database
            var slpEntries = await App.Sdatabase.GetSlpEntriesAsync();
            slpEntries = slpEntries.Where(ent => ent.SleepStart >= DateTime.Today.AddDays(-daysBack) && ent.SleepEnd <= DateTime.Today.AddDays(1));

            var sleepStartDays = slpEntries.Select(ent => ent.SleepStart.Date.Day);
            var sleepEndDays = slpEntries.Select(ent => ent.SleepEnd.Date.Day);

            var sleepAllDays = sleepStartDays.Concat(sleepEndDays);
            var distinctDays = sleepAllDays.Distinct().OrderBy(ent => ent);

            foreach (var day in distinctDays)
            {
                var allGroupSlp = slpEntries.Where(ent => ent.SleepStart.Day == day || ent.SleepEnd.Day == day);

                TimeSpan totalSleep;
                foreach (var group in allGroupSlp)
                {
                    // If Sleep start and sleep end is not on the same day
                    if (group.SleepStart.Day != group.SleepEnd.Day)
                    {
                        // If current day is looking at sleep start days
                        if (group.SleepStart.Day == day)
                        {
                            DateTime endOfDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, group.SleepStart.Day, 23, 59, 59);
                            totalSleep += endOfDay - group.SleepStart;
                        }
                        // Current day is looking at sleep end days
                        else
                        {
                            DateTime startOfDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, group.SleepEnd.Day);
                            totalSleep += group.SleepEnd - startOfDay;
                        }
                    }
                    else
                    {
                        totalSleep += (group.SleepEnd - group.SleepStart);
                    }
                }

                // Change datetime format
                string dateTime;
                if (day == DateTime.Today.Day) dateTime = "Today";
                else dateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, day).ToString(labelFormat);

                // Set graph color depending on the amount of sleep
                string graphColor;
                if ((float)totalSleep.TotalHours < recommendedSleepMin) graphColor = "#ff1447";
                else if ((float)totalSleep.TotalHours > recommendedSleepMax) graphColor = "#FFDA00";
                else graphColor = "#00e400";

                // Create chart entry
                chartEntries.Add(new ChartEntry((float)totalSleep.TotalHours)
                {
                    Color = SKColor.Parse(graphColor),
                    Label = dateTime,
                    ValueLabel = $"{Math.Round((float)totalSleep.TotalHours, 1)}H"
                });
            }
        }
    }
}