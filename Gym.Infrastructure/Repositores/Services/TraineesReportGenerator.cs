using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Gym.Core.Models;

namespace Gym.Infrastructure.Services
{
    public class TraineesReportGenerator
    {
        public static void GenerateReport(IReadOnlyList<Trainee> trainees, string filePath)
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
                            column.Item().AlignRight().Text("تقرير المتدربين").FontSize(16).SemiBold();
                            column.Item().AlignRight().Text($"تاريخ التقرير: {DateTime.Now:dd/MM/yyyy}").FontSize(10);
                            column.Item().AlignRight().Text($"إجمالي المتدربين: {trainees.Count}").FontSize(10).FontColor(Colors.Green.Medium);
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

                        // جدول المتدربين
                        column.Item().Element(ComposeTraineesTable);
                    });
                }

                void ComposeStatistics(IContainer container)
                {
                    container.Background(Colors.Grey.Lighten4).Padding(10).Column(column =>
                    {
                        column.Item().AlignRight().Text("إحصائيات المتدربين").FontSize(14).SemiBold();
                        column.Item().PaddingVertical(5);
                        
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"إجمالي المتدربين: {trainees.Count}");
                            row.RelativeItem().AlignRight().Text($"المتدربين النشطين: {trainees.Count}");
                        });
                    });
                }

                void ComposeTraineesTable(IContainer container)
                {
                    container.Table(table =>
                    {
                        // تعريف الأعمدة
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // الرقم
                            columns.RelativeColumn(1); // رقم المتدرب
                            columns.RelativeColumn(3); // الاسم الكامل
                            columns.RelativeColumn(2); // رقم الهاتف
                            columns.RelativeColumn(2); // تاريخ التسجيل
                            columns.RelativeColumn(1); // الحالة
                        });

                        // رأس الجدول
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).AlignRight().Text("الرقم");
                            header.Cell().Element(CellStyle).AlignRight().Text("رقم المتدرب");
                            header.Cell().Element(CellStyle).AlignRight().Text("الاسم الكامل");
                            header.Cell().Element(CellStyle).AlignRight().Text("رقم الهاتف");
                            header.Cell().Element(CellStyle).AlignRight().Text("تاريخ التسجيل");
                            header.Cell().Element(CellStyle).AlignRight().Text("الحالة");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        // بيانات الجدول
                        foreach (var (trainee, index) in trainees.Select((t, i) => (t, i)))
                        {
                            table.Cell().Element(CellStyle).AlignRight().Text($"{index + 1}");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{trainee.Id}");
                            table.Cell().Element(CellStyle).AlignRight().Text(trainee.FullName ?? "غير محدد");
                            table.Cell().Element(CellStyle).AlignRight().Text(trainee.PhoneNumber ?? "غير محدد");
                            table.Cell().Element(CellStyle).AlignRight().Text(trainee.JoinDate.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle).AlignRight().Text("نشط")
                                .FontColor(Colors.Green.Medium);

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