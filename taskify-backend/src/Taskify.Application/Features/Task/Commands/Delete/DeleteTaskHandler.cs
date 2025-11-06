using MediatR;
using Microsoft.Extensions.Logging;
using Taskify.Application.Common;
using Taskify.Core.Errors;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Application.Features.Task.Commands.Delete;

public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, Result<Unit>>
{
    private readonly ITaskItemRepository _repository;
    private readonly ILogger<DeleteTaskHandler> _logger;

    public DeleteTaskHandler(ITaskItemRepository repository, ILogger<DeleteTaskHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", request.Id);

        var task = await _repository.GetById(request.Id);

        if (task is null)
        {
            _logger.LogWarning("Task not found with ID: {TaskId}", request.Id);
            return Result<Unit>.Fail(TaskItemErrors.TaskNotFound);
        }

        await _repository.Delete(request.Id);

        _logger.LogInformation("Task deleted successfully with ID: {TaskId}", request.Id);

        return Result<Unit>.Ok(Unit.Value);
    }
}
