using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class TagService : ITagService
{
    private readonly ApplicationDbContext _context;

    public TagService(ApplicationDbContext context) => _context = context;

    public async Task<List<Tag>> GetTagsAsync(string userId) =>
        await _context.Tags.Where(t => t.UserId == userId || t.UserId == null).ToListAsync();

    public async Task<Tag?> GetTagByIdAsync(int id) =>
        await _context.Tags.FindAsync(id);

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateTagAsync(Tag tag)
    {
        _context.Entry(tag).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag == null) return false;
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return true;
    }
}
