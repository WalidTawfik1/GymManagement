using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Core.Interfaces;
using Gym.Core.Interfaces.Services;
using Gym.UI.Services;

namespace Gym.UI.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;

        public MainViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService, 
            Gym.UI.Services.Dialogs.IDialogService dialog, IReportService reportService) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            UpdateLocalizedLabels();
            
            // Initialize ViewModels
            MainMenuViewModel = new MainMenuViewModel(_localizationService);
            TraineeViewModel = new TraineeViewModel(_unitOfWork, _mapper, _localizationService, dialog, reportService);
            MembershipViewModel = new MembershipViewModel(_unitOfWork, _mapper, _localizationService, dialog, reportService);
            VisitViewModel = new VisitViewModel(_unitOfWork, _mapper, _localizationService, dialog, reportService);
            AdditionalServiceViewModel = new AdditionalServiceViewModel(_unitOfWork, _mapper, _localizationService, dialog, reportService);
            ExpenseRevenueViewModel = new ExpenseRevenueViewModel(_unitOfWork, _mapper, _localizationService, dialog, reportService);
            FinancialDetailsViewModel = new FinancialDetailsViewModel(_unitOfWork, _mapper, _localizationService, reportService, dialog);
            DashboardViewModel = new DashboardViewModel(_unitOfWork.DashboardRepository, _localizationService, reportService, dialog);
            
            // Subscribe to menu navigation
            MainMenuViewModel.NavigationRequested += OnNavigationRequested;
            
            // Subscribe to navigation events
            ExpenseRevenueViewModel.NavigateToDetails += OnNavigateToFinancialDetails;
            FinancialDetailsViewModel.NavigateBack += OnNavigateBackToExpenseRevenue;
            
            // Set default active view to menu
            CurrentViewModel = MainMenuViewModel;

            // Subscribe to language changes
            _localizationService.LanguageChanged += OnLanguageChanged;
        }

        // Called after construction (from App.xaml.cs) to load data sequentially and avoid parallel DbContext operations
        public async Task InitializeAsync()
        {
            try
            {
                // Load in sequence with small delays to prevent EF Core concurrency on a single scoped DbContext
                await TraineeViewModel.InitializeAsync().ConfigureAwait(false);
                await Task.Delay(10).ConfigureAwait(false); // Small delay to prevent DbContext threading issues
                
                await MembershipViewModel.InitializeAsync().ConfigureAwait(false);
                await Task.Delay(10).ConfigureAwait(false);
                
                await VisitViewModel.InitializeAsync().ConfigureAwait(false);
                await Task.Delay(10).ConfigureAwait(false);
                
                await AdditionalServiceViewModel.InitializeAsync().ConfigureAwait(false);
                await Task.Delay(10).ConfigureAwait(false);
                
                await ExpenseRevenueViewModel.InitializeAsync().ConfigureAwait(false);
                await Task.Delay(10).ConfigureAwait(false);
                
                await DashboardViewModel.InitializeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during initialization: {ex.Message}");
                // Don't rethrow to prevent app crash - individual ViewModels should handle their own errors
            }
        }

        [ObservableProperty]
        private BaseViewModel _currentViewModel;

        [ObservableProperty]
        private bool _isNotMainMenu = false;

    // Zoom scaling for responsive laptop screens (0.8x - 1.5x)
    [ObservableProperty]
    private double _zoomScale = 1.0;

    private const double MinZoom = 0.8;
    private const double MaxZoom = 1.5;
    private const double ZoomStep = 0.1;

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

        [ObservableProperty]
        private string _expenseRevenueLabel = string.Empty;

        public TraineeViewModel TraineeViewModel { get; }
        public MembershipViewModel MembershipViewModel { get; }
        public VisitViewModel VisitViewModel { get; }
        public AdditionalServiceViewModel AdditionalServiceViewModel { get; }
        public ExpenseRevenueViewModel ExpenseRevenueViewModel { get; }
        public FinancialDetailsViewModel FinancialDetailsViewModel { get; }
        public MainMenuViewModel MainMenuViewModel { get; }
        public DashboardViewModel DashboardViewModel { get; }

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
            ExpenseRevenueLabel = GetLocalizedString("ExpenseRevenue");
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
        private void ShowExpenseRevenue()
        {
            CurrentViewModel = ExpenseRevenueViewModel;
            IsNotMainMenu = true;
        }

        [RelayCommand]
        private void ShowDashboard()
        {
            CurrentViewModel = DashboardViewModel;
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
                case "ExpenseRevenue":
                    ShowExpenseRevenue();
                    break;
                case "Dashboard":
                    ShowDashboard();
                    break;
            }
        }

        private async void OnNavigateToFinancialDetails(int month, int year)
        {
            await FinancialDetailsViewModel.LoadDataAsync(month, year);
            CurrentViewModel = FinancialDetailsViewModel;
            IsNotMainMenu = true;
        }

        private void OnNavigateBackToExpenseRevenue()
        {
            CurrentViewModel = ExpenseRevenueViewModel;
            IsNotMainMenu = true;
        }

        [RelayCommand]
        private void ZoomIn()
        {
            ZoomScale = Math.Min(MaxZoom, Math.Round(ZoomScale + ZoomStep, 2));
        }

        [RelayCommand]
        private void ZoomOut()
        {
            ZoomScale = Math.Max(MinZoom, Math.Round(ZoomScale - ZoomStep, 2));
        }

        [RelayCommand]
        private void ResetZoom()
        {
            ZoomScale = 1.0;
        }
    }
}
