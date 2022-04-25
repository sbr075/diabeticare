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

    [QueryProperty(nameof(SlpID), nameof(SlpID))] // Used for passing selected sleep-entry ID to a different page during navigation
    public partial class SlpEntryPage : ContentPage
    {
        public string SlpID { get; set; }
        SleepModel slpEntry;
        public SlpEntryPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(SlpID, out var pRes);
            BindingContext = await App.Sdatabase.GetSlpEntryAsync(pRes);

            slpEntry = await App.Sdatabase.GetSlpEntryAsync(pRes);
            dateStart.Date = slpEntry.SleepStart;
            dateEnd.Date = slpEntry.SleepEnd;
            timeStart.Time = slpEntry.SleepStart.TimeOfDay;
            timeEnd.Time = slpEntry.SleepEnd.TimeOfDay;
        }

        private async void SaveUpdatedSlpEntry(object sender, EventArgs e)
        {
            // Check if user is trying to time travel
            if ((timeStart.Time > timeEnd.Time && dateStart.Date >= dateEnd.Date) || dateStart.Date > dateEnd.Date)
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Invalid sleep entry.", "OK");
                return;
            }

            DateTime updatedStart = dateStart.Date.Add(timeStart.Time);
            DateTime updatedEnd = dateEnd.Date.Add(timeEnd.Time);

            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateSleepAsync(updatedStart, updatedEnd, slpEntry.ServerID);
            if (code == 1)
            {
                await App.Sdatabase.UpdateSlpEntryAsync(slpEntry, updatedStart, updatedEnd, server_id);
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