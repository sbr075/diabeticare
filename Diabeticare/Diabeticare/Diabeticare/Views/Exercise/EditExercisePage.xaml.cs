using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Diabeticare.ViewModels;

namespace Diabeticare.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditExercisePage : ContentPage
    {
        public EditExercisePage(int day, string exerciseName)
        {
            InitializeComponent();
            BindingContext = new ExerciseViewModel(day, exerciseName);
        }
    }
}