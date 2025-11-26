using ProjectsManager.Core.Models;

namespace ProjectsManager.DataAccess.Entities;

public class AccountEntity
{
    public Guid EmployeeId { get; init; }
    public string Email => Employee.Email;
    public EmployeeRole Role { get; init; }
    
    public EmployeeEntity Employee { get; init; }
}