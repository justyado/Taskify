using MediatR;
using Microsoft.Extensions.Logging;
using Taskify.Application.Common;
using Taskify.Application.Mappers;
using Taskify.Core.Enums;
using Taskify.Core.Errors;
using Taskify.Core.Interfaces.Repository;

namespace Taskify.Application.Features.Task.Commands.Update;

public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, Result<TaskItemDto>>
{
    private readonly ITaskItemRepository _repository;
    private readonly ILogger<UpdateTaskHandler> _logger;

    public UpdateTaskHandler(ITaskItemRepository repository, ILogger<UpdateTaskHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<TaskItemDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", request.Id);

        var item = await _repository.GetById(request.Id);

        if (item is null)
        {
            _logger.LogWarning("Task not found with ID: {TaskId}", request.Id);
            return Result<TaskItemDto>.Fail(TaskItemErrors.TaskNotFound);
        }

        item.Title = request.Title;
        item.Description = request.Description;
        item.Status = request.Status;
        item.UpdatedAt = DateTime.UtcNow;

        await _repository.Update(item);

        _logger.LogInformation("Task updated successfully with ID: {TaskId}, new status: {Status}", item.Id, item.Status);

        return Result<TaskItemDto>.Ok(item.ToDto());
    }
}
