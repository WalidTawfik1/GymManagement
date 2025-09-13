using AutoMapper;
using Gym.Core.Interfaces;
using Gym.UI.Models.Reports;
using System.Globalization;
using System.IO;

namespace Gym.UI.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly IUnitofWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _reportsDirectory;

        public ReportService(IUnitofWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            
            // إنشاء مجلد التقارير في مجلد المستندات
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _reportsDirectory = Path.Combine(documentsPath, "GymReports");
            
            if (!Directory.Exists(_reportsDirectory))
            {
                Directory.CreateDirectory(_reportsDirectory);
            }
        }

        public async Task<string> GenerateDashboardReportAsync()
        {
            var reportData = await GetDashboardReportDataAsync();
            
            var fileName = $"تقرير_لوحة_التحكم_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(_reportsDirectory, fileName);
            
            DashboardReportGenerator.GenerateReport(reportData, filePath);
            
            return filePath;
        }

        public async Task<string> GenerateFinancialReportAsync(int month, int year)
        {
            var reportData = await GetFinancialReportDataAsync(month, year);
            
            var monthName = GetMonthName(month);
            var fileName = $"التقرير_المالي_{monthName}_{year}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(_reportsDirectory, fileName);
            
            FinancialReportGenerator.GenerateReport(reportData, filePath);
            
            return filePath;
        }

        public async Task<DashboardReportModel> GetDashboardReportDataAsync()
        {
            var dashboardData = await _unitOfWork.DashboardRepository.GetDashboardDataAsync();
            
            var reportModel = new DashboardReportModel
            {
                GeneratedDate = DateTime.Now,
                TotalActiveMembers = dashboardData.TotalActiveMembers,
                TotalVisitsThisMonth = dashboardData.TotalVisitsThisMonth,
                MembershipsEndingSoon = dashboardData.MembershipsEndingSoon,
                NetProfitThisMonth = dashboardData.NetProfitThisMonth,
                TotalRevenueThisMonth = dashboardData.TotalRevenueThisMonth,
                TotalExpensesThisMonth = dashboardData.TotalExpensesThisMonth,
                NewMembersThisMonth = dashboardData.NewMembersThisMonth,
                TotalTrainees = dashboardData.TotalTrainees,
                RevenueGrowthPercentage = dashboardData.MonthlyComparison.RevenueGrowthPercentage,
                ProfitGrowthPercentage = dashboardData.MonthlyComparison.ProfitGrowthPercentage,
                MemberGrowthPercentage = dashboardData.MonthlyComparison.MemberGrowthPercentage,
                VisitGrowthPercentage = dashboardData.MonthlyComparison.VisitGrowthPercentage,
                OneMonthMemberships = dashboardData.MembershipDistribution.OneMonthMemberships,
                ThreeMonthMemberships = dashboardData.MembershipDistribution.ThreeMonthMemberships,
                TwelveSessionMemberships = dashboardData.MembershipDistribution.TwelveSessionMemberships,
                CurrentMonthName = GetMonthName(DateTime.Now.Month),
                LastMonthName = GetMonthName(DateTime.Now.Month == 1 ? 12 : DateTime.Now.Month - 1)
            };

            // تحويل العضويات المنتهية قريباً
            foreach (var membership in dashboardData.UpcomingExpirations)
            {
                reportModel.UpcomingExpirations.Add(new MembershipExpirationItem
                {
                    TraineeName = membership.TraineeName,
                    MembershipType = membership.MembershipType,
                    EndDate = membership.EndDate.ToDateTime(TimeOnly.MinValue),
                    RemainingVisits = membership.RemainingSessions
                });
            }

            return reportModel;
        }

        public async Task<FinancialReportModel> GetFinancialReportDataAsync(int month, int year)
        {
            var reportModel = new FinancialReportModel
            {
                GeneratedDate = DateTime.Now,
                Month = month,
                Year = year,
                MonthName = GetMonthName(month)
            };

            // الحصول على البيانات المالية
            reportModel.TotalRevenue = await _unitOfWork.ExpenseAndRevenueRepository.GetTotalRevenueByMonthAsync(month);
            reportModel.TotalExpenses = await _unitOfWork.ExpenseAndRevenueRepository.GetTotalExpensesByMonthAsync(month);
            reportModel.NetProfit = reportModel.TotalRevenue - reportModel.TotalExpenses;

            // الحصول على تفاصيل المصروفات
            var expenses = await _unitOfWork.ExpenseAndRevenueRepository.GetExpensesByMonthAsync(month, year);
            foreach (var expense in expenses)
            {
                reportModel.ExpenseDetails.Add(new ExpenseItem
                {
                    Type = expense.ExpenseType,
                    Description = expense.Description ?? "",
                    Amount = expense.Amount,
                    Date = expense.IncurredAt
                });
            }

            // الحصول على تفاصيل الإيرادات من العضويات والخدمات الإضافية
            await LoadRevenueDetails(reportModel, month, year);
            
            // حساب متوسط الإيراد لكل عضو
            var totalMembers = await _unitOfWork.DashboardRepository.GetTotalActiveMembersAsync();
            reportModel.AverageRevenuePerMember = totalMembers > 0 ? reportModel.TotalRevenue / totalMembers : 0;

            return reportModel;
        }

        private Task LoadRevenueDetails(FinancialReportModel reportModel, int month, int year)
        {
            try
            {
                // يمكن تطوير هذا لاحقاً للحصول على تفاصيل الإيرادات من قاعدة البيانات
                // الآن سنضع بيانات تجريبية أو نتركها فارغة
                
                // إضافة بيانات تجريبية للعرض
                if (reportModel.TotalRevenue > 0)
                {
                    reportModel.RevenueDetails.Add(new RevenueItem
                    {
                        Type = "عضوية",
                        Description = "إجمالي إيرادات العضويات",
                        Amount = reportModel.TotalRevenue * 0.8m, // افتراض أن 80% من الإيرادات من العضويات
                        Date = new DateTime(year, month, 15),
                        TraineeName = "مجموع العضويات"
                    });

                    reportModel.RevenueDetails.Add(new RevenueItem
                    {
                        Type = "خدمة إضافية",
                        Description = "إجمالي إيرادات الخدمات الإضافية",
                        Amount = reportModel.TotalRevenue * 0.2m, // افتراض أن 20% من الإيرادات من الخدمات
                        Date = new DateTime(year, month, 15),
                        TraineeName = "مجموع الخدمات"
                    });
                }
            }
            catch (Exception)
            {
                // في حالة الخطأ، نترك قائمة الإيرادات فارغة
            }
            
            return Task.CompletedTask;
        }

        private string GetMonthName(int month)
        {
            var monthNames = new[]
            {
                "", "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
            };

            return month >= 1 && month <= 12 ? monthNames[month] : "";
        }
    }
}