using Gym.Core.DTO;

namespace Gym.Core.Models.Reports
{
    public class DashboardReportModel
    {
        public string CompanyName { get; set; } = "Champions Mambela Gym";
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
        public string ReportTitle { get; set; } = "تقرير لوحة التحكم";
        
        // المؤشرات الرئيسية
        public int TotalActiveMembers { get; set; }
        public int TotalVisitsThisMonth { get; set; }
        public int MembershipsEndingSoon { get; set; }
        public decimal NetProfitThisMonth { get; set; }
        public decimal TotalRevenueThisMonth { get; set; }
        public decimal TotalExpensesThisMonth { get; set; }
        public int NewMembersThisMonth { get; set; }
        public int TotalTrainees { get; set; }
        
        // معلومات النمو
        public decimal RevenueGrowthPercentage { get; set; }
        public decimal ProfitGrowthPercentage { get; set; }
        public decimal MemberGrowthPercentage { get; set; }
        public decimal VisitGrowthPercentage { get; set; }
        
        // توزيع العضويات
        public int OneMonthMemberships { get; set; }
        public int ThreeMonthMemberships { get; set; }
        public int TwelveSessionMemberships { get; set; }
        
        // العضويات المنتهية قريباً
        public List<MembershipExpirationItem> UpcomingExpirations { get; set; } = new();
        
        // ملخص شهري
        public string CurrentMonthName { get; set; } = string.Empty;
        public string LastMonthName { get; set; } = string.Empty;
    }
    
    public class MembershipExpirationItem
    {
        public string TraineeName { get; set; } = string.Empty;
        public string MembershipType { get; set; } = string.Empty;
        public DateTime EndDate { get; set; }
        public int? RemainingVisits { get; set; }
    }
}