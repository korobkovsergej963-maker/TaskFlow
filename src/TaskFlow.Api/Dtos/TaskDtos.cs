using System.ComponentModel.DataAnnotations;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Dtos;

public record TaskDto(
    int Id,
    int ProjectId,
    string ProjectName,
    string Title,
    string Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTime CreatedAt,
    DateTime? CompletedAt);

public record CreateTaskDto(
    [Required] int ProjectId,
    [Required, MaxLength(200)] string Title,
    [MaxLength(2000)] string? Description,
    TaskPriority Priority = TaskPriority.Medium);

public record ChangeStatusDto([Required] TaskItemStatus Status);
