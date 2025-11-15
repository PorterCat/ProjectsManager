using Microsoft.EntityFrameworkCore;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;
using ProjectsManager.DataAccess.Entities;

namespace ProjectsManager.DataAccess.Repository;

public class ProjectsRepository(ProjectsManagerDbContext dbContext) : IProjectsRepository
{
    public async Task<Project?> GetById(Guid id) =>
         (await dbContext.Projects
             .AsNoTracking()
             .Include(p => p.Employees)
             .FirstOrDefaultAsync(p => p.Id == id))
         ?.ToDomain();

    public async Task<ProjectWithEmployees?> GetWithEmployees(Guid id)
    {
        var entity = await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entity == null) return null;

        var project = entity.ToDomain();
        var employees = entity.Employees.Select(e => e.ToDomain());

        return new ProjectWithEmployees(project, employees);
    }

    public async Task<ICollection<Project>> GetAll() =>
        (await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.Employees)
            .ToListAsync())
        .Select(e => e.ToDomain())
        .ToList();

    public async Task<int> GetCount()
        => await dbContext.Projects.CountAsync();

    public async Task Add(Project project)
    {
        var entity = project.ToEntity();

        if (project.EmployeeIds.Count != 0)
        {
            var employees = await dbContext.Employees
                .Where(e => project.EmployeeIds.Contains(e.Id))
                .ToListAsync();

            entity.Employees.AddRange(employees);
        }

        await dbContext.Projects.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Project project)
    {
        var entity = await dbContext.Projects
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == project.Id);

        if (entity == null) return;
        entity.ApplyDomainChanges(project);

        var currentEmployeeIds = entity.Employees.Select(e => e.Id).ToList();
        var newEmployeeIds = project.EmployeeIds.ToList();

        var toRemove = entity.Employees
            .Where(e => !newEmployeeIds.Contains(e.Id))
            .ToList();
        foreach (var employee in toRemove)
            entity.Employees.Remove(employee);

        var toAddIds = newEmployeeIds.Except(currentEmployeeIds);
        var toAdd = await dbContext.Employees
            .Where(e => toAddIds.Contains(e.Id))
            .ToListAsync();
        foreach (var employee in toAdd)
            entity.Employees.Add(employee);

        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await dbContext.Projects
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (entity is null)
            return false;

        entity.Employees.Clear();

        dbContext.Projects.Remove(entity);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Project>> GetByFilter(PageQuery? page = null, ProjectFilterQuery? projectQuery = null)
    {
        IQueryable<ProjectEntity> query = dbContext.Projects.AsNoTracking()
            .Include(p => p.Employees);

        if (projectQuery is not null)
        {
            if (!string.IsNullOrWhiteSpace(projectQuery.SearchText))
            {
                var searchText = projectQuery.SearchText.Trim().ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(searchText) ||
                    p.CustomerCompanyName.ToLower().Contains(searchText) ||
                    p.ContractorCompanyName.ToLower().Contains(searchText));
            }

            if (projectQuery.StartDateFrom.HasValue)
                query = query.Where(p => p.StartDate >= projectQuery.StartDateFrom.Value);

            if (projectQuery.StartDateTo.HasValue)
                query = query.Where(p => p.StartDate <= projectQuery.StartDateTo.Value);

            if (projectQuery.PriorityFrom.HasValue)
                query = query.Where(p => p.Priority >= projectQuery.PriorityFrom.Value);

            if (projectQuery.PriorityTo.HasValue)
                query = query.Where(p => p.Priority <= projectQuery.PriorityTo.Value);

            query = ApplySorting(query, projectQuery);
        }

        if (page is not null)
            query = query.Skip((page.PageNum - 1) * page.PageSize).Take(page.PageSize);

        var result = await query.ToListAsync();
        return result.Select(e => e.ToDomain()).ToList();
    }

    private IQueryable<ProjectEntity> ApplySorting(IQueryable<ProjectEntity> query, ProjectFilterQuery projectFilterQuery)
    {
        if (string.IsNullOrWhiteSpace(projectFilterQuery.SortBy))
            return query.OrderByDescending(p => p.Priority).ThenByDescending(p => p.StartDate);

        return projectFilterQuery.SortBy.ToLower() switch
        {
            "title" => projectFilterQuery.SortDescending
                ? query.OrderByDescending(p => p.Title)
                : query.OrderBy(p => p.Title),

            "priority" => projectFilterQuery.SortDescending
                ? query.OrderByDescending(p => p.Priority)
                : query.OrderBy(p => p.Priority),

            "startdate" => projectFilterQuery.SortDescending
                ? query.OrderByDescending(p => p.StartDate)
                : query.OrderBy(p => p.StartDate),

            "enddate" => projectFilterQuery.SortDescending
                ? query.OrderByDescending(p => p.EndDate)
                : query.OrderBy(p => p.EndDate),

            "customercompany" => projectFilterQuery.SortDescending
                ? query.OrderByDescending(p => p.CustomerCompanyName)
                : query.OrderBy(p => p.CustomerCompanyName),

            "contractorcompany" => projectFilterQuery.SortDescending
                ? query.OrderByDescending(p => p.ContractorCompanyName)
                : query.OrderBy(p => p.ContractorCompanyName),

            _ => query.OrderByDescending(p => p.Priority).ThenByDescending(p => p.StartDate)
        };
    }
}