using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.ViewModels;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddExerciseDataPage : ContentPage
    {
        public AddExerciseDataPage(string exerciseName)
        {
            InitializeComponent();
            BindingContext = new ExerciseViewModel(0, exerciseName);
        }
    }
}