using System;
using System.Collections.Generic;
using SkiaSharp;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.Models;
using System.Linq;
using Diabeticare.ViewModels;
using System.Threading.Tasks;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseStatisticsPage : ContentPage
    {
        List<ChartEntry> ChartEntries = new List<ChartEntry>();

        public ExerciseStatisticsPage()
        {
            InitializeComponent();
            BindingContext = new ExerciseViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        private void ExercisePickerIndexChanged(object sender, EventArgs e)
        {
            int daysBack = 0;
            string LabelFormat = "MM/dd";
            GenExerciseChart(daysBack, LabelFormat);
        }

        private async void GenDayChart(object sender, EventArgs e)
        {
            LabelMid.Text = "Exercise Chart - Today";
            ChartDescription.Text = "Total exercise today.";
            int daysBack = 0;
            string LabelFormat = "MM/dd";

            // Create chart entries for today
            await GenExerciseChart(daysBack, LabelFormat);
        }

        private async void GenWeekChart(object sender, EventArgs e)
        {
            LabelMid.Text = "Exercise Chart - 7 Days";
            ChartDescription.Text = "Total exercise the last 7 days.";
            int daysBack = 7;
            string LabelFormat = "MMMM dd";

            // Create chart entries for the last 7 days
            await GenExerciseChart(daysBack, LabelFormat);
        }

        private async void GenMonthChart(object sender, EventArgs e)
        {
            LabelMid.Text = "Exercise Chart - 30 Days";
            ChartDescription.Text = "Total exercise the last 30 days.";
            int daysBack = 30;
            string LabelFormat = "MM/dd";

            // Create chart entries for the last 30 days
            await GenExerciseChart(daysBack, LabelFormat);
        }

        private async Task GenExerciseChart(int daysBack, string labelFormat)
        {
            string exercise = ExercisePicker.SelectedItem.ToString();
            ChartEntries.Clear(); // Empty list before adding new data

            var exerciseEntries = await App.Edatabase.GetExerciseEntriesAsync(exercise);
            exerciseEntries = exerciseEntries.Where(ent => ent.ExerciseStart >= DateTime.Today.AddDays(-daysBack) && ent.ExerciseEnd <= DateTime.Today.AddDays(1));

            var exerciseStartDays = exerciseEntries.Select(ent => ent.ExerciseStart.Date.Day);
            var exerciseEndDays = exerciseEntries.Select(ent => ent.ExerciseEnd.Date.Day);

            var exerciseAllDays = exerciseStartDays.Concat(exerciseEndDays);
            var distinctDays = exerciseAllDays.Distinct().OrderBy(ent => ent);

            foreach (var day in distinctDays)
            {
                var allGroupExercise = exerciseEntries.Where(ent => ent.ExerciseStart.Day == day || ent.ExerciseEnd.Day == day);

                TimeSpan totalExercise;
                foreach (var group in allGroupExercise)
                {
                    // If Exercise start and exercise end is not on the same day
                    if (group.ExerciseStart.Day != group.ExerciseEnd.Day)
                    {
                        // If current day is looking at exercise start days
                        if (group.ExerciseStart.Day == day)
                        {
                            DateTime endOfDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, group.ExerciseStart.Day, 23, 59, 59);
                            totalExercise += endOfDay - group.ExerciseStart;
                        }
                        // Current day is looking at exercise end days
                        else
                        {
                            DateTime startOfDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, group.ExerciseEnd.Day);
                            totalExercise += group.ExerciseEnd - startOfDay;
                        }
                    }
                    else
                    {
                        totalExercise += (group.ExerciseEnd - group.ExerciseStart);
                    }
                }

                // Change datetime format
                string dateTime, valueLabel;
                if (daysBack == 0)
                {
                    dateTime = "Today";
                    valueLabel = $"{Math.Floor((float)totalExercise.TotalHours)}H {(float)totalExercise.Minutes}Min";
                }
                else {
                    dateTime = new DateTime(DateTime.Today.Year, DateTime.Today.Month, day).ToString(labelFormat);
                    valueLabel = $"{Math.Round((float)totalExercise.TotalHours, 1)}H";
                }

                // Set graph color depending on the amount of sleep
                string graphColor = "#00e400";

                // Create chart entry
                ChartEntries.Add(new ChartEntry((float)totalExercise.TotalHours)
                {
                    Color = SKColor.Parse(graphColor),
                    Label = dateTime,
                    ValueLabel = valueLabel
                });
            }

            // Create sleep point chart
            ExerciseChart.Chart = new PointChart() { Entries = ChartEntries, LabelTextSize = 30, PointAreaAlpha = 200, PointSize = 20, MaxValue = 2, LabelOrientation = Orientation.Horizontal, ValueLabelOrientation = Orientation.Horizontal };
        }

        // Go to exercise entry
        async void ViewExerciseEntries(object sender, EventArgs e)
        {
            string exercise = ExercisePicker.SelectedItem.ToString();
            await App.Current.MainPage.Navigation.PushAsync(new ExerciseEntriesPage(exercise));
        }

        // Create exercise entry
        private async void CreateExerciseEntry(object sender, EventArgs e)
        {
            string exercise = ExercisePicker.SelectedItem.ToString();
            await App.Current.MainPage.Navigation.PushAsync(new AddExerciseDataPage(exercise));
        }
    }
}