namespace TaskFlow.Api.Models;

// Задача. Называется TaskItem, чтобы не путать с System.Threading.Tasks.Task. Таблица "Tasks".
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.New;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }            // null, пока задача не выполнена

    // Внешний ключ на Project
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
}
