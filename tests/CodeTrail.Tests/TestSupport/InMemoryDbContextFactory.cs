using CodeTrail.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeTrail.Tests.TestSupport;

public static class InMemoryDbContextFactory
{
    // A fresh, uniquely-named database per call keeps tests isolated from each other
    // even when they run in parallel.
    public static CodeTrailDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CodeTrailDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new CodeTrailDbContext(options);
    }
}
