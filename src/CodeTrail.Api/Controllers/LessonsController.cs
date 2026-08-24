using CodeTrail.Api.Common;
using CodeTrail.Application.Attempts;
using CodeTrail.Application.Attempts.Dtos;
using CodeTrail.Application.Lessons;
using CodeTrail.Application.Lessons.Dtos;
using CodeTrail.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/lessons")]
public class LessonsController(ILessonService lessonService, IAttemptService attemptService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<ActionResult<LessonDetailDto>> GetLesson(Guid id) =>
        Ok(await lessonService.GetLessonAsync(id, User.GetUserId()));

    [HttpPost("{id:guid}/attempts")]
    [Authorize(Roles = nameof(UserRole.Student))]
    public async Task<ActionResult<AttemptResultDto>> SubmitAttempt(Guid id, SubmitAttemptRequest request) =>
        Ok(await attemptService.SubmitAttemptAsync(id, User.GetUserId(), request));
}
