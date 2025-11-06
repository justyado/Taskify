using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Taskify.Application.Features.Task.Commands.Update;
using Taskify.Core.Entities;
using Taskify.Core.Enums;
using Taskify.Core.Errors;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Tests.Application.Features.Task.Commands;

public class UpdateTaskHandlerTests
{
    private readonly Mock<ITaskItemRepository> _repositoryMock;
    private readonly Mock<ILogger<UpdateTaskHandler>> _loggerMock;
    private readonly UpdateTaskHandler _handler;

    public UpdateTaskHandlerTests()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _loggerMock = new Mock<ILogger<UpdateTaskHandler>>();
        _handler = new UpdateTaskHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand(taskId, "New Title", "New Description", Status.InProgress);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_UpdatesAllFields()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand(taskId, "New Title", "New Description", Status.InProgress);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be("New Title");
        result.Value.Description.Should().Be("New Description");
        result.Value.Status.Should().Be("InProgress");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_TaskNotFound_ReturnsFailureResult()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new UpdateTaskCommand(taskId, "New Title", "New Description", Status.InProgress);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Code.Should().Be(TaskItemErrors.TaskNotFound.Code);
        result.Errors.First().Message.Should().Be(TaskItemErrors.TaskNotFound.Message);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_TaskNotFound_DoesNotCallUpdate()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new UpdateTaskCommand(taskId, "New Title", "New Description", Status.InProgress);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync((TaskItem?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.Update(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_UpdatesTimestamp()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var oldTimestamp = DateTime.UtcNow.AddDays(-1);
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Status.ToDo,
            CreatedAt = oldTimestamp,
            UpdatedAt = oldTimestamp
        };

        var command = new UpdateTaskCommand(taskId, "New Title", "New Description", Status.InProgress);
        var beforeExecution = DateTime.UtcNow;

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var afterExecution = DateTime.UtcNow;

        // Assert
        result.Value.UpdatedAt.Should().BeOnOrAfter(beforeExecution).And.BeOnOrBefore(afterExecution);
        result.Value.UpdatedAt.Should().BeAfter(oldTimestamp);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_CallsGetById()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand(taskId, "New Title", "New Description", Status.InProgress);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetById(taskId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_CallsUpdate()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand(taskId, "New Title", "New Description", Status.InProgress);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.Update(It.Is<TaskItem>(t =>
                t.Id == taskId &&
                t.Title == "New Title" &&
                t.Description == "New Description" &&
                t.Status == Status.InProgress)),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_UpdateToToDoStatus_Success()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Task",
            Description = "Description",
            Status = Status.InProgress,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand(taskId, "Task", "Description", Status.ToDo);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("ToDo");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_UpdateToDoneStatus_Success()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Task",
            Description = "Description",
            Status = Status.InProgress,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand(taskId, "Task", "Description", Status.Done);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Done");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_UpdateWithEmptyDescription_Success()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Task",
            Description = "Old Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand(taskId, "Task", string.Empty, Status.ToDo);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().Be(string.Empty);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_PreservesCreatedAt()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-5);
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Status.ToDo,
            CreatedAt = createdAt,
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand(taskId, "New Title", "New Description", Status.InProgress);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_PreservesId()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Old Title",
            Description = "Old Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskCommand(taskId, "New Title", "New Description", Status.InProgress);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Update(It.IsAny<TaskItem>()))
            .ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value.Id.Should().Be(taskId);
    }
}
