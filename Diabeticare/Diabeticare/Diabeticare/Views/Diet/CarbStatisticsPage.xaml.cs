using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarbStatisticsPage : ContentPage
    {
        public CarbStatisticsPage()
        {
            InitializeComponent();
        }

        // Go to AddCarbDataPage
        private async void Create_Carb_Data(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AddCarbDataPage));
        }
    }
}