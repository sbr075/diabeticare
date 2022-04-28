using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Diabeticare.Views;
using Diabeticare.Models;
using MvvmHelpers.Commands;
using Xamarin.Forms;
using MvvmHelpers;
using System.Linq;

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

        public UserModel selectedUser;
        public UserModel SelectedUser
        {
            get => selectedUser;
            set => SetProperty(ref selectedUser, value);
        }

        // Commands
        public AsyncCommand RegisterCommand { get; }
        public AsyncCommand LoginCommand { get; }
        public AsyncCommand<object> SelectedUserCommand { get; }
        public AsyncCommand<UserModel> DeleteUserCommand { get; }
        public AsyncCommand DisplayEntries { get; }

        // User list
        public ObservableRangeCollection<UserModel> UsrEntries { get; set; }

        // Constructor
        public UserViewModel()
        {
            // User list
            UsrEntries = new ObservableRangeCollection<UserModel>();

            // Commands
            RegisterCommand = new AsyncCommand(Register);
            LoginCommand = new AsyncCommand(Login);
            SelectedUserCommand = new AsyncCommand<object>(SelectedEntry);
            DeleteUserCommand = new AsyncCommand<UserModel>(DeleteUser);
            DisplayEntries = new AsyncCommand(LoadUserEntries);
        }

        // REGISTER
        private async Task<(bool, string)> validatePassword(string username, string password)
        {
            /*
             * Password requirements:
             * 1. Must atleast be eight (8) characters long
             * 2. Must contain atleast one (1) upper case letter
             * 3. Must contain atleast one (1) lower case letter
             * 4. Must contain atleast one (1) special character
             * 5. Must contain atleast one (1) number
             * 6. Password cannot contain username
             */

            // Check password length
            if (password.Length < 8)
                return (false, "Password needs to be atleast 8 characters long");

            if (password.Contains(username))
                return (false, "Password cannot contain username");

            // Check if password contains one special character
            if (password.All(char.IsLetterOrDigit))
                return (false, "Password needs to contain atleast 1 special character");

            // Check if password is only digits
            if (password.All(char.IsDigit))
                return (false, "Password needs to contain atleast 1 letter");

            // Check if passwor is only letters
            if (password.All(char.IsLetter))
                return (false, "Password needs to contain atleast 1 number");

            return (true, "Password valid");
        }

        public async Task Register()
        {
            if (Username == null || Email == null || Password == null || ConfirmPassword == null)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Fields cannot be left empty", "Ok");
                return;
            }

            string username = Username;
            string email = Email;
            string password = Password;
            string confirmPassword = ConfirmPassword;

            if (password.Equals(confirmPassword))
            {
                // Validate that passwords are complex enough
                (bool is_complex, string error_msg) = await validatePassword(username, password); 
                if (is_complex == false)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", error_msg, "Ok");
                    return;
                }

                string passwordHash = ComputeSHA256Hash(password);
                string confirmPasswordHash = ComputeSHA256Hash(confirmPassword);

                // Add to server
                (int code, string message) = await App.apiServices.RegisterAsync(username, email, passwordHash, confirmPasswordHash);
                if (code == 1)
                {
                    // Add locally
                    await App.Udatabase.AddUserEntryAsync(username, email);
                    await App.Current.MainPage.Navigation.PushAsync(new LoginPage());
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
            if (Username == null || Password == null)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Fields cannot be empty", "Ok");
                return;

            }
            string username = Username;
            string password = Password;

            string passwordHash = ComputeSHA256Hash(password);
            await _Login(username, passwordHash);
        }

        // Remove password from specified user entry
        async Task DeleteUser(UserModel user)
        {
            await App.Udatabase.UpdateUserEntryAsync(user, null, false);
            await LoadUserEntries();
        }

        async Task SelectedEntry(object arg)
        {
            UserModel usr = arg as UserModel;
            if (usr == null) return;

            IsChecked = true;
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
