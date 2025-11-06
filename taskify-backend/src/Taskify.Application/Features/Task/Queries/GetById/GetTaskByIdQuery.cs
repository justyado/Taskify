using MediatR;
using Taskify.Application.Common;

namespace Taskify.Application.Features.Task.Queries.GetById;

public record GetTaskByIdQuery(Guid Id) : IRequest<Result<TaskItemDto>>;