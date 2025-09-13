using Gym.UI.Models.Reports;

namespace Gym.UI.Services.Reports
{
    public interface IReportService
    {
        /// <summary>
        /// إنشاء تقرير لوحة التحكم وحفظه كملف PDF
        /// </summary>
        /// <returns>مسار الملف المحفوظ</returns>
        Task<string> GenerateDashboardReportAsync();
        
        /// <summary>
        /// إنشاء التقرير المالي وحفظه كملف PDF
        /// </summary>
        /// <param name="month">الشهر (1-12)</param>
        /// <param name="year">السنة</param>
        /// <returns>مسار الملف المحفوظ</returns>
        Task<string> GenerateFinancialReportAsync(int month, int year);
        
        /// <summary>
        /// الحصول على بيانات تقرير لوحة التحكم
        /// </summary>
        /// <returns>نموذج بيانات تقرير لوحة التحكم</returns>
        Task<DashboardReportModel> GetDashboardReportDataAsync();
        
        /// <summary>
        /// الحصول على بيانات التقرير المالي
        /// </summary>
        /// <param name="month">الشهر (1-12)</param>
        /// <param name="year">السنة</param>
        /// <returns>نموذج بيانات التقرير المالي</returns>
        Task<FinancialReportModel> GetFinancialReportDataAsync(int month, int year);
    }
}