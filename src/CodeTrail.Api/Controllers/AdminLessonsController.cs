using CodeTrail.Application.Admin;
using CodeTrail.Application.Admin.Dtos;
using CodeTrail.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/admin/lessons")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminLessonsController(IAdminLessonService adminLessonService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminLessonDetailDto>> GetLesson(Guid id) =>
        Ok(await adminLessonService.GetLessonAsync(id));

    [HttpPost]
    public async Task<ActionResult<AdminLessonDetailDto>> CreateLesson(UpsertLessonRequest request) =>
        Ok(await adminLessonService.CreateLessonAsync(request));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminLessonDetailDto>> UpdateLesson(Guid id, UpsertLessonRequest request) =>
        Ok(await adminLessonService.UpdateLessonAsync(id, request));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLesson(Guid id)
    {
        await adminLessonService.DeleteLessonAsync(id);
        return NoContent();
    }
}
