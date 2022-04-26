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
        public ObservableRangeCollection<BglModel> BglEntries { get; set; }
        public ObservableRangeCollection<BglGroupModel> BglGroups { get; set; }

        public Command AddBglCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<BglModel> DeleteBglCommand { get; }
        public AsyncCommand<object> SelectedBglGroupCommand { get; }
        public AsyncCommand<object> SelectedBglCommand { get; }
        public AsyncCommand DisplayGroupsCommand { get; }
        public AsyncCommand DisplayEntriesCommand { get; }
        public AsyncCommand LoadMoreCommand { get; }

        public BglViewModel(int month=0)
        {
            BglEntries = new ObservableRangeCollection<BglModel>();
            BglGroups = new ObservableRangeCollection<BglGroupModel>();

            AddBglCommand = new Command(AddBgl);
            RefreshCommand = new AsyncCommand(ViewRefresh);
            DeleteBglCommand = new AsyncCommand<BglModel>(DeleteBgl);
            SelectedBglGroupCommand = new AsyncCommand<object>(SelectedGroup);
            SelectedBglCommand = new AsyncCommand<object>(SelectedEntry);
            DisplayGroupsCommand = new AsyncCommand(LoadBglGroups);
            DisplayEntriesCommand = new AsyncCommand(LoadBglEntries);

            BglDate = DateTime.Now;
            BglTime = DateTime.Now.TimeOfDay;
            Month = month;
        }

        BglGroupModel selectedBglGroup;
        public BglGroupModel SelectedBglGroup
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

        int month;
        public int Month
        {
            get => month;
            set => SetProperty(ref month, value);
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

            if (code == 1)
                await App.Bdatabase.DeleteBglEntryAsync(bgl.ID);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await ViewRefresh();
        }

        async Task SelectedGroup(object arg)
        {
            BglGroupModel bglGroup = arg as BglGroupModel;
            if (bglGroup == null) return;

            SelectedBglGroup = null;
            BglGroups.Clear(); // Temp fix to not load listview twice after coming back from BglEntryPage
            await App.Current.MainPage.Navigation.PushAsync(new EditBglPage(bglGroup.GroupDate.Month));
        }

        async Task SelectedEntry(object arg)
        {
            BglModel bgl = arg as BglModel;
            if (bgl == null) return;

            SelectedBgl = null; // Deselect item
            BglEntries.Clear(); // Temp fix to not load listview twice after coming back from BglEntryPage
            await App.Current.MainPage.Navigation.PushAsync(new BglEntryPage(bgl.ID));
        }

        async Task LoadBglGroups()
        {
            IsBusy = true;
            BglGroups.Clear();
            var bglEntries = await App.Bdatabase.GetBglEntriesAsync();
            var distinctDates = bglEntries.Select(ent => ent.TimeOfMeasurment.Date).Distinct();
            foreach (var date in distinctDates)
            {
                var allGroupBgl = bglEntries.Where(ent => ent.TimeOfMeasurment.Date == date);
                var avgGroupBgl = allGroupBgl.Select(ent => ent.BGLmeasurement).Average();

                BglGroups.Add(new BglGroupModel { GroupDate=date, GroupAvgBgl=avgGroupBgl });
            }

            IsBusy = false;
        }

        // Loads BGL entries
        async Task LoadBglEntries()
        {
            IsBusy = true;
            BglEntries.Clear();
            var bglEntries = await App.Bdatabase.GetBglEntriesAsync();
            bglEntries = bglEntries.Where(ent => ent.TimeOfMeasurment.Month == Month);
            BglEntries.AddRange(bglEntries.Reverse());
            IsBusy = false;
        }
    }
}
