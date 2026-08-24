namespace CodeTrail.Application.Lessons.Exceptions;

public class NotEnrolledException(Guid courseId)
    : Exception($"You must enroll in course '{courseId}' before accessing its lessons.");
