using System.Threading.Tasks;
using Diabeticare.Services;
using MvvmHelpers.Commands;

namespace Diabeticare.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    // ViewModel for the registration process. This is triggered by the user clicking the button the RegisterPage. 
    // Utilizes the Api_services to asynchrously send an HTTP-Post with the information to the server. 
    // TODO: This might not be secure enough. 
    {
        ApiServices apiServices = new ApiServices();
        public AsyncCommand RegisterCommand { get; }
        
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Message { get; set; }

        public RegisterViewModel()
        {
            RegisterCommand = new AsyncCommand(Register);
        }

        public async Task Register()
        {
            var username = Username;
            var email    = Email;
            var password = Password;
            var confirmPassword = ConfirmPassword;

            await apiServices.RegisterAsync(username, email, password, confirmPassword);
        }
    }
}
