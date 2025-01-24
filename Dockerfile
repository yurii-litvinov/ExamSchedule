FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ExamSchedule/ExamSchedule.csproj", "ExamSchedule/"]
COPY ["ScheduleParser/ScheduleParser.csproj", "ScheduleParser/"]
COPY ["TimetableAdapter/TimetableAdapter.csproj", "TimetableAdapter/"]
COPY ["ReportGenerator/ReportGenerator.csproj", "ReportGenerator/"]
COPY ["ExamScheduleTests/ExamScheduleTests.csproj", "ExamScheduleTests/"]
RUN dotnet restore "ExamSchedule/ExamSchedule.csproj"

COPY . .
WORKDIR "/src/ExamSchedule"
RUN dotnet build "ExamSchedule.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ExamSchedule.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY ReportGenerator/report-template.docx /ReportGenerator/report-template.docx
ENTRYPOINT ["dotnet", "ExamSchedule.dll"]
