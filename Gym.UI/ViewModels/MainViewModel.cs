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

        public MainViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService, Gym.UI.Services.Dialogs.IDialogService dialog) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            UpdateLocalizedLabels();
            
            // Initialize ViewModels
            MainMenuViewModel = new MainMenuViewModel(_localizationService);
            TraineeViewModel = new TraineeViewModel(_unitOfWork, _mapper, _localizationService, dialog);
            MembershipViewModel = new MembershipViewModel(_unitOfWork, _mapper, _localizationService, dialog);
            VisitViewModel = new VisitViewModel(_unitOfWork, _mapper, _localizationService, dialog);
            AdditionalServiceViewModel = new AdditionalServiceViewModel(_unitOfWork, _mapper, _localizationService, dialog);
            
            // Subscribe to menu navigation
            MainMenuViewModel.NavigationRequested += OnNavigationRequested;
            
            // Set default active view to menu
            CurrentViewModel = MainMenuViewModel;

            // Subscribe to language changes
            _localizationService.LanguageChanged += OnLanguageChanged;
        }

        // Called after construction (from App.xaml.cs) to load data sequentially and avoid parallel DbContext operations
        public async Task InitializeAsync()
        {
            // Load in sequence to prevent EF Core concurrency on a single scoped DbContext
            await TraineeViewModel.InitializeAsync();
            await MembershipViewModel.InitializeAsync();
            await VisitViewModel.InitializeAsync();
            await AdditionalServiceViewModel.InitializeAsync();
        }

        [ObservableProperty]
        private BaseViewModel _currentViewModel;

        [ObservableProperty]
        private bool _isNotMainMenu = false;

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
        
        [ObservableProperty]
        private string _additionalServicesLabel = string.Empty;

        public TraineeViewModel TraineeViewModel { get; }
        public MembershipViewModel MembershipViewModel { get; }
        public VisitViewModel VisitViewModel { get; }
        public AdditionalServiceViewModel AdditionalServiceViewModel { get; }
        public MainMenuViewModel MainMenuViewModel { get; }

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
            AdditionalServicesLabel = GetLocalizedString("AdditionalServices");
        }

        protected override void OnLanguageChanged()
        {
            UpdateLocalizedLabels();
        }

        [RelayCommand]
        private void ShowMainMenu()
        {
            CurrentViewModel = MainMenuViewModel;
            IsNotMainMenu = false;
        }

        [RelayCommand]
        private void ShowTrainees()
        {
            CurrentViewModel = TraineeViewModel;
            IsNotMainMenu = true;
        }

        [RelayCommand]
        private void ShowMemberships()
        {
            CurrentViewModel = MembershipViewModel;
            IsNotMainMenu = true;
        }

        [RelayCommand]
        private void ShowVisits()
        {
            CurrentViewModel = VisitViewModel;
            IsNotMainMenu = true;
        }

        [RelayCommand]
        private void ShowAdditionalServices()
        {
            CurrentViewModel = AdditionalServiceViewModel;
            IsNotMainMenu = true;
        }

        [RelayCommand]
        private void SetLanguage(string languageCode)
        {
            _localizationService.SetLanguage(languageCode);
        }

        private void OnNavigationRequested(string destination)
        {
            switch (destination)
            {
                case "Trainees":
                    ShowTrainees();
                    break;
                case "Memberships":
                    ShowMemberships();
                    break;
                case "Visits":
                    ShowVisits();
                    break;
                case "AdditionalServices":
                    ShowAdditionalServices();
                    break;
            }
        }
    }
}
