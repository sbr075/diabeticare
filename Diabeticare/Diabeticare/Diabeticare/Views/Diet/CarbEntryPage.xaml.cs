using System;
using Diabeticare.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarbEntryPage : ContentPage
    {
        public int CarbID { get; set; }
        CarbohydrateModel carbEntry;
        public CarbEntryPage(int carbID)
        {
            InitializeComponent();
            CarbID = carbID;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            carbEntry = await App.Cdatabase.GetCarbEntryAsync(CarbID);
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

            await App.Current.MainPage.Navigation.PopAsync();
        }

        private async void CancelUpdate(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }
    }
}