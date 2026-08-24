using System.Net;
using CodeTrail.Application.Common.Exceptions;

namespace CodeTrail.Application.Courses.Exceptions;

public class CourseNotFoundException(string identifier)
    : AppException("Course not found", $"Course '{identifier}' was not found.", HttpStatusCode.NotFound);
