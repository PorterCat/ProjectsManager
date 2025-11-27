using Moq;
using ProjectsManager.Business;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Tests;

[TestFixture]
public class EmployeeServiceTests
{
    private Mock<IEmployeesRepository> _employeesRepoMock;
    private EmployeesService _employeesService;

    [SetUp]
    public void SetUp()
    {
        _employeesRepoMock = new Mock<IEmployeesRepository>();
        _employeesService = new EmployeesService(_employeesRepoMock.Object);
    }
    
    [Test]
    public async Task CreateEmployee_DuplicateEmail_ShouldFail()
    {
        // Arrange
        var existingEmployee = Employee.Create(
            Guid.NewGuid(), "Existing", "User", "duplicate@test.com").Value;
        
        var newEmployee = Employee.Create(
            Guid.NewGuid(), "New", "User", "duplicate@test.com").Value;

        _employeesRepoMock.Setup(x => x.GetByEmail("duplicate@test.com"))
            .ReturnsAsync(existingEmployee);

        // Act
        var result = await _employeesService.CreateEmployee(newEmployee);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Contains.Substring("already exists"));
    }
}