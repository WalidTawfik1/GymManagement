using Gym.Core.Models.Reports;

namespace Gym.Core.Interfaces.Services
{
    public interface IReportService
    {
        Task<string> GenerateDashboardReportAsync();
        Task<string> GenerateFinancialReportAsync(int month, int year);
        Task<DashboardReportModel> GetDashboardReportDataAsync();
        Task<FinancialReportModel> GetFinancialReportDataAsync(int month, int year);
        
        // New export methods for tables
        Task<string> ExportTraineesReportAsync();
        Task<string> ExportMembershipsReportAsync(int? month = null, int? year = null);
        Task<string> ExportVisitsReportAsync(int? month = null, int? year = null);
        Task<string> ExportAdditionalServicesReportAsync(int? month = null, int? year = null);
    }
}