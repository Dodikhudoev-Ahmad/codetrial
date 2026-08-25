using CodeTrail.Application.Admin.Dtos;

namespace CodeTrail.Application.Admin;

public interface IAdminQuestionService
{
    Task<AdminQuestionDetailDto> GetQuestionAsync(Guid id);

    Task<AdminQuestionDetailDto> CreateQuestionAsync(UpsertQuestionRequest request);

    Task<AdminQuestionDetailDto> UpdateQuestionAsync(Guid id, UpsertQuestionRequest request);

    Task DeleteQuestionAsync(Guid id);
}
