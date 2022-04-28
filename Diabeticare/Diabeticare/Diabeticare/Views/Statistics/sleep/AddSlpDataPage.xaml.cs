using Diabeticare.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddSlpDataPage : ContentPage
    {
        public AddSlpDataPage()
        {
            InitializeComponent();
            BindingContext = new SlpViewModel();
        }
    }
}