using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Diabeticare.Views;
using Diabeticare.Models;
using MvvmHelpers.Commands;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using MvvmHelpers;

namespace Diabeticare.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        // Variables
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        public User selectedUser;
        public User SelectedUser
        {
            get => selectedUser;
            set => SetProperty(ref selectedUser, value);
        }

        // Commands
        public AsyncCommand RegisterCommand { get; }
        public AsyncCommand LoginCommand { get; }
        public AsyncCommand LogoutCommand { get; }
        public AsyncCommand<object> SelectedUserCommand { get; }
        public AsyncCommand<User> DeleteUserCommand { get; }
        public AsyncCommand DisplayEntries { get; }

        // User list
        public ObservableRangeCollection<User> UsrEntries { get; set; }

        // Constructor
        public UserViewModel()
        {
            // User list
            UsrEntries = new ObservableRangeCollection<User>();

            // Commands
            RegisterCommand = new AsyncCommand(Register);
            LoginCommand = new AsyncCommand(Login);
            LogoutCommand = new AsyncCommand(Logout);
            SelectedUserCommand = new AsyncCommand<object>(SelectedEntry);
            DeleteUserCommand = new AsyncCommand<User>(DeleteUser);
            DisplayEntries = new AsyncCommand(LoadUserEntries);
        }

        // REGISTER
        public async Task Register()
        {
            string username = Username;
            string email = Email;
            string password = Password;
            string confirmPassword = ConfirmPassword;

            if (password.Equals(confirmPassword))
            {
                string passwordHash = ComputeSHA256Hash(password);
                string confirmPasswordHash = ComputeSHA256Hash(confirmPassword);

                // Add locally
                await App.Udatabase.AddUserEntryAsync(username, email);

                // Add to server
                (int code, string message) = await App.apiServices.RegisterAsync(username, email, passwordHash, confirmPasswordHash);
                if (code == 1)
                {
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Passwords do not match", "Ok");
            }
        }

        // LOGIN
        private async Task _Login(string username, string password)
        {
            (int code, string message) = await App.apiServices.LoginAsync(username, password);
            if (code == 1)
            {
                // If user choose to remember password
                if (IsChecked)
                    await App.Udatabase.UpdateUserEntryAsync(App.user, password, IsChecked);

                // Redirect user to default page
                App.Current.MainPage = new AppShell();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");
                await LoadUserEntries();
            }
        }

        public async Task Login()
        {
            string username = Username;
            string password = Password;

            string passwordHash = ComputeSHA256Hash(password);
            await _Login(username, passwordHash);
        }

        // Refresh the user listview
        async Task ViewRefresh()
        {
            IsBusy = true;
            UsrEntries.Clear();
            var usrEntries = await App.Udatabase.GetUserEntriesAsync();
            UsrEntries.AddRange(usrEntries);
            IsBusy = false;
        }

        // Remove password from specified user entry
        async Task DeleteUser(User user)
        {
            await App.Udatabase.UpdateUserEntryAsync(user, null, false);
            await ViewRefresh();
        }

        async Task SelectedEntry(object arg)
        {
            User usr = arg as User;
            if (usr == null) return;

            await _Login(usr.Username, usr.Password);
        }

        // Loads User entries
        async Task LoadUserEntries()
        {
            IsBusy = true;
            UsrEntries.Clear();
            var userEntries = await App.Udatabase.GetUserEntriesAsync();
            UsrEntries.AddRange(userEntries);
            IsBusy = false;
        }

        // LOGOUT
        public async Task Logout()
        {
            // Tell server user logs out
            await App.apiServices.LogoutAsync();

            // Set user to null (logged out)
            App.user = null;

            // Redirect to login page
            App.Current.MainPage = new LoginShell();
        }

        // HELPER FUNCTIONS
        private static string ComputeSHA256Hash(string input)
        {
            // Computes the SHA256 hash of the input and returns it as a string

            byte[] data = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();
            foreach (byte b in data)
            {
                sBuilder.Append(b.ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
