namespace Taskify.Application.Features.Task;

public record TaskItemDto(Guid Id, string Title, string? Description, string Status, DateTime CreatedAt, DateTime UpdatedAt);