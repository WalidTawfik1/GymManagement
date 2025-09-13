using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Core.Interfaces;
using Gym.Core.Interfaces.Services;
using Gym.UI.Services;
using Gym.UI.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Gym.UI.ViewModels
{
    public partial class FinancialDetailsViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IReportService _reportService;
        private readonly IDialogService _dialogService;

        public FinancialDetailsViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService,
            IReportService reportService, IDialogService dialogService)
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _reportService = reportService;
            _dialogService = dialogService;
            FinancialDetails = new ObservableCollection<FinancialDetailItem>();
        }

        [ObservableProperty]
        private ObservableCollection<FinancialDetailItem> _financialDetails;

        [ObservableProperty]
        private string _selectedMonthName = string.Empty;

        [ObservableProperty]
        private int _selectedMonth;

        [ObservableProperty]
        private int _selectedYear;

        [ObservableProperty]
        private decimal _totalRevenue;

        [ObservableProperty]
        private decimal _totalExpenses;

        [ObservableProperty]
        private decimal _netProfit;

        [ObservableProperty]
        private bool _isNetProfitPositive;

        // Event for navigation back to ExpenseRevenue
        public event Action? NavigateBack;

        [RelayCommand]
        private void GoBack()
        {
            NavigateBack?.Invoke();
        }

        public async Task LoadDataAsync(int month, int year)
        {
            SelectedMonth = month;
            SelectedYear = year;
            SelectedMonthName = GetMonthName(month);

            await LoadFinancialDetailsAsync();
            CalculateTotals();
        }

        private async Task LoadFinancialDetailsAsync()
        {
            try
            {
                FinancialDetails.Clear();

                // Load Memberships (Revenue)
                var memberships = await _unitOfWork.MembershipRepository.GetMembershipsByMonthAsync(SelectedMonth, SelectedYear);
                foreach (var membership in memberships)
                {
                    FinancialDetails.Add(new FinancialDetailItem
                    {
                        Type = "عضوية",
                        Description = $"عضوية {membership.TraineeName} - {membership.MembershipType}",
                        Amount = membership.Price,
                        FormattedAmount = $"+{membership.Price:F2} ج.م",
                        Date = membership.StartDate.ToDateTime(TimeOnly.MinValue),
                        IsIncome = true
                    });
                }

                // Load Additional Services (Revenue)
                var additionalServices = await _unitOfWork.AdditionalServiceRepository.GetAdditionalServicesByMonthAsync(SelectedMonth, SelectedYear);
                foreach (var service in additionalServices)
                {
                    FinancialDetails.Add(new FinancialDetailItem
                    {
                        Type = "خدمة إضافية",
                        Description = $"{service.ServiceType} - {service.TraineeName}",
                        Amount = service.Price,
                        FormattedAmount = $"+{service.Price:F2} ج.م",
                        Date = service.TakenAt,
                        IsIncome = true
                    });
                }

                // Load Expenses
                var expenses = await _unitOfWork.ExpenseAndRevenueRepository.GetExpensesByMonthAsync(SelectedMonth, SelectedYear);
                foreach (var expense in expenses)
                {
                    var description = string.IsNullOrEmpty(expense.Description) 
                        ? expense.ExpenseType 
                        : $"{expense.ExpenseType} - {expense.Description}";
                    
                    FinancialDetails.Add(new FinancialDetailItem
                    {
                        Type = "مصروف",
                        Description = description,
                        Amount = expense.Amount,
                        FormattedAmount = $"-{expense.Amount:F2} ج.م",
                        Date = expense.IncurredAt,
                        IsIncome = false
                    });
                }

                // Sort by date descending
                var sortedList = FinancialDetails.OrderByDescending(x => x.Date).ToList();
                FinancialDetails.Clear();
                foreach (var item in sortedList)
                {
                    FinancialDetails.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Handle error appropriately
                Console.WriteLine($"Error loading financial details: {ex.Message}");
            }
        }

        private void CalculateTotals()
        {
            TotalRevenue = FinancialDetails.Where(x => x.IsIncome).Sum(x => x.Amount);
            TotalExpenses = FinancialDetails.Where(x => !x.IsIncome).Sum(x => x.Amount);
            NetProfit = TotalRevenue - TotalExpenses;
            IsNetProfitPositive = NetProfit >= 0;
        }

        private string GetMonthName(int month)
        {
            var months = new[]
            {
                "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
            };
            return month >= 1 && month <= 12 ? months[month - 1] : "غير محدد";
        }

        [RelayCommand]
        private async Task ExportFinancialReportAsync()
        {
            try
            {
                IsBusy = true;
                
                var filePath = await _reportService.GenerateFinancialReportAsync(SelectedMonth, SelectedYear);
                
                await _dialogService.ShowAsync($"تم إنشاء التقرير المالي بنجاح وحفظه في:\n{filePath}", "تصدير التقرير المالي", DialogType.Success);
                
                // فتح مجلد التقارير
                Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(filePath) ?? "");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAsync($"حدث خطأ أثناء إنشاء التقرير المالي: {ex.Message}", "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
