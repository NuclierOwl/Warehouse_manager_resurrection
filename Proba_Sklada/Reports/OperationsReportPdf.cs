using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Proba_Sklada.Reports;

public static class OperationsReportPdf
{
    public static byte[] BuildPdf(
        IReadOnlyList<ReportRow> rows,
        DateOnly start,
        DateOnly end,
        ReportOperationFilter filter)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var culture = CultureInfo.GetCultureInfo("ru-RU");
        var title = "Отчёт по операциям склада";
        var subtitle = $"{start:dd.MM.yyyy} – {end:dd.MM.yyyy} • {FilterTitle(filter)}";

        var totalQty = rows.Sum(r => r.Quantity);
        var totalSum = rows.Sum(r => r.TotalPrice);

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(24);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Column(col =>
                    {
                        col.Item().Text(title).FontSize(16).SemiBold();
                        col.Item().Text(subtitle).FontSize(10).FontColor(Colors.Grey.Darken2);
                        col.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                page.Content().PaddingTop(10).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(70);  // date
                            columns.ConstantColumn(60);  // op
                            columns.ConstantColumn(70);  // doc
                            columns.RelativeColumn(2);   // counterparty
                            columns.RelativeColumn(3);   // product
                            columns.ConstantColumn(45);  // qty
                            columns.ConstantColumn(55);  // price
                            columns.ConstantColumn(65);  // total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellHeader).Text("Дата");
                            header.Cell().Element(CellHeader).Text("Опер.");
                            header.Cell().Element(CellHeader).Text("Док-т");
                            header.Cell().Element(CellHeader).Text("Контрагент");
                            header.Cell().Element(CellHeader).Text("Товар");
                            header.Cell().Element(CellHeader).AlignRight().Text("Кол-во");
                            header.Cell().Element(CellHeader).AlignRight().Text("Цена");
                            header.Cell().Element(CellHeader).AlignRight().Text("Сумма");
                        });

                        foreach (var r in rows.OrderBy(x => x.DateTime))
                        {
                            table.Cell().Element(CellBody).Text(r.DateTime.ToString("dd.MM.yyyy", culture));
                            table.Cell().Element(CellBody).Text(r.Operation);
                            table.Cell().Element(CellBody).Text(r.DocumentNumber);
                            table.Cell().Element(CellBody).Text(r.Counterparty);
                            table.Cell().Element(CellBody).Text(r.Product);
                            table.Cell().Element(CellBody).AlignRight().Text(r.Quantity.ToString(culture));
                            table.Cell().Element(CellBody).AlignRight().Text(r.UnitPrice.ToString("N2", culture));
                            table.Cell().Element(CellBody).AlignRight().Text(r.TotalPrice.ToString("N2", culture));
                        }
                    });

                    col.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().AlignRight().Text($"Итого позиций: {rows.Count}");
                        row.ConstantItem(12);
                        row.RelativeItem().AlignRight().Text($"Итого кол-во: {totalQty.ToString(culture)}");
                        row.ConstantItem(12);
                        row.RelativeItem().AlignRight().Text($"Итого сумма: {totalSum.ToString("N2", culture)}");
                    });
                });

                page.Footer()
                    .AlignRight()
                    .Text(x =>
                    {
                        x.Span("Страница ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
            });
        });

        return doc.GeneratePdf();
    }

    public static void SavePdf(
        string filePath,
        IReadOnlyList<ReportRow> rows,
        DateOnly start,
        DateOnly end,
        ReportOperationFilter filter)
    {
        var bytes = BuildPdf(rows, start, end, filter);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");
        File.WriteAllBytes(filePath, bytes);
    }

    private static string FilterTitle(ReportOperationFilter filter) =>
        filter switch
        {
            ReportOperationFilter.Incoming => "Приход",
            ReportOperationFilter.Outgoing => "Отгрузка",
            ReportOperationFilter.InternalMovement => "Внутренние перемещения",
            _ => "Все операции"
        };

    private static IContainer CellHeader(IContainer container) =>
        container.DefaultTextStyle(x => x.SemiBold())
            .PaddingVertical(6)
            .PaddingHorizontal(4)
            .Background(Colors.Grey.Lighten3)
            .Border(1)
            .BorderColor(Colors.Grey.Lighten1);

    private static IContainer CellBody(IContainer container) =>
        container.PaddingVertical(4)
            .PaddingHorizontal(4)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2);
}

