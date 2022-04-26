using Diabeticare.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarbEntriesPage : ContentPage
    {
        public CarbEntriesPage()
        {
            InitializeComponent();
            BindingContext = new CarbViewModel();
        }
    }
}