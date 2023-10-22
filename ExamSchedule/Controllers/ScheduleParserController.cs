using Microsoft.AspNetCore.Mvc;

namespace ExamSchedule.Controllers;

[ApiController]
[Route("api/update_table")]
public class ScheduleParserController : ControllerBase
{
    [HttpPut]
    public string Put(string token, IFormFile? formFile, string filePath)
    {
        var task = new ScheduleParser.ScheduleParser(formFile?.OpenReadStream()).ParseToTable(token, filePath);
        return task.Result;
    }
}