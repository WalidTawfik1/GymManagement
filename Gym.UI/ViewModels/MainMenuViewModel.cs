using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.UI.Services;
using System;

namespace Gym.UI.ViewModels
{
    public partial class MainMenuViewModel : BaseViewModel
    {
        public event Action<string>? NavigationRequested;

        public MainMenuViewModel(ILocalizationService localizationService) : base(localizationService)
        {
            Title = GetLocalizedString("MainMenu");
        }

        [RelayCommand]
        private void ShowTrainees()
        {
            NavigationRequested?.Invoke("Trainees");
        }

        [RelayCommand]
        private void ShowMemberships()
        {
            NavigationRequested?.Invoke("Memberships");
        }

        [RelayCommand]
        private void ShowVisits()
        {
            NavigationRequested?.Invoke("Visits");
        }

        protected override void OnLanguageChanged()
        {
            Title = GetLocalizedString("MainMenu");
        }
    }
}
