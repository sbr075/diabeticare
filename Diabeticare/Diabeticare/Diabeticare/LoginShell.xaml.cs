using Diabeticare.Views;
using Xamarin.Forms;

namespace Diabeticare
{
    public partial class LoginShell : Shell
    {
        public LoginShell()
        {
            InitializeComponent();

            // User pages
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
        }
    }
}