using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.Core.Models;
using Gym.UI.Services;
using Gym.UI.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;

namespace Gym.UI.ViewModels
{
    public partial class VisitViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDialogService _dialog;

        public VisitViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService, IDialogService dialog) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dialog = dialog;
            UpdateLocalizedLabels();
        }

        [ObservableProperty]
        private ObservableCollection<VisitDTO> _visits = new();

        [ObservableProperty]
        private VisitDTO? _selectedVisit;

        [ObservableProperty]
        private string _visitManagementLabel = string.Empty;

        private void UpdateLocalizedLabels()
        {
            Title = GetLocalizedString("VisitManagement");
            VisitManagementLabel = GetLocalizedString("VisitManagement");
        }

        protected override void OnLanguageChanged()
        {
            UpdateLocalizedLabels();
        }

        // Explicit initialization instead of fire-and-forget in constructor to avoid concurrent DbContext use
        public async Task InitializeAsync()
        {
            await LoadVisits();
        }

        [ObservableProperty]
        private string _traineeId = string.Empty;

        [ObservableProperty]
        private DateTime _visitDate = DateTime.Now;

        // Properties for displaying membership information after visit
        [ObservableProperty]
        private string _lastVisitTraineeName = string.Empty;

        [ObservableProperty]
        private string _lastVisitMembershipType = string.Empty;

        [ObservableProperty]
        private int? _lastVisitRemainingSessions;

        [ObservableProperty]
        private bool _lastVisitIsActive;

        [ObservableProperty]
        private string _lastVisitMessage = string.Empty;

        [ObservableProperty]
        private bool _showLastVisitInfo = false;

        [ObservableProperty]
        private int _todayVisitsCount = 0;

        [RelayCommand]
        private async Task LoadVisits()
        {
            try
            {
                IsBusy = true;
                var today = DateOnly.FromDateTime(DateTime.Now);
                
                // Get today's visits only
                var todayVisits = await _unitOfWork.VisitRepository.GetTodayVisits(today);
                Visits.Clear();
                foreach (var visit in todayVisits)
                {
                    Visits.Add(visit);
                }
                
                // Get today's visit count
                TodayVisitsCount = await _unitOfWork.VisitRepository.GetTodayVisitsCountAsync();
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync(GetLocalizedString("ErrorLoadingVisits").Replace("{0}", ex.Message), GetLocalizedString("ErrorTitle"), DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddVisit()
        {
            if (string.IsNullOrWhiteSpace(TraineeId))
            {
                await _dialog.ShowAsync(GetLocalizedString("PleaseEnterTraineeId"), GetLocalizedString("WarningTitle"), DialogType.Warning);
                return;
            }

            if (!int.TryParse(TraineeId, out int traineeIdInt))
            {
                await _dialog.ShowAsync(GetLocalizedString("EnterValidTraineeIdShort"), GetLocalizedString("WarningTitle"), DialogType.Warning);
                return;
            }

            try
            {
                IsBusy = true;

                var visitResponse = await _unitOfWork.VisitRepository.AddVisitAsync(traineeIdInt);

                if (visitResponse == null)
                {
                    // Attempt to load membership info anyway
                    await PopulateMembershipInfoFallback(traineeIdInt, GetLocalizedString("ErrorRecordingVisit"));
                }
                else
                {
                    // Always show the returned info (even if message indicates a failure like already checked in)
                    LastVisitTraineeName = visitResponse.TraineeName;
                    LastVisitMembershipType = visitResponse.MembershipType;
                    LastVisitRemainingSessions = visitResponse.RemainingSessions;
                    LastVisitIsActive = visitResponse.IsActive;
                    LastVisitMessage = visitResponse.Message;
                    ShowLastVisitInfo = true;

                    // If response indicates success text, show success dialog else info/warning
                    var type = visitResponse.Message.Contains("تم تسجيل الزيارة") ? DialogType.Success : DialogType.Info;
                    await _dialog.ShowAsync(visitResponse.Message, type == DialogType.Success ? GetLocalizedString("SuccessTitle") : GetLocalizedString("NotificationTitle"), type);

                    // Only clear and reload list if a visit was actually recorded (success phrase)
                    if (type == DialogType.Success)
                    {
                        TraineeId = string.Empty;
                        VisitDate = DateTime.Now;
                        await LoadVisits();
                    }
                    else
                    {
                        // Ensure membership info present; if empty fetch fallback
                        if (string.IsNullOrWhiteSpace(LastVisitTraineeName))
                        {
                            await PopulateMembershipInfoFallback(traineeIdInt, visitResponse.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync(GetLocalizedString("ErrorRecordingVisit").Replace("{0}", ex.Message), GetLocalizedString("ErrorTitle"), DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PopulateMembershipInfoFallback(int traineeId, string message)
        {
            try
            {
                var membership = await _unitOfWork.MembershipRepository.GetMembershipByTraineeIdAsync(traineeId);
                LastVisitTraineeName = membership?.TraineeName ?? string.Empty;
                LastVisitMembershipType = membership?.MembershipType ?? string.Empty;
                LastVisitRemainingSessions = membership?.RemainingSessions;
                LastVisitIsActive = membership?.IsActive ?? false;
                LastVisitMessage = message;
                ShowLastVisitInfo = true;
            }
            catch { /* swallow fallback errors */ }
        }

        [RelayCommand]
        private async Task DeleteVisit(VisitDTO? visitDto)
        {
            if (visitDto == null) return;

            var confirm = await _dialog.ConfirmAsync(GetLocalizedString("ConfirmDeleteVisit"), GetLocalizedString("ConfirmTitle"));
            if (confirm)
            {
                try
                {
                    IsBusy = true;
                    
                    // Get the actual Visit entity from the repository
                    var visit = await _unitOfWork.VisitRepository.GetByIdAsync(visitDto.Id);
                    if (visit != null)
                    {
                        visit.IsDeleted = true;
                        visit.UpdatedAt = DateTime.Now;
                        await _unitOfWork.VisitRepository.UpdateAsync(visit);
                        
                        await _dialog.ShowAsync(GetLocalizedString("VisitDeletedSuccess"), GetLocalizedString("SuccessTitle"), DialogType.Success);
                        await LoadVisits();
                    }
                }
                catch (Exception ex)
                {
                    await _dialog.ShowAsync(GetLocalizedString("ErrorDeletingVisit").Replace("{0}", ex.Message), GetLocalizedString("ErrorTitle"), DialogType.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        private void ClearForm()
        {
            TraineeId = string.Empty;
            VisitDate = DateTime.Now;
            SelectedVisit = null;
            ShowLastVisitInfo = false;
            LastVisitTraineeName = string.Empty;
            LastVisitMembershipType = string.Empty;
            LastVisitRemainingSessions = null;
            LastVisitIsActive = false;
            LastVisitMessage = string.Empty;
        }
    }
}
