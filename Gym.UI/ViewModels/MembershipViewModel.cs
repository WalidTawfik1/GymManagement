using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Core.DTO;
using Gym.Core.Interfaces;
using Gym.Core.Models;
using Gym.UI.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Gym.UI.ViewModels
{
    public partial class MembershipViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            UpdateLocalizedLabels();
            _ = LoadMemberships();
            _ = LoadTrainees();
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
                MessageBox.Show($"Error loading memberships: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Error loading trainees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Please enter a valid trainee ID number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else if (SelectedTraineeId > 0)
            {
                traineeId = SelectedTraineeId;
            }

            if (traineeId == 0 || string.IsNullOrWhiteSpace(MembershipType))
            {
                MessageBox.Show("Please enter trainee ID and select membership type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                
                // Check if trainee exists
                var trainee = await _unitOfWork.TraineeRepository.GetByIdAsync(traineeId);
                if (trainee == null || trainee.IsDeleted)
                {
                    MessageBox.Show("Trainee not found. Please check the trainee ID.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    var sessionInfo = MembershipType == "محدود" ? " (12 حصة)" : " (مفتوح)";
                    MessageBox.Show($"تم إضافة العضوية بنجاح{sessionInfo}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadMemberships();
                }
                else
                {
                    MessageBox.Show("Failed to add membership.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding membership: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    MessageBox.Show("Membership updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadMemberships();
                }
                else
                {
                    MessageBox.Show("Failed to update membership.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating membership: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            var result = MessageBox.Show($"Are you sure you want to delete this membership?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
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
                        
                        MessageBox.Show("Membership deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadMemberships();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting membership: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

    }
}
