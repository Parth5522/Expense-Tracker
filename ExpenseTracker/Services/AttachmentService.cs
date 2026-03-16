using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class AttachmentService : IAttachmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AttachmentService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<Attachment> UploadAsync(IFormFile file, int? expenseId, string userId)
    {
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        var attachment = new Attachment
        {
            FileName = file.FileName,
            FilePath = $"/uploads/{uniqueFileName}",
            ContentType = file.ContentType,
            FileSize = file.Length,
            ExpenseId = expenseId,
            UserId = userId,
            UploadedAt = DateTime.UtcNow
        };
        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync();
        return attachment;
    }

    public async Task<Attachment?> GetByIdAsync(int id) =>
        await _context.Attachments.FindAsync(id);

    public async Task<bool> DeleteAsync(int id)
    {
        var attachment = await _context.Attachments.FindAsync(id);
        if (attachment == null) return false;

        var uploadsFolder = Path.GetFullPath(Path.Combine(_environment.WebRootPath, "uploads"));
        var fullPath = Path.GetFullPath(Path.Combine(_environment.WebRootPath, attachment.FilePath.TrimStart('/')));

        // Guard against path traversal
        if (fullPath.StartsWith(uploadsFolder, StringComparison.OrdinalIgnoreCase) && File.Exists(fullPath))
            File.Delete(fullPath);

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Attachment>> GetByExpenseIdAsync(int expenseId) =>
        await _context.Attachments.Where(a => a.ExpenseId == expenseId).ToListAsync();
}
