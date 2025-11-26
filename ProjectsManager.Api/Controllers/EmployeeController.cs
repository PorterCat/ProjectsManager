using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager,Director")]
public class EmployeeController(
    IEmployeesService employeesService,
    IAssignmentService assignmentService) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees([FromQuery] string? searchText = null)
    {
        var employees = await employeesService.GetAllEmployees(searchText);
        if(employees.Count == 0)
            return NoContent();
        return Ok(employees);
    }

    [HttpGet("{employeeId:guid}")]
    public async Task<ActionResult<Employee>> GetEmployee(Guid employeeId)
    {
        var employee = await employeesService.GetEmployeeById(employeeId);
        if (employee is null)
            return NotFound($"Employee [{employeeId}] not found.");
        return Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<Guid>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var employee = Employee.Create(
            Guid.NewGuid(),
            request.FirstName,
            request.LastName,
            request.Email,
            request.Patronymic);
        
        if(employee.IsFailure)
            return BadRequest(employee.Error);
        
        var result = await employeesService.CreateEmployee(employee.Value);
        if(result.IsFailure)
            return BadRequest(result.Error);
        
        return Created(
            Url.Action(nameof(GetEmployee), new { id = employee.Value.Id }), employee.Value.Id);
    }

    [HttpPatch("{employeeId:guid}")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult> UpdateEmployee(Guid employeeId, [FromBody] UpdateEmployeeRequest request)
    {
        var employee = await employeesService.GetEmployeeById(employeeId);
        if (employee is null)
            return NotFound($"Employee [{employeeId}] not found.");

        var employeeBefore = employee with { };

        var result = await employeesService.UpdateEmployee(employee, request);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(employeeBefore.CreatePatchResponse(employee, employee.Id));
    }

    [HttpDelete("{employeeId:guid}")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult> DeleteEmployee(Guid employeeId)
    {
        var employee = await employeesService.GetEmployeeById(employeeId);
        if (employee is null)
            return NotFound($"Employee [{employeeId}] not found.");
        
        var result = await employeesService.DeleteEmployee(employee);
        if (result.IsFailure)
            return BadRequest(result.Error);
        
        return Ok($"Employee {employeeId} was deleted.");
    }

    [HttpGet("{employeeId:guid}/projects")]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects(Guid employeeId)
    {
        var employee = await employeesService.GetEmployeeById(employeeId);
        if (employee is null)
            return NotFound($"Employee [{employeeId}] not found.");

        var projects = assignmentService.GetProjectsByEmployee(employeeId);
        return Ok(projects);
    }
}