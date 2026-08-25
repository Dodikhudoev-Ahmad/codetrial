using CodeTrail.Api.Common;
using CodeTrail.Application.Admin;
using CodeTrail.Application.Admin.Dtos;
using CodeTrail.Application.Common;
using CodeTrail.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/admin/courses")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminCoursesController(IAdminCourseService adminCourseService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminCourseListItemDto>>> GetCourses(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
        Ok(await adminCourseService.GetCoursesAsync(page, pageSize));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminCourseDetailDto>> GetCourse(Guid id) =>
        Ok(await adminCourseService.GetCourseAsync(id));

    [HttpPost]
    public async Task<ActionResult<AdminCourseDetailDto>> CreateCourse(UpsertCourseRequest request) =>
        Ok(await adminCourseService.CreateCourseAsync(User.GetUserId(), request));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminCourseDetailDto>> UpdateCourse(Guid id, UpsertCourseRequest request) =>
        Ok(await adminCourseService.UpdateCourseAsync(id, request));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        await adminCourseService.DeleteCourseAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult<CourseStatsDto>> GetStats(Guid id) =>
        Ok(await adminCourseService.GetCourseStatsAsync(id));
}
