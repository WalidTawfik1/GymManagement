using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.Core.Interfaces.Services;
using Gym.UI.Services;
using Gym.UI.Services.Dialogs;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gym.UI.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IDashboardRepository _dashboardRepository;

        [ObservableProperty]
        private DashboardDTO _dashboardData = new();

        [ObservableProperty]
        private int _totalActiveMembers;

        [ObservableProperty]
        private int _totalVisitsThisMonth;

        [ObservableProperty]
        private int _membershipsEndingSoon;

        [ObservableProperty]
        private decimal _netProfitThisMonth;

        [ObservableProperty]
        private decimal _netProfitLastMonth;

        [ObservableProperty]
        private decimal _totalRevenueThisMonth;

        [ObservableProperty]
        private decimal _totalExpensesThisMonth;

        [ObservableProperty]
        private int _newMembersThisMonth;

        [ObservableProperty]
        private int _totalTrainees;

        [ObservableProperty]
        private MembershipDistributionDTO _membershipDistribution = new();

        [ObservableProperty]
        private ObservableCollection<MembershipDTO> _upcomingExpirations = new();

        [ObservableProperty]
        private MonthlyComparisonDTO _monthlyComparison = new();

        [ObservableProperty]
        private string _profitGrowthText = "";

        [ObservableProperty]
        private string _revenueGrowthText = "";

        [ObservableProperty]
        private string _memberGrowthText = "";

        [ObservableProperty]
        private string _visitGrowthText = "";

        [ObservableProperty]
        private bool _isLoading;

        // Localized text properties
        [ObservableProperty]
        private string _dashboardTitle = "";

        [ObservableProperty]
        private string _dashboardSubtitle = "";

        [ObservableProperty]
        private string _loadingText = "";

        [ObservableProperty]
        private string _activeMembersText = "";

        [ObservableProperty]
        private string _visitsThisMonthText = "";

        [ObservableProperty]
        private string _endingSoonText = "";

        [ObservableProperty]
        private string _netProfitThisMonthText = "";

        [ObservableProperty]
        private string _totalRevenueText = "";

        [ObservableProperty]
        private string _totalExpensesText = "";

        [ObservableProperty]
        private string _newMembersText = "";

        [ObservableProperty]
        private string _totalTraineesText = "";

        [ObservableProperty]
        private string _membershipDistributionText = "";

        [ObservableProperty]
        private string _monthlyGrowthText = "";

        [ObservableProperty]
        private string _revenueGrowthLabel = "";

        [ObservableProperty]
        private string _memberGrowthLabel = "";

        [ObservableProperty]
        private string _visitGrowthLabel = "";

        [ObservableProperty]
        private string _upcomingExpirationsText = "";

        [ObservableProperty]
        private string _traineeNameHeaderText = "";

        [ObservableProperty]
        private string _membershipTypeHeaderText = "";

        [ObservableProperty]
        private string _endDateHeaderText = "";

        [ObservableProperty]
        private string _remainingSessionsHeaderText = "";

        [ObservableProperty]
        private string _oneMonthChartText = "";

        [ObservableProperty]
        private string _threeMonthsChartText = "";

        [ObservableProperty]
        private string _twelveSessionsChartText = "";

        // LiveCharts Values
        [ObservableProperty]
        private ChartValues<int> _oneMonthValues = new();

        [ObservableProperty]
        private ChartValues<int> _threeMonthValues = new();

        [ObservableProperty]
        private ChartValues<int> _twelveSessionValues = new();

        [ObservableProperty]
        private SeriesCollection _membershipSeries = new();

        private readonly IReportService _reportService;
        private readonly IDialogService _dialogService;

        public DashboardViewModel(IDashboardRepository dashboardRepository, ILocalizationService localizationService, 
            IReportService reportService, IDialogService dialogService) 
            : base(localizationService)
        {
            _dashboardRepository = dashboardRepository;
            _reportService = reportService;
            _dialogService = dialogService;
            Title = GetLocalizedString("Dashboard");
            LoadDashboardDataCommand = new AsyncRelayCommand(LoadDashboardDataAsync);
            
            UpdateLocalizedTexts();
        }

        public IAsyncRelayCommand LoadDashboardDataCommand { get; }

        public async Task InitializeAsync()
        {
            await LoadDashboardDataCommand.ExecuteAsync(null);
        }

        public async Task LoadDashboardDataAsync()
        {
            try
            {
                IsLoading = true;
                
                DashboardData = await _dashboardRepository.GetDashboardDataAsync().ConfigureAwait(false);
                
                // Ensure UI updates happen on the UI thread
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    // Update individual properties
                    TotalActiveMembers = DashboardData.TotalActiveMembers;
                    TotalVisitsThisMonth = DashboardData.TotalVisitsThisMonth;
                    MembershipsEndingSoon = DashboardData.MembershipsEndingSoon;
                    NetProfitThisMonth = DashboardData.NetProfitThisMonth;
                    NetProfitLastMonth = DashboardData.NetProfitLastMonth;
                    TotalRevenueThisMonth = DashboardData.TotalRevenueThisMonth;
                    TotalExpensesThisMonth = DashboardData.TotalExpensesThisMonth;
                    NewMembersThisMonth = DashboardData.NewMembersThisMonth;
                    TotalTrainees = DashboardData.TotalTrainees;
                    MembershipDistribution = DashboardData.MembershipDistribution ?? new MembershipDistributionDTO();
                    MonthlyComparison = DashboardData.MonthlyComparison ?? new MonthlyComparisonDTO();

                    // Update observable collections
                    UpcomingExpirations.Clear();
                    if (DashboardData.UpcomingExpirations != null)
                    {
                        foreach (var expiration in DashboardData.UpcomingExpirations)
                        {
                            UpcomingExpirations.Add(expiration);
                        }
                    }

                    // Update growth text
                    UpdateGrowthTexts();

                    // Update chart values
                    UpdateChartValues();
                });
                
                // Debug information
                System.Diagnostics.Debug.WriteLine($"Dashboard Data Loaded:");
                System.Diagnostics.Debug.WriteLine($"Active Members: {TotalActiveMembers}");
                System.Diagnostics.Debug.WriteLine($"One Month: {MembershipDistribution.OneMonthMemberships}");
                System.Diagnostics.Debug.WriteLine($"Three Months: {MembershipDistribution.ThreeMonthMemberships}");
                System.Diagnostics.Debug.WriteLine($"Twelve Sessions: {MembershipDistribution.TwelveSessionMemberships}");
                System.Diagnostics.Debug.WriteLine($"Upcoming Expirations: {UpcomingExpirations.Count}");
            }
            catch (Exception ex)
            {
                // Handle error with detailed logging
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Initialize with empty data to prevent null reference exceptions on UI thread
                await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MembershipDistribution = new MembershipDistributionDTO();
                    MonthlyComparison = new MonthlyComparisonDTO();
                    UpcomingExpirations.Clear();
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateChartValues()
        {
            // Ensure this runs on the UI thread
            if (!System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(UpdateChartValues);
                return;
            }

            OneMonthValues.Clear();
            ThreeMonthValues.Clear();
            TwelveSessionValues.Clear();

            OneMonthValues.Add(MembershipDistribution.OneMonthMemberships);
            ThreeMonthValues.Add(MembershipDistribution.ThreeMonthMemberships);
            TwelveSessionValues.Add(MembershipDistribution.TwelveSessionMemberships);

            // Create localized series for the chart
            UpdateMembershipSeries();
            
            // Force property change notification for chart update
            OnPropertyChanged(nameof(MembershipSeries));
        }

        private void UpdateMembershipSeries()
        {
            // Ensure this runs on the UI thread
            if (!System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                System.Windows.Application.Current.Dispatcher.Invoke(UpdateMembershipSeries);
                return;
            }

            MembershipSeries.Clear();
            
            // Only add series with non-zero values to avoid empty chart
            if (MembershipDistribution.OneMonthMemberships > 0)
            {
                MembershipSeries.Add(new PieSeries
                {
                    Title = OneMonthChartText,
                    Values = new ChartValues<int> { MembershipDistribution.OneMonthMemberships },
                    Fill = new SolidColorBrush(Color.FromRgb(220, 20, 60)), // Crimson
                    DataLabels = true
                });
            }
            
            if (MembershipDistribution.ThreeMonthMemberships > 0)
            {
                MembershipSeries.Add(new PieSeries
                {
                    Title = ThreeMonthsChartText,
                    Values = new ChartValues<int> { MembershipDistribution.ThreeMonthMemberships },
                    Fill = new SolidColorBrush(Color.FromRgb(34, 139, 34)), // Forest Green
                    DataLabels = true
                });
            }
            
            if (MembershipDistribution.TwelveSessionMemberships > 0)
            {
                MembershipSeries.Add(new PieSeries
                {
                    Title = TwelveSessionsChartText,
                    Values = new ChartValues<int> { MembershipDistribution.TwelveSessionMemberships },
                    Fill = new SolidColorBrush(Color.FromRgb(30, 144, 255)), // Dodger Blue
                    DataLabels = true
                });
            }
            
            // If no data available, add a placeholder
            if (MembershipSeries.Count == 0)
            {
                MembershipSeries.Add(new PieSeries
                {
                    Title = GetLocalizedString("NoData") ?? "No Data",
                    Values = new ChartValues<int> { 1 },
                    Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200)), // Light gray
                    DataLabels = true
                });
            }
        }

        private void UpdateGrowthTexts()
        {
            ProfitGrowthText = FormatGrowthText(MonthlyComparison.ProfitGrowthPercentage);
            RevenueGrowthText = FormatGrowthText(MonthlyComparison.RevenueGrowthPercentage);
            MemberGrowthText = FormatGrowthText(MonthlyComparison.MemberGrowthPercentage);
            VisitGrowthText = FormatGrowthText(MonthlyComparison.VisitGrowthPercentage);
        }

        private static string FormatGrowthText(decimal percentage)
        {
            var sign = percentage >= 0 ? "+" : "";
            return $"{sign}{percentage:F1}%";
        }

        private void UpdateLocalizedTexts()
        {
            DashboardTitle = GetLocalizedString("DashboardTitle");
            DashboardSubtitle = GetLocalizedString("DashboardSubtitle");
            LoadingText = GetLocalizedString("LoadingDashboardData");
            ActiveMembersText = GetLocalizedString("ActiveMembers");
            VisitsThisMonthText = GetLocalizedString("VisitsThisMonth");
            EndingSoonText = GetLocalizedString("EndingSoon");
            NetProfitThisMonthText = GetLocalizedString("NetProfitThisMonth");
            TotalRevenueText = GetLocalizedString("TotalRevenue");
            TotalExpensesText = GetLocalizedString("TotalExpenses");
            NewMembersText = GetLocalizedString("NewMembers");
            TotalTraineesText = GetLocalizedString("TotalTrainees");
            MembershipDistributionText = GetLocalizedString("MembershipDistribution");
            MonthlyGrowthText = GetLocalizedString("MonthlyGrowth");
            RevenueGrowthLabel = GetLocalizedString("RevenueGrowth");
            MemberGrowthLabel = GetLocalizedString("MemberGrowth");
            VisitGrowthLabel = GetLocalizedString("VisitGrowth");
            UpcomingExpirationsText = GetLocalizedString("UpcomingMembershipExpirations");
            TraineeNameHeaderText = GetLocalizedString("TraineeName");
            MembershipTypeHeaderText = GetLocalizedString("MembershipType");
            EndDateHeaderText = GetLocalizedString("EndDate");
            RemainingSessionsHeaderText = GetLocalizedString("RemainingSessions");
            OneMonthChartText = GetLocalizedString("OneMonth");
            ThreeMonthsChartText = GetLocalizedString("ThreeMonths");
            TwelveSessionsChartText = GetLocalizedString("TwelveSessions");
            
            // Update chart series with new localized text
            if (MembershipSeries.Count > 0)
            {
                UpdateMembershipSeries();
            }
        }

        protected override void OnLanguageChanged()
        {
            Title = GetLocalizedString("Dashboard");
            UpdateLocalizedTexts();
        }

        [RelayCommand]
        private async Task ExportDashboardReportAsync()
        {
            try
            {
                IsBusy = true;
                
                var filePath = await _reportService.GenerateDashboardReportAsync();
                
                await _dialogService.ShowAsync($"تم إنشاء التقرير بنجاح وحفظه في:\n{filePath}", "تصدير التقرير", DialogType.Success);
                
                // فتح مجلد التقارير
                Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(filePath) ?? "");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAsync($"حدث خطأ أثناء إنشاء التقرير: {ex.Message}", "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
