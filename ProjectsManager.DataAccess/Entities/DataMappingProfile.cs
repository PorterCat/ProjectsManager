using AutoMapper;
using ProjectsManager.Core.Models;

namespace ProjectsManager.DataAccess.Entities;

public class DataMappingProfile : Profile
{
    public DataMappingProfile()
    {
        CreateMap<EmployeeEntity, Employee>();
        CreateMap<Employee, EmployeeEntity>();
        
        CreateMap<ProjectEntity, Project>();
        CreateMap<Project, ProjectEntity>();
    }
}