using MediatR;
using Microsoft.Extensions.Logging;
using Taskify.Application.Common;
using Taskify.Application.Mappers;
using Taskify.Core.Errors;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Application.Features.Task.Queries.GetById;

public class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskItemDto>>
{
    private readonly ITaskItemRepository _repository;
    private readonly ILogger<GetTaskByIdHandler> _logger;

    public GetTaskByIdHandler(ITaskItemRepository repository, ILogger<GetTaskByIdHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<TaskItemDto>> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting task by ID: {TaskId}", request.Id);

        var item = await _repository.GetById(request.Id);

        if (item is null)
        {
            _logger.LogWarning("Task not found with ID: {TaskId}", request.Id);
            return Result<TaskItemDto>.Fail(TaskItemErrors.TaskNotFound);
        }

        _logger.LogInformation("Task retrieved successfully with ID: {TaskId}", request.Id);

        return Result<TaskItemDto>.Ok(item.ToDto());
    }
}
