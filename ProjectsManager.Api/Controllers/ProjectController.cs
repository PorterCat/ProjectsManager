using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager,Director")]
public class ProjectController(
    IProjectsService projectsService, 
    IAssignmentService assignmentService,
    IMapper mapper) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<PageResponse<Project>>> GetAllProjects(
        [FromQuery] PageQuery? pageQuery = null,
        [FromQuery] ProjectFilterQuery? query = null)
    {
        var projects = await projectsService.GetAllProjects(pageQuery, query);

        if (projects.Count == 0)
            return NoContent();

        var count = await projectsService.GetCount();
        return Ok(new PageResponse<Project>(
            Items: projects,
            Total: count,
            TotalPages: pageQuery is not null ? (int)Math.Ceiling((double)count / pageQuery.PageSize) : 1
        ));
    }

    [HttpGet("{projectId:guid}")]
    public async Task<ActionResult<Project>> GetProject(Guid projectId)
    {
        var project = await projectsService.GetProjectById(projectId);
        if (project is null)
            return NotFound($"Project [{projectId}] not found.");

        var leader = await assignmentService.GetProjectLeader(projectId);
        var response = new ProjectResponse
        (
            Id: project.Id,
            Title: project.Title,
            CustomerCompanyName: project.CustomerCompanyName,
            ContractorCompanyName: project.ContractorCompanyName,
            Priority: project.Priority,
            StartDate: project.StartDate,
            EndDate: project.EndDate,
            LeaderId: leader?.Id
        );
    
        return Ok(response);
    }
    
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProject([FromBody] CreateProjectRequest request)
    {
        var project = Project.Create(
            Guid.NewGuid(),
            request.Title,
            request.CustomerCompanyName,
            request.ContractorCompanyName,
            request.Priority,
            request.StartDate,
            request.EndDate);
        
        if(project.IsFailure)
            return BadRequest(project.Error);
        
        var result = await projectsService.CreateProject(project.Value);
        if(result.IsFailure)
            return BadRequest(result.Error);

        if (request.EmployeeIds?.Count > 0)
        {
            var assignmentResult = await assignmentService.AssignEmployeesToProject(project.Value.Id, request.EmployeeIds);
            if(assignmentResult.IsFailure)
                return BadRequest(result.Error);
        }
        
        return Created(Url.Action(nameof(GetProject), new { id = project.Value.Id }), project.Value.Id);
    }

    [HttpPatch("{projectId:guid}")]
    public async Task<ActionResult> UpdateProject(Guid projectId, [FromBody] UpdateProjectRequest request)
    {
        var project = await projectsService.GetProjectById(projectId);
        if (project is null)
            return NotFound($"Project [{projectId}] not found.");
        
        var result = await projectsService.UpdateProject(project, request);
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return Ok(result.Value);
    }
    
    [HttpDelete("{projectId:guid}")]
    public async Task<ActionResult> DeleteProject(Guid projectId)
    {
        var employee = await projectsService.GetProjectById(projectId);
        if (employee is null)
            return NotFound($"Project [{projectId}] not found.");
        
        var result = await projectsService.DeleteProject(employee);
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return Ok($"Employee {projectId} was deleted.");
    }
    
    [HttpGet("{projectId:guid}/employees")]
    public async Task<ActionResult<ProjectWithEmployees>> GetProjectWithEmployees(Guid projectId)
    {
        var project = await projectsService.GetProjectById(projectId);
        if (project is null)
            return NotFound($"Project [{projectId}] not found.");

        var employees = await assignmentService.GetEmployeesByProject(projectId);
        if(employees.Count == 0)
            return NoContent();
        
        return Ok(new ProjectWithEmployees(project, employees));
    }

    [HttpPost("{projectId:guid}/leader")]
    public async Task<ActionResult> AssignLeader(Guid projectId, [FromBody] AssignLeaderRequest request)
    {
        var project = await projectsService.GetProjectById(projectId);
        if (project is null)
            return NotFound($"Project [{projectId}] not found.");
        
        var result = await assignmentService.AssignProjectLeader(projectId, request.LeaderId);
        if (result.IsFailure)
            return BadRequest(result.Error);
    
        return Ok($"Employee [{request.LeaderId}] is now leader of Project [{projectId}]");
    }

    [HttpPost("{projectId:guid}/employees")]
    public async Task<ActionResult> AssignEmployees(Guid projectId, [FromBody] IEnumerable<Guid> employeeIds)
    {
        var project = await projectsService.GetProjectById(projectId);
        if (project is null)
            return NotFound($"Project [{projectId}] not found.");
    
        var result = await assignmentService.AssignEmployeesToProject(projectId, employeeIds);
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return Ok($"Project [{projectId}]. Employees: {result.Value}");
    }
}