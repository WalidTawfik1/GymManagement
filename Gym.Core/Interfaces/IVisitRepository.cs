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
        Task<bool> AddVisitAsync(int traineeId);
        Task<bool> HasActiveMembershipAsync(int traineeId);
        Task<bool> DecrementRemainingSessionsAsync(int traineeId);
        Task<bool> IsTraineeCheckedInAsync(int traineeId);
    }
}
