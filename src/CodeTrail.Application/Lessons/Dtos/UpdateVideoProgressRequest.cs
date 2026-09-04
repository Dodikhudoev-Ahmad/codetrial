using System.ComponentModel.DataAnnotations;

namespace CodeTrail.Application.Lessons.Dtos;

public class UpdateVideoProgressRequest
{
    [Range(0, 100)]
    public int WatchedPercent { get; set; }
}
