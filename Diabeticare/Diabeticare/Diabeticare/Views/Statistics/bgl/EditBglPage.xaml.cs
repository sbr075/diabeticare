using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.ViewModels;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditBglPage : ContentPage
    {
        public EditBglPage(int month)
        {
            InitializeComponent();
            BindingContext = new BglViewModel(month);
        }
    }
}