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

        public string BglEntry { get; set; }
        //public TimeSpan BglTime { get; set; }

        public BglViewModel()
        {
            BglEntries = new ObservableRangeCollection<Bgl>();
            AddBglCommand = new Command(AddBgl);
            RefreshCommand = new AsyncCommand(ViewRefresh);
            DeleteBglCommand = new AsyncCommand<Bgl>(DeleteBgl);
            SelectedBglCommand = new AsyncCommand<object>(SelectedEntry);
            DisplayEntries = new AsyncCommand(LoadBglEntries);
            BglTime = DateTime.Now.TimeOfDay;
        }

        // Creates a new BGL entry
        public async void AddBgl()
        {
            // TODO: display text if the entry field is empty
            if (BglEntry == null)
                return;
            var measurement = float.Parse(BglEntry);
            var date = DateTime.Now;
            var time = BglTime;
            await App.Bdatabase.AddBglEntryAsync(measurement, date, time);
            await Shell.Current.GoToAsync("..");
        }

        Bgl selectedBgl;
        public Bgl SelectedBgl
        {
            get => selectedBgl;
            set => SetProperty(ref selectedBgl, value);
        }

        TimeSpan bglTime;
        public TimeSpan BglTime
        {
            get => bglTime;
            set => SetProperty(ref bglTime, value);
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
            await App.Bdatabase.DeleteBglEntryAsync(bgl.ID);
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
        /*
        async Task SelectedEntry(object args)
        {
            var bgl = args as Bgl;
            if (bgl == null) return;
            SelectedBgl = null; // Deselect item

            var update = await App.Current.MainPage.DisplayPromptAsync("Edit Measurement", "Blood Glucose Level", "OK", "CANCEL", initialValue: $"{bgl.BGLmeasurement}", keyboard: Keyboard.Numeric);
            if (string.IsNullOrEmpty(update)) return; // TODO: display text saying that the entry field was empty

            await App.Bdatabase.UpdateBglEntryAsync(bgl, float.Parse(update), BglTime);
            await ViewRefresh();
        }
        */

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
