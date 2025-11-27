using System.Net;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using ProjectsManager.Api.Controllers;
using ProjectsManager.Core.Abstractions;
using ProjectsManager.Core.Contracts;
using ProjectsManager.Core.Models;

namespace ProjectsManager.Tests.Controllers;

[TestFixture]
public class ProjectControllerTests
{
    private Mock<IProjectsService> _mockProjectsService;
    private Mock<IAssignmentService> _mockAssignmentService;
    private ProjectController _controller;
    
    private readonly Guid _directorId = Guid.NewGuid();
    private readonly Guid _managerId = Guid.NewGuid();
    private readonly Guid _employeeId = Guid.NewGuid();
    private readonly Guid _otherEmployeeId = Guid.NewGuid();
    private readonly Guid _projectId = Guid.NewGuid();
    
    [SetUp]
    public void Setup()
    {
        _mockProjectsService = new Mock<IProjectsService>();
        _mockAssignmentService = new Mock<IAssignmentService>();
        _controller = new ProjectController(_mockProjectsService.Object, _mockAssignmentService.Object);
        
        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
            .Returns("http://localhost/api/project/test-url");
        _controller.Url = mockUrlHelper.Object;
    }
    
    private void SetupControllerContext(ClaimsPrincipal user)
    {
        var context = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        _controller.ControllerContext = context;
    }
    
    #region Manager Operations Tests

    [Test]
    public async Task AssignEmployees_ManagerAssignsToOwnProject_ShouldSucceed()
    {
        // Arrange
        var manager = TestDataHelper.CreateEmployee(_managerId);
        var project = TestDataHelper.CreateProject(_projectId);
        var employeeIds = new List<Guid> { _employeeId };
        var user = TestDataHelper.CreateClaimsPrincipal(_managerId, EmployeeRole.Manager);
        
        SetupControllerContext(user);
        
        _mockProjectsService.Setup(x => x.GetProjectById(_projectId))
            .ReturnsAsync(project);
        _mockAssignmentService.Setup(x => x.GetProjectLeader(_projectId))
            .ReturnsAsync(manager);
        _mockAssignmentService.Setup(x => x.AssignEmployeesToProject(_projectId, employeeIds))
            .ReturnsAsync(employeeIds.Count);

        // Act
        var result = await _controller.AssignEmployees(_projectId, employeeIds);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _mockAssignmentService.Verify(x => x.AssignEmployeesToProject(_projectId, employeeIds), Times.Once);
    }

