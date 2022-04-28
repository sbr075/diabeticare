using Diabeticare.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddBglDataPage : ContentPage
    {
        public AddBglDataPage()
        {
            InitializeComponent();
            BindingContext = new BglViewModel();
        }
    }
}