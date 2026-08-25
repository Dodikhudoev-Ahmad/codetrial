using CodeTrail.Application.Admin;
using CodeTrail.Application.Attempts;
using CodeTrail.Application.Attempts.AnswerCheckers;
using CodeTrail.Application.Auth;
using CodeTrail.Application.Courses;
using CodeTrail.Application.Leaderboard;
using CodeTrail.Application.Lessons;
using CodeTrail.Application.Options;
using CodeTrail.Application.Profile;
using CodeTrail.Infrastructure.Admin;
using CodeTrail.Infrastructure.Attempts;
using CodeTrail.Infrastructure.Auth;
using CodeTrail.Infrastructure.Courses;
using CodeTrail.Infrastructure.Leaderboard;
using CodeTrail.Infrastructure.Lessons;
using CodeTrail.Infrastructure.Persistence;
using CodeTrail.Infrastructure.Profile;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeTrail.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<CodeTrailDbContext>(options =>
            options.UseSqlite(connectionString));

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<ILessonAccessGuard, LessonAccessGuard>();

        services.AddScoped<IAnswerChecker, SingleChoiceAnswerChecker>();
        services.AddScoped<IAnswerChecker, MultiChoiceAnswerChecker>();
        services.AddScoped<IAnswerChecker, ShortAnswerAnswerChecker>();
        services.AddScoped<IAnswerCheckerResolver, AnswerCheckerResolver>();
        services.AddScoped<IAttemptService, AttemptService>();

        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();

        services.AddScoped<IAdminCourseService, AdminCourseService>();
        services.AddScoped<IAdminLessonService, AdminLessonService>();
        services.AddScoped<IAdminQuestionService, AdminQuestionService>();

        return services;
    }
}
