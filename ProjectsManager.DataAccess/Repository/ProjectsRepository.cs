using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;
using ProjectsManager.DataAccess.Entities;

namespace ProjectsManager.DataAccess.Repository;

public class ProjectsRepository(
    ProjectsManagerDbContext dbContext,
    IMapper mapper) : IProjectsRepository
{
    public async Task<ICollection<Project>> GetAll(PageQuery? page = null, ProjectFilterQuery? projectQuery = null)
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
        return mapper.Map<ICollection<Project>>(result);
    }
    
    public async Task<Project?> GetById(Guid id) =>
        mapper.Map<Project>
        (await dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id));
    
    public async Task<int> GetCount() => 
        await dbContext.Projects.CountAsync();

    public async Task Add(Project project)
    {
        await dbContext.Projects.AddAsync(mapper.Map<ProjectEntity>(project));
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(Project project) =>
        await dbContext.Projects
            .Where(p => p.Id == project.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.Title, project.Title)
                .SetProperty(p => p.CustomerCompanyName, project.CustomerCompanyName)
                .SetProperty(p => p.ContractorCompanyName, project.ContractorCompanyName)
                .SetProperty(p => p.Priority, project.Priority)
                .SetProperty(p => p.StartDate, project.StartDate)
                .SetProperty(p => p.EndDate, project.EndDate)
            );

    public async Task Delete(Guid id) =>
        await dbContext.Projects
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();

    public async Task<ICollection<Employee>> GetEmployeesByProject(Guid projectId)
    {
        var projectEntity = await dbContext.Projects
            .AsNoTracking()
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        return projectEntity?.Employees
            .Select(mapper.Map<Employee>)
            .ToList() ?? [];
    }

    public async Task<Employee?> GetProjectLeader(Guid projectId)
    {
        var projectEntity = await dbContext.Projects
            .Include(p => p.Leader)
            .FirstOrDefaultAsync(p => p.Id == projectId);
    
        return projectEntity is null ? null : mapper.Map<Employee?>(projectEntity.Leader);
    }

    public async Task<int> UpdateProjectEmployees(Guid projectId, IEnumerable<Guid> employeeIds)
    {
        var projectEntity = await dbContext.Projects
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == projectId);
        
        projectEntity?.Employees.Clear();
        var employeeEntities = await dbContext.Employees
            .Where(e => employeeIds.Contains(e.Id))
            .ToListAsync();
        
        projectEntity?.Employees.AddRange(employeeEntities);
        await dbContext.SaveChangesAsync();
        return employeeEntities.Count;
    }

    public async Task UpdateProjectLeader(Guid projectId, Guid? leaderId) =>
        await dbContext.Projects
            .Where(p => p.Id == projectId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.LeaderId, leaderId));

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