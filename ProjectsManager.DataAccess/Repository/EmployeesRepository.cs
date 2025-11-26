using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Models;
using ProjectsManager.DataAccess.Entities;

namespace ProjectsManager.DataAccess.Repository;

public class EmployeesRepository(
    ProjectsManagerDbContext dbContext,
    IMapper mapper) : IEmployeesRepository
{
    public async Task<Employee?> GetById(Guid id) =>
        mapper.Map<Employee>(
            await dbContext.Employees
                .FindAsync(id));

    public async Task<Employee?> GetByIdWithProjects(Guid id) =>
        mapper.Map<Employee>(
            await dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Projects)
                .FirstOrDefaultAsync(e => e.Id == id));

    public async Task<Employee?> GetByIdWithLeadingProjects(Guid id) =>
        mapper.Map<Employee>(
            await dbContext.Employees
                .AsNoTracking()
                .Include(e => e.LeadingProjects)
                .FirstOrDefaultAsync(e => e.Id == id));

    public async Task<Employee?> GetByEmail(string email) =>
        mapper.Map<Employee>(
            await dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email == email));

    public async Task<ICollection<Employee>> GetAll(string? searchText = null)
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

            return mapper.Map<ICollection<Employee>>(filteredEmployees);
        }

        var result = await query.ToListAsync();
        return mapper.Map<ICollection<Employee>>(result);
    }

    public async Task Add(Employee employee)
    {
        await dbContext.Employees.AddAsync(mapper.Map<EmployeeEntity>(employee));
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Employee employee) =>
        await dbContext.Employees
            .Where(e => e.Id == employee.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(e => e.FirstName, employee.FirstName)
                .SetProperty(e => e.LastName, employee.LastName)
                .SetProperty(e => e.Email, employee.Email)
                .SetProperty(e => e.Patronymic, employee.Patronymic)
            );

    public async Task Delete(Guid id) => 
        await dbContext.Employees
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();
    
    public async Task<ICollection<Project>> GetProjectsByEmployee(Guid employeeId)
    {
        var employeeEntity = await dbContext.Employees
            .AsNoTracking()
            .Include(e => e.Projects)
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        return employeeEntity?.Projects
            .Select(mapper.Map<Project>)
            .ToList() ?? [];
    }

    public async Task<ICollection<Project>> GetLeadingProjectsByEmployee(Guid employeeId)
    {
        var employeeEntity = await dbContext.Employees
            .AsNoTracking()
            .Include(e => e.LeadingProjects)
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        return employeeEntity?.LeadingProjects
            .Select(mapper.Map<Project>)
            .ToList() ?? [];
    }
}