namespace ProjectsManager.DataAccess.Entities;

public class ProjectEntity
{
    public Guid Id { get; init; }
    public string Title { get; set; }
    public string CustomerCompanyName { get; set; }
    public string ContractorCompanyName { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int Priority { get; set; }
    public Guid? LeaderId { get; set; }

    public EmployeeEntity? Leader { get; set; }
    public List<EmployeeEntity> Employees { get; set; } = [];
}