namespace Gym.Core.Models.Reports
{
    public class FinancialReportModel
    {
        public string CompanyName { get; set; } = "Mambela Gym";
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
        public string ReportTitle { get; set; } = "التقرير المالي";
        
        // معلومات الفترة
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
        
        // الملخص المالي
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin => TotalRevenue > 0 ? (NetProfit / TotalRevenue) * 100 : 0;
        
        // تفاصيل الإيرادات
        public List<RevenueItem> RevenueDetails { get; set; } = new();
        
        // تفاصيل المصروفات  
        public List<ExpenseItem> ExpenseDetails { get; set; } = new();
        
        // إحصائيات إضافية
        public decimal AverageRevenuePerMember { get; set; }
        public int TotalTransactions => RevenueDetails.Count + ExpenseDetails.Count;
    }
    
    public class RevenueItem
    {
        public string Type { get; set; } = string.Empty; // عضوية أو خدمة إضافية
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string TraineeName { get; set; } = string.Empty;
    }
    
    public class ExpenseItem
    {
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}