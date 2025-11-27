using System.Security.Claims;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Tests.Controllers;

public static class TestDataHelper
{
    public static Employee CreateEmployee(
        Guid id, 
        string firstName = "Test", 
        string lastName = "User", 
        string email = "test@test.com") =>
            Employee.Create(id, firstName, lastName, email).Value;

    public static Project CreateProject(
        Guid id, 
        string title = "Test Project") =>
            Project.Create(id, title, "Customer", "Contractor", 1, DateOnly.FromDateTime(DateTime.Now)).Value;

    public static ClaimsPrincipal CreateClaimsPrincipal(Guid userId, EmployeeRole role, string email = "test@test.com")
    {
        var claims = new List<Claim>
        {
            new("id", userId.ToString()),
            new("email", email),
            new(ClaimTypes.Role, role.ToString())
        };
        
        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }
}