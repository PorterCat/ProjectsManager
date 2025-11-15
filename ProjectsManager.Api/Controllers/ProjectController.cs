using Microsoft.AspNetCore.Mvc;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;

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
        [FromQuery] ProjectFilterQuery? query = null)
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
    public async Task<ActionResult<ProjectResponse>> GetProject(Guid id)
    {
        var project = await projectsRepository.GetById(id);
        if (project is null)
            return NotFound($"Project [{id}] not found.");

        return Ok(new ProjectResponse(
            project.Id, project.Title, project.StartDate, project.Priority,
            project.CustomerCompanyName, project.ContractorCompanyName, project.EndDate));
    }

    [HttpGet("{id:guid}/employees")]
    public async Task<ActionResult<ProjectWithEmployeesResponse>> GetProjectWithEmployees(Guid id)
    {
        var projectWithEmployees = await projectsRepository.GetWithEmployees(id);
        if (projectWithEmployees is null)
            return NotFound($"Project [{id}] not found.");

        var employeeResponses = projectWithEmployees.Employees.Select(e =>
            new EmployeeResponse(e.Id, e.Firstname, e.Lastname, e.Patronymic, e.Email));

        return Ok(new ProjectWithEmployeesResponse(
            projectWithEmployees.Project.Id,
            projectWithEmployees.Project.Title,
            projectWithEmployees.Project.LeaderId,
            employeeResponses));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProject([FromBody] CreateProjectRequest request)
    {
        var result = await projectsService.CreateProject(request);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetProject), new { id = result.Value }, result.Value);
    }

    [HttpPatch("{id:guid}/leader")]
    public async Task<ActionResult> AssignLeader(Guid id, [FromBody] AssignLeaderRequest request)
    {
        var result = await projectsService.UpdateProjectLeader(id, request.LeaderId);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok();
    }

    [HttpPatch("{id:guid}/employees")]
    public async Task<ActionResult> AssignEmployees(Guid id, [FromBody] AssignEmployeesRequest request)
    {
        var result = await projectsService.AssignEmployees(id,
            request.EmployeesToAdd, request.EmployeesToRemove);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok();
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> UpdateProject(Guid id, [FromBody] PatchProjectRequest request)
    {
        if (await projectsRepository.GetById(id) is null)
            return NotFound($"Project [{id}] not found.");

        var result = await projectsService.UpdateProject(id, request);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteProject(Guid id)
    {
        var result = await projectsRepository.Delete(id);
        if (!result)
            return NotFound($"Project [{id}] not found.");

        return Ok();
    }
}