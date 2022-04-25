using Diabeticare.Models;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Command = MvvmHelpers.Commands.Command;
using Xamarin.CommunityToolkit.Extensions;
using Diabeticare.Views;

namespace Diabeticare.ViewModels
{
    public class BglViewModel : ViewModelBase
    {
        // Data collection of BGL entries
        public ObservableRangeCollection<Bgl> BglEntries { get; set; }

        public Command AddBglCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<Bgl> DeleteBglCommand { get; }
        public AsyncCommand<object> SelectedBglCommand { get; }
        public AsyncCommand DisplayEntries { get; }
        public AsyncCommand LoadMoreCommand { get; }

        public BglViewModel()
        {
            BglEntries = new ObservableRangeCollection<Bgl>();
            AddBglCommand = new Command(AddBgl);
            RefreshCommand = new AsyncCommand(ViewRefresh);
            DeleteBglCommand = new AsyncCommand<Bgl>(DeleteBgl);
            SelectedBglCommand = new AsyncCommand<object>(SelectedEntry);
            DisplayEntries = new AsyncCommand(LoadBglEntries);

            BglDate = DateTime.Now;
            BglTime = DateTime.Now.TimeOfDay;
        }

        Bgl selectedBgl;
        public Bgl SelectedBgl
        {
            get => selectedBgl;
            set => SetProperty(ref selectedBgl, value);
        }

        string bglEntry;
        public string BglEntry
        {
            get => bglEntry;
            set => SetProperty(ref bglEntry, value);
        }


        TimeSpan bglTime;
        public TimeSpan BglTime
        {
            get => bglTime;
            set => SetProperty(ref bglTime, value);
        }

        DateTime bglDate;
        public DateTime BglDate
        {
            get => bglDate;
            set => SetProperty(ref bglDate, value);
        }

        // Creates a new BGL entry
        public async void AddBgl()
        {
            // TODO: display text if the entry field is empty
            if (BglEntry == null)
                return;

            var measurement = float.Parse(BglEntry);

            DateTime timeOfMeasurment = BglDate.Date.Add(BglTime);

            // Asks server to add entry
            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateBGLAsync(measurement, timeOfMeasurment);

            if (code == 1)
                await App.Bdatabase.AddBglEntryAsync(measurement, timeOfMeasurment, server_id);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await Shell.Current.GoToAsync("..");
        }

        // Refresh the bgl listview
        async Task ViewRefresh()
        {
            IsBusy = true;
            BglEntries.Clear();
            var bglEntries = await App.Bdatabase.GetBglEntriesAsync();
            BglEntries.AddRange(bglEntries.Reverse());
            IsBusy = false;
        }

        // Delete specified BGL entry
        async Task DeleteBgl(Bgl bgl)
        {
            // Asks server to delete entry
            (int code, string message) = await App.apiServices.DeleteBGLAsync(bgl.ServerID);

            if (code == 1)
                await App.Bdatabase.DeleteBglEntryAsync(bgl.ID);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await ViewRefresh();
        }

        async Task SelectedEntry(object arg)
        {
            Bgl bgl = arg as Bgl;
            if (bgl == null) return;

            SelectedBgl = null; // Deselect item
            BglEntries.Clear(); // Temp fix to not load listview twice after coming back from BglEntryPage
            var route = $"{nameof(BglEntryPage)}?BglID={bgl.ID}";
            await Shell.Current.GoToAsync(route);
        }

        // Loads BGL entries
        async Task LoadBglEntries()
        {
            IsBusy = true;
            var bglEntries = await App.Bdatabase.GetBglEntriesAsync();
            BglEntries.AddRange(bglEntries.Reverse());
            IsBusy = false;
        }
    }
}
