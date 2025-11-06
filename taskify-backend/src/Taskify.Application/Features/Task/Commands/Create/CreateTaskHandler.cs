using MediatR;
using Microsoft.Extensions.Logging;
using Taskify.Application.Common;
using Taskify.Application.Mappers;
using Taskify.Core.Entities;
using Taskify.Core.Enums;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Application.Features.Task.Commands.Create;

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result<TaskItemDto>>
{
    private readonly ITaskItemRepository _repository;
    private readonly ILogger<CreateTaskHandler> _logger;

    public CreateTaskHandler(ITaskItemRepository repository, ILogger<CreateTaskHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<TaskItemDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new task with title: {Title}", request.Title);

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Status = Status.ToDo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _repository.Create(task);

        _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);

        return Result<TaskItemDto>.Ok(task.ToDto());
    }
}
