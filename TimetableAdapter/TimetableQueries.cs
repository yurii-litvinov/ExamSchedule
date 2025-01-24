// <copyright file="TimetableQueries.cs" company="Gleb Kargin">
// Copyright (c) Gleb Kargin. All rights reserved.
// </copyright>

// ReSharper disable RedundantNameQualifier
namespace TimetableAdapter;

using TimetableAdapter.Models.Classroom;
using TimetableAdapter.Models.Educator;
using TimetableAdapter.Models.Group;

/// <summary>
/// Timetable queries.
/// </summary>
public class TimetableQueries
{
    /// <summary>
    /// Gets educators timetable.
    /// </summary>
    /// <param name="educatorInitials">Educator initials.</param>
    /// <returns>Returns educators timetable.</returns>
    public async Task<EducatorEventsDay[]> GetEducatorsTimetable(string educatorInitials)
    {
        var surname = educatorInitials.Split()[0];
        using var client = new HttpClient();
        var educatorsResponseMessage =
            await client.GetAsync("https://timetable.spbu.ru/api/v1/educators/search/" + surname);
        if (!educatorsResponseMessage.IsSuccessStatusCode)
        {
            return [];
        }

        var educators = await educatorsResponseMessage.Content.ReadAsAsync<EducatorsResponse>();
        var educatorId = educators.Educators.FirstOrDefault(educator => educator.FullName == educatorInitials)?.Id;
        if (educatorId == null)
        {
            return [];
        }

        var educatorTimetableResponseMessage =
            await client.GetAsync($"https://timetable.spbu.ru/api/v1/educators/{educatorId}/events");

        if (!educatorTimetableResponseMessage.IsSuccessStatusCode)
        {
            return [];
        }

        var educatorTimetable = await educatorTimetableResponseMessage.Content.ReadAsAsync<EducatorTimetableResponse>();

        return educatorTimetable.EducatorEventsDays;
    }

    /// <summary>
    /// Gets classrooms timetable.
    /// </summary>
    /// <param name="location">Location of exam.</param>
    /// <param name="startDate">Start date of timetable.</param>
    /// <param name="endDate">End date of timetable.</param>
    /// <returns>Classroom timetable.</returns>
    public async Task<ClassroomEventsDay[]> GetClassroomsTimetable(string location, string startDate, string endDate)
    {
        using var client = new HttpClient();
        var addressesResponseMessage =
            await client.GetAsync("https://timetable.spbu.ru/api/v1/addresses/");
        if (!addressesResponseMessage.IsSuccessStatusCode)
        {
            return [];
        }

        var addresses = await addressesResponseMessage.Content.ReadAsAsync<AddressesResponse[]>();
        var address = addresses.FirstOrDefault(addressResponse => location.Contains(addressResponse.DisplayName1));
        var addressId = address?.Oid;
        if (addressId == null)
        {
            return [];
        }

        var classroomsResponseMessage =
            await client.GetAsync("https://timetable.spbu.ru/api/v1/addresses/" + addressId + "/classrooms/");
        if (!classroomsResponseMessage.IsSuccessStatusCode)
        {
            return [];
        }

        var classrooms = await classroomsResponseMessage.Content.ReadAsAsync<ClassroomsResponse[]>();

        var classroomId = classrooms
            .FirstOrDefault(classroom => location == $"{address?.DisplayName1}, {classroom.DisplayName1}")?.Oid;
        if (classroomId == null)
        {
            return [];
        }

        var classroomTimetableResponseMessage =
            await client.GetAsync(
                $"https://timetable.spbu.ru/api/v1/classrooms/{classroomId}/events/{startDate}/{endDate}/");

        if (!classroomTimetableResponseMessage.IsSuccessStatusCode)
        {
            return [];
        }

        var classroomTimetable =
            await classroomTimetableResponseMessage.Content.ReadAsAsync<ClassroomTimetableResponse>();

        return classroomTimetable.ClassroomEventsDays;
    }

    /// <summary>
    /// Gets group timetable.
    /// </summary>
    /// <param name="studentGroupOid">Student group oid.</param>
    /// <param name="startDate">Start date.</param>
    /// <returns>Returns group timetable.</returns>
    public async Task<Day[]> GetGroupTimetable(int studentGroupOid, string startDate = "")
    {
        using var client = new HttpClient();

        var groupTimetableResponseMessage =
            await client.GetAsync($"https://timetable.spbu.ru/api/v1/groups/{studentGroupOid}/events/{startDate}");

        if (!groupTimetableResponseMessage.IsSuccessStatusCode)
        {
            return [];
        }

        var groupTimetable = await groupTimetableResponseMessage.Content.ReadAsAsync<GroupTimetableResponse>();

        return groupTimetable.Days;
    }
}