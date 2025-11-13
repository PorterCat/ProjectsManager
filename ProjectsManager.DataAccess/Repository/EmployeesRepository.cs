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

    public async Task<ICollection<Employee>> GetAllByText(string searchText)
    {
        var query = dbContext.Employees.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            searchText = searchText.Trim().ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().StartsWith(searchText) ||
                p.LastName.ToLower().StartsWith(searchText) ||
                (p.Patronymic != null && p.Patronymic.ToLower().StartsWith(searchText)));
        }

        var result = await query.ToListAsync();
        return result.Select(e => e.ToDomain()).ToList();
    }

    public async Task Add(Employee employee)
    {
        await dbContext.Employees.AddAsync(employee.ToEntity());
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> Delete(Employee employee)
    {
        // dbContext.Employees.Remove(entity);
        // await dbContext.SaveChangesAsync();
        return true;
    }
}