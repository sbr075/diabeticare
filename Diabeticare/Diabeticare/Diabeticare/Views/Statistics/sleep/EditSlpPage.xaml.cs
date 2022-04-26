using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.ViewModels;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSlpPage : ContentPage
    {
        public EditSlpPage(int month)
        {
            InitializeComponent();
            BindingContext = new SlpViewModel(month);
        }
    }
}