namespace CodeTrail.Application.Courses.Dtos;

public class EnrollmentDto
{
    public Guid CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
}
