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
    public partial class AddBglDataPage : ContentPage
    {
        public AddBglDataPage()
        {
            InitializeComponent();
        }

        private void SaveBglData(object sender, EventArgs e)
        {
            /* Save entry (dataField.Text) to database */
        }


        // Called on "TextChanged" instead of "Completed" to avoid confusion for the user.
        private void Bgl_Entry_Changed(object sender, EventArgs e)
        {
            bool emptyEntry = string.IsNullOrEmpty(dataField.Text);
            if (!emptyEntry)
            {
                addData.IsEnabled = true;
            }
        }
    }
}