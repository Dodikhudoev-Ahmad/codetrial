using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Admin.Exceptions;

public class CourseTitleOrSlugInUseException()
    : AppException(
        "Title or slug already in use",
        "A course with this title or slug already exists.",
        HttpStatusCode.Conflict);
