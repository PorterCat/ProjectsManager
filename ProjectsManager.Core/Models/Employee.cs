using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;

namespace ProjectsManager.Core.Models;

public record Employee
{
    public Guid Id { get; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string Email { get; }
    public string? Patronymic { get; init; }

    private Employee(Guid id, string firstName, string lastName,
        string email, string? patronymic)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Patronymic = patronymic;
    }
    
    public static Result<Employee> Create(
        Guid id,
        string firstName,
        string lastName,
        string email,
        string? patronymic = null)
    {
        var validationResult = Validate(firstName, lastName, email);
        return validationResult.IsFailure 
            ? Result.Failure<Employee>(validationResult.Error) 
            : new Employee(id, firstName, lastName, email, patronymic);
    }

    public static Result Validate(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Failure($"{nameof(firstName)} cannot be empty");
            
        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Failure($"{nameof(lastName)} cannot be empty");

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure($"{nameof(email)} cannot be empty");

        if (!new EmailAddressAttribute().IsValid(email))
            return Result.Failure($"{nameof(email)} is invalid");

        return Result.Success();
    }
    
    public static Result Validate(Employee employee) =>
        Validate(employee.FirstName, employee.LastName, employee.Email);
}