using Gym.Core.DTO;
using Gym.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Interfaces
{
    public interface ITraineeRepository : IGenericRepository<Trainee>
    {
        Task<bool> AddTraineeAsync(TraineeDTO trainee);
        Task<bool> UpdateTraineeAsync(UpdateTraineeDTO trainee);
        Task<IReadOnlyList<Trainee>> GetTraineeByNameAsync(string name);
        Task<IReadOnlyList<Trainee>> GetAllTraineesAsync();
    }
}
