using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.ViewModels;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditCarbPage : ContentPage
    {
        public EditCarbPage(int day)
        {
            InitializeComponent();
            BindingContext = new CarbViewModel(day);
        }
    }
}