    [Test]
    public async Task AssignEmployees_ManagerAssignsToOtherProject_ShouldReturnForbidden()
    {
        // Arrange
        var otherManager = TestDataHelper.CreateEmployee(Guid.NewGuid());
        var project = TestDataHelper.CreateProject(_projectId);
        var employeeIds = new List<Guid> { _employeeId };
        var user = TestDataHelper.CreateClaimsPrincipal(_managerId, EmployeeRole.Manager);
        
        SetupControllerContext(user);
        
        _mockProjectsService.Setup(x => x.GetProjectById(_projectId))
            .ReturnsAsync(project);
        _mockAssignmentService.Setup(x => x.GetProjectLeader(_projectId))
            .ReturnsAsync(otherManager);

        // Act
        var result = await _controller.AssignEmployees(_projectId, employeeIds);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        Assert.That((result as ObjectResult)?.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
    }
    #endregion
    
    #region Director Operations Tests

    [Test]
    public async Task ProjectCRUD_AsDirector_ShouldSucceed()
    {
        // Arrange
        var directorUser = TestDataHelper.CreateClaimsPrincipal(_directorId, EmployeeRole.Director);
        SetupControllerContext(directorUser);

        var createRequest = new CreateProjectRequest(
            Title: "Test Project",
            CustomerCompanyName: "Test Customer",
            ContractorCompanyName: "Test Contractor",
            Priority: 2,
            StartDate: DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            EndDate: DateOnly.FromDateTime(DateTime.Now.AddDays(60)),
            EmployeeIds: [Guid.NewGuid()]);

        var updateRequest = new UpdateProjectRequest(
            Title: "Updated Project",
            CustomerCompanyName: "Updated Customer",
            ContractorCompanyName: "Updated Contractor",
            Priority: 1,
            StartDate: DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
            EndDate: DateOnly.FromDateTime(DateTime.Now.AddDays(30))
        );

        var testProject = TestDataHelper.CreateProject(_projectId);
        
        _mockProjectsService.Setup(x => x.CreateProject(It.IsAny<Project>()))
            .ReturnsAsync(Result.Success());
        _mockProjectsService.Setup(x => x.GetProjectById(_projectId))
            .ReturnsAsync(testProject);
        _mockProjectsService.Setup(x => x.UpdateProject(testProject, updateRequest))
            .ReturnsAsync(testProject.CreatePatchResponse(testProject, testProject.Id));
        _mockProjectsService.Setup(x => x.DeleteProject(testProject))
            .ReturnsAsync(Result.Success());
        _mockAssignmentService.Setup(x => x.AssignEmployeesToProject(_projectId, It.IsAny<List<Guid>>()))
            .ReturnsAsync(1);
        
        var createResult = await _controller.CreateProject(createRequest);
        
        Assert.That(createResult.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = createResult.Result as CreatedResult;
        Assert.That(createdResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        
        var getResult = await _controller.GetProject(_projectId);
        
        Assert.That(getResult.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = getResult.Result as OkObjectResult;
        Assert.That(okResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        
        var updateResult = await _controller.UpdateProject(_projectId, updateRequest);
        
        Assert.That(updateResult, Is.InstanceOf<OkObjectResult>());
        var updateOkResult = updateResult as OkObjectResult;
        Assert.That(updateOkResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        
        var deleteResult = await _controller.DeleteProject(_projectId);
        
        Assert.That(deleteResult, Is.InstanceOf<OkObjectResult>());
        var deleteOkResult = deleteResult as OkObjectResult;
        Assert.That(deleteOkResult?.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
    }

    [Test]
    public async Task AssignLeader_AsDirector_ShouldSucceed()
    {
        // Arrange
        var directorUser = TestDataHelper.CreateClaimsPrincipal(_directorId, EmployeeRole.Director);
        SetupControllerContext(directorUser);

        var leaderId = Guid.NewGuid();
        var assignLeaderRequest = new AssignLeaderRequest { LeaderId = leaderId };
        var testProject = TestDataHelper.CreateProject(_projectId);
        
        _mockProjectsService.Setup(x => x.GetProjectById(_projectId))
            .ReturnsAsync(testProject);
        _mockAssignmentService.Setup(x => x.AssignProjectLeader(_projectId, leaderId))
            .ReturnsAsync(Result.Success);

        // Act
        var result = await _controller.AssignLeader(_projectId, assignLeaderRequest);
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        
        _mockAssignmentService.Verify(x => x.AssignProjectLeader(_projectId, leaderId), Times.Once);
    }

    [Test]
    public async Task AssignEmployees_AsDirector_ShouldSucceed()
    {
        // Arrange
        var directorUser = TestDataHelper.CreateClaimsPrincipal(_directorId, EmployeeRole.Director);
        SetupControllerContext(directorUser);

        var employeeIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var testProject = TestDataHelper.CreateProject(_projectId);
        
        _mockProjectsService.Setup(x => x.GetProjectById(_projectId))
            .ReturnsAsync(testProject);
        _mockAssignmentService.Setup(x => x.AssignEmployeesToProject(_projectId, employeeIds))
            .ReturnsAsync(employeeIds.Count);

        // Act
        var result = await _controller.AssignEmployees(_projectId, employeeIds);
        
        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        
        _mockAssignmentService.Verify(x => x.AssignEmployeesToProject(_projectId, employeeIds), Times.Once);
    }

    #endregion
}