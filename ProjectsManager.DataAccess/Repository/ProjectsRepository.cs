using Microsoft.EntityFrameworkCore;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;
using ProjectsManager.DataAccess.Entities;

namespace ProjectsManager.DataAccess.Repository;

public class ProjectsRepository(ProjectsManagerDbContext dbContext) : IProjectsRepository
{
    public async Task<ICollection<Project>> GetAll() =>
        (await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.Employees)
            .ToListAsync())
        .Select(e => e.ToDomain())
        .ToList();

    public async Task<Project?> GetById(Guid id) =>
        (await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id))
        ?.ToDomain();

    public async Task<int> GetCount() =>
        await dbContext.Projects.CountAsync();

    public async Task Add(Project project)
    {
        var entity = project.ToEntity();
        if (project.LeaderId is not null)
            entity.Employees.Add(dbContext.Employees.FirstOrDefault(e => e.Id == project.LeaderId)!);
        await dbContext.Projects.AddAsync(entity);
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

    public async Task<bool> Save(Project project)
    {
        var entity = await dbContext.Projects
            .Include(p => p.Employees)
            .SingleOrDefaultAsync(p => p.Id == project.Id);
        if (entity is null) return false;

        entity.Apply(project);

        if (project.HasEmployeeChanges)
        {
            var toRemove = entity.Employees
                .Where(e => project.EmployeesToRemove.Contains(e.Id))
                .ToList();
            foreach (var employee in toRemove)
                entity.Employees.Remove(employee);

            var toAdd = await dbContext.Employees
                .Where(e => project.EmployeesToAdd.Contains(e.Id))
                .ToListAsync();
            foreach (var employee in toAdd)
                entity.Employees.Add(employee);
        }

        project.ClearEmployeeBuffers();
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<ICollection<Project>> GetByFilter(PageQuery? page = null, ProjectQuery? projectQuery = null)
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

    private IQueryable<ProjectEntity> ApplySorting(IQueryable<ProjectEntity> query, ProjectQuery projectQuery)
    {
        if (string.IsNullOrWhiteSpace(projectQuery.SortBy))
            return query.OrderByDescending(p => p.Priority).ThenByDescending(p => p.StartDate);

        return projectQuery.SortBy.ToLower() switch
        {
            "title" => projectQuery.SortDescending
                ? query.OrderByDescending(p => p.Title)
                : query.OrderBy(p => p.Title),

            "priority" => projectQuery.SortDescending
                ? query.OrderByDescending(p => p.Priority)
                : query.OrderBy(p => p.Priority),

            "startdate" => projectQuery.SortDescending
                ? query.OrderByDescending(p => p.StartDate)
                : query.OrderBy(p => p.StartDate),

            "enddate" => projectQuery.SortDescending
                ? query.OrderByDescending(p => p.EndDate)
                : query.OrderBy(p => p.EndDate),

            "customercompany" => projectQuery.SortDescending
                ? query.OrderByDescending(p => p.CustomerCompanyName)
                : query.OrderBy(p => p.CustomerCompanyName),

            "contractorcompany" => projectQuery.SortDescending
                ? query.OrderByDescending(p => p.ContractorCompanyName)
                : query.OrderBy(p => p.ContractorCompanyName),

            _ => query.OrderByDescending(p => p.Priority).ThenByDescending(p => p.StartDate)
        };
    }
}