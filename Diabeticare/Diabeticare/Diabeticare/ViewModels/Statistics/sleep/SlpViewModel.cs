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
            var start = SlpStart.Add(SlpTimeStart);
            var end = SlpEnd.Add(SlpTimeEnd);
            var created = DateTime.Now;
            await App.Sdatabase.AddSlpEntryAsync(start, end, created);
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
            await App.Sdatabase.DeleteSlpEntryAsync(slp.ID);
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
