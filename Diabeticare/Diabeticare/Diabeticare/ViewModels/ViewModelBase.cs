using System;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Xamarin.Forms;
using Command = MvvmHelpers.Commands.Command;
using System.Collections.Generic;
using System.Text;
using Diabeticare.Views;

namespace Diabeticare.ViewModels
{
    public class ViewModelBase : BaseViewModel
    {
        public Command InfoCommand { get; }
        public Command GoBackCommand { get; }
        public Command SettingsCommand { get; }
        public Command AboutCommand { get; }
        public Command DeleteAccountCommand { get; }
        public Command DeleteDataCommand { get; }
        public Command LogoutCommand { get; }


        public ViewModelBase()
        {
            GoBackCommand = new Command(GoBackOnePage);
            InfoCommand = new Command(DisplayInfo);
            SettingsCommand = new Command(DisplaySettings);
            DeleteAccountCommand = new Command(DeleteAccount);
            DeleteDataCommand = new Command(DeleteData);
            AboutCommand = new Command(DisplayAbout);
            LogoutCommand = new Command(Logout);
        }

        public async void GoBackOnePage()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        public async void DisplayInfo()
        {
            await App.Current.MainPage.Navigation.PushAsync(new InfoPage());
        }

        public async void DisplaySettings()
        {
            await App.Current.MainPage.Navigation.PushAsync(new SettingsPage());
        }

        public async void DisplayAbout()
        {
            await App.Current.MainPage.Navigation.PushAsync(new AboutPage());
        }

        private async void DeleteAllDataHelper()
        {
            await App.Bdatabase.DeleteUserBglEntriesAsync();
            await App.Sdatabase.DeleteUserSlpEntriesAsync();
            await App.Cdatabase.DeleteUserCarbEntriesAsync();
        }

        public async void DeleteAccount()
        {
            string result = await App.Current.MainPage.DisplayPromptAsync("Warning", "Are you sure you want to delete your account? Type 'Delete' to confirm");

            if (result == "Delete")
            {
                // Asks server to add entry
                (int code, string message) = await App.apiServices.DeleteAccount();

                if (code == 1)
                {
                    DeleteAllDataHelper();
                    await App.Udatabase.DeleteUserEntryAsync();
                    await App.Current.MainPage.DisplayAlert("Notice", "Your account has been deleted.", "OK");

                    App.user = null;
                    await App.Current.MainPage.Navigation.PushAsync(new LoginPage());
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");
                }
            }
        }

        public async void DeleteData()
        {
            string result = await App.Current.MainPage.DisplayPromptAsync("Warning", "Are you sure you want to delete all your data? Type 'Delete' to confirm");

            if (result == "Delete")
            {
                // Asks server to add entry
                (int code, string message) = await App.apiServices.DeleteAllData();

                if (code == 1)
                {
                    DeleteAllDataHelper();
                    await App.Current.MainPage.DisplayAlert("Notice", "All your data has been deleted.", "OK");
                    await App.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");
                }
            }
        }

        public async void Logout()
        {
            // Tell server user logs out
            await App.apiServices.LogoutAsync();

            // Set user to null (logged out)
            App.user = null;

            // Redirect to login page
            App.Current.MainPage = new LoginShell();
        }
    }
}
