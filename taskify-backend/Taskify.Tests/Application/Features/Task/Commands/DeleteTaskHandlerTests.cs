using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Taskify.Application.Features.Task.Commands.Delete;
using Taskify.Core.Entities;
using Taskify.Core.Enums;
using Taskify.Core.Errors;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Tests.Application.Features.Task.Commands;

public class DeleteTaskHandlerTests
{
    private readonly Mock<ITaskItemRepository> _repositoryMock;
    private readonly Mock<ILogger<DeleteTaskHandler>> _loggerMock;
    private readonly DeleteTaskHandler _handler;

    public DeleteTaskHandlerTests()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _loggerMock = new Mock<ILogger<DeleteTaskHandler>>();
        _handler = new DeleteTaskHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Task to Delete",
            Description = "Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var command = new DeleteTaskCommand(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Delete(taskId))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_ReturnsUnitValue()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Task to Delete",
            Description = "Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var command = new DeleteTaskCommand(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Delete(taskId))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value.Should().Be(Unit.Value);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_TaskNotFound_ReturnsFailureResult()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand(taskId);

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
    public async System.Threading.Tasks.Task Handle_TaskNotFound_DoesNotCallDelete()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskCommand(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync((TaskItem?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_CallsGetById()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Task to Delete",
            Description = "Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var command = new DeleteTaskCommand(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Delete(taskId))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetById(taskId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_CallsDelete()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Task to Delete",
            Description = "Description",
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var command = new DeleteTaskCommand(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Delete(taskId))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.Delete(taskId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_DoneTask_CanBeDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Completed Task",
            Description = "Description",
            Status = Status.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new DeleteTaskCommand(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Delete(taskId))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.Delete(taskId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_InProgressTask_CanBeDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "In Progress Task",
            Description = "Description",
            Status = Status.InProgress,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow
        };

        var command = new DeleteTaskCommand(taskId);

        _repositoryMock
            .Setup(x => x.GetById(taskId))
            .ReturnsAsync(existingTask);

        _repositoryMock
            .Setup(x => x.Delete(taskId))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.Delete(taskId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_EmptyGuid_TaskNotFound()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        var command = new DeleteTaskCommand(emptyGuid);

        _repositoryMock
            .Setup(x => x.GetById(emptyGuid))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Code.Should().Be(TaskItemErrors.TaskNotFound.Code);
    }
}
