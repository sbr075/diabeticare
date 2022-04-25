﻿using System;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Xamarin.Forms;
using Command = MvvmHelpers.Commands.Command;
using System.Collections.Generic;
using System.Text;
using Diabeticare.Views;

namespace Diabeticare.ViewModels
{
    public class ViewModelBase : BaseViewModel
    {
        public Command InfoCommand { get; }
        public Command GoBackCommand { get; }
        public Command SettingsCommand { get; }
        public Command AboutCommand { get; }
        public Command DeleteAccountCommand { get; }
        public Command DeleteDataCommand { get; }
        public Command LogoutCommand { get; }


        public ViewModelBase()
        {
            GoBackCommand = new Command(GoBackOnePage);
            InfoCommand = new Command(DisplayInfo);
            SettingsCommand = new Command(DisplaySettings);
            DeleteAccountCommand = new Command(DeleteAccount);
            DeleteDataCommand = new Command(DeleteData);
            AboutCommand = new Command(DisplayAbout);
            LogoutCommand = new Command(Logout);
        }

        public async void GoBackOnePage()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async void DisplayInfo()
        {
            await Shell.Current.GoToAsync(nameof(InfoPage));
        }

        public async void DisplaySettings()
        {
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }

        public async void DisplayAbout()
        {
            await Shell.Current.GoToAsync(nameof(AboutPage));
        }

        public async void DeleteAccount()
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to delete your account?", "Yes", "No");
            if(answer)
            {
                // Delete account (Sondre pls fix)
                await App.Current.MainPage.DisplayAlert("Notice", "Your account has been deleted.", "OK");
            }
        }

        public async void DeleteData()
        {
            bool answer =  await App.Current.MainPage.DisplayAlert("Warning", "Are you sure you want to delete all your data?", "Yes", "No");
            if (answer)
            {
                // Delete all data (Sondre pls fix)
                await App.Current.MainPage.DisplayAlert("Notice", "All your data has been deleted.", "OK");
            }
        }

        public async void Logout()
        {
            // Move code here
            await App.Current.MainPage.DisplayAlert("Notice", "You have been logged out.", "OK");
        }
    }
}
