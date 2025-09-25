using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Gym.Core.DTO;

namespace Gym.Infrastructure.Services
{
    public class VisitsReportGenerator
    {
        public static void GenerateReport(IReadOnlyList<VisitDTO> visits, string filePath)
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
                            column.Item().AlignRight().Text("نظام إدارة الجيم").Style(titleStyle);
                            column.Item().AlignRight().Text("تقرير الزيارات").FontSize(16).SemiBold();
                            column.Item().AlignRight().Text($"تاريخ التقرير: {DateTime.Now:dd/MM/yyyy}").FontSize(10);
                            column.Item().AlignRight().Text($"إجمالي الزيارات: {visits.Count}").FontSize(10).FontColor(Colors.Green.Medium);
                        });
                    });
                }

                void ComposeContent(IContainer container)
                {
                    container.PaddingVertical(1, Unit.Centimetre).Column(column =>
                    {
                        // إحصائيات سريعة
                        column.Item().Element(ComposeStatistics);
                        
                        column.Item().PaddingVertical(5);

                        // جدول الزيارات
                        column.Item().Element(ComposeVisitsTable);
                    });
                }

                void ComposeStatistics(IContainer container)
                {
                    container.Background(Colors.Grey.Lighten4).Padding(10).Column(column =>
                    {
                        column.Item().AlignRight().Text("إحصائيات الزيارات").FontSize(14).SemiBold();
                        column.Item().PaddingVertical(5);
                        
                        var thisMonth = visits.Count(v => v.VisitDate.Month == DateTime.Now.Month && v.VisitDate.Year == DateTime.Now.Year);
                        var thisWeek = visits.Count(v => v.VisitDate >= DateTime.Now.AddDays(-7));
                        var today = visits.Count(v => v.VisitDate.Date == DateTime.Today);
                        
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"إجمالي الزيارات: {visits.Count}");
                            row.RelativeItem().AlignRight().Text($"زيارات هذا الشهر: {thisMonth}");
                        });
                        
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"زيارات هذا الأسبوع: {thisWeek}");
                            row.RelativeItem().AlignRight().Text($"زيارات اليوم: {today}");
                        });
                    });
                }

                void ComposeVisitsTable(IContainer container)
                {
                    container.Table(table =>
                    {
                        // تعريف الأعمدة
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // الرقم
                            columns.RelativeColumn(3); // اسم المتدرب
                            columns.RelativeColumn(2); // تاريخ الزيارة
                            columns.RelativeColumn(2); // وقت الزيارة
                        });

                        // رأس الجدول
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).AlignRight().Text("الرقم");
                            header.Cell().Element(CellStyle).AlignRight().Text("اسم المتدرب");
                            header.Cell().Element(CellStyle).AlignRight().Text("تاريخ الزيارة");
                            header.Cell().Element(CellStyle).AlignRight().Text("وقت الزيارة");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        // بيانات الجدول (مرتبة بالتاريخ الأحدث أولاً)
                        foreach (var (visit, index) in visits.OrderByDescending(v => v.VisitDate).Select((v, i) => (v, i)))
                        {
                            table.Cell().Element(CellStyle).AlignRight().Text($"{index + 1}");
                            table.Cell().Element(CellStyle).AlignRight().Text(visit.TraineeName ?? "غير محدد");
                            table.Cell().Element(CellStyle).AlignRight().Text(visit.VisitDate.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle).AlignRight().Text(visit.VisitDate.ToString("HH:mm"));

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(3);
                            }
                        }
                    });
                }
            })
            .GeneratePdf(filePath);
        }
    }
}