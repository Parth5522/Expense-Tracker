using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers;

[Authorize]
public class TagsController : Controller
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public async Task<IActionResult> Index()
    {
        var tags = await _tagService.GetTagsAsync(GetUserId());
        return View(tags);
    }

    public IActionResult Create() => View(new Tag { Color = "#6c757d" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tag tag)
    {
        if (ModelState.IsValid)
        {
            tag.UserId = GetUserId();
            await _tagService.CreateTagAsync(tag);
            TempData["Success"] = "Tag created.";
            return RedirectToAction(nameof(Index));
        }
        return View(tag);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null || (tag.UserId != null && tag.UserId != GetUserId())) return NotFound();
        return View(tag);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Tag tag)
    {
        if (id != tag.Id) return NotFound();
        var existing = await _tagService.GetTagByIdAsync(id);
        if (existing == null || (existing.UserId != null && existing.UserId != GetUserId())) return NotFound();
        if (ModelState.IsValid)
        {
            tag.UserId = GetUserId();
            await _tagService.UpdateTagAsync(tag);
            TempData["Success"] = "Tag updated.";
            return RedirectToAction(nameof(Index));
        }
        return View(tag);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null || (tag.UserId != null && tag.UserId != GetUserId())) return NotFound();
        await _tagService.DeleteTagAsync(id);
        TempData["Success"] = "Tag deleted.";
        return RedirectToAction(nameof(Index));
    }
}
