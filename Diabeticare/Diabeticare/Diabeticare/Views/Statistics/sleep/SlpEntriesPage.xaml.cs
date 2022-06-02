using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.ViewModels;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlpEntriesPage : ContentPage
    {
        public SlpEntriesPage()
        {
            InitializeComponent();
            BindingContext = new SlpViewModel();
        }
    }
}