// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

string? token = null;
while (token == null)
{
    Console.WriteLine(
        "Введите токен (можно получить тут https://oauth.yandex.ru/authorize?response_type=token&client_id=2076405a93374428b3f436c290b3663a):");
    token = Console.ReadLine();
}

string? pathToTable = null;
while (pathToTable == null)
{
    Console.WriteLine("Файл таблицы в браузере должен быть закрыт!");
    Console.WriteLine("Введите путь до таблицы на Диске (например, /ExamSchedule.xlsx):");
    pathToTable = Console.ReadLine();
}

string? pathToFile = null;
while (true)
{
    while (pathToFile == null)
    {
        Console.WriteLine("Введите абсолютный путь до файла индивидуального графика:");
        pathToFile = Console.ReadLine();
    }

    var fileStream = new FileStream(pathToFile, FileMode.Open);
    var result = await new ScheduleParser.ScheduleParser(fileStream).ParseToTable(token, pathToTable);
    Console.ForegroundColor = result == "File uploaded successfully." ? ConsoleColor.Green : ConsoleColor.Red;
    Console.WriteLine($"\n=====   {result}   =====\n");
    fileStream.Close();
    Console.ResetColor();
    pathToFile = null;
}