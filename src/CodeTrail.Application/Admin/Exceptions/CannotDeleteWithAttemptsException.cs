using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Admin.Exceptions;

public class CannotDeleteWithAttemptsException(string resource, Guid id)
    : AppException(
        "Cannot delete",
        $"{resource} '{id}' cannot be deleted: students already have attempts recorded against it.",
        HttpStatusCode.Conflict);
