using MediatR;
using Taskify.Application.Common;
using Taskify.Core.Enums;

namespace Taskify.Application.Features.Task.Commands.Update;

public record UpdateTaskCommand(Guid Id, string Title, string Description, Status Status) : IRequest<Result<TaskItemDto>>;