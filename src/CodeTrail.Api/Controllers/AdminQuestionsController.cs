using CodeTrail.Application.Admin;
using CodeTrail.Application.Admin.Dtos;
using CodeTrail.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeTrail.Api.Controllers;

[ApiController]
[Route("api/admin/questions")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminQuestionsController(IAdminQuestionService adminQuestionService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminQuestionDetailDto>> GetQuestion(Guid id) =>
        Ok(await adminQuestionService.GetQuestionAsync(id));

    [HttpPost]
    public async Task<ActionResult<AdminQuestionDetailDto>> CreateQuestion(UpsertQuestionRequest request) =>
        Ok(await adminQuestionService.CreateQuestionAsync(request));

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AdminQuestionDetailDto>> UpdateQuestion(Guid id, UpsertQuestionRequest request) =>
        Ok(await adminQuestionService.UpdateQuestionAsync(id, request));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteQuestion(Guid id)
    {
        await adminQuestionService.DeleteQuestionAsync(id);
        return NoContent();
    }
}
