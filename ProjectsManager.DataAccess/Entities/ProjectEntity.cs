namespace ProjectsManager.DataAccess.Entities;

public class ProjectEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CustomerCompanyName { get; set; } = string.Empty;
    public string ContractorCompanyName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int Priority { get; set; }
    public Guid? LeaderId { get; set; }

    public EmployeeEntity? Leader { get; set; }
    public List<EmployeeEntity> Employees { get; set; } = [];
}