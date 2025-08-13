// Veritabaný ile Entity Framework Core köprüsü
using Microsoft.EntityFrameworkCore;
using Ankets.Entities;

namespace Ankets.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyCategory> SurveyCategories { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public DbSet<SurveyShare> SurveyShares { get; set; }
        public DbSet<SurveyAnalytic> SurveyAnalytics { get; set; }
        public DbSet<AiAnalysis> AiAnalyses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Ýliþkiler ve ek konfigürasyonlar burada yapýlabilir.
        }
    }
}