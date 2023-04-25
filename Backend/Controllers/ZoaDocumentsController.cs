using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZoaIdsBackend.Data;

namespace ZoaIdsBackend.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ZoaDocumentsController : ControllerBase
{
    private readonly ILogger<ZoaDocumentsController> _logger;
    private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

    public ZoaDocumentsController(ILogger<ZoaDocumentsController> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
    {
        _logger = logger;
        _contextFactory = contextFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDocuments()
    {
        using var db = await _contextFactory.CreateDbContextAsync();
        return Ok(await db.ZoaDocuments.ToListAsync());
    }
}
