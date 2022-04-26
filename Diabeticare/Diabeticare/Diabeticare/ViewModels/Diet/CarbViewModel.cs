using System;
using System.Collections.Generic;
using System.Text;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using Diabeticare.Models;

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
        }

        string carbEntry;
        public string CarbEntry
        {
            get => carbEntry;
            set => SetProperty(ref carbEntry, value);
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

        public async void AddCarb()
        {
            // TODO: display text if the entry field is empty
            if (CarbEntry == null)
                return;

            var carbohydrates = float.Parse(CarbEntry);

            DateTime dateOfInput = CarbDate.Date.Add(CarbTime);

            // Asks server to add entry
            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateCIAsync(carbohydrates, dateOfInput);

            if (code == 1)
                await App.Bdatabase.AddBglEntryAsync(carbohydrates, dateOfInput, server_id);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await Shell.Current.GoToAsync("..");
        }
    }
}
