namespace TaskFlow.Api.Models;

// Статус задачи. В БД хранится как число (0, 1, 2).
public enum TaskItemStatus
{
    New = 0,        // Новая
    InProgress = 1, // В работе
    Done = 2        // Выполнена
}
