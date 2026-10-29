namespace TaskFlow.Api.Models;

// Проект, в котором лежат задачи. Таблица "Projects".
public class Project
{
    public int Id { get; set; }                          // Первичный ключ
    public string Name { get; set; } = string.Empty;     // "Сайт портфолио"
    public string Key { get; set; } = string.Empty;      // Короткий уникальный код: "WEB"
    public string Description { get; set; } = string.Empty;

    // Навигационное свойство: все задачи проекта (связь один-ко-многим)
    public List<TaskItem> Tasks { get; set; } = new();
}
