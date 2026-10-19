using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace aweXpect.Web.Samples;

/// <summary>
///     Sample application for aweXpect.Web
/// </summary>
#pragma warning disable S1118 // Add a 'protected' constructor or the 'static' keyword to the class declaration
public class Program
{
	/// <summary>
	///     The main method.
	/// </summary>
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		builder.Services.AddSingleton<TrackStore>();

		WebApplication app = builder.Build();

		app.MapGet("/tracks", (TrackStore store) => store.GetTracks());

		app.MapGet("/tracks/{id}", (int id, TrackStore store, HttpContext httpContext) =>
		{
			httpContext.Response.Headers["x-vendor"] = "VENDOR";
			return store.GetTrack(id);
		});

		app.MapGet("/cover", () =>
		{
			string mimeType = "image/png";
			string path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location)!, "data",
				"cover.png");
			return Results.File(path, mimeType, "cover.png");
		});

		app.Run();
	}
}
#pragma warning restore S1118
