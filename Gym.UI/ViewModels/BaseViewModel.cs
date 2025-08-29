using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.UI.Services;
using System.Windows;

namespace Gym.UI.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        protected readonly ILocalizationService _localizationService;

        protected BaseViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            _localizationService.LanguageChanged += OnLocalizationLanguageChanged;
        }

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        public ILocalizationService LocalizationService => _localizationService;

        public FlowDirection FlowDirection => _localizationService.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        [RelayCommand]
        private void ChangeLanguage(string languageCode)
        {
            _localizationService.SetLanguage(languageCode);
            OnLanguageChanged();
        }

        private void OnLocalizationLanguageChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(FlowDirection));
            OnLanguageChanged();
        }

        protected virtual void OnLanguageChanged()
        {
            // Override in derived classes to update UI text
        }

        protected string GetLocalizedString(string key)
        {
            return _localizationService.GetString(key);
        }
    }
}
