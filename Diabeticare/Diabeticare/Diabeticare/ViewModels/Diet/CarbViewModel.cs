using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using Diabeticare.Models;
using Xamarin.Forms;
using System.Threading.Tasks;
using Diabeticare.Views;

namespace Diabeticare.ViewModels
{
    public class CarbViewModel : ViewModelBase
    {
        public ObservableRangeCollection<CarbohydrateModel> CarbEntries { get; set; }
        public ObservableRangeCollection<GroupModel> CarbGroups { get; set; }
        public Command AddCarbCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<CarbohydrateModel> DeleteCarbCommand { get; }
        public AsyncCommand<object> SelectedCarbGroupCommand { get; }
        public AsyncCommand<object> SelectedCarbCommand { get; }
        public AsyncCommand DisplayGroupsCommand { get; }
        public AsyncCommand DisplayEntries { get; }


        public CarbViewModel(int day=0)
        {
            CarbEntries = new ObservableRangeCollection<CarbohydrateModel>();
            CarbGroups = new ObservableRangeCollection<GroupModel>();

            AddCarbCommand = new Command(AddCarb);
            RefreshCommand = new AsyncCommand(ViewRefresh);
            DeleteCarbCommand = new AsyncCommand<CarbohydrateModel>(DeleteCarb);
            SelectedCarbGroupCommand = new AsyncCommand<object>(SelectedGroup);
            SelectedCarbCommand = new AsyncCommand<object>(SelectedEntry);
            DisplayGroupsCommand = new AsyncCommand(LoadCarbGroups);
            DisplayEntries = new AsyncCommand(LoadCarbEntries);

            carbTime = DateTime.Now.TimeOfDay;
            carbDate = DateTime.Now.Date;
            Day = day;
        }

        GroupModel selectedCarbGroup;
        public GroupModel SelectedCarbGroup
        {
            get => selectedCarbGroup;
            set => SetProperty(ref selectedCarbGroup, value);
        }

        CarbohydrateModel selectedCarb;
        public CarbohydrateModel SelectedCarb
        {
            get => selectedCarb;
            set => SetProperty(ref selectedCarb, value);
        }

        string carbEntry;
        public string CarbEntry
        {
            get => carbEntry;
            set => SetProperty(ref carbEntry, value);
        }

        string foodName;
        public string FoodName
        {
            get => foodName;
            set => SetProperty(ref foodName, value);
        }

        TimeSpan carbTime;
        public TimeSpan CarbTime
        {
            get => carbTime;
            set => SetProperty(ref carbTime, value);
        }

        DateTime carbDate;
        public DateTime CarbDate
        {
            get => carbDate;
            set => SetProperty(ref carbDate, value);
        }

        int day;
        public int Day
        {
            get => day;
            set => SetProperty(ref day, value);
        }

        public async void AddCarb()
        {
            // TODO: display text if the entry field is empty
            if (CarbEntry == null)
                return;

            // Parse data to be stored
            var carbohydrates = float.Parse(CarbEntry);
            string foodname = FoodName;
            DateTime dateOfInput = CarbDate.Date.Add(CarbTime);

            // Asks server to add entry
            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateCIAsync(carbohydrates, foodname, dateOfInput);

            if (code == 1) // Successfull request
                await App.Cdatabase.AddCarbEntryAsync(carbohydrates, dateOfInput, foodname, server_id);
            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await App.Current.MainPage.Navigation.PopAsync();
        }

        async Task ViewRefresh()
        {
            IsBusy = true;
            CarbEntries.Clear();
            var carbEntries = await App.Cdatabase.GetCarbEntriesAsync();
            CarbEntries.AddRange(carbEntries.Reverse());
            IsBusy = false;
        }

        async Task DeleteCarb(CarbohydrateModel carb)
        {
            // Asks server to delete entry
            (int code, string message) = await App.apiServices.DeleteCIAsync(carb.ServerID);

            if (code == 1) // Successfull request
                await App.Cdatabase.DeleteCarbEntryAsync(carb.ID);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await ViewRefresh();
        }

        async Task SelectedGroup(object arg)
        {
            // Convert object to GroupModel object
            GroupModel carbGroup = arg as GroupModel;
            if (carbGroup == null) return;

            // Load group into edit page
            SelectedCarbGroup = null;
            CarbGroups.Clear(); // Temp fix to not load listview twice after coming back from CarbEntryPage
            await App.Current.MainPage.Navigation.PushAsync(new EditCarbPage(carbGroup.GroupDate.Day));
        }

        async Task SelectedEntry(object arg)
        {
            // Convert object to CarbohydrateModel
            CarbohydrateModel carb = arg as CarbohydrateModel;
            if (carb == null) return;

            // Load entry into carb entry page
            SelectedCarb = null; // Deselect item
            CarbEntries.Clear();
            await App.Current.MainPage.Navigation.PushAsync(new CarbEntryPage(carb.ID));
        }

        async Task LoadCarbGroups()
        {
            // Fetches all distinct dates of when carbohydrate entries were entered
            IsBusy = true;
            CarbGroups.Clear();
            var carbEntries = await App.Cdatabase.GetCarbEntriesAsync();
            var distinctDays = carbEntries.Select(ent => ent.DateOfInput.Date.Day).Distinct().OrderByDescending(ent => ent);

            foreach (var day in distinctDays)
            {
                var allGroupCarb = carbEntries.Where(ent => ent.DateOfInput.Date.Day == day);
                var avgGroupCarb = allGroupCarb.Select(ent => ent.Carbohydrates).Sum();

                CarbGroups.Add(new GroupModel { GroupDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day), GroupAvg = avgGroupCarb });
            }

            IsBusy = false;
        }

        // Loads carb entries
        async Task LoadCarbEntries()
        {
            // Loads all carbohydrate entries for the given day
            IsBusy = true;
            CarbEntries.Clear();
            var carbEntries = await App.Cdatabase.GetCarbEntriesAsync();
            carbEntries = carbEntries.Where(ent => ent.DateOfInput.Day == Day);
            CarbEntries.AddRange(carbEntries.Reverse());
            IsBusy = false;
        }
    }
}
