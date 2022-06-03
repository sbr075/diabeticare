using MvvmHelpers;
using System;
using Diabeticare.Models;
using MvvmHelpers.Commands;
using Command = MvvmHelpers.Commands.Command;
using Diabeticare.Views;
using System.Threading.Tasks;
using System.Linq;

namespace Diabeticare.ViewModels
{
    public class ExerciseViewModel : ViewModelBase
    {
        // Data collection of exercise entries
        public ObservableRangeCollection<ExerciseModel> ExerciseEntries { get; set; }
        public ObservableRangeCollection<GroupModel> ExerciseGroups { get; set; }

        public Command AddExerciseCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<ExerciseModel> DeleteExerciseCommand { get; }
        public AsyncCommand<object> SelectedExerciseGroupCommand { get; }
        public AsyncCommand<object> SelectedExerciseCommand { get; }
        public AsyncCommand DisplayGroupsCommand { get; }
        public AsyncCommand DisplayEntriesCommand { get; }

        public ExerciseViewModel(int day = 0, string exerciseName = "Walking")
        {
            ExerciseEntries = new ObservableRangeCollection<ExerciseModel>();
            ExerciseGroups = new ObservableRangeCollection<GroupModel>();

            AddExerciseCommand = new Command(AddExercise);
            RefreshCommand = new AsyncCommand(ViewRefresh);
            DeleteExerciseCommand = new AsyncCommand<ExerciseModel>(DeleteExercise);
            SelectedExerciseGroupCommand = new AsyncCommand<object>(SelectedGroup);
            SelectedExerciseCommand = new AsyncCommand<object>(SelectedEntry);
            DisplayGroupsCommand = new AsyncCommand(LoadExerciseGroups);
            DisplayEntriesCommand = new AsyncCommand(LoadExerciseEntries);

            ExerciseName = exerciseName;
            ExerciseStart = DateTime.Today;
            ExerciseEnd = DateTime.Today;
            Day = day;
        }

        GroupModel selectedExerciseGroup;
        public GroupModel SelectedExerciseGroup
        {
            get => selectedExerciseGroup;
            set => SetProperty(ref selectedExerciseGroup, value);
        }

        ExerciseModel selectedExercise;
        public ExerciseModel SelectedExercise
        {
            get => selectedExercise;
            set => SetProperty(ref selectedExercise, value);
        }

        string exerciseName;
        public String ExerciseName
        {
            get => exerciseName;
            set => SetProperty(ref exerciseName, value);
        }

        DateTime exerciseStart;
        public DateTime ExerciseStart
        {
            get => exerciseStart;
            set => SetProperty(ref exerciseStart, value);
        }

        DateTime exerciseEnd;
        public DateTime ExerciseEnd
        {
            get => exerciseEnd;
            set => SetProperty(ref exerciseEnd, value);
        }

        TimeSpan exerciseTimeStart;
        public TimeSpan ExerciseTimeStart
        {
            get => exerciseTimeStart;
            set => SetProperty(ref exerciseTimeStart, value);
        }

        TimeSpan exerciseTimeEnd;
        public TimeSpan ExerciseTimeEnd
        {
            get => exerciseTimeEnd;
            set => SetProperty(ref exerciseTimeEnd, value);
        }

        int day;
        public int Day
        {
            get => day;
            set => SetProperty(ref day, value);
        }

        // Creates a new exercise entry
        public async void AddExercise()
        {
            // Check if user is trying to time travel
            if ((ExerciseTimeStart > ExerciseTimeEnd && exerciseStart >= exerciseEnd) || exerciseStart > exerciseEnd)
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Invalid exercise entry.", "OK");
                return;
            }

            // Parse data
            DateTime start = ExerciseStart.Date.Add(ExerciseTimeStart);
            DateTime end = ExerciseEnd.Date.Add(ExerciseTimeEnd);

            // Asks server to add entry
            (int code, string message, int server_id) = await App.apiServices.AddOrUpdateExerciseAsync(ExerciseName, start, end);

            if (code == 1) // Successfull request
                await App.Edatabase.AddExerciseEntryAsync(ExerciseName, start, end, server_id);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await App.Current.MainPage.Navigation.PopAsync();
        }

