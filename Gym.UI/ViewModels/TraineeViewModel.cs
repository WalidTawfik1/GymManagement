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
    public partial class TraineeViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDialogService _dialog;

        public TraineeViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService, IDialogService dialog) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _dialog = dialog;
            UpdateLocalizedLabels();
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

    // Search input binding
    [ObservableProperty]
    private string _searchTraineeName = string.Empty;

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

        // Explicit initialization to load data sequentially outside constructor
        public async Task InitializeAsync()
        {
            await LoadTrainees();
        }

        [RelayCommand]
        private async Task SearchTrainees()
        {
            if (string.IsNullOrWhiteSpace(SearchTraineeName))
            {
                await _dialog.ShowAsync(GetLocalizedString("EnterTraineeNameToSearch"), GetLocalizedString("NotificationTitle"), DialogType.Info);
                return;
            }
            try
            {
                IsBusy = true;
                var list = await _unitOfWork.TraineeRepository.GetTraineeByNameAsync(SearchTraineeName.Trim());
                Trainees.Clear();
                foreach (var t in list)
                {
                    Trainees.Add(t);
                }
                if (Trainees.Count == 0)
                {
                    await _dialog.ShowAsync(GetLocalizedString("NoTraineesFound"), GetLocalizedString("NotificationTitle"), DialogType.Info);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync(string.Format(GetLocalizedString("ErrorLoadingTrainees"), ex.Message), GetLocalizedString("ErrorTitle"), DialogType.Error);
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
                await _dialog.ShowAsync(string.Format(GetLocalizedString("ErrorLoadingTrainees"), ex.Message), GetLocalizedString("ErrorTitle"), DialogType.Error);
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
                await _dialog.ShowAsync(GetLocalizedString("FillRequiredFields"), GetLocalizedString("WarningTitle"), DialogType.Warning);
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
                    await _dialog.ShowAsync(GetLocalizedString("TraineeAddedSuccess"), GetLocalizedString("SuccessTitle"), DialogType.Success);
                    ClearForm();
                    await LoadTrainees();
                }
                else
                {
                    await _dialog.ShowAsync(GetLocalizedString("FailedAddTrainee"), GetLocalizedString("ErrorTitle"), DialogType.Error);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync(string.Format(GetLocalizedString("ErrorAddingTrainee"), ex.Message), GetLocalizedString("ErrorTitle"), DialogType.Error);
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
                await _dialog.ShowAsync(GetLocalizedString("EnterAllRequiredFields"), GetLocalizedString("ValidationErrorTitle"), DialogType.Warning);
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
                    await _dialog.ShowAsync(GetLocalizedString("TraineeUpdatedSuccess"), GetLocalizedString("SuccessTitle"), DialogType.Success);
                    ClearForm();
                    await LoadTrainees();
                }
                else
                {
                    await _dialog.ShowAsync(GetLocalizedString("FailedUpdateTrainee"), GetLocalizedString("ErrorTitle"), DialogType.Error);
                }
            }
            catch (Exception ex)
            {
                await _dialog.ShowAsync(string.Format(GetLocalizedString("ErrorUpdatingTrainee"), ex.Message), GetLocalizedString("ErrorTitle"), DialogType.Error);
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

        // Localized confirm delete message
        var confirm = await _dialog.ConfirmAsync(string.Format(GetLocalizedString("ConfirmDelete"), trainee.FullName), GetLocalizedString("ConfirmTitle"));
        if (confirm)
            {
                try
                {
                    IsBusy = true;
                    trainee.IsDeleted = true;
                    trainee.UpdatedAt = DateTime.Now;
                    await _unitOfWork.TraineeRepository.UpdateAsync(trainee);
            await _dialog.ShowAsync(GetLocalizedString("TraineeDeletedSuccess"), GetLocalizedString("SuccessTitle"), DialogType.Success);
                    await LoadTrainees();
                }
                catch (Exception ex)
                {
            await _dialog.ShowAsync(string.Format(GetLocalizedString("ErrorDeletingTrainee"), ex.Message), GetLocalizedString("ErrorTitle"), DialogType.Error);
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
