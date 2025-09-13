using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Gym.UI.Models.Reports;
using System.Globalization;

namespace Gym.UI.Services.Reports
{
    public class FinancialReportGenerator
    {
        public static void GenerateReport(FinancialReportModel data, string filePath)
        {
            // تعيين إعدادات QuestPDF للاستخدام المجاني
            QuestPDF.Settings.License = LicenseType.Community;
            
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial Unicode MS"));

                    // الرأس
                    page.Header().Element(ComposeHeader);

                    // المحتوى
                    page.Content().Element(ComposeContent);

                    // التذييل
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" من ");
                        x.TotalPages();
                    });
                });

                void ComposeHeader(IContainer container)
                {
                    var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    container.Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().AlignRight().Text(data.CompanyName).Style(titleStyle);
                            column.Item().AlignRight().Text(data.ReportTitle).FontSize(16).SemiBold();
                            column.Item().AlignRight().Text($"الفترة: {data.MonthName} {data.Year}").FontSize(12);
                            column.Item().AlignRight().Text($"تاريخ التقرير: {data.GeneratedDate:dd/MM/yyyy}").FontSize(10);
                        });
                    });
                }

                void ComposeContent(IContainer container)
                {
                    container.PaddingVertical(40).Column(column =>
                    {
                        // الملخص المالي
                        column.Item().Element(ComposeFinancialSummary);
                        column.Item().PaddingTop(20).Element(ComposeRevenueDetails);
                        column.Item().PaddingTop(20).Element(ComposeExpenseDetails);
                        column.Item().PaddingTop(20).Element(ComposeFinancialAnalysis);
                    });
                }

                void ComposeFinancialSummary(IContainer container)
                {
                    container.Column(column =>
                    {
                        column.Item().AlignRight().Text("الملخص المالي").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Element(c => CreateFinancialCard(c, "إجمالي الإيرادات", $"{data.TotalRevenue:F2} ج.م", Colors.Green.Medium));
                            row.ConstantItem(10);
                            row.RelativeItem().Element(c => CreateFinancialCard(c, "إجمالي المصروفات", $"{data.TotalExpenses:F2} ج.م", Colors.Red.Medium));
                            row.ConstantItem(10);
                            row.RelativeItem().Element(c => CreateFinancialCard(c, "صافي الربح", $"{data.NetProfit:F2} ج.م", 
                                data.NetProfit >= 0 ? Colors.Green.Medium : Colors.Red.Medium));
                        });
                        
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Element(c => CreateFinancialCard(c, "هامش الربح", $"{data.ProfitMargin:F1}%", 
                                data.ProfitMargin >= 0 ? Colors.Blue.Medium : Colors.Orange.Medium));
                            row.ConstantItem(10);
                            row.RelativeItem().Element(c => CreateFinancialCard(c, "متوسط الإيراد لكل عضو", $"{data.AverageRevenuePerMember:F2} ج.م", Colors.Teal.Medium));
                            row.ConstantItem(10);
                            row.RelativeItem().Element(c => CreateFinancialCard(c, "إجمالي العمليات", data.TotalTransactions.ToString(), Colors.Purple.Medium));
                        });
                    });
                }

                void ComposeRevenueDetails(IContainer container)
                {
                    if (data.RevenueDetails.Any())
                    {
                        container.Column(column =>
                        {
                            column.Item().AlignRight().Text("تفاصيل الإيرادات").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                            column.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1.5f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).AlignRight().Text("النوع");
                                    header.Cell().Element(CellStyle).AlignRight().Text("الوصف");
                                    header.Cell().Element(CellStyle).AlignRight().Text("اسم المتدرب");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("المبلغ");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("التاريخ");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                                    }
                                });

                                foreach (var revenue in data.RevenueDetails.OrderByDescending(x => x.Date))
                                {
                                    table.Cell().Element(CellStyle).AlignRight().Text(revenue.Type);
                                    table.Cell().Element(CellStyle).AlignRight().Text(revenue.Description);
                                    table.Cell().Element(CellStyle).AlignRight().Text(revenue.TraineeName);
                                    table.Cell().Element(CellStyle).AlignCenter().Text($"{revenue.Amount:F2}");
                                    table.Cell().Element(CellStyle).AlignCenter().Text(revenue.Date.ToString("dd/MM"));
                                }

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            });
                        });
                    }
                }

                void ComposeExpenseDetails(IContainer container)
                {
                    if (data.ExpenseDetails.Any())
                    {
                        container.Column(column =>
                        {
                            column.Item().AlignRight().Text("تفاصيل المصروفات").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                            column.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1.5f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).AlignRight().Text("النوع");
                                    header.Cell().Element(CellStyle).AlignRight().Text("الوصف");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("المبلغ");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("التاريخ");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                                    }
                                });

                                foreach (var expense in data.ExpenseDetails.OrderByDescending(x => x.Date))
                                {
                                    table.Cell().Element(CellStyle).AlignRight().Text(expense.Type);
                                    table.Cell().Element(CellStyle).AlignRight().Text(expense.Description);
                                    table.Cell().Element(CellStyle).AlignCenter().Text($"{expense.Amount:F2}");
                                    table.Cell().Element(CellStyle).AlignCenter().Text(expense.Date.ToString("dd/MM"));
                                }

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            });
                        });
                    }
                }

                void ComposeFinancialAnalysis(IContainer container)
                {
                    container.Column(column =>
                    {
                        column.Item().AlignRight().Text("التحليل المالي").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                        column.Item().PaddingTop(10).Column(analysisColumn =>
                        {
                            // تحليل أداء الإيرادات
                            var revenueAnalysis = GetRevenueAnalysis(data);
                            analysisColumn.Item().PaddingBottom(10).Text(revenueAnalysis).FontSize(11).LineHeight(1.5f);
                            
                            // تحليل المصروفات
                            var expenseAnalysis = GetExpenseAnalysis(data);
                            analysisColumn.Item().PaddingBottom(10).Text(expenseAnalysis).FontSize(11).LineHeight(1.5f);
                            
                            // التوصيات
                            var recommendations = GetRecommendations(data);
                            analysisColumn.Item().Text(recommendations).FontSize(11).LineHeight(1.5f).FontColor(Colors.Blue.Darken1);
                        });
                    });
                }

                void CreateFinancialCard(IContainer container, string title, string value, string color)
                {
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(15).Column(column =>
                    {
                        column.Item().AlignRight().Text(title).FontSize(10).FontColor(Colors.Grey.Darken1);
                        column.Item().AlignRight().Text(value).FontSize(18).SemiBold().FontColor(color);
                    });
                }
            })
            .GeneratePdf(filePath);
        }

        private static string GetRevenueAnalysis(FinancialReportModel data)
        {
            var membershipRevenue = data.RevenueDetails.Where(r => r.Type == "عضوية").Sum(r => r.Amount);
            var serviceRevenue = data.RevenueDetails.Where(r => r.Type == "خدمة إضافية").Sum(r => r.Amount);
            
            return $"• تحليل الإيرادات: إجمالي الإيرادات {data.TotalRevenue:F2} ج.م، منها {membershipRevenue:F2} ج.م من العضويات ({(membershipRevenue / data.TotalRevenue * 100):F1}%) و {serviceRevenue:F2} ج.م من الخدمات الإضافية ({(serviceRevenue / data.TotalRevenue * 100):F1}%).";
        }

        private static string GetExpenseAnalysis(FinancialReportModel data)
        {
            var expensesByType = data.ExpenseDetails.GroupBy(e => e.Type)
                .Select(g => new { Type = g.Key, Total = g.Sum(e => e.Amount) })
                .OrderByDescending(x => x.Total);
                
            var topExpense = expensesByType.FirstOrDefault();
            
            return topExpense != null ? 
                $"• تحليل المصروفات: أكبر بند مصروفات هو '{topExpense.Type}' بمبلغ {topExpense.Total:F2} ج.م ({(topExpense.Total / data.TotalExpenses * 100):F1}% من إجمالي المصروفات)." :
                "• لا توجد مصروفات مسجلة لهذا الشهر.";
        }

        private static string GetRecommendations(FinancialReportModel data)
        {
            var recommendations = new List<string> { "التوصيات:" };
            
            if (data.NetProfit < 0)
            {
                recommendations.Add("• يُنصح بمراجعة المصروفات وتقليل التكاليف غير الضرورية لتحسين الربحية.");
            }
            else if (data.ProfitMargin < 20)
            {
                recommendations.Add("• يمكن تحسين هامش الربح عبر زيادة الأسعار أو تحسين كفاءة العمليات.");
            }
            else
            {
                recommendations.Add("• الأداء المالي جيد، يُنصح بالحفاظ على هذا المستوى والتوسع في الخدمات.");
            }
            
            var serviceRevenue = data.RevenueDetails.Where(r => r.Type == "خدمة إضافية").Sum(r => r.Amount);
            if (serviceRevenue < data.TotalRevenue * 0.3m)
            {
                recommendations.Add("• يُنصح بالتركيز على تطوير الخدمات الإضافية لزيادة الإيرادات.");
            }
            
            return string.Join("\n", recommendations);
        }
    }
}