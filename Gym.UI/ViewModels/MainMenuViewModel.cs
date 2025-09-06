using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.UI.Services;
using System;

namespace Gym.UI.ViewModels
{
    public partial class MainMenuViewModel : BaseViewModel
    {
        public event Action<string>? NavigationRequested;

        [ObservableProperty]
        private string _traineeManagementTitle = string.Empty;
        
        [ObservableProperty]
        private string _membershipManagementTitle = string.Empty;
        
        [ObservableProperty]
        private string _visitManagementTitle = string.Empty;
        
        [ObservableProperty]
        private string _traineeManagementDesc = string.Empty;
        
        [ObservableProperty]
        private string _membershipManagementDesc = string.Empty;
        
        [ObservableProperty]
        private string _visitManagementDesc = string.Empty;

        public MainMenuViewModel(ILocalizationService localizationService) : base(localizationService)
        {
            Title = GetLocalizedString("MainMenu");
            UpdateLocalizedLabels();
            
            // Subscribe to language changes
            _localizationService.LanguageChanged += OnLanguageChangedEvent;
        }

        private void OnLanguageChangedEvent(object? sender, EventArgs e)
        {
            UpdateLocalizedLabels();
        }

        private void UpdateLocalizedLabels()
        {
            TraineeManagementTitle = GetLocalizedString("TraineeManagementTitle");
            MembershipManagementTitle = GetLocalizedString("MembershipManagementTitle");
            VisitManagementTitle = GetLocalizedString("VisitManagementTitle");
            TraineeManagementDesc = GetLocalizedString("TraineeManagementDesc");
            MembershipManagementDesc = GetLocalizedString("MembershipManagementDesc");
            VisitManagementDesc = GetLocalizedString("VisitManagementDesc");
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
            UpdateLocalizedLabels();
        }
    }
}
