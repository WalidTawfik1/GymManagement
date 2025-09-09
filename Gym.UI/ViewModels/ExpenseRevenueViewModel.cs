using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.UI.Services;
using Gym.UI.Services.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Gym.UI.ViewModels
{
    public partial class ExpenseRevenueViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ObservableCollection<ExpenseDTO> _expenses = new();

        [ObservableProperty]
        private ExpenseDTO? _selectedExpense;

        [ObservableProperty]
        private string _expenseType = string.Empty;

        [ObservableProperty]
        private string _amount = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private bool _isEditMode = false;

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private int _selectedMonth;

        [ObservableProperty]
        private decimal _totalExpenses = 0m;

        [ObservableProperty]
        private decimal _totalRevenue = 0m;

        [ObservableProperty]
        private decimal _netProfit = 0m;

        // Localized Labels
        [ObservableProperty]
        private string _expenseManagementLabel = string.Empty;

        [ObservableProperty]
        private string _monthLabel = string.Empty;

        [ObservableProperty]
        private string _expenseTypeLabel = string.Empty;

        [ObservableProperty]
        private string _amountLabel = string.Empty;

        [ObservableProperty]
        private string _descriptionLabel = string.Empty;

        [ObservableProperty]
        private string _addExpenseLabel = string.Empty;

        [ObservableProperty]
        private string _updateExpenseLabel = string.Empty;

        [ObservableProperty]
        private string _deleteExpenseLabel = string.Empty;

        [ObservableProperty]
        private string _cancelLabel = string.Empty;

        [ObservableProperty]
        private string _totalExpensesLabel = string.Empty;

        [ObservableProperty]
        private string _totalRevenueLabel = string.Empty;

        [ObservableProperty]
        private string _netProfitLabel = string.Empty;

        [ObservableProperty]
        private string _refreshLabel = string.Empty;

        public List<KeyValuePair<int, string>> Months { get; } = new()
        {
            new(1, "يناير"),
            new(2, "فبراير"),
            new(3, "مارس"),
            new(4, "أبريل"),
            new(5, "مايو"),
            new(6, "يونيو"),
            new(7, "يوليو"),
            new(8, "أغسطس"),
            new(9, "سبتمبر"),
            new(10, "أكتوبر"),
            new(11, "نوفمبر"),
            new(12, "ديسمبر")
        };

        public ExpenseRevenueViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService, IDialogService dialogService) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dialogService = dialogService;
            
            // Set default month to current month without triggering property change
            _selectedMonth = DateTime.Now.Month;
            
            UpdateLocalizedLabels();
            
        // Subscribe to language changes
        _localizationService.LanguageChanged += OnLanguageChangedEvent;
    }

    public async Task InitializeAsync()
    {
        await LoadDataSequentiallyAsync();
    }

        private void OnLanguageChangedEvent(object? sender, EventArgs e)
        {
            UpdateLocalizedLabels();
        }

        private void UpdateLocalizedLabels()
        {
            ExpenseManagementLabel = GetLocalizedString("ExpenseManagement");
            MonthLabel = GetLocalizedString("Month");
            ExpenseTypeLabel = GetLocalizedString("ExpenseType");
            AmountLabel = GetLocalizedString("Amount");
            DescriptionLabel = GetLocalizedString("Description") + " (" + GetLocalizedString("Optional") + ")";
            AddExpenseLabel = GetLocalizedString("AddExpense");
            UpdateExpenseLabel = GetLocalizedString("UpdateExpense");
            DeleteExpenseLabel = GetLocalizedString("DeleteExpense");
            CancelLabel = GetLocalizedString("Cancel");
            TotalExpensesLabel = GetLocalizedString("TotalExpenses");
            TotalRevenueLabel = GetLocalizedString("TotalRevenue");
            NetProfitLabel = GetLocalizedString("NetProfit");
            RefreshLabel = GetLocalizedString("Refresh");
        }

        protected override void OnLanguageChanged()
        {
            UpdateLocalizedLabels();
        }

        partial void OnSelectedMonthChanged(int value)
        {
            if (value > 0)
            {
                _ = LoadDataSequentiallyAsync();
            }
        }

        private async Task LoadDataSequentiallyAsync()
        {
            await LoadExpensesAsync();
            await CalculateFinancialsAsync();
        }

        partial void OnSelectedExpenseChanged(ExpenseDTO? value)
        {
            // Don't automatically populate form when selecting an item
            // Only populate when Edit button is pressed
        }

        [RelayCommand]
        private async Task LoadExpensesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var expenses = await _unitOfWork.ExpenseAndRevenueRepository.GetAllExpensesByMonthAsync(SelectedMonth);
                
                App.Current.Dispatcher.Invoke(() =>
                {
                    Expenses.Clear();
                    foreach (var expense in expenses)
                    {
                        Expenses.Add(expense);
                    }
                });
                
                // Force UI refresh
                OnPropertyChanged(nameof(Expenses));
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAsync("خطأ في تحميل المصروفات: " + ex.Message, "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                await RefreshDataSilentAsync();
                await _dialogService.ShowAsync("تم تحديث البيانات بنجاح", "تحديث", DialogType.Success);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshDataSilentAsync()
        {
            try
            {
                // Load expenses
                var expenses = await _unitOfWork.ExpenseAndRevenueRepository.GetAllExpensesByMonthAsync(SelectedMonth);
                
                App.Current.Dispatcher.Invoke(() =>
                {
                    Expenses.Clear();
                    foreach (var expense in expenses)
                    {
                        Expenses.Add(expense);
                    }
                });
                
                // Calculate financials
                var totalExpenses = await _unitOfWork.ExpenseAndRevenueRepository.GetTotalExpensesByMonthAsync(SelectedMonth);
                var totalRevenue = await _unitOfWork.ExpenseAndRevenueRepository.GetTotalRevenueByMonthAsync(SelectedMonth);
                
                App.Current.Dispatcher.Invoke(() =>
                {
                    TotalExpenses = totalExpenses;
                    TotalRevenue = totalRevenue;
                    NetProfit = totalRevenue - totalExpenses;
                });
                
                // Force UI refresh
                OnPropertyChanged(nameof(Expenses));
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(TotalRevenue));
                OnPropertyChanged(nameof(NetProfit));
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAsync("خطأ في تحديث البيانات: " + ex.Message, "خطأ", DialogType.Error);
            }
        }

        [RelayCommand]
        private async Task CalculateFinancialsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                
                var totalExpenses = await _unitOfWork.ExpenseAndRevenueRepository.GetTotalExpensesByMonthAsync(SelectedMonth);
                var totalRevenue = await _unitOfWork.ExpenseAndRevenueRepository.GetTotalRevenueByMonthAsync(SelectedMonth);
                
                App.Current.Dispatcher.Invoke(() =>
                {
                    TotalExpenses = totalExpenses;
                    TotalRevenue = totalRevenue;
                    NetProfit = totalRevenue - totalExpenses;
                });
                
                // Force UI refresh for financial properties
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(TotalRevenue));
                OnPropertyChanged(nameof(NetProfit));
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    TotalExpenses = 0m;
                    TotalRevenue = 0m;
                    NetProfit = 0m;
                });
                await _dialogService.ShowAsync("خطأ في حساب الإيرادات: " + ex.Message, "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddExpenseAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(ExpenseType) || string.IsNullOrWhiteSpace(Amount))
            {
                await _dialogService.ShowAsync("يرجى ملء جميع الحقول المطلوبة", "تحذير", DialogType.Warning);
                return;
            }

            // Handle both Arabic and English decimal separators
            var normalizedAmount = Amount.Replace(',', '.');
            if (!decimal.TryParse(normalizedAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amountValue) || amountValue <= 0)
            {
                await _dialogService.ShowAsync("يرجى إدخال مبلغ صحيح", "تحذير", DialogType.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                
                var addExpenseDto = new AddExpenseDTO
                {
                    ExpenseType = ExpenseType,
                    Amount = amountValue,
                    Description = string.IsNullOrWhiteSpace(Description) ? null : Description
                };

                var result = await _unitOfWork.ExpenseAndRevenueRepository.AddExpenseAsync(addExpenseDto);
                
                if (result)
                {
                    ClearForm();
                    await RefreshDataSilentAsync();
                    await _dialogService.ShowAsync("تم إضافة المصروف بنجاح", "نجح", DialogType.Success);
                }
                else
                {
                    await _dialogService.ShowAsync("فشل في إضافة المصروف", "خطأ", DialogType.Error);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAsync("خطأ في إضافة المصروف: " + ex.Message, "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateExpenseAsync()
        {
            if (IsBusy || SelectedExpense == null) return;

            if (string.IsNullOrWhiteSpace(ExpenseType) || string.IsNullOrWhiteSpace(Amount))
            {
                await _dialogService.ShowAsync("يرجى ملء جميع الحقول المطلوبة", "تحذير", DialogType.Warning);
                return;
            }

            // Handle both Arabic and English decimal separators
            var normalizedAmount = Amount.Replace(',', '.');
            if (!decimal.TryParse(normalizedAmount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amountValue) || amountValue <= 0)
            {
                await _dialogService.ShowAsync("يرجى إدخال مبلغ صحيح", "تحذير", DialogType.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                
                var updateExpenseDto = new ExpenseDTO
                {
                    Id = SelectedExpense.Id,
                    ExpenseType = ExpenseType,
                    Amount = amountValue,
                    Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
                    IncurredAt = SelectedExpense.IncurredAt,
                    UpdatedAt = DateTime.Now
                };

                var result = await _unitOfWork.ExpenseAndRevenueRepository.UpdateExpenseAsync(updateExpenseDto);
                
                if (result)
                {
                    ClearForm();
                    await RefreshDataSilentAsync();
                    await _dialogService.ShowAsync("تم تحديث المصروف بنجاح", "نجح", DialogType.Success);
                }
                else
                {
                    await _dialogService.ShowAsync("فشل في تحديث المصروف", "خطأ", DialogType.Error);
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAsync("خطأ في تحديث المصروف: " + ex.Message, "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void EditExpense(ExpenseDTO expense)
        {
            SelectedExpense = expense;
            IsEditMode = true;
            ExpenseType = expense.ExpenseType;
            Amount = expense.Amount.ToString("F2");
            Description = expense.Description ?? string.Empty;
        }

        [RelayCommand]
        private async Task DeleteExpense(ExpenseDTO expense)
        {
            try
            {
                var confirmResult = await _dialogService.ConfirmAsync(
                    "هل أنت متأكد من حذف هذا المصروف؟", 
                    "تأكيد الحذف");

                if (confirmResult)
                {
                    var result = await _unitOfWork.ExpenseAndRevenueRepository.DeleteExpenseAsync(expense.Id);
                    
                    if (result)
                    {
                        ClearForm();
                        await RefreshDataSilentAsync();
                        await _dialogService.ShowAsync("تم حذف المصروف بنجاح", "نجح", DialogType.Success);
                    }
                    else
                    {
                        await _dialogService.ShowAsync("فشل في حذف المصروف", "خطأ", DialogType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowAsync("خطأ في حذف المصروف: " + ex.Message, "خطأ", DialogType.Error);
            }
        }

        [RelayCommand]
        private void CancelEdit()
        {
            ClearForm();
        }

        private void ClearForm()
        {
            IsEditMode = false;
            ExpenseType = string.Empty;
            Amount = string.Empty;
            Description = string.Empty;
            SelectedExpense = null;
        }
    }
}
