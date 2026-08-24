namespace CodeTrail.Application.Courses.Exceptions;

public class AlreadyEnrolledException(Guid courseId) : Exception($"Already enrolled in course '{courseId}'.");
