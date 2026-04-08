using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ICTMasterSuite.Infrastructure.Services;

/// <summary>PDF institucional com cabeçalho, seções, tabela, rodapé e paginação (QuestPDF).</summary>
public static class InstitutionalPdfDocumentBuilder
{
    public sealed record Definition(
        string ReportTitle,
        string Subtitle,
        string SectionTitle,
        IReadOnlyList<string> ColumnHeaders,
        IReadOnlyList<string[]> Rows,
        DateTimeOffset GeneratedAtUtc);

    public static void GeneratePdf(string filePath, Definition definition)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(ComposeHeader);

                page.Content().Column(column =>
                {
                    column.Spacing(8);
                    column.Item().Text(definition.ReportTitle).FontSize(18).Bold().FontColor("#0F172A");
                    column.Item().Text(definition.Subtitle).FontSize(10).FontColor("#475569");
                    column.Item().Text($"Gerado em (UTC): {definition.GeneratedAtUtc:yyyy-MM-dd HH:mm:ss}")
                        .FontSize(8).FontColor("#64748B");

                    column.Item().PaddingTop(12).Text(definition.SectionTitle).FontSize(12).Bold().FontColor("#0F172A");

                    column.Item().PaddingTop(6).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in definition.ColumnHeaders)
                            {
                                columns.RelativeColumn();
                            }
                        });

                        table.Header(header =>
                        {
                            foreach (var h in definition.ColumnHeaders)
                            {
                                header.Cell().Background("#E2E8F0").Border(0.5f).BorderColor("#CBD5E1").Padding(6)
                                    .Text(h).Bold().FontSize(8).FontColor("#0F172A");
                            }
                        });

                        var rowIndex = 0;
                        foreach (var row in definition.Rows)
                        {
                            var bg = rowIndex % 2 == 0 ? "#FFFFFF" : "#F8FAFC";
                            for (var i = 0; i < definition.ColumnHeaders.Count; i++)
                            {
                                var text = i < row.Length ? row[i] : string.Empty;
                                table.Cell().Background(bg).Border(0.5f).BorderColor("#E2E8F0").Padding(5)
                                    .Text(text).FontSize(8).FontColor("#1E293B");
                            }

                            rowIndex++;
                        }
                    });
                });

                page.Footer().Element(ComposeFooter);
            });
        }).GeneratePdf(filePath);
    }

    private static void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Background("#0F172A").Padding(14).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("ICT Master Suite").FontColor("#F8FAFC").Bold().FontSize(13);
                    col.Item().Text("Relatório técnico institucional").FontColor("#94A3B8").FontSize(9);
                });
                row.ConstantItem(100).AlignRight().Text("Enterprise").FontColor("#64748B").FontSize(9);
            });
            column.Item().Height(3).Background("#0EA5E9");
        });
    }

    private static void ComposeFooter(IContainer container)
    {
        container.PaddingTop(10).Column(column =>
        {
            column.Item().LineHorizontal(0.5f).LineColor("#CBD5E1");
            column.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text("ICT Master Suite · Uso interno · Documento gerado eletronicamente")
                    .FontSize(7).FontColor("#94A3B8");
                row.ConstantItem(120).AlignRight().Text(text =>
                {
                    text.Span("Pág. ").FontSize(7).FontColor("#94A3B8");
                    text.CurrentPageNumber().FontSize(7).FontColor("#94A3B8");
                    text.Span(" / ").FontSize(7).FontColor("#94A3B8");
                    text.TotalPages().FontSize(7).FontColor("#94A3B8");
                });
            });
        });
    }
}
