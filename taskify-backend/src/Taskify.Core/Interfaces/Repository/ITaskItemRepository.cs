using Taskify.Core.Entities;
using Taskify.Core.Enums;

namespace Taskify.Core.Interfaces.Repository;

public interface ITaskItemRepository
{
    Task<ICollection<TaskItem>> GetAll(Status? status);
    Task<TaskItem?> GetById(Guid id);
    Task<TaskItem> Create(TaskItem item);
    Task<TaskItem?> Update(TaskItem item);
    Task Delete(Guid id);
}