        // Refresh the exercise listview
        async Task ViewRefresh()
        {
            IsBusy = true;
            ExerciseEntries.Clear();
            var exerciseEntries = await App.Edatabase.GetExerciseEntriesAsync(ExerciseName);
            ExerciseEntries.AddRange(exerciseEntries.Reverse());
            IsBusy = false;
        }

        // Delete specified exercise entry
        async Task DeleteExercise(ExerciseModel exercise)
        {
            // Asks server to delete entry
            (int code, string message) = await App.apiServices.DeleteExerciseAsync(exercise.ServerID);

            if (code == 1) // Successfull request
                await App.Edatabase.DeleteExerciseEntryAsync(exercise.ID);

            else
                await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");

            await ViewRefresh();
        }

        async Task SelectedGroup(object arg)
        {
            // Convert object to GroupModel object
            GroupModel exerciseGroup = arg as GroupModel;
            if (exerciseGroup == null) return;

            SelectedExerciseGroup = null;
            ExerciseGroups.Clear(); // Temp fix to not load listview twice after coming back from BglEntryPage
            await App.Current.MainPage.Navigation.PushAsync(new EditExercisePage(exerciseGroup.GroupDate.Day, ExerciseName));
        }

        async Task SelectedEntry(object arg)
        {
            // Convert object to ExerciseModel object
            ExerciseModel exercise = arg as ExerciseModel;
            if (exercise == null) return;

            SelectedExercise = null; // Deselect item
            ExerciseEntries.Clear(); // Temp fix to not load listview twice after coming back from ExerciseEntryPage
            await App.Current.MainPage.Navigation.PushAsync(new ExerciseEntryPage(exercise.ID));
        }

        async Task LoadExerciseGroups()
        {
            // Fetches all distinct days of when exercise entries were registered
            // Distinguishes between exercises which started and ended on different days
            // (counts as one exercise, but seperates hours to both days)
            IsBusy = true;
            ExerciseGroups.Clear();
            var exerciseEntries = await App.Edatabase.GetExerciseEntriesAsync(ExerciseName);

            var exerciseStartDays = exerciseEntries.Select(ent => ent.ExerciseStart.Date.Day);
            var exerciseEndDays = exerciseEntries.Select(ent => ent.ExerciseEnd.Date.Day);

            var exerciseAllDays = exerciseStartDays.Concat(exerciseEndDays);
            var distinctDays = exerciseAllDays.Distinct().OrderByDescending(ent => ent);

            foreach (var day in distinctDays)
            {
                var allGroupExercise = exerciseEntries.Where(ent => ent.ExerciseStart.Day == day || ent.ExerciseEnd.Day == day);

                TimeSpan totalExercise;
                foreach (var group in allGroupExercise)
                {
                    // If Exercise start and exercise end is not on the same day
                    if (group.ExerciseStart.Day != group.ExerciseEnd.Day)
                    {
                        // If current day is looking at exercise start days
                        if (group.ExerciseStart.Day == day)
                        {
                            DateTime endOfDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, group.ExerciseStart.Day, 23, 59, 59);
                            totalExercise += endOfDay - group.ExerciseStart;
                        }
                        // Current day is looking at exercise end days
                        else
                        {
                            DateTime startOfDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, group.ExerciseEnd.Day);
                            totalExercise += group.ExerciseEnd - startOfDay;
                        }
                    }
                    else
                    {
                        totalExercise += (group.ExerciseEnd - group.ExerciseStart);
                    }
                }

                ExerciseGroups.Add(new GroupModel { GroupDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, day), GroupDuration = totalExercise });
            }

            IsBusy = false;
        }

        // Loads exercise entries
        async Task LoadExerciseEntries()
        {
            // Loads ell exercise entries for that given day
            IsBusy = true;
            ExerciseEntries.Clear();
            var exerciseEntries = await App.Edatabase.GetExerciseEntriesAsync(ExerciseName);
            exerciseEntries = exerciseEntries.Where(ent => ent.ExerciseStart.Day == Day || ent.ExerciseEnd.Day == Day);
            ExerciseEntries.AddRange(exerciseEntries.Reverse());
            IsBusy = false;
        }
    }
}
