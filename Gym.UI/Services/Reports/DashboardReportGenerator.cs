using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Gym.UI.Models.Reports;
using System.Globalization;

namespace Gym.UI.Services.Reports
{
    public class DashboardReportGenerator
    {
        public static void GenerateReport(DashboardReportModel data, string filePath)
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
                            column.Item().AlignRight().Text($"تاريخ التقرير: {data.GeneratedDate:dd/MM/yyyy}").FontSize(10);
                        });
                    });
                }

                void ComposeContent(IContainer container)
                {
                    container.PaddingVertical(40).Column(column =>
                    {
                        // المؤشرات الرئيسية
                        column.Item().Element(ComposeKeyMetrics);
                        column.Item().PaddingTop(20).Element(ComposeGrowthMetrics);
                        column.Item().PaddingTop(20).Element(ComposeMembershipDistribution);
                        column.Item().PaddingTop(20).Element(ComposeUpcomingExpirations);
                    });
                }

                void ComposeKeyMetrics(IContainer container)
                {
                    container.Column(column =>
                    {
                        column.Item().AlignRight().Text("المؤشرات الرئيسية").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            // العمود الأول
                            row.RelativeItem().Element(c => CreateMetricCard(c, "إجمالي الأعضاء النشطين", data.TotalActiveMembers.ToString(), Colors.Green.Medium));
                            row.ConstantItem(10);
                            row.RelativeItem().Element(c => CreateMetricCard(c, "الزيارات هذا الشهر", data.TotalVisitsThisMonth.ToString(), Colors.Blue.Medium));
                        });
                        
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            // العمود الثاني
                            row.RelativeItem().Element(c => CreateMetricCard(c, "العضويات المنتهية قريباً", data.MembershipsEndingSoon.ToString(), Colors.Orange.Medium));
                            row.ConstantItem(10);
                            row.RelativeItem().Element(c => CreateMetricCard(c, "أعضاء جدد هذا الشهر", data.NewMembersThisMonth.ToString(), Colors.Purple.Medium));
                        });

                        column.Item().PaddingTop(10).Row(row =>
                        {
                            // العمود الثالث - المالية
                            row.RelativeItem().Element(c => CreateMetricCard(c, "صافي الربح", $"{data.NetProfitThisMonth:F2} ج.م", 
                                data.NetProfitThisMonth >= 0 ? Colors.Green.Medium : Colors.Red.Medium));
                            row.ConstantItem(10);
                            row.RelativeItem().Element(c => CreateMetricCard(c, "إجمالي الإيرادات", $"{data.TotalRevenueThisMonth:F2} ج.م", Colors.Teal.Medium));
                        });
                    });
                }

                void ComposeGrowthMetrics(IContainer container)
                {
                    container.Column(column =>
                    {
                        column.Item().AlignRight().Text("مؤشرات النمو الشهري").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Element(c => CreateGrowthCard(c, "نمو الإيرادات", data.RevenueGrowthPercentage));
                            row.ConstantItem(10);
                            row.RelativeItem().Element(c => CreateGrowthCard(c, "نمو الأرباح", data.ProfitGrowthPercentage));
                        });
                        
                        column.Item().PaddingTop(10).Row(row =>
                        {
                            row.RelativeItem().Element(c => CreateGrowthCard(c, "نمو الأعضاء", data.MemberGrowthPercentage));
                            row.ConstantItem(10);
                            row.RelativeItem().Element(c => CreateGrowthCard(c, "نمو الزيارات", data.VisitGrowthPercentage));
                        });
                    });
                }

                void ComposeMembershipDistribution(IContainer container)
                {
                    container.Column(column =>
                    {
                        column.Item().AlignRight().Text("توزيع العضويات").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).AlignRight().Text("نوع العضوية");
                                header.Cell().Element(CellStyle).AlignCenter().Text("العدد");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                                }
                            });

                            table.Cell().Element(CellStyle).AlignRight().Text("عضوية شهر واحد");
                            table.Cell().Element(CellStyle).AlignCenter().Text(data.OneMonthMemberships.ToString());

                            table.Cell().Element(CellStyle).AlignRight().Text("عضوية ثلاثة أشهر");
                            table.Cell().Element(CellStyle).AlignCenter().Text(data.ThreeMonthMemberships.ToString());

                            table.Cell().Element(CellStyle).AlignRight().Text("عضوية 12 جلسة");
                            table.Cell().Element(CellStyle).AlignCenter().Text(data.TwelveSessionMemberships.ToString());

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                            }
                        });
                    });
                }

                void ComposeUpcomingExpirations(IContainer container)
                {
                    if (data.UpcomingExpirations.Any())
                    {
                        container.Column(column =>
                        {
                            column.Item().AlignRight().Text("العضويات المنتهية قريباً").FontSize(16).SemiBold().FontColor(Colors.Blue.Medium);
                            column.Item().PaddingTop(10).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).AlignRight().Text("اسم المتدرب");
                                    header.Cell().Element(CellStyle).AlignRight().Text("نوع العضوية");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("تاريخ الانتهاء");
                                    header.Cell().Element(CellStyle).AlignCenter().Text("الجلسات المتبقية");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                                    }
                                });

                                foreach (var expiration in data.UpcomingExpirations.Take(10))
                                {
                                    table.Cell().Element(CellStyle).AlignRight().Text(expiration.TraineeName);
                                    table.Cell().Element(CellStyle).AlignRight().Text(expiration.MembershipType);
                                    table.Cell().Element(CellStyle).AlignCenter().Text(expiration.EndDate.ToString("dd/MM/yyyy"));
                                    table.Cell().Element(CellStyle).AlignCenter().Text(expiration.RemainingVisits?.ToString() ?? "-");
                                }

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                }
                            });
                        });
                    }
                }

                void CreateMetricCard(IContainer container, string title, string value, string color)
                {
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(15).Column(column =>
                    {
                        column.Item().AlignRight().Text(title).FontSize(10).FontColor(Colors.Grey.Darken1);
                        column.Item().AlignRight().Text(value).FontSize(18).SemiBold().FontColor(color);
                    });
                }

                void CreateGrowthCard(IContainer container, string title, decimal percentage)
                {
                    var color = percentage >= 0 ? Colors.Green.Medium : Colors.Red.Medium;
                    var arrow = percentage >= 0 ? "↗" : "↘";
                    
                    container.Border(1).BorderColor(Colors.Grey.Lighten1).Padding(15).Column(column =>
                    {
                        column.Item().AlignRight().Text(title).FontSize(10).FontColor(Colors.Grey.Darken1);
                        column.Item().AlignRight().Text($"{arrow} {Math.Abs(percentage):F1}%").FontSize(16).SemiBold().FontColor(color);
                    });
                }
            })
            .GeneratePdf(filePath);
        }
    }
}