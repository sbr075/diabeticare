using System;
using Diabeticare.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExerciseEntryPage : ContentPage
    {
        public int ExerciseID { get; set; }
        ExerciseModel exerciseEntry;

        public ExerciseEntryPage(int exerciseID)
        {
            InitializeComponent();
            ExerciseID = exerciseID;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            exerciseEntry = await App.Edatabase.GetExerciseEntryAsync(ExerciseID);
            dateStart.Date = exerciseEntry.ExerciseStart;
            dateEnd.Date = exerciseEntry.ExerciseEnd;
            timeStart.Time = exerciseEntry.ExerciseStart.TimeOfDay;
            timeEnd.Time = exerciseEntry.ExerciseEnd.TimeOfDay;
        }

        private async void SaveUpdatedExerciseEntry(object sender, EventArgs e)
        {
            // Check if user is trying to time travel
            if ((timeStart.Time > timeEnd.Time && dateStart.Date >= dateEnd.Date) || dateStart.Date > dateEnd.Date)
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Invalid exercise entry.", "OK");
                return;
            }

            DateTime updatedStart = dateStart.Date.Add(timeStart.Time);
            DateTime updatedEnd = dateEnd.Date.Add(timeEnd.Time);

            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateExerciseAsync(exerciseEntry.Name, updatedStart, updatedEnd, exerciseEntry.ServerID);
            if (code == 1)
            {
                await App.Edatabase.UpdateExerciseEntryAsync(exerciseEntry, updatedStart, updatedEnd, server_id);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");
            }

            await App.Current.MainPage.Navigation.PopAsync();
        }
        private async void CancelUpdate(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }
    }
}