using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.Dtos;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Controllers;

// CRUD для проектов: /api/projects
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db) => _db = db;

    // GET /api/projects?search=сайт
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll([FromQuery] string? search)
    {
        var query = _db.Projects.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || p.Key.Contains(search));

        // LINQ-запрос, EF превращает его в SQL с подзапросом COUNT
        var result = await query
            .OrderBy(p => p.Name)
            .Select(p => new ProjectDto(
                p.Id, p.Name, p.Key, p.Description,
                p.Tasks.Count(t => t.Status != TaskItemStatus.Done)))
            .ToListAsync();

        return Ok(result);
    }

    // GET /api/projects/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var dto = await _db.Projects.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProjectDto(
                p.Id, p.Name, p.Key, p.Description,
                p.Tasks.Count(t => t.Status != TaskItemStatus.Done)))
            .FirstOrDefaultAsync();

        return dto is null ? NotFound() : Ok(dto); // 404 или 200
    }

    // POST /api/projects
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto input)
    {
        var key = input.Key.ToUpperInvariant();

        if (await _db.Projects.AnyAsync(p => p.Key == key))
            return Conflict($"Проект с ключом {key} уже есть"); // 409

        var entity = new Project
        {
            Name = input.Name,
            Key = key,
            Description = input.Description ?? string.Empty
        };

        _db.Projects.Add(entity);
        await _db.SaveChangesAsync();

        var dto = new ProjectDto(entity.Id, entity.Name, entity.Key, entity.Description, 0);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto); // 201
    }

    // PUT /api/projects/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateProjectDto input)
    {
        var entity = await _db.Projects.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = input.Name;
        entity.Description = input.Description ?? string.Empty;
        await _db.SaveChangesAsync();

        return NoContent(); // 204
    }

    // DELETE /api/projects/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Projects.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Projects.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
