using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers.Api;

[ApiController]
[Route("api/v1/tags")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TagsApiController : ControllerBase
{
    private readonly ITagService _tagService;

    public TagsApiController(ITagService tagService) => _tagService = tagService;

    private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _tagService.GetTagsAsync(GetUserId());
        return Ok(tags);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null) return NotFound();
        if (tag.UserId != null && tag.UserId != GetUserId()) return NotFound();
        return Ok(tag);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Tag tag)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        tag.UserId = GetUserId();
        var created = await _tagService.CreateTagAsync(tag);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Tag tag)
    {
        if (id != tag.Id) return BadRequest();
        var existing = await _tagService.GetTagByIdAsync(id);
        if (existing == null) return NotFound();
        if (existing.UserId != null && existing.UserId != GetUserId()) return NotFound();
        tag.UserId = GetUserId();
        await _tagService.UpdateTagAsync(tag);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _tagService.GetTagByIdAsync(id);
        if (existing == null) return NotFound();
        if (existing.UserId != null && existing.UserId != GetUserId()) return NotFound();
        await _tagService.DeleteTagAsync(id);
        return NoContent();
    }
}
