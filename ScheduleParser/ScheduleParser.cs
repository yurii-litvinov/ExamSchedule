// <copyright file="ScheduleParser.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

namespace ScheduleParser;

using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

/// <summary>
/// Class for parsing a schedule.
/// </summary>
public class ScheduleParser
{
    // ReSharper disable once InconsistentNaming
    private DataTable dataTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleParser"/> class.
    /// Parse .docx file with schedule to <see cref="DataTable"/>.
    /// </summary>
    /// <param name="stream">Stream of .docx file, which contains a schedule.</param>
    public ScheduleParser(Stream? stream)
    {
        this.dataTable = new DataTable();
        this.dataTable.Columns.Add("Студент", typeof(string));
        this.dataTable.Columns.Add("Дисциплина", typeof(string));
        this.dataTable.Columns.Add("Группа", typeof(string));
        this.dataTable.Columns.Add("Экзамен", typeof(string));
        this.dataTable.Columns.Add("Пересдача", typeof(string));
        this.dataTable.Columns.Add("Комиссия", typeof(string));
        if (stream == null)
        {
            return;
        }

        this.ParseNewFile(stream);
    }

    /// <summary>
    /// Parse .docx file with schedule to <see cref="DataTable"/>.
    /// </summary>
    /// <param name="stream">Stream of .docx file, which contains a schedule.</param>
    public void ParseNewFile(Stream stream)
    {
        using WordprocessingDocument doc = WordprocessingDocument.Open(stream, false);
        MainDocumentPart? mainPart = doc.MainDocumentPart;

        if (mainPart?.Document.Body == null)
        {
            return;
        }

        var paragraphs = mainPart.Document.Body.Elements<Paragraph>()
            .Where(p => p.InnerText.Contains("группы")).ToList();
        var tables = mainPart.Document.Body.Elements<DocumentFormat.OpenXml.Wordprocessing.Table>().ToList();

        for (var i = 0; i < paragraphs.Count; i++)
        {
            var parList = paragraphs[i].InnerText.Split(", ");
            foreach (var row in tables[i + 1].Elements<TableRow>().ToList()
                         .Where(row => !row.InnerText.Contains("Дата")))
            {
                var aboutExam = row.Skip(3).Take(2).Select(cell => cell.InnerText).ToList();
                foreach (var examCell in row.Skip(5))
                {
                    var examCellText = examCell.InnerText;
                    try
                    {
                        var par = examCell.Descendants<Paragraph>().First(p => p.InnerText != string.Empty);
                        var lecturer =
                            par.Descendants<DocumentFormat.OpenXml.Wordprocessing.Run>()
                                .Where(r => r.InnerText != string.Empty).ToList().Last().InnerText;

                        aboutExam.Add(
                            string.Concat(examCellText.Take(examCellText.IndexOf(lecturer, StringComparison.Ordinal))) +
                            " " + lecturer);
                    }
                    catch
                    {
                        aboutExam.Add(string.Empty);
                    }
                }

                this.dataTable.Rows
                    .Add(parList[0], aboutExam[0], parList[2], aboutExam[^3], aboutExam[^2], aboutExam[^1]);
            }
        }
    }

    /// <summary>
    /// Open Excel table,
    /// update it with values from <see cref="DataTable"/> and save.
    /// </summary>
    /// <param name="spreadSheetStream">Excel file stream.</param>
    /// <returns>Result message.</returns>
    public Task<string> ParseToTable(Stream spreadSheetStream)
    {
        var fillingMessage = this.FillSpreadSheet(spreadSheetStream);
        if (fillingMessage != null)
        {
            return Task.FromResult(fillingMessage);
        }

        spreadSheetStream.Seek(0, SeekOrigin.Begin);
        return Task.FromResult("Successfully");
    }

