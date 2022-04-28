using MvvmHelpers;
using Command = MvvmHelpers.Commands.Command;
using Diabeticare.Views;

namespace Diabeticare.ViewModels
{
    public class ViewModelBase : BaseViewModel
    {
        public Command InfoCommand { get; }
        public Command GoBackCommand { get; }
        public Command SettingsCommand { get; }
        public Command AboutCommand { get; }
        public Command LogoutCommand { get; }


        public ViewModelBase()
        {
            GoBackCommand = new Command(GoBackOnePage);
            InfoCommand = new Command(DisplayInfo);
            SettingsCommand = new Command(DisplaySettings);
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
