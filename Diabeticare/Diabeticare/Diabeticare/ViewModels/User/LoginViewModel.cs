using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Diabeticare.Views;
using Diabeticare.Models;
using MvvmHelpers.Commands;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using MvvmHelpers;

namespace Diabeticare.ViewModels
{
    public class LoginViewModel : ViewModelBase
    // ViewModel for the registration process. This is triggered by the user clicking the button the RegisterPage. 
    // Utilizes the Api_services to asynchrously send an HTTP-Post with the information to the server. 
    // TODO: This might not be secure enough. 
    {
        public ObservableRangeCollection<User> UsrEntries { get; set; }

        public AsyncCommand LoginCommand { get; }
        public AsyncCommand<object> SelectedUserCommand { get; }
        public AsyncCommand<User> DeleteUserCommand { get; }
        public AsyncCommand DisplayEntries { get; }

        public string Username { get; set; }
        public string Password { get; set; }
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

        public LoginViewModel()
        {
            UsrEntries = new ObservableRangeCollection<User>();
            LoginCommand = new AsyncCommand(Login);
            SelectedUserCommand = new AsyncCommand<object>(SelectedEntry);
            DeleteUserCommand = new AsyncCommand<User>(DeleteUser);
            DisplayEntries = new AsyncCommand(LoadUserEntries);
        }

        public async Task Login()
        {
            string username = Username;
            string password = Password;

            string passwordHash = ComputeSHA256Hash(password);
            await _Login(username, passwordHash);
        }

        private async Task _Login(string username, string password)
        {
            HttpResponseMessage response = await App.apiServices.LoginAsync(username, password);
            if (response.IsSuccessStatusCode)
            {
                App.user = await App.Udatabase.GetUserEntryAsync(username);

                string responseBody = await response.Content.ReadAsStringAsync();
                var token = (string)JObject.Parse(responseBody)["X-CSRFToken"];

                // If user choose to remember password
                if (IsChecked)
                    await App.Udatabase.UpdateUserEntryAsync(App.user, password, IsChecked);

                // Update token in database for user
                await App.Udatabase.UpdateUserTokenAsync(App.user, token);

                // Redirect user to default page
                await Shell.Current.GoToAsync(nameof(DefaultPage));
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Invalid Credentials", "Ok");
            }
        }

        private static string ComputeSHA256Hash(string input)
        {
            byte[] data = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();
            foreach (byte b in data)
            {
                sBuilder.Append(b.ToString("x2"));
            }

            return sBuilder.ToString();
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
    }
}