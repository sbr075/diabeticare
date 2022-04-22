using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Diabeticare.Views;
using MvvmHelpers.Commands;
using System.Security.Cryptography;
using Xamarin.Forms;


namespace Diabeticare.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    // ViewModel for the registration process. This is triggered by the user clicking the button the RegisterPage. 
    // Utilizes the Api_services to asynchrously send an HTTP-Post with the information to the server. 
    // TODO: This might not be secure enough. 
    {
        public AsyncCommand RegisterCommand { get; }
        
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public RegisterViewModel()
        {
            RegisterCommand = new AsyncCommand(Register);
        }

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
                HttpResponseMessage response = await App.apiServices.RegisterAsync(username, email, passwordHash, confirmPasswordHash);
                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Invalid Credentials", "Ok");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Passwords do not match", "Ok");
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
    }
}
