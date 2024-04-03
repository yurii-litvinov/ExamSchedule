// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

var pathToTable = args[0];
if (!File.Exists(pathToTable))
{
    Console.WriteLine("Неверный путь до таблицы");
    return;
}

string? pathToFile = null;
if (args.Length > 1)
{
    pathToFile = args[1];
    if (!File.Exists(pathToFile))
    {
        Console.WriteLine("Неверный путь до файла индивидуального графика");
        return;
    }
}

try
{
    await using var fileStream = pathToFile != null ? new FileStream(pathToFile, FileMode.Open) : null;
    await using var spreadSheetStream = new FileStream(pathToTable, FileMode.Open);
    var result = await new ScheduleParser.ScheduleParser(fileStream).ParseToTable(spreadSheetStream);
    Console.ForegroundColor = result == "Successfully" ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine($"\n=====   {result}   =====\n");
    Console.ResetColor();
}
catch (FileNotFoundException e)
{
    Console.WriteLine(e.Message);
}