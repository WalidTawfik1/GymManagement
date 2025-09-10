using Gym.Core.DTO;
using System.Threading.Tasks;

namespace Gym.Core.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardDTO> GetDashboardDataAsync();
        Task<int> GetTotalActiveMembersAsync();
        Task<int> GetTotalVisitsThisMonthAsync();
        Task<int> GetMembershipsEndingSoonAsync();
        Task<decimal> GetNetProfitThisMonthAsync();
        Task<decimal> GetNetProfitLastMonthAsync();
        Task<MembershipDistributionDTO> GetMembershipDistributionAsync();
        Task<MonthlyComparisonDTO> GetMonthlyComparisonAsync();
    }
}
