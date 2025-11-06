using MediatR;
using Taskify.Application.Common;

namespace Taskify.Application.Features.Task.Commands.Create;

public record CreateTaskCommand(string Title, string Description) : IRequest<Result<TaskItemDto>>;