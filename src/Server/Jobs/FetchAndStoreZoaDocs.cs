using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Coravel.Invocable;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using ZoaIds.Server.Data;
using ZoaIds.Shared.Models;

namespace ZoaIds.Server.Jobs;

public partial class FetchAndStoreZoaDocs : IInvocable
{
	private readonly ILogger<FetchAndStoreZoaDocs> _logger;
	private readonly IDbContextFactory<ZoaIdsContext> _contextFactory;
	private readonly HttpClient _httpClient;
	private readonly IWebHostEnvironment _environment;

	public FetchAndStoreZoaDocs(ILogger<FetchAndStoreZoaDocs> logger, IDbContextFactory<ZoaIdsContext> contextFactory, HttpClient httpClient, IWebHostEnvironment environment)
	{
		_logger = logger;
		_httpClient = httpClient;
		_contextFactory = contextFactory;
		_environment = environment;
	}

	public async Task Invoke()
	{
		// TODO -- log and add try/catch

		// Fetch OAK ARTCC site and parse HTML
		using var stream = await _httpClient.GetStreamAsync(Constants.Urls.ZoaProcedures);
		var parser = new HtmlParser();
		using var document = await parser.ParseDocumentAsync(stream);

		// Select all the TR elements on the page and try to parse document info
		var foundDocs = new List<ArtccDocument>();
		var trElements = document.QuerySelectorAll(".table>tbody>tr");
		foreach (var trElement in trElements)
		{
			if (TryParseZoaDocument(trElement, out var doc)) foundDocs.Add(doc!);
		}

		// Add new or updated docs to the DB
		using var db = await _contextFactory.CreateDbContextAsync();
		var existingDocsDict = await db.ZoaDocuments.ToDictionaryAsync(d => d.Name);
		foreach (var newDoc in foundDocs)
		{
			// If there isn't a doc in the DB with the same name as the new doc, or the new doc is more recent, add to DB
			if(!existingDocsDict.TryGetValue(newDoc.Name, out var existingDoc) || newDoc.EffectiveDate > existingDoc.EffectiveDate)
			{
				// Open stream to PDF file on OAKARTCC site
				using var pdfStream = await _httpClient.GetStreamAsync(newDoc.OriginalPdfUrl);

				// Create the sanitized paths for our stored version of the PDF in wwwroot
				var s = _environment.WebRootPath;
				var pdfFolderPath = Path.Combine(_environment.WebRootPath, Constants.ZoaDocumentsPdfPath);
				var newPdfPath = Path.ChangeExtension(Path.Combine(pdfFolderPath, newDoc.Name), ".pdf").Replace("/", "-");

				// Copy stream to new file
				using var pdfNewFile = File.Create(newPdfPath);
				await pdfStream.CopyToAsync(pdfNewFile);

				// Write the new relative path so we can serve from wwwroot later
				var baseUri = new Uri(Constants.ZoaDocumentsPdfPath, UriKind.Relative);
				var sanitizedFilename = Path.ChangeExtension(newDoc.Name, ".pdf").Replace("/", "-");
				newDoc.LocalRelativePdfUrl = $"{Constants.ZoaDocumentsPdfPath}/{sanitizedFilename}";

				// Delete old doc from db if it exists, then add new doc
				if (existingDoc is not null)
				{
					db.ZoaDocuments.Remove(existingDoc);
				}
				await db.ZoaDocuments.AddAsync(newDoc);
			}

		}
		await db.SaveChangesAsync();
	}

	private static bool TryParseZoaDocument(IElement trElement, out ArtccDocument? zoaDocument)
	{
		zoaDocument = null;
		try
		{
			var tds = trElement.QuerySelectorAll("td").ToList();
			var pdfUrl = TryParsePdfUrl(tds[3].QuerySelector("a")?.GetAttribute("href"), out var fullUrl) ? fullUrl : string.Empty;
			var type = tds[0].TextContent switch
			{
				string n when n.Contains("CPS") => ArtccDocumentType.CentralPolicyStatement,
				string n when n.Contains("SOP") => ArtccDocumentType.StandardOperatingProcedures,
				string n when n.Contains("LOA") => ArtccDocumentType.LetterOfAgreement,
				_                               => ArtccDocumentType.Other
			};

			zoaDocument = new ArtccDocument
			{
				Name = tds[0].TextContent,
				Description = tds[1].TextContent,
				OriginalPdfUrl = pdfUrl,
				Type = type,
				EffectiveDate = DateOnly.Parse(tds[2].TextContent)
			};

			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private static bool TryParsePdfUrl(string pdfUrlFromTag, out string? fullPdfUrl)
	{
		fullPdfUrl = null;
		try
		{
			var matches = PdfUrlRegex().Match(pdfUrlFromTag);
			fullPdfUrl = matches.Success ? $"https://oakartcc.org{matches.Groups[1].Value}" : null;
			return matches.Success;
		}
		catch (Exception)
		{
			return false;
		}
	}

	[GeneratedRegex("\\?file=([\\S]+)")]
	private static partial Regex PdfUrlRegex();
}
