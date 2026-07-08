using Gym.Core.DTO;
using Gym.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Interfaces
{
    public interface IMembershipRepository: IGenericRepository<Membership>
    {
        Task<bool> AddMembership(AddMembershipDTO membershipDTO);
        Task<bool> UpdateMembership(MembershipDTO membershipDTO);
        Task<IReadOnlyList<MembershipDTO>> GetAllMembershipsAsync(int? month = null, int? year = null);
        Task<PagedResult<MembershipDTO>> GetMembershipsPagedAsync(int pageNumber, int pageSize, int? month = null, int? year = null, string searchQuery = "", string sortOrder = "Newest");
        Task<(int TotalCount, int ActiveCount, decimal TotalRevenue)> GetMembershipsSummaryAsync(int month, int year);
        Task<MembershipDTO?> GetMembershipByTraineeIdAsync(int id);
        Task<IReadOnlyList<MembershipDTO>> GetMembershipsByMonthAsync(int month, int year);
        Task<IReadOnlyList<MembershipDTO>> GetNearFinishMemberships();
        Task DeactivateExpiredMembershipsAsync();
    }
}
