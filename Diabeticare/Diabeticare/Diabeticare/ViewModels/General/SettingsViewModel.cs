using Command = MvvmHelpers.Commands.Command;

namespace Diabeticare.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public Command DeleteAccountCommand { get; }
        public Command DeleteDataCommand { get; }

        public SettingsViewModel()
        {
            DeleteAccountCommand = new Command(DeleteAccount);
            DeleteDataCommand = new Command(DeleteData);
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
                    App.Current.MainPage = new LoginShell();
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
    }
}
