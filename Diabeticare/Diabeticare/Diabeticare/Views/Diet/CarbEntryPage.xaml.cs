using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diabeticare.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(CarbID), nameof(CarbID))] // Used for passing selected carb ID to a different page during navigation
    public partial class CarbEntryPage : ContentPage
    {

        public string CarbID { get; set; }
        CarbohydrateModel carbEntry;
        public CarbEntryPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(CarbID, out var pRes);

            carbEntry = await App.Cdatabase.GetCarbEntryAsync(pRes);
            entryCarb.Text = $"{carbEntry.Carbohydrates}";
            entryFood.Text = carbEntry.FoodName;
            timeSelector.Time = carbEntry.DateOfInput.TimeOfDay;
            dateSelector.Date = carbEntry.DateOfInput.Date;
        }
        private async void SaveUpdatedCarbEntry(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entryCarb.Text) || string.IsNullOrEmpty(entryFood.Text))
            {
                await DisplayAlert("Warning", "Field cannot be empty.", "OK");
                return;
            }

            var carbohydrates = float.Parse(entryCarb.Text);
            string foodname = entryFood.Text;
            DateTime dateOfInput = dateSelector.Date.Add(timeSelector.Time);

            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateCIAsync(carbohydrates, foodname, dateOfInput, carbEntry.ServerID);
            if (code == 1)
            {
                await App.Cdatabase.UpdateCarbEntryAsync(carbEntry, carbohydrates, dateOfInput, foodname, server_id);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");
            }

            await Shell.Current.GoToAsync("..");
        }

        private async void CancelUpdate(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}