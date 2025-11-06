namespace Taskify.Core.Errors;

public static class TaskItemErrors
{
    public static Error TaskNotFound => new Error("Task not found", "TaskItem.TaskNotFound");
    public static Error InvalidStatus => new Error("Invalid status", "TaskItem.InvalidStatus");
    public static Error InvalidTitle => new Error("Invalid title", "TaskItem.InvalidTitle");
    public static Error TaskAlreadyCompleted => new Error("Task already completed", "TaskItem.TaskAlreadyCompleted");
    public static Error CannotModifyCompletedTask => new Error("Cannot modify completed task", "TaskItem.CannotModifyCompletedTask");
}