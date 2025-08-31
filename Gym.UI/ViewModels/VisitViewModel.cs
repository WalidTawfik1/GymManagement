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

                var visitResponse = await _unitOfWork.VisitRepository.AddVisitAsync(traineeIdInt);

                if (visitResponse != null)
                {
                    // Display the membership information after successful visit
                    LastVisitTraineeName = visitResponse.TraineeName;
                    LastVisitMembershipType = visitResponse.MembershipType;
                    LastVisitRemainingSessions = visitResponse.RemainingSessions;
                    LastVisitIsActive = visitResponse.IsActive;
                    LastVisitMessage = visitResponse.Message;
                    ShowLastVisitInfo = true;

                    // Clear the trainee ID for next visit
                    TraineeId = string.Empty;
                    VisitDate = DateTime.Now;

                    MessageBox.Show(visitResponse.Message, 
                        GetLocalizedString("SuccessTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Reload visits to show the new visit
                    await LoadVisits();
                }
                else
                {
                    MessageBox.Show(GetLocalizedString("ErrorRecordingVisit"), 
                        GetLocalizedString("ErrorTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
        private async Task DeleteVisit(VisitDTO? visitDto)
        {
            if (visitDto == null) return;

            var result = MessageBox.Show(GetLocalizedString("ConfirmDeleteVisit"), 
                GetLocalizedString("ConfirmTitle"), MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
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
                        
                        MessageBox.Show(GetLocalizedString("VisitDeletedSuccess"), 
                            GetLocalizedString("SuccessTitle"), MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadVisits();
                    }
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
            ShowLastVisitInfo = false;
            LastVisitTraineeName = string.Empty;
            LastVisitMembershipType = string.Empty;
            LastVisitRemainingSessions = null;
            LastVisitIsActive = false;
            LastVisitMessage = string.Empty;
        }
    }
}
