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
            
            _selectedMonth = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingMonth();
            _selectedYear = Gym.Core.Helpers.AccountingDateHelper.GetCurrentAccountingYear();
        }

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private int _selectedMonth;

        [ObservableProperty]
        private int _selectedYear;

        public List<KeyValuePair<int, string>> Months { get; } = new()
        {
            new(1, "يناير"), new(2, "فبراير"), new(3, "مارس"), new(4, "أبريل"),
            new(5, "مايو"), new(6, "يونيو"), new(7, "يوليو"), new(8, "أغسطس"),
            new(9, "سبتمبر"), new(10, "أكتوبر"), new(11, "نوفمبر"), new(12, "ديسمبر")
        };

        public List<int> Years { get; } = System.Linq.Enumerable.Range(2020, 31).ToList();

        partial void OnSelectedMonthChanged(int value)
        {
            OnMonthChanged(value);
        }

        partial void OnSelectedYearChanged(int value)
        {
            OnYearChanged(value);
        }

        protected virtual void OnMonthChanged(int value) { }
        protected virtual void OnYearChanged(int value) { }

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
