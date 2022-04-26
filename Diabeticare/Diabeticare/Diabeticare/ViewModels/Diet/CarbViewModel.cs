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
        public Command AddCarbCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<CarbohydrateModel> DeleteCarbCommand { get; }
        public AsyncCommand<object> SelectedCarbCommand { get; }
        public AsyncCommand DisplayEntries { get; }


        public CarbViewModel()
        {
            CarbEntries = new ObservableRangeCollection<CarbohydrateModel>();
            AddCarbCommand = new Command(AddCarb);
            RefreshCommand = new AsyncCommand(ViewRefresh);
            DeleteCarbCommand = new AsyncCommand<CarbohydrateModel>(DeleteCarb);
            SelectedCarbCommand = new AsyncCommand<object>(SelectedEntry);
            DisplayEntries = new AsyncCommand(LoadCarbEntries);
            carbTime = DateTime.Now.TimeOfDay;
            carbDate = DateTime.Now.Date;
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

        CarbohydrateModel selectedCarb;
        public CarbohydrateModel SelectedCarb
        {
            get => selectedCarb;
            set => SetProperty(ref selectedCarb, value);
        }

        public async void AddCarb()
        {
            // TODO: display text if the entry field is empty
            if (CarbEntry == null)
                return;

            var carbohydrates = float.Parse(CarbEntry);
            string foodname = FoodName;
            DateTime dateOfInput = CarbDate.Date.Add(CarbTime);

            // Asks server to add entry
            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateCIAsync(carbohydrates, foodname, dateOfInput);

            if (code == 1)
                await App.Cdatabase.AddCarbEntryAsync(carbohydrates, dateOfInput, foodname, server_id);
            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await Shell.Current.GoToAsync("..");
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

            if (code == 1)
                await App.Cdatabase.DeleteCarbEntryAsync(carb.ID);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await ViewRefresh();
        }

        async Task SelectedEntry(object arg)
        {
            CarbohydrateModel carb = arg as CarbohydrateModel;
            if (carb == null) return;

            SelectedCarb = null; // Deselect item
            CarbEntries.Clear();
            var route = $"{nameof(CarbEntryPage)}?CarbID={carb.ID}";
            await Shell.Current.GoToAsync(route);
        }

        // Loads carb entries
        async Task LoadCarbEntries()
        {
            IsBusy = true;
            CarbEntries.Clear();
            var carbEntries = await App.Cdatabase.GetCarbEntriesAsync();
            CarbEntries.AddRange(carbEntries.Reverse());
            IsBusy = false;
        }
    }
}
