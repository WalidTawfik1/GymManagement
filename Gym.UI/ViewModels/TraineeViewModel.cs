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
    public partial class TraineeViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;

        public TraineeViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            UpdateLocalizedLabels();
            _ = LoadTrainees();
        }

        [ObservableProperty]
        private ObservableCollection<Trainee> _trainees = new();

        [ObservableProperty]
        private Trainee? _selectedTrainee;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _phoneNumber = string.Empty;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private int _editTraineeId;

        // Localized Labels
        [ObservableProperty]
        private string _traineeManagementLabel = string.Empty;
        
        [ObservableProperty]
        private string _traineesListLabel = string.Empty;
        
        [ObservableProperty]
        private string _refreshLabel = string.Empty;
        
        [ObservableProperty]
        private string _fullNameLabel = string.Empty;
        
        [ObservableProperty]
        private string _phoneNumberLabel = string.Empty;
        
        [ObservableProperty]
        private string _joinDateLabel = string.Empty;
        
        [ObservableProperty]
        private string _actionsLabel = string.Empty;
        
        [ObservableProperty]
        private string _editLabel = string.Empty;
        
        [ObservableProperty]
        private string _deleteLabel = string.Empty;
        
        [ObservableProperty]
        private string _addNewTraineeLabel = string.Empty;
        
        [ObservableProperty]
        private string _addTraineeLabel = string.Empty;
        
        [ObservableProperty]
        private string _updateTraineeLabel = string.Empty;
        
        [ObservableProperty]
        private string _clearFormLabel = string.Empty;

        private void UpdateLocalizedLabels()
        {
            Title = GetLocalizedString("TraineeManagement");
            TraineeManagementLabel = GetLocalizedString("TraineeManagement");
            TraineesListLabel = GetLocalizedString("TraineesList");
            RefreshLabel = GetLocalizedString("Refresh");
            FullNameLabel = GetLocalizedString("FullName");
            PhoneNumberLabel = GetLocalizedString("PhoneNumber");
            JoinDateLabel = GetLocalizedString("JoinDate");
            ActionsLabel = GetLocalizedString("Actions");
            EditLabel = GetLocalizedString("Edit");
            DeleteLabel = GetLocalizedString("Delete");
            AddNewTraineeLabel = GetLocalizedString("AddNewTrainee");
            AddTraineeLabel = GetLocalizedString("AddTrainee");
            UpdateTraineeLabel = GetLocalizedString("UpdateTrainee");
            ClearFormLabel = GetLocalizedString("ClearForm");
        }

        protected override void OnLanguageChanged()
        {
            UpdateLocalizedLabels();
        }

        [RelayCommand]
        private async Task LoadTrainees()
        {
            try
            {
                IsBusy = true;
                var trainees = await _unitOfWork.TraineeRepository.GetAllAsync();
                Trainees.Clear();
                foreach (var trainee in trainees.Where(t => !t.IsDeleted))
                {
                    Trainees.Add(trainee);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(GetLocalizedString("ErrorLoadingTrainees"), ex.Message), GetLocalizedString("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddTrainee()
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show(GetLocalizedString("FillRequiredFields"), GetLocalizedString("WarningTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                var traineeDto = new TraineeDTO
                {
                    FullName = FullName,
                    PhoneNumber = PhoneNumber
                };

                var success = await _unitOfWork.TraineeRepository.AddTraineeAsync(traineeDto);
                if (success)
                {
                    MessageBox.Show(GetLocalizedString("TraineeAddedSuccess"), GetLocalizedString("SuccessTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadTrainees();
                }
                else
                {
                    MessageBox.Show(GetLocalizedString("FailedAddTrainee"), GetLocalizedString("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(GetLocalizedString("ErrorAddingTrainee"), ex.Message), GetLocalizedString("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateTrainee()
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                var updateDto = new UpdateTraineeDTO
                {
                    Id = EditTraineeId,
                    FullName = FullName,
                    PhoneNumber = PhoneNumber
                };

                var success = await _unitOfWork.TraineeRepository.UpdateTraineeAsync(updateDto);
                if (success)
                {
                    MessageBox.Show("Trainee updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    await LoadTrainees();
                }
                else
                {
                    MessageBox.Show("Failed to update trainee.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating trainee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteTrainee(Trainee? trainee)
        {
            if (trainee == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete {trainee.FullName}?", 
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsBusy = true;
                    trainee.IsDeleted = true;
                    trainee.UpdatedAt = DateTime.Now;
                    await _unitOfWork.TraineeRepository.UpdateAsync(trainee);
                    
                    MessageBox.Show("Trainee deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadTrainees();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting trainee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        [RelayCommand]
        private void EditTrainee(Trainee? trainee)
        {
            if (trainee == null) return;

            IsEditMode = true;
            EditTraineeId = trainee.Id;
            FullName = trainee.FullName;
            PhoneNumber = trainee.PhoneNumber;
        }

        [RelayCommand]
        private void ClearForm()
        {
            IsEditMode = false;
            EditTraineeId = 0;
            FullName = string.Empty;
            PhoneNumber = string.Empty;
            SelectedTrainee = null;
        }
    }
}