    private static TimeSpan SortingFunction(DataRow row)
    {
        var examCells = new List<string?>
            { row["Экзамен"].ToString(), row["Пересдача"].ToString(), row["Комиссия"].ToString() };
        var timeSpans = examCells.Where(s => !string.IsNullOrEmpty(s))
            .Select(
                s =>
                    DateTime.ParseExact(
                        string.Join(" ", s?.Split().Take(2).ToList() ?? new List<string>()),
                        "dd.MM.yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture).Subtract(DateTime.Now)).ToList();
        if (timeSpans.Count == 0)
        {
            return TimeSpan.MaxValue;
        }

        if (timeSpans.All(span => span < TimeSpan.Zero))
        {
            return timeSpans.Min();
        }

        return timeSpans.Where(span => span >= TimeSpan.Zero).Min();
    }

    /// <summary>
    /// Fill table with data from <see cref="DataTable"/>.
    /// </summary>
    /// <param name="spreadSheetStream">Stream of Spreadsheet.</param>
    /// <returns>Error message or null.</returns>
    private string? FillSpreadSheet(Stream spreadSheetStream)
    {
        using SpreadsheetDocument doc = SpreadsheetDocument.Open(spreadSheetStream, true);
        WorkbookPart? workbookPart = doc.WorkbookPart;
        if (workbookPart == null)
        {
            return "Error handled on getting WorkbookPart of table";
        }

        var worksheetPart = workbookPart.WorksheetParts.First();

        this.ExportRowsToDataTable(
            worksheetPart.Worksheet.Descendants<Row>(),
            workbookPart.SharedStringTablePart?.SharedStringTable);

        try
        {
            var dataRows = this.dataTable.Select().OrderBy(SortingFunction).ToArray();
            this.dataTable = dataRows.CopyToDataTable();
        }
        catch (FormatException exception)
        {
            return "An error occurred: " + exception.Message;
        }

        var sheetData = new SheetData();
        worksheetPart.Worksheet = new Worksheet(sheetData);

        var headerRow = new Row();

        var columns = new List<string>();
        foreach (DataColumn column in this.dataTable.Columns)
        {
            columns.Add(column.ColumnName);
            var run1 = new DocumentFormat.OpenXml.Spreadsheet.Run();
            run1.Append(new DocumentFormat.OpenXml.Spreadsheet.Text(column.ColumnName));
            var run1Properties = new DocumentFormat.OpenXml.Spreadsheet.RunProperties();
            run1Properties.Append(new DocumentFormat.OpenXml.Spreadsheet.Bold());
            run1.RunProperties = run1Properties;
            var cell = new Cell
            {
                DataType = CellValues.InlineString,
                InlineString = new InlineString(run1),
            };

            headerRow.AppendChild(cell);
        }

        sheetData.AppendChild(headerRow);

        foreach (DataRow row in this.dataTable.Rows)
        {
            var newRow = new Row();
            foreach (var col in columns)
            {
                var cell = new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(row[col].ToString() ?? "-"),
                };
                newRow.AppendChild(cell);
            }

            sheetData.AppendChild(newRow);
        }

        workbookPart.Workbook.Save();
        return null;
    }

    /// <summary>
    /// Export rows to <see cref="DataTable"/>.
    /// </summary>
    /// <param name="rows">Rows from table.</param>
    /// /// <param name="sharedStringTable">Shared string table.</param>
    private void ExportRowsToDataTable(IEnumerable<Row> rows, SharedStringTable? sharedStringTable)
    {
        foreach (Row row in rows.Skip(1))
        {
            DataRow tempRow = this.dataTable.NewRow();
            var cells = row.Descendants<Cell>().ToList();

            if (!cells.Any())
            {
                continue;
            }

            for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
            {
                var cell = row.Descendants<Cell>().ElementAt(i);
                if (cell.CellValue == null)
                {
                    return;
                }

                var value = cell.CellValue.InnerXml;

                if (cell.DataType != null && cell.DataType == CellValues.SharedString && sharedStringTable != null)
                {
                    tempRow[i] = sharedStringTable.ChildElements[int.Parse(value)].InnerText;
                }
                else
                {
                    tempRow[i] = value;
                }
            }

            if (tempRow.Field<string>(0) == string.Empty || this.dataTable.AsEnumerable().Any(
                    r =>
                        r.Field<string?>("Студент") == tempRow.Field<string>(0) &&
                        r.Field<string>("Дисциплина") == tempRow.Field<string>(1) &&
                        r.Field<string>("Группа") == tempRow.Field<string>(2)))
            {
                continue;
            }

            this.dataTable.Rows.Add(tempRow);
        }
    }
}