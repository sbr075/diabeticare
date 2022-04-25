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
        BglModel bglEntry;
        public BglEntryPage()
        {
            InitializeComponent();
            
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(BglID, out var pRes);
            BindingContext = await App.Bdatabase.GetBglEntryAsync(pRes);
            
            // Change this later
            bglEntry = await App.Bdatabase.GetBglEntryAsync(pRes);
            entryField.Text = $"{bglEntry.BGLmeasurement}";
            timeSelector.Time = bglEntry.BGLtime;
        }

        private async void SaveUpdatedBglEntry(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entryField.Text))
            {
                await DisplayAlert("Warning", "BGL field cannot be empty.", "OK");
                return;
            }
            await App.Bdatabase.UpdateBglEntryAsync(bglEntry, float.Parse(entryField.Text), timeSelector.Time);
            await Shell.Current.GoToAsync("..");
        }
        private async void CancelUpdate(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
        
    }
}