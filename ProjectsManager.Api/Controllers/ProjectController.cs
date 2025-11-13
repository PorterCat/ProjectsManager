using Microsoft.AspNetCore.Mvc;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController(
    IProjectsRepository projectsRepository,
    IProjectsService projectsService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<PageResponse<ProjectResponse>>> GetAllProjects(
        [FromQuery] PageQuery? pageQuery = null,
        [FromQuery] ProjectQuery? query = null)
    {
        var projects = pageQuery is null
            ? query is null
                ? await projectsRepository.GetAll()
                : await projectsRepository.GetByFilter(projectQuery: query)
            : query is null
                ? await projectsRepository.GetByFilter(pageQuery)
                : await projectsRepository.GetByFilter(pageQuery, query);

        if (projects.Count == 0)
            return NoContent();

        var items = projects.Select(p => new ProjectResponse(
            p.Id, p.Title, p.StartDate, p.Priority,
            p.CustomerCompanyName, p.ContractorCompanyName, p.EndDate)).ToList();

        var count = await projectsRepository.GetCount();
        return Ok(new PageResponse<ProjectResponse>(
            Items: items,
            Total: count,
            TotalPages: pageQuery is not null ? (int)Math.Ceiling((double)count / pageQuery.PageSize) : 1
        ));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<(Guid, string)>> GetProjectById(Guid id)
    {
        var project = await projectsRepository.GetById(id);
        if (project is null)
            return NotFound($"Project [{id}] not found.");

        return Ok(new { id = project.Id, title = project.Title });
    }

    [HttpGet("{id:guid}/employees")]
    public async Task<ActionResult<ProjectWithEmployeesResponse>> GetProjectWithEmployees(Guid id)
    {
        var project = await projectsRepository.GetById(id);
        if (project is null)
            return NotFound($"Project [{id}] not found.");

        return Ok(new { id = project.Id, title = project.Title, employees = project.Employees });
    }

    [HttpPatch("{id:guid}/employees")]
    public async Task<ActionResult<Guid>> AssignEmployees(Guid id, [FromBody] AssignEmployeesRequest request)
    {
        var project = await projectsRepository.GetById(id);
        if (project is null)
            return NotFound($"Project [{id}] not found.");

        var result = await projectsService.AssignEmployees(project,
            request.EmployeesToAdd, request.EmployeesToRemove);
        if (result.IsFailure)
            return BadRequest(result.Error);

        await projectsRepository.Save(project);
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProject([FromBody] CreateProjectRequest request)
    {
        var project = Project.Create(
            request.Title,
            request.CustomerCompanyName,
            request.ContractorCompanyName,
            request.Priority,
            request.StartDate,
            request.EndDate);

        if (project.IsFailure)
            return BadRequest(project.Error);

        var result = await projectsService.AddProject(project.Value, request.LeaderId);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Created(
            Url.Action(nameof(GetProjectById), new { id = result.Value }), project.Value.Id);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> PatchProject(Guid id, [FromBody] PatchProjectRequest request)
    {
        var project = await projectsRepository.GetById(id);
        if (project is null)
            return NotFound($"Project [{id}] not found.");

        var projectBefore = project with { };

        var result = project.ApplyPatch(request);
        if (result.IsFailure)
            return BadRequest(result.Error);

        var saveResult = await projectsRepository.Save(project);
        if (!saveResult)
            return BadRequest("Cannot save changes");

        return Ok(projectBefore.CreatePatchResponse(project, project.Id));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteProject(Guid id)
    {
        var result = await projectsRepository.Delete(id);
        if (!result)
            return NotFound($"Project [{id}] not found.");
        return Ok("Successfully deleted");
    }
}