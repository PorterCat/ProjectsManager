using Microsoft.AspNetCore.Mvc;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController(
    IEmployeesRepository employeesRepository,
    IEmployeesServices employeesServices) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<EmployeeResponse>>> GetAllEmployees([FromQuery] string? searchText = null)
    {
        ICollection<Employee> employees;

        if (searchText is null)
            employees = await employeesRepository.GetAll();
        else
            employees = await employeesRepository.GetByFilter(searchText);

        if (employees.Count == 0)
            return NoContent();
        return Ok(employees.Select(e =>
            new EmployeeResponse(e.Id, e.Firstname, e.Lastname, e.Patronymic, e.Email)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeById(Guid id)
    {
        var employee = await employeesRepository.GetById(id);
        if (employee is null)
            return NotFound($"Employee [{id}] not found.");
        return Ok(
            new EmployeeResponse(employee.Id, employee.Firstname, employee.Lastname, employee.Patronymic, employee.Email));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateEmployee([FromBody] CreateEmployeeRequest request)
    {
        var employee = Employee.Create(
            Guid.NewGuid(),
            request.Firstname,
            request.Lastname,
            request.Email,
            patronymic: request.Patronymic);

        if (employee.IsFailure)
            return BadRequest(employee.Error);

        var result = await employeesRepository.Add(employee.Value);
        if (result.IsFailure)
            return Conflict(new { error = result.Error, email = request.Email });

        return Created(
            Url.Action(nameof(GetEmployeeById), new { id = employee.Value.Id }), employee.Value.Id);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeRequest request)
    {
        if (await employeesRepository.GetById(id) is null)
            return NotFound($"Employee [{id}] not found.");

        var result = await employeesServices.UpdateEmployee(id, request);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeleteEmployee(Guid id)
    {
        var result = await employeesRepository.Delete(id);
        if (!result)
            return NotFound($"Employee [{id}] not found.");
        return Ok("Successfully deleted");
    }
}