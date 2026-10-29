using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Data;

// Контекст EF Core: через него идут все запросы к PostgreSQL.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Уникальный индекс по ключу проекта: быстрый поиск + запрет дублей
        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Key)
            .IsUnique();

        modelBuilder.Entity<Project>().Property(p => p.Name).HasMaxLength(200);
        modelBuilder.Entity<Project>().Property(p => p.Key).HasMaxLength(10);

        modelBuilder.Entity<TaskItem>().ToTable("Tasks");
        modelBuilder.Entity<TaskItem>().Property(t => t.Title).HasMaxLength(200);

        // Связь: в проекте много задач; удаляем проект -> удаляются его задачи
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
