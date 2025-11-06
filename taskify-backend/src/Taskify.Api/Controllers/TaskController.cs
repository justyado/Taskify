using MediatR;
using Microsoft.AspNetCore.Mvc;
using Taskify.Application.Common;
using Taskify.Application.Features.Task;
using Taskify.Application.Features.Task.Commands.Create;
using Taskify.Application.Features.Task.Commands.Delete;
using Taskify.Application.Features.Task.Commands.Update;
using Taskify.Application.Features.Task.Queries.GetAll;
using Taskify.Application.Features.Task.Queries.GetById;

namespace Taskify.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Result<List<TaskItemDto>>> GetAll([FromQuery] string? status)
        => await _mediator.Send(new GetAllTasksQuery(status));

    [HttpGet("{id:guid}")]
    public async Task<Result<TaskItemDto>> GetById(Guid id)
        => await _mediator.Send(new GetTaskByIdQuery(id));

    [HttpPost]
    public async Task<Result<TaskItemDto>> Create([FromBody] CreateTaskCommand command)
        => await _mediator.Send(command);

    [HttpPut("{id:guid}")]
    public async Task<Result<TaskItemDto>> Update([FromBody] UpdateTaskCommand command, [FromRoute] Guid id)
        => await _mediator.Send(command with { Id = id });

    [HttpDelete("{id:guid}")]
    public async Task<Result> Delete(Guid id)
        => await _mediator.Send(new DeleteTaskCommand(id));
}
