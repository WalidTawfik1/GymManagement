using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Gym.Core.DTO;

namespace Gym.Infrastructure.Services
{
    public class AdditionalServicesReportGenerator
    {
        public static void GenerateReport(IReadOnlyList<AdditionalServiceDTO> services, string filePath)
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
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial Unicode MS"));

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
                            column.Item().AlignRight().Text("تقرير الخدمات الإضافية").FontSize(16).SemiBold();
                            column.Item().AlignRight().Text($"تاريخ التقرير: {DateTime.Now:dd/MM/yyyy}").FontSize(10);
                            column.Item().AlignRight().Text($"إجمالي الخدمات: {services.Count}").FontSize(10).FontColor(Colors.Green.Medium);
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

                        // جدول الخدمات الإضافية
                        column.Item().Element(ComposeServicesTable);
                    });
                }

                void ComposeStatistics(IContainer container)
                {
                    container.Background(Colors.Grey.Lighten4).Padding(10).Column(column =>
                    {
                        column.Item().AlignRight().Text("إحصائيات الخدمات الإضافية").FontSize(14).SemiBold();
                        column.Item().PaddingVertical(5);
                        
                        var totalRevenue = services.Sum(s => s.Price);
                        var thisMonth = services.Count(s => s.TakenAt.Month == DateTime.Now.Month && s.TakenAt.Year == DateTime.Now.Year);
                        var serviceTypes = services.GroupBy(s => s.ServiceType).Count();
                        
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"إجمالي الخدمات: {services.Count}");
                            row.RelativeItem().AlignRight().Text($"خدمات هذا الشهر: {thisMonth}");
                        });
                        
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"إجمالي الإيرادات: {totalRevenue:N2} ج.م");
                            row.RelativeItem().AlignRight().Text($"أنواع الخدمات: {serviceTypes}");
                        });
                    });
                }

                void ComposeServicesTable(IContainer container)
                {
                    container.Table(table =>
                    {
                        // تعريف الأعمدة
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // الرقم
                            columns.RelativeColumn(2); // اسم المتدرب
                            columns.RelativeColumn(2); // نوع الخدمة
                            columns.RelativeColumn(1); // المدة
                            columns.RelativeColumn(1); // السعر
                            columns.RelativeColumn(2); // التاريخ
                            columns.RelativeColumn(1); // الوقت
                        });

                        // رأس الجدول
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).AlignRight().Text("م");
                            header.Cell().Element(CellStyle).AlignRight().Text("اسم المتدرب");
                            header.Cell().Element(CellStyle).AlignRight().Text("نوع الخدمة");
                            header.Cell().Element(CellStyle).AlignRight().Text("المدة (دقيقة)");
                            header.Cell().Element(CellStyle).AlignRight().Text("السعر");
                            header.Cell().Element(CellStyle).AlignRight().Text("التاريخ");
                            header.Cell().Element(CellStyle).AlignRight().Text("الوقت");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(3).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        // بيانات الجدول (مرتبة بالتاريخ الأحدث أولاً)
                        foreach (var (service, index) in services.OrderByDescending(s => s.TakenAt).Select((s, i) => (s, i)))
                        {
                            table.Cell().Element(CellStyle).AlignRight().Text($"{index + 1}");
                            table.Cell().Element(CellStyle).AlignRight().Text(service.TraineeName ?? "غير محدد");
                            table.Cell().Element(CellStyle).AlignRight().Text(service.ServiceType ?? "غير محدد");
                            table.Cell().Element(CellStyle).AlignRight().Text(service.DurationInMinutes?.ToString() ?? "غير محدد");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{service.Price:N2} ج.م");
                            table.Cell().Element(CellStyle).AlignRight().Text(service.TakenAt.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle).AlignRight().Text(service.TakenAt.ToString("HH:mm"));

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(2);
                            }
                        }
                    });
                }
            })
            .GeneratePdf(filePath);
        }
    }
}