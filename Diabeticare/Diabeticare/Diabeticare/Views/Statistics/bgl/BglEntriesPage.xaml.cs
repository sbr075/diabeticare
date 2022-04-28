using Diabeticare.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BglEntriesPage : ContentPage
    {
        public BglEntriesPage()
        {
            InitializeComponent();
            BindingContext = new BglViewModel();
        }
    }
}