using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.Core.Interfaces.Services;
using Gym.Core.Models;
using Gym.UI.Services;
using Gym.UI.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Gym.UI.ViewModels
{
    public partial class AdditionalServiceViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDialogService _dialog;
        private readonly IReportService _reportService;

        public AdditionalServiceViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService, IDialogService dialog, IReportService reportService)
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dialog = dialog;
            _reportService = reportService;
            UpdateLocalizedLabels();
        }

        [ObservableProperty]
        private ObservableCollection<AdditionalServiceDTO> _additionalServices = new();

        [ObservableProperty]
        private string _additionalServiceManagementLabel = string.Empty;

        [ObservableProperty]
        private string _lastUpdatedLabel = string.Empty;

        private void UpdateLocalizedLabels()
        {
            Title = GetLocalizedString("AdditionalServiceManagement");
            AdditionalServiceManagementLabel = GetLocalizedString("AdditionalServiceManagement");
            LastUpdatedLabel = GetLocalizedString("LastUpdated");
        }

        // Explicit initialization to avoid starting concurrent DbContext operations in constructor
        public async Task InitializeAsync()
        {
            // Run sequentially to prevent EF Core "second operation started" errors on the same context
            await LoadAdditionalServices();
            await LoadTrainees();
        }

        protected override void OnLanguageChanged()
        {
            UpdateLocalizedLabels();
        }

        [ObservableProperty]
        private ObservableCollection<Trainee> _trainees = new();

        [ObservableProperty]
        private AdditionalServiceDTO? _selectedAdditionalService;

        [ObservableProperty]
        private int _selectedTraineeId;

        [ObservableProperty]
        private string _traineeIdInput = string.Empty;

        [ObservableProperty]
        private string _serviceType = string.Empty;

        [ObservableProperty]
        private int? _durationInMinutes;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private int _editAdditionalServiceId;

        // Search input for trainee ID
        [ObservableProperty]
        private string _searchTraineeId = string.Empty;

        public List<string> ServiceTypes { get; } = new()
        {
            "مشاية",
            "ميزان",
            "InBody"
        };

        [RelayCommand]
        private async Task LoadAdditionalServices()
        {
            try
            {
                IsBusy = true;
                var services = await _unitOfWork.AdditionalServiceRepository.GetAllAdditionalServicesAsync();
                AdditionalServices.Clear();
                
                foreach (var service in services)
                {
                    AdditionalServices.Add(service);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"خطأ في تحميل الخدمات الإضافية: {ex.Message}", "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task LoadTrainees()
        {
            try
            {
                var trainees = await _unitOfWork.TraineeRepository.GetAllAsync();
                Trainees.Clear();
                
                foreach (var trainee in trainees.Where(t => !t.IsDeleted))
                {
                    Trainees.Add(trainee);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"خطأ في تحميل المتدربين: {ex.Message}", "خطأ", DialogType.Error);
            }
        }

        [RelayCommand]
        private async Task AddAdditionalService()
        {
            // Determine trainee ID from input or selection
            int traineeId = 0;
            if (!string.IsNullOrWhiteSpace(TraineeIdInput))
            {
                if (!int.TryParse(TraineeIdInput, out traineeId))
                {
                    await _dialog.ShowAsync(GetLocalizedString("EnterValidTraineeId"), GetLocalizedString("ValidationErrorTitle"), DialogType.Warning);
                    return;
                }
            }
            else if (SelectedTraineeId > 0)
            {
                traineeId = SelectedTraineeId;
            }

            if (traineeId == 0 || string.IsNullOrWhiteSpace(ServiceType))
            {
                await _dialog.ShowAsync("يرجى إدخال رقم المتدرب ونوع الخدمة", "خطأ في التحقق", DialogType.Warning);
                return;
            }

            // Validate duration for تمشاية service
            if (ServiceType == "مشاية" && (!DurationInMinutes.HasValue || DurationInMinutes <= 0))
            {
                await _dialog.ShowAsync("يرجى إدخال مدة صحيحة للمشاية بالدقائق", "خطأ في التحقق", DialogType.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                
                var trainee = await _unitOfWork.TraineeRepository.GetByIdAsync(traineeId);
                if (trainee == null || trainee.IsDeleted)
                {
                    await _dialog.ShowAsync("المتدرب غير موجود أو محذوف", "خطأ", DialogType.Error);
                    return;
                }

                var addServiceDto = new AddAdditionalServiceDTO
                {
                    TraineeId = traineeId,
                    ServiceType = ServiceType,
                    DurationInMinutes = ServiceType == "مشاية" ? DurationInMinutes : null
                };

                var success = await _unitOfWork.AdditionalServiceRepository.AddAdditionalServiceAsync(addServiceDto);
                
                if (success)
                {
                    await _dialog.ShowAsync("تم إضافة الخدمة الإضافية بنجاح", "نجح", DialogType.Success);
                    ClearForm();
                    await LoadAdditionalServices();
                }
                else
                {
                    await _dialog.ShowAsync("فشل في إضافة الخدمة الإضافية", "خطأ", DialogType.Error);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"خطأ في إضافة الخدمة الإضافية: {ex.Message}", "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateAdditionalService()
        {
            if (SelectedTraineeId == 0 || string.IsNullOrWhiteSpace(ServiceType))
            {
                await _dialog.ShowAsync("يرجى إدخال جميع الحقول المطلوبة", "خطأ في التحقق", DialogType.Warning);
                return;
            }

            // Validate duration for مشاية service
            if (ServiceType == "مشاية" && (!DurationInMinutes.HasValue || DurationInMinutes <= 0))
            {
                await _dialog.ShowAsync("يرجى إدخال مدة صحيحة للمشاية بالدقائق", "خطأ في التحقق", DialogType.Warning);
                return;
            }

            try
            {
                IsBusy = true;

                var updateServiceDto = new AdditionalServiceDTO
                {
                    Id = EditAdditionalServiceId,
                    TraineeId = SelectedTraineeId,
                    ServiceType = ServiceType,
                    DurationInMinutes = ServiceType == "مشاية" ? DurationInMinutes : null,
                    Price = CalculatePrice(ServiceType, DurationInMinutes)
                };

                var success = await _unitOfWork.AdditionalServiceRepository.UpdateAdditionalServiceAsync(updateServiceDto);
                
                if (success)
                {
                    await _dialog.ShowAsync("تم تحديث الخدمة الإضافية بنجاح", "نجح", DialogType.Success);
                    ClearForm();
                    await LoadAdditionalServices();
                }
                else
                {
                    await _dialog.ShowAsync("فشل في تحديث الخدمة الإضافية", "خطأ", DialogType.Error);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"خطأ في تحديث الخدمة الإضافية: {ex.Message}", "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteAdditionalService(AdditionalServiceDTO? serviceDto)
        {
            if (serviceDto == null) return;

            var confirm = await _dialog.ConfirmAsync("هل أنت متأكد من حذف هذه الخدمة الإضافية؟", "تأكيد الحذف");
            if (confirm)
            {
                try
                {
                    IsBusy = true;
                    var success = await _unitOfWork.AdditionalServiceRepository.DeleteAdditionalServiceAsync(serviceDto.Id);
                    
                    if (success)
                    {
                        await _dialog.ShowAsync("تم حذف الخدمة الإضافية بنجاح", "نجح", DialogType.Success);
                        await LoadAdditionalServices();
                    }
                    else
                    {
                        await _dialog.ShowAsync("فشل في حذف الخدمة الإضافية", "خطأ", DialogType.Error);
                    }
                }
                catch (Exception ex)
                {
                    await _dialog.ShowAsync($"خطأ في حذف الخدمة الإضافية: {ex.Message}", "خطأ", DialogType.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        private void EditAdditionalService(AdditionalServiceDTO? service)
        {
            if (service == null) return;

            IsEditMode = true;
            EditAdditionalServiceId = service.Id;
            SelectedTraineeId = service.TraineeId;
            ServiceType = service.ServiceType;
            DurationInMinutes = service.DurationInMinutes;
        }

        [RelayCommand]
        private void ClearForm()
        {
            IsEditMode = false;
            EditAdditionalServiceId = 0;
            SelectedTraineeId = 0;
            TraineeIdInput = string.Empty;
            ServiceType = string.Empty;
            DurationInMinutes = null;
        }

        [RelayCommand]
        private async Task SearchAdditionalService()
        {
            if (string.IsNullOrWhiteSpace(SearchTraineeId))
            {
                await LoadAdditionalServices();
                return;
            }

            if (!int.TryParse(SearchTraineeId.Trim(), out var traineeId))
            {
                await _dialog.ShowAsync("يرجى إدخال رقم متدرب صحيح", "خطأ في التحقق", DialogType.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                var services = await _unitOfWork.AdditionalServiceRepository.GetAdditionalServiceByTraineeIdAsync(traineeId);
                
                AdditionalServices.Clear();
                if (services != null && services.Any())
                {
                    foreach (var service in services)
                    {
                        AdditionalServices.Add(service);
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"خطأ في البحث: {ex.Message}", "خطأ", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private decimal CalculatePrice(string serviceType, int? duration)
        {
            return serviceType switch
            {
                "مشاية" => (decimal)(1.5 * (duration ?? 0)),
                "ميزان" => 5,
                "InBody" => 10,
                _ => 0
            };
        }

        [RelayCommand]
        private async Task ExportAdditionalServicesReport()
        {
            try
            {
                IsBusy = true;
                var filePath = await _reportService.ExportAdditionalServicesReportAsync();
                
                await _dialog.ShowAsync(GetLocalizedString("ReportExportedSuccess"), GetLocalizedString("SuccessTitle"), DialogType.Success);
                
                // فتح مجلد التقارير
                var folderPath = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = folderPath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"{GetLocalizedString("ReportExportError")}: {ex.Message}", GetLocalizedString("ErrorTitle"), DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}