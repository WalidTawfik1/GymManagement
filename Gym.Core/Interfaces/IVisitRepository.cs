using Gym.Core.DTO;
using Gym.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Interfaces
{
    public interface IVisitRepository:IGenericRepository<Visit>
    {
        Task<int> GetTodayVisitsCountAsync();
        Task<VisitResponseDTO?> AddVisitAsync(int traineeId);
        Task<bool> HasActiveMembershipAsync(int traineeId);
        Task<bool> DecrementRemainingSessionsAsync(int traineeId, bool saveChanges = true);
        Task<bool> IsTraineeCheckedInAsync(int traineeId);
        Task<IReadOnlyList<VisitDTO>> GetAllVisitsAsync();
        Task<IReadOnlyList<VisitResponseDTO>> GetAllVisitsWithResponseAsync();
        Task<IReadOnlyList<VisitDTO>> GetTodayVisits(DateOnly today);
        Task<int> GetVisitsCountByMonthAsync(int month, int year);
    }
}
