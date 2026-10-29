using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Dtos;

// DTO — то, что API принимает и отдаёт. Сущности БД наружу не светим.

public record ProjectDto(int Id, string Name, string Key, string Description, int OpenTasks);

public record CreateProjectDto(
    [Required, MaxLength(200)] string Name,
    [Required, MaxLength(10)] string Key,
    [MaxLength(1000)] string? Description);

public record UpdateProjectDto(
    [Required, MaxLength(200)] string Name,
    [MaxLength(1000)] string? Description);
