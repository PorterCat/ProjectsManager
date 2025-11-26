using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjectsManager.Business.Auth;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IEmployeesService employeeService,
    JwtService jwtService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request) // Simplified, without any repos and password hashes
    {
        var employee = await employeeService.GetEmployeeByEmail(request.Email);
        if (employee is null)
            return NotFound($"Employee [{request.Email}] not found.");
        
        var token = jwtService.GenerateJwtToken(employee, request.Role);
        return Ok(token);
    }
}

public record LoginRequest(
    [Required] string Email, 
    [Required] EmployeeRole Role);