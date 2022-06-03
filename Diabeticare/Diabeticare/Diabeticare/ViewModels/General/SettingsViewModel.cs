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
            // Calls all databases and ask them to delete the data related to currently logged in user
            await App.Bdatabase.DeleteUserBglEntriesAsync();
            await App.Sdatabase.DeleteUserSlpEntriesAsync();
            await App.Cdatabase.DeleteUserCarbEntriesAsync();
            await App.Mdatabase.DeleteUserMoodEntriesAsync();
            await App.Edatabase.DeleteUserExerciseEntriesAsync();
        }

        public async void DeleteAccount()
        {
            // Ask the user if they really want to delete their account
            string result = await App.Current.MainPage.DisplayPromptAsync("Warning", "Are you sure you want to delete your account? Type 'Delete' to confirm");

            if (result == "Delete")
            {
                // Asks server to add entry
                (int code, string message) = await App.apiServices.DeleteAccount();

                if (code == 1) // Successfull request
                {
                    // Delete all data connected to user, and then user object
                    DeleteAllDataHelper();
                    await App.Udatabase.DeleteUserEntryAsync();
                    await App.Current.MainPage.DisplayAlert("Notice", "Your account has been deleted.", "OK");

                    // Logout user
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
            // Ask user if they really want to delete all their data
            string result = await App.Current.MainPage.DisplayPromptAsync("Warning", "Are you sure you want to delete all your data? Type 'Delete' to confirm");

            if (result == "Delete")
            {
                // Asks server to add entry
                (int code, string message) = await App.apiServices.DeleteAllData();

                if (code == 1) // Successfull request
                {
                    // Delete all data related to user
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
