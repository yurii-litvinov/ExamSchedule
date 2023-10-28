namespace ScheduleParser;

using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

/// <summary>
/// Class for parsing a schedule
/// </summary>
public class ScheduleParser
{
    private readonly DataTable _dataTable;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduleParser"/> class.
    /// Parse .docx file with schedule to <see cref="DataTable"/>
    /// </summary>
    /// <param name="stream">Stream of .docx file, which contains a schedule.</param>
    public ScheduleParser(Stream? stream)
    {
        _dataTable = new DataTable();
        _dataTable.Columns.Add("Студент", typeof(string));
        _dataTable.Columns.Add("Дисциплина", typeof(string));
        _dataTable.Columns.Add("Группа", typeof(string));
        _dataTable.Columns.Add("Экзамен", typeof(string));
        _dataTable.Columns.Add("Пересдача", typeof(string));
        _dataTable.Columns.Add("Комиссия", typeof(string));
        if (stream == null)
        {
            return;
        }

        ParseNewFile(stream);
    }

    /// <summary>
    /// Parse .docx file with schedule to <see cref="DataTable"/>
    /// </summary>
    /// <param name="stream">Stream of .docx file, which contains a schedule</param>
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
                var aboutExam = row.Skip(3).Select(cell => cell.InnerText).ToList();
                _dataTable.Rows
                    .Add(parList[0], aboutExam[0], parList[2], aboutExam[^3], aboutExam[^2], aboutExam[^1]);
            }
        }
    }

    /// <summary>
    /// Download table from <a href="https://disk.yandex.ru">Yandex.Disk</a>,
    /// update it with values from <see cref="DataTable"/> and upload back
    /// </summary>
    /// <param name="token">OAuth token.</param>
    /// <param name="path">Path to table from the root of <a href="https://disk.yandex.ru">Yandex.Disk</a>.</param>
    /// <returns>Result message.</returns>
    public async Task<string> ParseToTable(string token, string path)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", "OAuth " + token);
        var downloadLinkResult =
            await client.GetAsync("https://cloud-api.yandex.net/v1/disk/resources/download?path=" + path);
        if (!downloadLinkResult.IsSuccessStatusCode)
        {
            return await downloadLinkResult.Content.ReadAsStringAsync();
        }

        var downloadResponse = await downloadLinkResult.Content.ReadAsAsync<ParserResponse>();
        if (downloadResponse.Href == null)
        {
            return "Error handled on getting download link";
        }

        using var spreadSheetStream = new MemoryStream();
        await using (var downloadedStream = await client.GetStreamAsync(downloadResponse.Href))
        {
            await downloadedStream.CopyToAsync(spreadSheetStream);
        }

        var fillingMessage = FillSpreadSheet(spreadSheetStream);
        if (fillingMessage != null)
        {
            return fillingMessage;
        }

        var uploadLinkResult =
            await client.GetAsync("https://cloud-api.yandex.net/v1/disk/resources/upload?path=" +
                                  path
                                  + "&overwrite=true");
        if (!uploadLinkResult.IsSuccessStatusCode)
        {
            return await uploadLinkResult.Content.ReadAsStringAsync();
        }

        var uploadResponse = await uploadLinkResult.Content.ReadAsAsync<ParserResponse>();
        try
        {
            using StreamContent streamContent = new StreamContent(spreadSheetStream);
            using HttpResponseMessage response = await client.PutAsync(uploadResponse.Href, streamContent);
            return response.IsSuccessStatusCode
                ? "File uploaded successfully."
                : $"Error: {response.StatusCode} - {response.ReasonPhrase}";
        }
        catch (Exception ex)
        {
            return $"An error occurred: {ex.Message}";
        }
    }

    /// <summary>
    /// Fill table with data from <see cref="DataTable"/>
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

        ExportRowsToDataTable(worksheetPart.Worksheet.Descendants<Row>());
        var sheetData = new SheetData();
        worksheetPart.Worksheet = new Worksheet(sheetData);

        var headerRow = new Row();

        var columns = new List<string>();
        foreach (DataColumn column in _dataTable.Columns)
        {
            columns.Add(column.ColumnName);
            var cell = new Cell();
            var run1 = new DocumentFormat.OpenXml.Spreadsheet.Run();
            run1.Append(new DocumentFormat.OpenXml.Spreadsheet.Text(column.ColumnName));
            var run1Properties = new DocumentFormat.OpenXml.Spreadsheet.RunProperties();
            run1Properties.Append(new DocumentFormat.OpenXml.Spreadsheet.Bold());
            run1.RunProperties = run1Properties;
            var inlineString = new InlineString(run1);
            cell.Append(inlineString);
            headerRow.AppendChild(cell);
        }

        sheetData.AppendChild(headerRow);

        foreach (DataRow row in _dataTable.Rows)
        {
            var newRow = new Row();
            foreach (var col in columns)
            {
                var cell = new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(row[col].ToString() ?? "-")
                };
                newRow.AppendChild(cell);
            }

            sheetData.AppendChild(newRow);
        }

        workbookPart.Workbook.Save();
        return null;
    }

    /// <summary>
    /// Export rows to <see cref="DataTable"/>
    /// </summary>
    /// <param name="rows">Rows from table.</param>
    private void ExportRowsToDataTable(IEnumerable<Row> rows)
    {
        foreach (Row row in rows.Skip(1))
        {
            DataRow tempRow = _dataTable.NewRow();
            var cells = row.Descendants<Cell>().ToList();

            if (!cells.Any())
            {
                continue;
            }

            if (cells.First().InnerText == string.Empty || _dataTable.AsEnumerable().Any(r =>
                    r.Field<string>("Студент") == cells[0].InnerText &&
                    r.Field<string>("Дисциплина") == cells[1].InnerText &&
                    r.Field<string>("Группа") == cells[2].InnerText))
            {
                continue;
            }

            for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
            {
                var cell = row.Descendants<Cell>().ElementAt(i);
                tempRow[i] = cell.InnerText;
            }

            _dataTable.Rows.Add(tempRow);
        }
    }
}