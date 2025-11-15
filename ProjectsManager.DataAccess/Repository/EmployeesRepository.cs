using System.Globalization;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Models;
using ProjectsManager.DataAccess.Entities;

namespace ProjectsManager.DataAccess.Repository;

public class EmployeesRepository(ProjectsManagerDbContext dbContext) : IEmployeesRepository
{
    public async Task<ICollection<Employee>> GetAll() =>
        (await dbContext.Employees.AsNoTracking().ToListAsync())
        .Select(e => e.ToDomain())
        .ToList();

    public async Task<Employee?> GetById(Guid id) =>
        (await dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id))
        ?.ToDomain();

    public async Task<ICollection<Employee>> GetByFilter(string? searchText = null)
    {
        var query = dbContext.Employees.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            searchText = searchText.Trim();
            var compareInfo = CultureInfo.CurrentCulture.CompareInfo;

            var allEmployees = await query.ToListAsync();
            var filteredEmployees = allEmployees.Where(p =>
                compareInfo.IsPrefix(p.FirstName, searchText, CompareOptions.IgnoreCase) ||
                compareInfo.IsPrefix(p.LastName, searchText, CompareOptions.IgnoreCase) ||
                (p.Patronymic != null && compareInfo.IsPrefix(p.Patronymic, searchText, CompareOptions.IgnoreCase))
            ).ToList();

            return filteredEmployees.Select(e => e.ToDomain()).ToList();
        }

        var result = await query.ToListAsync();
        return result.Select(e => e.ToDomain()).ToList();
    }

    public async Task<Result> Add(Employee employee)
    {
        var existingEmployee = await dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email == employee.Email);

        if (existingEmployee != null)
            return Result.Failure($"Employee with email {employee.Email} already exists"); // Not repo's responsibility, TODO: move to service 

        await dbContext.Employees.AddAsync(employee.ToEntity());
        await dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task Update(Employee employee)
    {
        var entity = await dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == employee.Id);

        if (entity == null) return;

        entity.ApplyDomainChanges(employee);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await dbContext.Employees
            .Include(e => e.Projects)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entity is null)
            return false;

        entity.Projects.Clear();

        dbContext.Employees.Remove(entity);
        await dbContext.SaveChangesAsync();
        return true;
    }
}