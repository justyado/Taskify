using Taskify.Application.Features.Task;
using Taskify.Core.Entities;

namespace Taskify.Application.Mappers;

public static class TaskItemMapper
{
    public static TaskItemDto ToDto(this TaskItem item)
    {
        return new TaskItemDto
        (
            item.Id,
            item.Title,
            item.Description,
            item.Status.ToString(),
            item.CreatedAt,
            item.UpdatedAt
        );
    }
}
