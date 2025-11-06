using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Taskify.Application.Features.Task.Queries.GetById;
using Taskify.Core.Entities;
using Taskify.Core.Enums;
using Taskify.Core.Errors;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Tests.Application.Features.Task.Queries;

public class GetTaskByIdHandlerTests
{
    private readonly Mock<ITaskItemRepository> _repositoryMock;
    private readonly Mock<ILogger<GetTaskByIdHandler>> _loggerMock;
    private readonly GetTaskByIdHandler _handler;

    public GetTaskByIdHandlerTests()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _loggerMock = new Mock<ILogger<GetTaskByIdHandler>>();
        _handler = new GetTaskByIdHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidId_ReturnsSuccessResult()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidId_ReturnsCorrectDto()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-5);
        var updatedAt = DateTime.UtcNow.AddDays(-1);

        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = Status.InProgress,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(taskId);
        result.Value.Title.Should().Be("Test Task");
        result.Value.Description.Should().Be("Test Description");
        result.Value.Status.Should().Be("InProgress");
        result.Value.CreatedAt.Should().Be(createdAt);
        result.Value.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_TaskNotFound_ReturnsFailureResult()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Code.Should().Be(TaskItemErrors.TaskNotFound.Code);
        result.Errors.First().Message.Should().Be(TaskItemErrors.TaskNotFound.Message);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidId_CallsGetById()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetById(taskId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ToDoTask_MapsStatusCorrectly()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("ToDo");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_InProgressTask_MapsStatusCorrectly()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = Status.InProgress,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("InProgress");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_DoneTask_MapsStatusCorrectly()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            Status = Status.Done,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Done");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_TaskWithNullDescription_MapsCorrectly()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Description = null,
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_TaskWithEmptyDescription_MapsCorrectly()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            Description = string.Empty,
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().Be(string.Empty);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_EmptyGuid_TaskNotFound()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        var query = new GetTaskByIdQuery(emptyGuid);

        _repositoryMock
            .Setup(x => x.GetById(emptyGuid))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Code.Should().Be(TaskItemErrors.TaskNotFound.Code);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_DifferentIds_ReturnsDifferentTasks()
    {
        // Arrange
        var taskId1 = Guid.NewGuid();
        var taskId2 = Guid.NewGuid();

        var task1 = new TaskItem
        {
            Id = taskId1,
            Title = "Task 1",
            Description = "Description 1",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var task2 = new TaskItem
        {
            Id = taskId2,
            Title = "Task 2",
            Description = "Description 2",
            Status = Status.InProgress,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query1 = new GetTaskByIdQuery(taskId1);
        var query2 = new GetTaskByIdQuery(taskId2);

        _repositoryMock
            .Setup(x => x.GetById(taskId1))
            .ReturnsAsync(task1);

        _repositoryMock
            .Setup(x => x.GetById(taskId2))
            .ReturnsAsync(task2);

        // Act
        var result1 = await _handler.Handle(query1, CancellationToken.None);
        var result2 = await _handler.Handle(query2, CancellationToken.None);

        // Assert
        result1.Value.Id.Should().Be(taskId1);
        result1.Value.Title.Should().Be("Task 1");
        result2.Value.Id.Should().Be(taskId2);
        result2.Value.Title.Should().Be("Task 2");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidId_PreservesAllTaskProperties()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddMonths(-2);
        var updatedAt = DateTime.UtcNow.AddDays(-3);

        var task = new TaskItem
        {
            Id = taskId,
            Title = "Complex Task",
            Description = "This is a very detailed description of the task",
            Status = Status.InProgress,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var query = new GetTaskByIdQuery(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(taskId);
        result.Value.Title.Should().Be("Complex Task");
        result.Value.Description.Should().Be("This is a very detailed description of the task");
        result.Value.Status.Should().Be("InProgress");
        result.Value.CreatedAt.Should().Be(createdAt);
        result.Value.UpdatedAt.Should().Be(updatedAt);
    }
}
