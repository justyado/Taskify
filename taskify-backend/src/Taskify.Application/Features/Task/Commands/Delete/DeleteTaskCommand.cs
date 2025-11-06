using MediatR;
using Taskify.Application.Common;

namespace Taskify.Application.Features.Task.Commands.Delete;

public record DeleteTaskCommand(Guid Id) : IRequest<Result<Unit>>;