// Veritabaný ile Entity Framework Core köprüsü
using Microsoft.EntityFrameworkCore;
using Ankets.Entities;
using System.ComponentModel.DataAnnotations.Schema;

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
            // Tablo adlarýný küçük harflerle eþleþtir
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<UserSession>().ToTable("user_sessions");
            modelBuilder.Entity<AuditLog>().ToTable("audit_logs");
            modelBuilder.Entity<Survey>().ToTable("surveys");
            modelBuilder.Entity<SurveyCategory>().ToTable("survey_categories");
            modelBuilder.Entity<Question>().ToTable("questions");
            modelBuilder.Entity<QuestionType>().ToTable("question_types");
            modelBuilder.Entity<QuestionOption>().ToTable("question_options");
            modelBuilder.Entity<SurveyResponse>().ToTable("survey_responses");
            modelBuilder.Entity<QuestionAnswer>().ToTable("question_answers");
            modelBuilder.Entity<SurveyShare>().ToTable("survey_shares");
            modelBuilder.Entity<SurveyAnalytic>().ToTable("survey_analytics");
            modelBuilder.Entity<AiAnalysis>().ToTable("ai_analysis");
        }
    }
}