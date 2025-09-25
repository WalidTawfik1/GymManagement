using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Gym.Core.DTO;

namespace Gym.Infrastructure.Services
{
    public class MembershipsReportGenerator
    {
        public static void GenerateReport(IReadOnlyList<MembershipDTO> memberships, string filePath)
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
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial Unicode MS"));

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
                            column.Item().AlignRight().Text("تقرير العضويات").FontSize(16).SemiBold();
                            column.Item().AlignRight().Text($"تاريخ التقرير: {DateTime.Now:dd/MM/yyyy}").FontSize(10);
                            column.Item().AlignRight().Text($"إجمالي العضويات: {memberships.Count}").FontSize(10).FontColor(Colors.Green.Medium);
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

                        // جدول العضويات
                        column.Item().Element(ComposeMembershipsTable);
                    });
                }

                void ComposeStatistics(IContainer container)
                {
                    container.Background(Colors.Grey.Lighten4).Padding(10).Column(column =>
                    {
                        column.Item().AlignRight().Text("إحصائيات العضويات").FontSize(14).SemiBold();
                        column.Item().PaddingVertical(5);
                        
                        var activeMemberships = memberships.Count(m => m.IsActive);
                        var totalRevenue = memberships.Sum(m => m.Price);
                        
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"إجمالي العضويات: {memberships.Count}");
                            row.RelativeItem().AlignRight().Text($"العضويات النشطة: {activeMemberships}");
                        });
                        
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text($"إجمالي الإيرادات: {totalRevenue:N2} ج.م");
                            row.RelativeItem().AlignRight().Text($"العضويات المنتهية: {memberships.Count - activeMemberships}");
                        });
                    });
                }

                void ComposeMembershipsTable(IContainer container)
                {
                    container.Table(table =>
                    {
                        // تعريف الأعمدة
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1); // الرقم
                            columns.RelativeColumn(2); // اسم المتدرب
                            columns.RelativeColumn(2); // نوع العضوية
                            columns.RelativeColumn(1); // السعر
                            columns.RelativeColumn(2); // تاريخ البداية
                            columns.RelativeColumn(2); // تاريخ النهاية
                            columns.RelativeColumn(1); // الحصص المتبقية
                            columns.RelativeColumn(1); // الحالة
                        });

                        // رأس الجدول
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).AlignRight().Text("م");
                            header.Cell().Element(CellStyle).AlignRight().Text("اسم المتدرب");
                            header.Cell().Element(CellStyle).AlignRight().Text("نوع العضوية");
                            header.Cell().Element(CellStyle).AlignRight().Text("السعر");
                            header.Cell().Element(CellStyle).AlignRight().Text("تاريخ البداية");
                            header.Cell().Element(CellStyle).AlignRight().Text("تاريخ النهاية");
                            header.Cell().Element(CellStyle).AlignRight().Text("الحصص المتبقية");
                            header.Cell().Element(CellStyle).AlignRight().Text("الحالة");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(3).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        // بيانات الجدول
                        foreach (var (membership, index) in memberships.Select((m, i) => (m, i)))
                        {
                            table.Cell().Element(CellStyle).AlignRight().Text($"{index + 1}");
                            table.Cell().Element(CellStyle).AlignRight().Text(membership.TraineeName ?? "غير محدد");
                            table.Cell().Element(CellStyle).AlignRight().Text(membership.MembershipType ?? "غير محدد");
                            table.Cell().Element(CellStyle).AlignRight().Text($"{membership.Price:N2} ج.م");
                            table.Cell().Element(CellStyle).AlignRight().Text(membership.StartDate.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle).AlignRight().Text(membership.EndDate.ToString("dd/MM/yyyy"));
                            table.Cell().Element(CellStyle).AlignRight().Text(membership.RemainingSessions?.ToString() ?? "غير محدود");
                            table.Cell().Element(CellStyle).AlignRight().Text(membership.IsActive ? "نشطة" : "منتهية")
                                .FontColor(membership.IsActive ? Colors.Green.Medium : Colors.Red.Medium);

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