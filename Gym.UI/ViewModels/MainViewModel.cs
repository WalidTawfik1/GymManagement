using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Core.Interfaces;
using Gym.UI.Services;

namespace Gym.UI.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;

        public MainViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            UpdateLocalizedLabels();
            
            // Initialize ViewModels
            TraineeViewModel = new TraineeViewModel(_unitOfWork, _mapper, _localizationService);
            MembershipViewModel = new MembershipViewModel(_unitOfWork, _mapper, _localizationService);
            VisitViewModel = new VisitViewModel(_unitOfWork, _mapper, _localizationService);
            
            // Set default active view
            CurrentViewModel = TraineeViewModel;

            // Subscribe to language changes
            _localizationService.LanguageChanged += OnLanguageChanged;
        }

        [ObservableProperty]
        private BaseViewModel _currentViewModel;

        // Localized Labels
        [ObservableProperty]
        private string _languageLabel = string.Empty;
        
        [ObservableProperty]
        private string _arabicLabel = string.Empty;
        
        [ObservableProperty]
        private string _englishLabel = string.Empty;
        
        [ObservableProperty]
        private string _traineesLabel = string.Empty;
        
        [ObservableProperty]
        private string _membershipsLabel = string.Empty;
        
        [ObservableProperty]
        private string _visitsLabel = string.Empty;

        public TraineeViewModel TraineeViewModel { get; }
        public MembershipViewModel MembershipViewModel { get; }
        public VisitViewModel VisitViewModel { get; }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            // Update title when language changes
            UpdateLocalizedLabels();
            OnPropertyChanged(nameof(LocalizationService));
        }

        private void UpdateLocalizedLabels()
        {
            Title = GetLocalizedString("AppTitle");
            LanguageLabel = GetLocalizedString("Language");
            ArabicLabel = GetLocalizedString("Arabic");
            EnglishLabel = GetLocalizedString("English");
            TraineesLabel = GetLocalizedString("Trainees");
            MembershipsLabel = GetLocalizedString("Memberships");
            VisitsLabel = GetLocalizedString("Visits");
        }

        protected override void OnLanguageChanged()
        {
            UpdateLocalizedLabels();
        }

        [RelayCommand]
        private void ShowTrainees()
        {
            CurrentViewModel = TraineeViewModel;
        }

        [RelayCommand]
        private void ShowMemberships()
        {
            CurrentViewModel = MembershipViewModel;
        }

        [RelayCommand]
        private void ShowVisits()
        {
            CurrentViewModel = VisitViewModel;
        }

        [RelayCommand]
        private void SetLanguage(string languageCode)
        {
            _localizationService.SetLanguage(languageCode);
        }
    }
}
