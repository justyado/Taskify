using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Taskify.Application.Features.Task.Queries.GetAll;
using Taskify.Core.Entities;
using Taskify.Core.Enums;
using Taskify.Core.Errors;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Tests.Application.Features.Task.Queries;

public class GetAllTasksHandlerTests
{
    private readonly Mock<ITaskItemRepository> _repositoryMock;
    private readonly Mock<ILogger<GetAllTasksHandler>> _loggerMock;
    private readonly GetAllTasksHandler _handler;

    public GetAllTasksHandlerTests()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _loggerMock = new Mock<ILogger<GetAllTasksHandler>>();
        _handler = new GetAllTasksHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_NoFilter_ReturnsAllTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = "Description 1",
                Status = Status.ToDo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 2",
                Description = "Description 2",
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 3",
                Description = "Description 3",
                Status = Status.Done,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery(null);

        _repositoryMock
            .Setup(x => x.GetAll(null))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_FilterByToDo_ReturnsOnlyToDoTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = "Description 1",
                Status = Status.ToDo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery("ToDo");

        _repositoryMock
            .Setup(x => x.GetAll(Status.ToDo))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Status.Should().Be("ToDo");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_FilterByInProgress_ReturnsOnlyInProgressTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = "Description 1",
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery("InProgress");

        _repositoryMock
            .Setup(x => x.GetAll(Status.InProgress))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Status.Should().Be("InProgress");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_FilterByDone_ReturnsOnlyDoneTasks()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = "Description 1",
                Status = Status.Done,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery("Done");

        _repositoryMock
            .Setup(x => x.GetAll(Status.Done))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Status.Should().Be("Done");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_InvalidStatus_ReturnsFailureResult()
    {
        // Arrange
        var query = new GetAllTasksQuery("InvalidStatus");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors.First().Code.Should().Be(TaskItemErrors.InvalidStatus.Code);
        result.Errors.First().Message.Should().Be(TaskItemErrors.InvalidStatus.Message);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_InvalidStatus_DoesNotCallRepository()
    {
        // Arrange
        var query = new GetAllTasksQuery("InvalidStatus");

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetAll(It.IsAny<Status?>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_CaseInsensitiveStatus_Success()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = "Description 1",
                Status = Status.ToDo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery("todo"); // lowercase

        _repositoryMock
            .Setup(x => x.GetAll(Status.ToDo))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_MixedCaseStatus_Success()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = "Description 1",
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery("inPROGRESS"); // mixed case

        _repositoryMock
            .Setup(x => x.GetAll(Status.InProgress))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_EmptyString_TreatsAsNoFilter()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = "Description 1",
                Status = Status.ToDo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery(string.Empty);

        _repositoryMock
            .Setup(x => x.GetAll(null))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.GetAll(null), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhitespaceString_TreatsAsNoFilter()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = "Description 1",
                Status = Status.ToDo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery("   ");

        _repositoryMock
            .Setup(x => x.GetAll(null))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(x => x.GetAll(null), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_EmptyCollection_ReturnsEmptyList()
    {
        // Arrange
        var tasks = new List<TaskItem>();
        var query = new GetAllTasksQuery(null);

        _repositoryMock
            .Setup(x => x.GetAll(null))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidQuery_MapsToDto()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var updatedAt = DateTime.UtcNow;

        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description",
                Status = Status.ToDo,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            }
        };

        var query = new GetAllTasksQuery(null);

        _repositoryMock
            .Setup(x => x.GetAll(null))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        var dto = result.Value.First();
        dto.Id.Should().Be(taskId);
        dto.Title.Should().Be("Test Task");
        dto.Description.Should().Be("Test Description");
        dto.Status.Should().Be("ToDo");
        dto.CreatedAt.Should().Be(createdAt);
        dto.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_MultipleTasks_ReturnsAllMapped()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = "Description 1",
                Status = Status.ToDo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 2",
                Description = "Description 2",
                Status = Status.InProgress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery(null);

        _repositoryMock
            .Setup(x => x.GetAll(null))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().HaveCount(2);
        result.Value[0].Title.Should().Be("Task 1");
        result.Value[1].Title.Should().Be("Task 2");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidQuery_CallsRepositoryOnce()
    {
        // Arrange
        var tasks = new List<TaskItem>();
        var query = new GetAllTasksQuery("ToDo");

        _repositoryMock
            .Setup(x => x.GetAll(Status.ToDo))
            .ReturnsAsync(tasks);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetAll(Status.ToDo), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_TaskWithNullDescription_MapsCorrectly()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                Description = null,
                Status = Status.ToDo,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTasksQuery(null);

        _repositoryMock
            .Setup(x => x.GetAll(null))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.First().Description.Should().BeNull();
    }
}
