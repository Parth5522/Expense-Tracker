using ExpenseTracker.Models;
using Microsoft.AspNetCore.Http;

namespace ExpenseTracker.Services;

public interface IAttachmentService
{
    Task<Attachment> UploadAsync(IFormFile file, int? expenseId, string userId);
    Task<Attachment?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<List<Attachment>> GetByExpenseIdAsync(int expenseId);
}
