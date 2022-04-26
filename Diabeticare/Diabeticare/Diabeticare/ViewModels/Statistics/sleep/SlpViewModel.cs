using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Diabeticare.Models;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using Diabeticare.Views;
using System.Threading.Tasks;
using System.Linq;

namespace Diabeticare.ViewModels
{
    public class SlpViewModel : ViewModelBase
    {

        // Data collection of sleep entries
        public ObservableRangeCollection<SleepModel> SlpEntries { get; set; }

        public Command AddSlpCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<SleepModel> DeleteSlpCommand { get; }
        public AsyncCommand<object> SelectedSlpCommand { get; }
        public AsyncCommand DisplayEntries { get; }
        public SlpViewModel()
        {
            
            SlpEntries = new ObservableRangeCollection<SleepModel>();
            AddSlpCommand = new Command(AddSlp);
            RefreshCommand = new AsyncCommand(ViewRefresh);
            DeleteSlpCommand = new AsyncCommand<SleepModel>(DeleteSlp);
            SelectedSlpCommand = new AsyncCommand<object>(SelectedEntry);
            DisplayEntries = new AsyncCommand(LoadSlpEntries);
            SlpStart = DateTime.Today.AddDays(-1);
            SlpEnd = DateTime.Today;
            slpTimeEnd = DateTime.Now.TimeOfDay;
        }

        // Creates a new sleep entry
        public async void AddSlp()
        {
            // Check if user is trying to time travel
            if((SlpTimeStart > SlpTimeEnd && slpStart >= slpEnd) || slpStart > slpEnd)
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Invalid sleep entry.", "OK");
                return;
            }
            DateTime start = SlpStart.Date.Add(SlpTimeStart);
            DateTime end = SlpEnd.Date.Add(SlpTimeEnd);

            // Asks server to add entry
            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateSleepAsync(start, end);

            if (code == 1)
                await App.Sdatabase.AddSlpEntryAsync(start, end, server_id);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await Shell.Current.GoToAsync("..");
        }

        // Refresh the sleep listview
        async Task ViewRefresh()
        {
            IsBusy = true;
            SlpEntries.Clear();
            var slpEntries = await App.Sdatabase.GetSlpEntriesAsync();
            SlpEntries.AddRange(slpEntries.Reverse());
            IsBusy = false;
        }

        // Delete specified sleep entry
        async Task DeleteSlp(SleepModel slp)
        {
            // Asks server to delete entry
            (int code, string message) = await App.apiServices.DeleteSleepAsync(slp.ServerID);

            if (code == 1)
                await App.Sdatabase.DeleteSlpEntryAsync(slp.ID);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await ViewRefresh();
        }

        async Task SelectedEntry(object arg)
        {

            SleepModel slp = arg as SleepModel;
            if (slp == null) return;

            SelectedSlp = null; // Deselect item
            SlpEntries.Clear(); // Temp fix to not load listview twice after coming back from SlpEntryPage
            var route = $"{nameof(SlpEntryPage)}?SlpID={slp.ID}";
            await Shell.Current.GoToAsync(route);
        }

        // Loads sleep entries
        async Task LoadSlpEntries()
        {
            IsBusy = true;
            SlpEntries.Clear();
            var slpEntries = await App.Sdatabase.GetSlpEntriesAsync();
            SlpEntries.AddRange(slpEntries.Reverse());
            IsBusy = false;
        }

        SleepModel selectedSlp;
        public SleepModel SelectedSlp
        {
            get => selectedSlp;
            set => SetProperty(ref selectedSlp, value);
        }

        DateTime slpStart;
        public DateTime SlpStart
        {
            get => slpStart;
            set => SetProperty(ref slpStart, value);
        }

        DateTime slpEnd;
        public DateTime SlpEnd
        {
            get => slpEnd;
            set => SetProperty(ref slpEnd, value);
        }
        
        TimeSpan slpTimeStart;
        public TimeSpan SlpTimeStart
        {
            get => slpTimeStart;
            set => SetProperty(ref slpTimeStart, value);
        }

        TimeSpan slpTimeEnd;
        public TimeSpan SlpTimeEnd
        {
            get => slpTimeEnd;
            set => SetProperty(ref slpTimeEnd, value);
        }
    }
}
