using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Taskify.Application.Features.Task.Commands.Create;
using Taskify.Core.Entities;
using Taskify.Core.Enums;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Tests.Application.Features.Task.Commands;

public class CreateTaskHandlerTests
{
    private readonly Mock<ITaskItemRepository> _repositoryMock;
    private readonly Mock<ILogger<CreateTaskHandler>> _loggerMock;
    private readonly CreateTaskHandler _handler;

    public CreateTaskHandlerTests()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _loggerMock = new Mock<ILogger<CreateTaskHandler>>();
        _handler = new CreateTaskHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateTaskCommand("Test Task", "Test Description");

        _repositoryMock
            .Setup(x => x.Create(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem item) => item);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_ReturnsCorrectDto()
    {
        // Arrange
        var command = new CreateTaskCommand("Test Task", "Test Description");

        _repositoryMock
            .Setup(x => x.Create(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem item) => item);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be("Test Task");
        result.Value.Description.Should().Be("Test Description");
        result.Value.Status.Should().Be("ToDo");
        result.Value.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_SetsCorrectStatus()
    {
        // Arrange
        var command = new CreateTaskCommand("Test Task", "Test Description");
        TaskItem? capturedTask = null;

        _repositoryMock
            .Setup(x => x.Create(It.IsAny<TaskItem>()))
            .Callback<TaskItem>(task => capturedTask = task)
            .ReturnsAsync((TaskItem item) => item);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedTask.Should().NotBeNull();
        capturedTask!.Status.Should().Be(Status.ToDo);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_SetsTimestamps()
    {
        // Arrange
        var command = new CreateTaskCommand("Test Task", "Test Description");
        var beforeExecution = DateTime.UtcNow;
        TaskItem? capturedTask = null;

        _repositoryMock
            .Setup(x => x.Create(It.IsAny<TaskItem>()))
            .Callback<TaskItem>(task => capturedTask = task)
            .ReturnsAsync((TaskItem item) => item);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        var afterExecution = DateTime.UtcNow;

        // Assert
        capturedTask.Should().NotBeNull();
        capturedTask!.CreatedAt.Should().BeOnOrAfter(beforeExecution).And.BeOnOrBefore(afterExecution);
        capturedTask.UpdatedAt.Should().BeOnOrAfter(beforeExecution).And.BeOnOrBefore(afterExecution);
        capturedTask.CreatedAt.Should().BeCloseTo(capturedTask.UpdatedAt, TimeSpan.FromMilliseconds(100));
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_GeneratesUniqueId()
    {
        // Arrange
        var command = new CreateTaskCommand("Test Task", "Test Description");
        TaskItem? capturedTask = null;

        _repositoryMock
            .Setup(x => x.Create(It.IsAny<TaskItem>()))
            .Callback<TaskItem>(task => capturedTask = task)
            .ReturnsAsync((TaskItem item) => item);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedTask.Should().NotBeNull();
        capturedTask!.Id.Should().NotBeEmpty();
        result.Value.Id.Should().Be(capturedTask.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_CallsRepositoryCreate()
    {
        // Arrange
        var command = new CreateTaskCommand("Test Task", "Test Description");

        _repositoryMock
            .Setup(x => x.Create(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem item) => item);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.Create(It.Is<TaskItem>(t =>
                t.Title == "Test Task" &&
                t.Description == "Test Description" &&
                t.Status == Status.ToDo)),
            Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_EmptyDescription_CreatesTaskSuccessfully()
    {
        // Arrange
        var command = new CreateTaskCommand("Test Task", string.Empty);

        _repositoryMock
            .Setup(x => x.Create(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem item) => item);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().Be(string.Empty);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_MultipleInvocations_GeneratesDifferentIds()
    {
        // Arrange
        var command1 = new CreateTaskCommand("Task 1", "Description 1");
        var command2 = new CreateTaskCommand("Task 2", "Description 2");

        _repositoryMock
            .Setup(x => x.Create(It.IsAny<TaskItem>()))
            .ReturnsAsync((TaskItem item) => item);

        // Act
        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.Value.Id.Should().NotBe(result2.Value.Id);
    }
}
