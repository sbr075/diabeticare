using Diabeticare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(BglID), nameof(BglID))] // Used for passing selected BGL ID to a different page during navigation
    public partial class BglEntryPage : ContentPage
    {
        public string BglID { get; set; }
        Bgl bglEntry;
        public BglEntryPage()
        {
            InitializeComponent();
            
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(BglID, out var pRes);

            // Change this later
            bglEntry = await App.Bdatabase.GetBglEntryAsync(pRes);
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
            
            await Shell.Current.GoToAsync("..");
        }
        private async void CancelUpdate(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
        
    }
}