using Microsoft.EntityFrameworkCore;
using Taskify.Core.Entities;
using Taskify.Core.Enums;
using Taskify.Core.Interfaces.Repository;
using Taskify.Infrastructure.Database;

namespace Taskify.Infrastructure.Repositories;

public class TaskItemRepository(DatabaseContext context) : ITaskItemRepository
{
    private readonly DatabaseContext _context = context;

    public async Task<ICollection<TaskItem>> GetAll(Status? status = null)
    {
        var query = _context.TaskItems
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query.ToListAsync();
    }


    public async Task<TaskItem?> GetById(Guid id)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TaskItem> Create(TaskItem item)
    {
        await _context.TaskItems.AddAsync(item);
        await _context.SaveChangesAsync();
        
        return item;
    }
//TODO: рефактор Update
    public async Task<TaskItem?> Update(TaskItem item)
    {
        var existing = await _context.TaskItems.FindAsync(item.Id);

        if (existing == null)
            return null;
        
        _context.Entry(existing).CurrentValues.SetValues(item);

        await _context.SaveChangesAsync();
        
        return existing;
    }

    public async Task Delete(Guid id)
    {
        // TODO: проверить в Api как работает если id == null
        await _context.TaskItems
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
    }
}