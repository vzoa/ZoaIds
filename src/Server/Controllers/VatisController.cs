using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;
using ZoaIds.Server.Data;

namespace ZoaIds.Server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class VatisController : ControllerBase
{
	private readonly ILogger<VatisController> _logger;
	private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;

	public VatisController(ILogger<VatisController> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
	{
		_logger = logger;
		_contextFactory = contextFactory;
	}

	[HttpPost]
	[Consumes(MediaTypeNames.Application.Json)]
	public async Task<IActionResult> PostVatis(VatisJson vatisJson)
	{
		using var context = await _contextFactory.CreateDbContextAsync();
		// add new object to context
		//return CreatedAtAction(nameof(GetById_IActionResult), new { id = product.Id }, product);
	}
}
