using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Courses.Exceptions;

public class AlreadyEnrolledException(Guid courseId)
    : AppException("Already enrolled", $"Already enrolled in course '{courseId}'.", HttpStatusCode.Conflict);
