using MediatR;
using Taskify.Application.Common;

namespace Taskify.Application.Features.Task.Queries.GetAll;

public record GetAllTasksQuery(string? Status) : IRequest<Result<List<TaskItemDto>>>;