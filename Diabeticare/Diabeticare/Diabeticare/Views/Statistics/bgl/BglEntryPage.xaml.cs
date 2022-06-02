using Diabeticare.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BglEntryPage : ContentPage
    {
        public int BglID { get; set; }
        BglModel bglEntry;
        public BglEntryPage(int bglID)
        {
            InitializeComponent();
            BglID = bglID;
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Change this later
            bglEntry = await App.Bdatabase.GetBglEntryAsync(BglID);
            entryField.Text = $"{bglEntry.BGLmeasurement}";
            timeSelector.Time = bglEntry.TimeOfMeasurment.TimeOfDay;
            dateSelector.Date = bglEntry.TimeOfMeasurment.Date;
        }

        private async void SaveUpdatedBglEntry(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entryField.Text))
            {
                await DisplayAlert("Warning", "BGL field cannot be empty.", "OK");
                return;
            }

            var measurement = float.Parse(entryField.Text);
            DateTime timeOfMeasurment = dateSelector.Date.Add(timeSelector.Time);

            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateBGLAsync(measurement, timeOfMeasurment, bglEntry.ServerID);
            if (code == 1)
            {
                await App.Bdatabase.UpdateBglEntryAsync(bglEntry, float.Parse(entryField.Text), timeOfMeasurment, server_id);
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