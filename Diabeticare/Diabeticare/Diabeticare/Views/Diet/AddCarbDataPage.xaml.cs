using Diabeticare.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCarbDataPage : ContentPage
    {
        public AddCarbDataPage()
        {
            InitializeComponent();
            BindingContext = new CarbViewModel();
        }

    }
}