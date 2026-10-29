using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Dtos;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Controllers;

// Задачи: /api/tasks
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db) => _db = db;

    // GET /api/tasks?status=New&projectId=1
    [HttpGet]
    public async Task<ActionResult<List<TaskDto>>> GetAll(
        [FromQuery] TaskItemStatus? status, [FromQuery] int? projectId)
    {
        var query = _db.Tasks.AsNoTracking().AsQueryable();

        if (status is not null) query = query.Where(t => t.Status == status);
        if (projectId is not null) query = query.Where(t => t.ProjectId == projectId);

        // t.Project!.Name в Select — EF сделает JOIN с таблицей Projects
        var result = await query
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.CreatedAt)
            .Select(t => new TaskDto(
                t.Id, t.ProjectId, t.Project!.Name, t.Title, t.Description,
                t.Status, t.Priority, t.CreatedAt, t.CompletedAt))
            .ToListAsync();

        return Ok(result);
    }

    // GET /api/tasks/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetById(int id)
    {
        var dto = await _db.Tasks.AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TaskDto(
                t.Id, t.ProjectId, t.Project!.Name, t.Title, t.Description,
                t.Status, t.Priority, t.CreatedAt, t.CompletedAt))
            .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Ok(dto);
    }

    // POST /api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create(CreateTaskDto input)
    {
        var project = await _db.Projects.FindAsync(input.ProjectId);
        if (project is null)
            return BadRequest($"Проект с id={input.ProjectId} не найден"); // 400

        var entity = new TaskItem
        {
            ProjectId = input.ProjectId,
            Title = input.Title,
            Description = input.Description ?? string.Empty,
            Priority = input.Priority
        };

        _db.Tasks.Add(entity);
        await _db.SaveChangesAsync();

        var dto = new TaskDto(entity.Id, project.Id, project.Name, entity.Title, entity.Description,
            entity.Status, entity.Priority, entity.CreatedAt, entity.CompletedAt);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
    }

    // PATCH /api/tasks/5/status   body: { "status": "Done" }
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(int id, ChangeStatusDto input)
    {
        var entity = await _db.Tasks.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Status = input.Status;
        entity.CompletedAt = input.Status == TaskItemStatus.Done ? DateTime.UtcNow : null;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/tasks/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Tasks.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Tasks.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
