using MediatR;
using Microsoft.Extensions.Logging;
using Taskify.Application.Common;
using Taskify.Application.Mappers;
using Taskify.Core.Enums;
using Taskify.Core.Errors;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Application.Features.Task.Queries.GetAll;

public class GetAllTasksHandler : IRequestHandler<GetAllTasksQuery, Result<List<TaskItemDto>>>
{
    private readonly ITaskItemRepository _repository;
    private readonly ILogger<GetAllTasksHandler> _logger;

    public GetAllTasksHandler(ITaskItemRepository repository, ILogger<GetAllTasksHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<List<TaskItemDto>>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all tasks with status filter: {StatusFilter}", request.Status ?? "None");

        Status? statusEnum = null;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<Status>(request.Status, ignoreCase: true, out var parsed))
            {
                statusEnum = parsed;
            }
            else
            {
                _logger.LogWarning("Invalid status provided: {Status}", request.Status);
                return Result<List<TaskItemDto>>.Fail(TaskItemErrors.InvalidStatus);
            }
        }

        var items = await _repository.GetAll(statusEnum);
        _logger.LogInformation("Retrieved {Count} tasks", items.Count);

        return Result<List<TaskItemDto>>.Ok(items.Select(x => x.ToDto()).ToList());
    }
}
