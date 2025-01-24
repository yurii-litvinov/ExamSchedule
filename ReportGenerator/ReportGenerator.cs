// <copyright file="ReportGenerator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ReportGenerator;

using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

/// <summary>
/// Report generator.
/// </summary>
public class ReportGenerator
{
    /// <summary>
    /// Generate report.
    /// </summary>
    /// <param name="examDtos">Exam dtos.</param>
    /// <returns>Returns report.</returns>
    /// <exception cref="Exception">Tags for repeat not found.</exception>
    public Stream GenerateReport(IEnumerable<ExamDto> examDtos)
    {
        const string templatePath = "../ReportGenerator/report-template.docx";
        const string outputPath = "../ReportGenerator/generated-report.docx";

        var data = (from exam in examDtos
            let lecturers = string.Join(", ", exam.Lecturers)
            let course = ((DateTime.Now - DateTime.Parse($"01/09/20{exam.StudentGroup[..2]}")).Days / 365) + 1
            select new Dictionary<string, string>
            {
                { "Name", exam.StudentInitials },
                { "Group", exam.StudentGroup },
                { "Course", course.ToString() },
                { "Speciality", exam.StudentGroupDescription },
                { "Exam", exam.Title },
                { "Datetime", $"{exam.DateTime.ToLocalTime():g}" },
                { "Location", exam.Location },
                { "Lecturer", lecturers },
            }).ToList();

        File.Copy(templatePath, outputPath, true);

        using (var fs = new FileStream(outputPath, FileMode.Open))
        {
            using WordprocessingDocument wordDoc = WordprocessingDocument.Open(fs, true);
            Body? body = wordDoc.MainDocumentPart?.Document.Body;

            var beginPara = body?.Elements<Paragraph>()
                .FirstOrDefault(p => p.InnerText.Contains("{{BEGIN}}"));
            var endPara = body?.Elements<Paragraph>()
                .FirstOrDefault(p => p.InnerText.Contains("{{END}}"));

            if (beginPara == null || endPara == null)
            {
                throw new Exception("Template does not contain {{BEGIN}} or {{END}} tags.");
            }

            var blockElements = new List<OpenXmlElement?>();
            for (var element = beginPara.NextSibling(); element != endPara; element = element?.NextSibling())
            {
                blockElements.Add(element?.CloneNode(true));
            }

            var beginFooter = body?.Elements<Paragraph>()
                .FirstOrDefault(p => p.InnerText.Contains("{{BEGIN_FOOTER}}"));
            var endFooter = body?.Elements<Paragraph>()
                .FirstOrDefault(p => p.InnerText.Contains("{{END_FOOTER}}"));

            if (beginFooter == null || endFooter == null)
            {
                throw new Exception("Template does not contain {{BEGIN_FOOTER}} or {{END_FOOTER}} tags.");
            }

            var footerElements = new List<OpenXmlElement?>();
            for (var element = beginFooter.NextSibling(); element != endFooter; element = element?.NextSibling())
            {
                footerElements.Add(element?.CloneNode(true));
            }

            foreach (var item in data)
            {
                foreach (var blockElement in blockElements)
                {
                    var clonedElement = blockElement?.CloneNode(true);
                    if (clonedElement == null)
                    {
                        continue;
                    }

                    ReplacePlaceholders(clonedElement, item);
                    body?.AppendChild(clonedElement);
                }
            }

            foreach (var footerElement in footerElements)
            {
                var clonedElement = footerElement?.CloneNode(true);
                body?.AppendChild(clonedElement);
            }

            OpenXmlElement? elementToRemove = beginPara;
            while (elementToRemove != endPara)
            {
                var nextElement = elementToRemove?.NextSibling();
                elementToRemove?.Remove();
                elementToRemove = nextElement;
            }

            endPara.Remove();

            elementToRemove = beginFooter;
            while (elementToRemove != endFooter)
            {
                var nextElement = elementToRemove?.NextSibling();
                elementToRemove?.Remove();
                elementToRemove = nextElement;
            }

            endFooter.Remove();

            wordDoc.MainDocumentPart?.Document.Save();
        }

        var docx = File.Open(outputPath, FileMode.Open);
        return docx;
    }

    private static void ReplacePlaceholders(OpenXmlElement element, Dictionary<string, string> data)
    {
        foreach (var key in data.Keys)
        {
            var placeholders = element.Descendants<Text>().Where(t => t.Text.Contains($"{key}"));

            foreach (var placeholder in placeholders)
            {
                placeholder.Text = placeholder.Text.Replace($"{key}", data[key]);
            }
        }
    }
}