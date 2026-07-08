using Gym.Core.DTO;
using Gym.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Interfaces
{
    public interface IAdditionalServiceRepository
    {
        Task<bool> AddAdditionalServiceAsync(AddAdditionalServiceDTO additionalServiceDTO);
        Task<bool> UpdateAdditionalServiceAsync(AdditionalServiceDTO additionalServiceDTO);
        Task<IReadOnlyList<AdditionalServiceDTO>> GetAllAdditionalServicesAsync(int? month = null, int? year = null);
        Task<PagedResult<AdditionalServiceDTO>> GetAdditionalServicesPagedAsync(int pageNumber, int pageSize, int? month = null, int? year = null, string searchQuery = "");
        Task<IReadOnlyList<AdditionalServiceDTO>> GetAdditionalServiceByTraineeIdAsync(int traineeId);
        Task<bool> DeleteAdditionalServiceAsync(int id);
        Task<IReadOnlyList<AdditionalServiceDTO>> GetAdditionalServicesByMonthAsync(int month, int year);
    }
}
