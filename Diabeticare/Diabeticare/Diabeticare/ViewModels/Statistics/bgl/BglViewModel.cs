using Diabeticare.Models;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Command = MvvmHelpers.Commands.Command;
using Diabeticare.Views;


namespace Diabeticare.ViewModels
{
    public class BglViewModel : ViewModelBase
    {
        // Data collection of BGL entries
        public ObservableRangeCollection<BglModel> BglEntries { get; set; }
        public ObservableRangeCollection<GroupModel> BglGroups { get; set; }

        public Command AddBglCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<BglModel> DeleteBglCommand { get; }
        public AsyncCommand<object> SelectedBglGroupCommand { get; }
        public AsyncCommand<object> SelectedBglCommand { get; }
        public AsyncCommand DisplayGroupsCommand { get; }
        public AsyncCommand DisplayEntriesCommand { get; }
        public AsyncCommand LoadMoreCommand { get; }

        public BglViewModel(int day=0)
        {
            BglEntries = new ObservableRangeCollection<BglModel>();
            BglGroups = new ObservableRangeCollection<GroupModel>();

            AddBglCommand = new Command(AddBgl);
            RefreshCommand = new AsyncCommand(ViewRefresh);
            DeleteBglCommand = new AsyncCommand<BglModel>(DeleteBgl);
            SelectedBglGroupCommand = new AsyncCommand<object>(SelectedGroup);
            SelectedBglCommand = new AsyncCommand<object>(SelectedEntry);
            DisplayGroupsCommand = new AsyncCommand(LoadBglGroups);
            DisplayEntriesCommand = new AsyncCommand(LoadBglEntries);

            BglDate = DateTime.Now;
            BglTime = DateTime.Now.TimeOfDay;
            Day = day;
        }

        GroupModel selectedBglGroup;
        public GroupModel SelectedBglGroup
        {
            get => selectedBglGroup;
            set => SetProperty(ref selectedBglGroup, value);
        }

        BglModel selectedBgl;
        public BglModel SelectedBgl
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

        int day;
        public int Day
        {
            get => day;
            set => SetProperty(ref day, value);
        }

        // Creates a new BGL entry
        public async void AddBgl()
        {
            // Cannot add new entry if no entry was created
            if (BglEntry == null) return;

            var measurement = float.Parse(BglEntry);

            DateTime timeOfMeasurment = BglDate.Date.Add(BglTime);

            // Asks server to add entry
            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateBGLAsync(measurement, timeOfMeasurment);

            if (code == 1) // Successfull request
                await App.Bdatabase.AddBglEntryAsync(measurement, timeOfMeasurment, server_id);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await App.Current.MainPage.Navigation.PopAsync();
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
        async Task DeleteBgl(BglModel bgl)
        {
            // Asks server to delete entry
            (int code, string message) = await App.apiServices.DeleteBGLAsync(bgl.ServerID);

            if (code == 1) // Successfull request
                await App.Bdatabase.DeleteBglEntryAsync(bgl.ID);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await ViewRefresh();
        }

        async Task SelectedGroup(object arg)
        {
            // Converts selected group to GroupModel
            GroupModel bglGroup = arg as GroupModel;
            if (bglGroup == null) return;

            // Call edit page (dispaly all entries) to display all for given day
            SelectedBglGroup = null;
            BglGroups.Clear(); // Temp fix to not load listview twice after coming back from BglEntryPage
            await App.Current.MainPage.Navigation.PushAsync(new EditBglPage(bglGroup.GroupDate.Day));
        }

        async Task SelectedEntry(object arg)
        {
            // Loads specific bgl entry into entry view to be edited
            BglModel bgl = arg as BglModel;
            if (bgl == null) return;

            SelectedBgl = null; // Deselect item
            BglEntries.Clear(); // Temp fix to not load listview twice after coming back from BglEntryPage
            await App.Current.MainPage.Navigation.PushAsync(new BglEntryPage(bgl.ID));
        }

        async Task LoadBglGroups()
        {
            // Fetches all distinct days and gathers information about the entries for those days to be displayed
            IsBusy = true;
            BglGroups.Clear();
            var bglEntries = await App.Bdatabase.GetBglEntriesAsync();
            var distinctDays = bglEntries.Select(ent => ent.TimeOfMeasurment.Date.Day).Distinct().OrderByDescending(ent => ent);
            foreach (var day in distinctDays)
            {
                var allGroupBgl = bglEntries.Where(ent => ent.TimeOfMeasurment.Date.Day == day);
                var avgGroupBgl = allGroupBgl.Select(ent => ent.BGLmeasurement).Average();

                BglGroups.Add(new GroupModel { GroupDate=new DateTime(DateTime.Now.Year, DateTime.Now.Month, day), GroupAvg=avgGroupBgl });
            }

            IsBusy = false;
        }

        // Loads BGL entries
        async Task LoadBglEntries()
        {
            IsBusy = true;
            BglEntries.Clear();
            var bglEntries = await App.Bdatabase.GetBglEntriesAsync();
            bglEntries = bglEntries.Where(ent => ent.TimeOfMeasurment.Day == Day);
            BglEntries.AddRange(bglEntries.Reverse());
            IsBusy = false;
        }
    }
}
