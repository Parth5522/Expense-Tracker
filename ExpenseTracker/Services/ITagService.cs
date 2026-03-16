using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface ITagService
{
    Task<List<Tag>> GetTagsAsync(string userId);
    Task<Tag?> GetTagByIdAsync(int id);
    Task<Tag> CreateTagAsync(Tag tag);
    Task<Tag> UpdateTagAsync(Tag tag);
    Task<bool> DeleteTagAsync(int id);
}
