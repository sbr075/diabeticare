using MvvmHelpers;
using System;
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
        public ObservableRangeCollection<GroupModel> SlpGroups { get; set; }

        public Command AddSlpCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<SleepModel> DeleteSlpCommand { get; }
        public AsyncCommand<object> SelectedSlpGroupCommand { get; }
        public AsyncCommand<object> SelectedSlpCommand { get; }
        public AsyncCommand DisplayGroupsCommand { get; }
        public AsyncCommand DisplayEntriesCommand { get; }
        public SlpViewModel(int month=0)
        {
            
            SlpEntries = new ObservableRangeCollection<SleepModel>();
            SlpGroups = new ObservableRangeCollection<GroupModel>();

            AddSlpCommand = new Command(AddSlp);
            RefreshCommand = new AsyncCommand(ViewRefresh);
            DeleteSlpCommand = new AsyncCommand<SleepModel>(DeleteSlp);
            SelectedSlpGroupCommand = new AsyncCommand<object>(SelectedGroup);
            SelectedSlpCommand = new AsyncCommand<object>(SelectedEntry);
            DisplayGroupsCommand = new AsyncCommand(LoadSlpGroups);
            DisplayEntriesCommand = new AsyncCommand(LoadSlpEntries);

            SlpStart = DateTime.Today.AddDays(-1);
            SlpEnd = DateTime.Today;
            slpTimeEnd = DateTime.Now.TimeOfDay;
            Month = month;
        }

        GroupModel selectedSlpGroup;
        public GroupModel SelectedSlpGroup
        {
            get => selectedSlpGroup;
            set => SetProperty(ref selectedSlpGroup, value);
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

        int month;
        public int Month
        {
            get => month;
            set => SetProperty(ref month, value);
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

            await App.Current.MainPage.Navigation.PopAsync();
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

        async Task SelectedGroup(object arg)
        {
            GroupModel slpGroup = arg as GroupModel;
            if (slpGroup == null) return;

            SelectedSlpGroup = null;
            SlpGroups.Clear(); // Temp fix to not load listview twice after coming back from BglEntryPage
            await App.Current.MainPage.Navigation.PushAsync(new EditSlpPage(slpGroup.GroupDate.Month));
        }

        async Task SelectedEntry(object arg)
        {

            SleepModel slp = arg as SleepModel;
            if (slp == null) return;

            SelectedSlp = null; // Deselect item
            SlpEntries.Clear(); // Temp fix to not load listview twice after coming back from SlpEntryPage
            await App.Current.MainPage.Navigation.PushAsync(new SlpEntryPage(slp.ID));
        }

        async Task LoadSlpGroups()
        {
            IsBusy = true;
            SlpGroups.Clear();
            var bglEntries = await App.Sdatabase.GetSlpEntriesAsync();
            var distinctDates = bglEntries.Select(ent => ent.SleepStart.Date).Distinct().OrderByDescending(ent => ent.Date);
            foreach (var date in distinctDates)
            {
                var allGroupBgl = bglEntries.Where(ent => ent.SleepStart.Date == date);
                var avgGroupBgl = (float) allGroupBgl.Select(ent => (ent.SleepEnd - ent.SleepStart).Hours).Average();

                SlpGroups.Add(new GroupModel { GroupDate = date, GroupAvg = avgGroupBgl });
            }

            IsBusy = false;
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
    }
}
