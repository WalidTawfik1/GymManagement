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
    public partial class MembershipViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDialogService _dialog;

        public MembershipViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService, IDialogService dialog) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dialog = dialog;
            UpdateLocalizedLabels();
        }

        [ObservableProperty]
        private ObservableCollection<MembershipDTO> _memberships = new();

        [ObservableProperty]
        private string _membershipManagementLabel = string.Empty;

        private void UpdateLocalizedLabels()
        {
            Title = GetLocalizedString("MembershipManagement");
            MembershipManagementLabel = GetLocalizedString("MembershipManagement");
        }

        // Explicit initialization to avoid starting concurrent DbContext operations in constructor
        public async Task InitializeAsync()
        {
            // Run sequentially to prevent EF Core "second operation started" errors on the same context
            await LoadMemberships();
            await LoadTrainees();
        }

        protected override void OnLanguageChanged()
        {
            UpdateLocalizedLabels();
        }

    [ObservableProperty]
    private ObservableCollection<Trainee> _trainees = new();

        [ObservableProperty]
        private MembershipDTO? _selectedMembership;

        [ObservableProperty]
        private int _selectedTraineeId;

        [ObservableProperty]
        private string _traineeIdInput = string.Empty;

        [ObservableProperty]
        private string _membershipType = string.Empty;

        [ObservableProperty]
        private DateOnly _startDate = DateOnly.FromDateTime(DateTime.Now);

        [ObservableProperty]
        private DateOnly _endDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

        [ObservableProperty]
        private int? _remainingSessions;

        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private int _editMembershipId;

    // Search input for trainee ID
    [ObservableProperty]
    private string _searchTraineeId = string.Empty;

        public List<string> MembershipTypes { get; } = new()
        {
            "مفتوح",
            "محدود"
        };

        [RelayCommand]
        private async Task LoadMemberships()
        {
            try
            {
                IsBusy = true;
                var membershipDTOs = await _unitOfWork.MembershipRepository.GetAllMembershipsAsync();
                
                Memberships.Clear();
                foreach (var membershipDTO in membershipDTOs)
                {
                    Memberships.Add(membershipDTO);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"Error loading memberships: {ex.Message}", "Error", DialogType.Error);
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
                await _dialog.ShowAsync($"Error loading trainees: {ex.Message}", "Error", DialogType.Error);
            }
        }

        [RelayCommand]
        private async Task AddMembership()
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

            if (traineeId == 0 || string.IsNullOrWhiteSpace(MembershipType))
            {
                await _dialog.ShowAsync(GetLocalizedString("EnterTraineeIdAndType"), GetLocalizedString("ValidationErrorTitle"), DialogType.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                
                // Check if trainee exists
                var trainee = await _unitOfWork.TraineeRepository.GetByIdAsync(traineeId);
                if (trainee == null || trainee.IsDeleted)
                {
                    await _dialog.ShowAsync(GetLocalizedString("TraineeNotFound"), GetLocalizedString("ValidationErrorTitle"), DialogType.Warning);
                    return;
                }

                var membershipDto = new AddMembershipDTO
                {
                    TraineeId = traineeId,
                    MembershipType = MembershipType
                };

                var success = await _unitOfWork.MembershipRepository.AddMembership(membershipDto);
                if (success)
                {
                    var successKey = MembershipType == "محدود" ? "MembershipAddedSuccessLimited" : "MembershipAddedSuccessOpen";
                    await _dialog.ShowAsync(GetLocalizedString(successKey), GetLocalizedString("SuccessTitle"), DialogType.Success);
                    ClearForm();
                    await LoadMemberships();
                }
                else
                {
                    await _dialog.ShowAsync(GetLocalizedString("FailedAddMembershipGeneric"), GetLocalizedString("ErrorTitle"), DialogType.Error);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"Error adding membership: {ex.Message}", "Error", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateMembership()
        {
            if (SelectedTraineeId == 0 || string.IsNullOrWhiteSpace(MembershipType))
            {
                await _dialog.ShowAsync(GetLocalizedString("EnterAllRequiredFields"), GetLocalizedString("ValidationErrorTitle"), DialogType.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                var membershipDto = new MembershipDTO
                {
                    Id = EditMembershipId,
                    TraineeId = SelectedTraineeId,
                    MembershipType = MembershipType,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    RemainingSessions = RemainingSessions,
                    IsActive = IsActive
                };

                var success = await _unitOfWork.MembershipRepository.UpdateMembership(membershipDto);
                if (success)
                {
                    await _dialog.ShowAsync(GetLocalizedString("MembershipUpdatedSuccess"), GetLocalizedString("SuccessTitle"), DialogType.Success);
                    ClearForm();
                    await LoadMemberships();
                }
                else
                {
                    await _dialog.ShowAsync(GetLocalizedString("FailedUpdateMembershipGeneric"), GetLocalizedString("ErrorTitle"), DialogType.Error);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"Error updating membership: {ex.Message}", "Error", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteMembership(MembershipDTO? membershipDto)
        {
            if (membershipDto == null) return;

            var confirm = await _dialog.ConfirmAsync(GetLocalizedString("ConfirmDeleteMembership"), GetLocalizedString("ConfirmTitle"));
            if (confirm)
            {
                try
                {
                    IsBusy = true;
                    
                    // Get the actual entity to delete
                    var membership = await _unitOfWork.MembershipRepository.GetByIdAsync(membershipDto.Id);
                    if (membership != null)
                    {
                        membership.IsDeleted = true;
                        membership.UpdatedAt = DateTime.Now;
                        await _unitOfWork.MembershipRepository.UpdateAsync(membership);
                        
                        await _dialog.ShowAsync(GetLocalizedString("MembershipDeletedSuccess"), GetLocalizedString("SuccessTitle"), DialogType.Success);
                        await LoadMemberships();
                    }
                }
                catch (Exception ex)
                {
                    await _dialog.ShowAsync(string.Format(GetLocalizedString("ErrorDeletingMembership"), ex.Message), GetLocalizedString("ErrorTitle"), DialogType.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        private void EditMembership(MembershipDTO? membership)
        {
            if (membership == null) return;

            IsEditMode = true;
            EditMembershipId = membership.Id;
            SelectedTraineeId = membership.TraineeId;
            MembershipType = membership.MembershipType;
            StartDate = membership.StartDate;
            EndDate = membership.EndDate;
            RemainingSessions = membership.RemainingSessions;
            IsActive = membership.IsActive;
        }

        [RelayCommand]
        private void ClearForm()
        {
            IsEditMode = false;
            EditMembershipId = 0;
            SelectedTraineeId = 0;
            TraineeIdInput = string.Empty;
            MembershipType = string.Empty;
            StartDate = DateOnly.FromDateTime(DateTime.Now);
            EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));
            RemainingSessions = null;
            IsActive = true;
            SelectedMembership = null;
        }

        [RelayCommand]
        private async Task SearchMembership()
        {
            if (string.IsNullOrWhiteSpace(SearchTraineeId))
            {
                await _dialog.ShowAsync(GetLocalizedString("EnterTraineeIdToSearch"), GetLocalizedString("NotificationTitle"), DialogType.Info);
                return;
            }

            if (!int.TryParse(SearchTraineeId.Trim(), out var traineeId))
            {
                await _dialog.ShowAsync(GetLocalizedString("InvalidTraineeNumber"), GetLocalizedString("WarningTitle"), DialogType.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                var membership = await _unitOfWork.MembershipRepository.GetMembershipByTraineeIdAsync(traineeId);
                if (membership == null)
                {
                    await _dialog.ShowAsync(GetLocalizedString("NoActiveMembership"), GetLocalizedString("SearchResultTitle"), DialogType.Info);
                    return;
                }

                // Ensure memberships list contains the found membership (reload if needed)
                var existing = Memberships.FirstOrDefault(m => m.Id == membership.Id);
                if (existing == null)
                {
                    // Option 1: add directly
                    Memberships.Add(membership);
                }
                SelectedMembership = Memberships.First(m => m.Id == membership.Id);

                // Switch to edit mode pre-filled
                EditMembership(SelectedMembership);
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync($"خطأ أثناء البحث: {ex.Message}", "Error", DialogType.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
