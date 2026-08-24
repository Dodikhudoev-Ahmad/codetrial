namespace CodeTrail.Application.Courses.Exceptions;

public class CourseNotFoundException(string identifier) : Exception($"Course '{identifier}' was not found.");
