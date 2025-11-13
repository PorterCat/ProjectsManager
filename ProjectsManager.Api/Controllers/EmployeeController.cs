using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController(
    IEmployeesRepository employeesRepository) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees([FromQuery] string? searchText = null)
    {
        ICollection<Employee> employees;

        if (searchText is null)
            employees = await employeesRepository.GetAll();
        else
            employees = await employeesRepository.GetAllByText(searchText);

        if (employees.Count == 0)
            return NoContent();
        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployeeById(Guid id)
    {
        var employee = await employeesRepository.GetById(id);
        if (employee is null)
            return NotFound($"Employee [{id}] not found.");
        return Ok(employee);
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

        try
        {
            await employeesRepository.Add(employee.Value);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteErrorCode: 19 }) // Hardcode. what if we will change Sqlite provider for smth else?
                                                                                                         // what if email is not only unique value?
        {
            return Conflict(new { error = "Employee with this email already exists", email = request.Email });
        }

        return Created(
            Url.Action(nameof(GetEmployeeById), new { id = employee.Value.Id }), employee.Value.Id);
    }
}