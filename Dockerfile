FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ExamSchedule/ExamSchedule.csproj", "ExamSchedule/"]
COPY ["ScheduleParser/ScheduleParser.csproj", "ScheduleParser/"]
RUN dotnet restore "ExamSchedule/ExamSchedule.csproj"

COPY . .
WORKDIR "/src/ExamSchedule"
RUN dotnet build "ExamSchedule.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ExamSchedule.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ExamSchedule.dll"]
