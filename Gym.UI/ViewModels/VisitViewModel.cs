using AutoMapper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Gym.Core.Interfaces;
using Gym.Core.Models;
using Gym.UI.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Gym.UI.ViewModels
{
    public partial class VisitViewModel : BaseViewModel
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;

        public VisitViewModel(IUnitofWork unitOfWork, IMapper mapper, ILocalizationService localizationService) 
            : base(localizationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            UpdateLocalizedLabels();
            _ = LoadVisits();
        }

        [ObservableProperty]
        private ObservableCollection<Visit> _visits = new();

        [ObservableProperty]
        private Visit? _selectedVisit;

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

        [ObservableProperty]
        private string _traineeId = string.Empty;

        [ObservableProperty]
        private DateTime _visitDate = DateTime.Now;

        [RelayCommand]
        private async Task LoadVisits()
        {
            try
            {
                IsBusy = true;
                var visits = await _unitOfWork.VisitRepository.GetAllAsync();
                Visits.Clear();
                foreach (var visit in visits.Where(v => !v.IsDeleted).OrderByDescending(v => v.VisitDate))
                {
                    Visits.Add(visit);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetLocalizedString("ErrorLoadingVisits").Replace("{0}", ex.Message), 
                    GetLocalizedString("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(GetLocalizedString("PleaseEnterTraineeId"), 
                    GetLocalizedString("WarningTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TraineeId, out int traineeIdInt))
            {
                MessageBox.Show("Please enter a valid trainee ID.", 
                    GetLocalizedString("WarningTitle"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                IsBusy = true;
                
                // Check if trainee exists
                var trainee = await _unitOfWork.TraineeRepository.GetByIdAsync(traineeIdInt);
                if (trainee == null || trainee.IsDeleted)
                {
                    MessageBox.Show(GetLocalizedString("TraineeNotFound"), 
                        GetLocalizedString("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var visit = new Visit
                {
                    TraineeId = traineeIdInt,
                    VisitDate = VisitDate
                };

                await _unitOfWork.VisitRepository.AddAsync(visit);
                
                MessageBox.Show(GetLocalizedString("VisitRecordedSuccess"), 
                    GetLocalizedString("SuccessTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                await LoadVisits();
            }
            catch (Exception ex)
            {
                MessageBox.Show(GetLocalizedString("ErrorRecordingVisit").Replace("{0}", ex.Message), 
                    GetLocalizedString("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteVisit(Visit? visit)
        {
            if (visit == null) return;

            var result = MessageBox.Show(GetLocalizedString("ConfirmDeleteVisit"), 
                GetLocalizedString("ConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    IsBusy = true;
                    visit.IsDeleted = true;
                    visit.UpdatedAt = DateTime.Now;
                    await _unitOfWork.VisitRepository.UpdateAsync(visit);
                    
                    MessageBox.Show(GetLocalizedString("VisitDeletedSuccess"), 
                        GetLocalizedString("SuccessTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadVisits();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(GetLocalizedString("ErrorDeletingVisit").Replace("{0}", ex.Message), 
                        GetLocalizedString("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
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
        }
    }
}
