using MvvmHelpers.Commands;
using Diabeticare.Models;
using MvvmHelpers;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Diabeticare.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ObservableRangeCollection<MoodModel> MoodEntries { get; set; }

        public Command AddMoodCommand { get; }
        public AsyncCommand<MoodModel> DeleteMoodCommand { get; }
        public AsyncCommand LoadMoodEntriesCommand { get; }

        public HomeViewModel()
        {
            AddMoodCommand = new Command(AddMood);
            DeleteMoodCommand = new AsyncCommand<MoodModel>(DeleteMood);
            LoadMoodEntriesCommand = new AsyncCommand(LoadMoodEntries);

            Username = App.user.Username;
        }

        string username;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public async void AddMood(object sender)
        {
            // Fetches value of button user pressed
            var moodButton = sender as Xamarin.Forms.Button;
            int moodValue = int.Parse(moodButton.Text);

            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateMoodAsync(moodValue, DateTime.Today);

            if (code == 1) // Successfull request
                await App.Mdatabase.AddMoodEntryAsync(moodValue, DateTime.Today, server_id);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");
        }

        async Task DeleteMood(MoodModel mood)
        {
            // Asks server to delete entry
            (int code, string message) = await App.apiServices.DeleteMoodAsync(mood.ServerID);

            if (code == 1) // Successfull request
                await App.Bdatabase.DeleteBglEntryAsync(mood.ID);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");
        }

        async Task LoadMoodEntries()
        {
            // Loads all mood entries for the current month
            IsBusy = true;
            MoodEntries.Clear();
            var moodEntries = await App.Mdatabase.GetMoodEntriesAsync();
            moodEntries = moodEntries.Where(ent => ent.Date.Month == DateTime.Today.Month);
            MoodEntries.AddRange(moodEntries.Reverse());
            IsBusy = false;
        }
    }
}
