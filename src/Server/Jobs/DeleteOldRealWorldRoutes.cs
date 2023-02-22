using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using ZoaIds.Server.Data;

namespace ZoaIds.Server.Jobs;

public class DeleteOldRealWorldRoutes : IInvocable
{
	private readonly ILogger<DeleteOldRealWorldRoutes> _logger;
	private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
	private readonly TimeSpan _keepForDuration;

	public DeleteOldRealWorldRoutes(ILogger<DeleteOldRealWorldRoutes> logger, IDbContextFactory<ZoaIdsContext> contextFactory)
	{
		_logger = logger;
		_contextFactory = contextFactory;
		_keepForDuration = TimeSpan.FromSeconds(Constants.RoutesCacheTtlSeconds);
	}

	public async Task Invoke()
	{
		try
		{
			using var db = await _contextFactory.CreateDbContextAsync();
			var cutoff = DateTime.UtcNow - _keepForDuration;
			var routes = await db.RealWorldRoutings.ToListAsync();
			var toDeleteList = routes.Where(r => (DateTime)db.Entry(r).Property("Created").CurrentValue < cutoff).ToList();
			db.RealWorldRoutings.RemoveRange(toDeleteList);
			await db.SaveChangesAsync();
			_logger.LogInformation("Deleted {n} old FlightAware route summaries", toDeleteList.Count);
		}
		catch (Exception ex)
		{
			_logger.LogError("Error deleting old FlightAware route summaries", ex.ToString());
		}
	}
}