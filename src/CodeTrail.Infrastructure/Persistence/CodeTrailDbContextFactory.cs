using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CodeTrail.Infrastructure.Persistence;

// Used only by `dotnet ef migrations add` to build the model at design time.
// The real connection string is supplied via configuration at runtime.
public class CodeTrailDbContextFactory : IDesignTimeDbContextFactory<CodeTrailDbContext>
{
    public CodeTrailDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CodeTrailDbContext>();
        optionsBuilder.UseSqlite("Data Source=codetrail.db");

        return new CodeTrailDbContext(optionsBuilder.Options);
    }
}
