using Moq;
using ProjectsManager.Business;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Tests;

[TestFixture]
public class ProjectsServiceTests
{
    private Mock<IProjectsRepository> _projectsRepoMock = null!;
    private Mock<IEmployeesRepository> _employeesRepoMock = null!;

    private ProjectsService _service = null!;

    [SetUp]
    public void Setup()
    {
        _projectsRepoMock = new Mock<IProjectsRepository>(MockBehavior.Strict);
        _employeesRepoMock = new Mock<IEmployeesRepository>(MockBehavior.Strict);
        _service = new ProjectsService(_projectsRepoMock.Object, _employeesRepoMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _projectsRepoMock.VerifyAll();
        _employeesRepoMock.VerifyAll();
    }

    [Test]
    public async Task AddProject_When_LeaderId_IsNull()
    {
        // Arrange
        var createResult = Project.Create(
            "Title",
            "Customer",
            "Contractor",
            1,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
        );
        Assert.That(createResult.IsSuccess, "Project.Create failed in test setup");
        var project = createResult.Value;

        _projectsRepoMock
            .Setup(r => r.Add(It.Is<Project>(p => p.Id == project.Id)))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddProject(project, leaderId: null);

        // Assert
        Assert.That(result.IsSuccess, "Expected service to return success");
        Assert.That(project.Id, Is.EqualTo(result.Value));
    }

    [Test]
    public async Task AddProject_When_LeaderId_IsNotNull_AndLeaderExists()
    {

    }

    [Test]
    public async Task AddProject_When_LeaderId_IsNotNull_ButEmployeeNotFound()
    {

    }
}