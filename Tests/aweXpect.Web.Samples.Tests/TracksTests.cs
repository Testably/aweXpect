using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Sdk;

namespace aweXpect.Web.Samples.Tests;

public class TracksTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
	[Fact]
	public async Task GetTrack_WhenCheckingForInvalidStatusCode_ShouldHaveFailureMessageFromReadme()
	{
		HttpClient httpClient = factory.CreateClient();

		async Task Act()
		{
			HttpResponseMessage response = await httpClient.GetAsync("/tracks/1");

			await Expect.That(response).HasStatusCode(HttpStatusCode.NotFound);
		}

		await Expect.That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that response
			             has status code 404 NotFound,
			             but it had status code 200 OK

			             HTTP-Request:
			               GET http://localhost/tracks/1 HTTP/1.1

			             HTTP-Response:
			               200 OK HTTP/1.1
			                 x-vendor: VENDOR
			                 Content-Type: application/json; charset=utf-8
			                 Content-Length: 51
			               {
			                 "id": 1,
			                 "title": "Let It Be",
			                 "artist": "The Beatles"
			               }
			             """)
			.Because("the README shows this code and its failure message");
	}

	[Fact]
	public async Task GetTracks_ShouldReturn200Ok()
	{
		HttpClient client = factory.CreateClient();

		HttpResponseMessage response = await client.GetAsync("/tracks");

		await Expect.That(response).HasStatusCode().EqualTo(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetTracks_ShouldReturnExpectedBody()
	{
		HttpClient client = factory.CreateClient();

		HttpResponseMessage response = await client.GetAsync("/tracks");

		await Expect.That(response).HasContent(which =>
			which.IsValidJsonMatching([
				new
				{
					title = "Let It Be",
					artist = "The Beatles",
				},
				new
				{
					title = "Jóga",
					artist = "Björk",
				},
			]));
	}

	[Fact]
	public async Task GetTracks_WhenCheckingForInvalidStatusCode_ShouldHaveCorrectFailureMessage()
	{
		HttpClient client = factory.CreateClient();

		HttpResponseMessage response = await client.GetAsync("/tracks");

		async Task Act() => await Expect.That(response).HasStatusCode().EqualTo(HttpStatusCode.NotFound);

		await Expect.That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that response
			             has status code 404 NotFound,
			             but it had status code 200 OK

			             HTTP-Request:
			               GET http://localhost/tracks HTTP/1.1

			             HTTP-Response:
			               200 OK HTTP/1.1
			                 Content-Type: application/json; charset=utf-8
			                 Content-Length: 96
			               [
			                 {
			                   "id": 1,
			                   "title": "Let It Be",
			                   "artist": "The Beatles"
			                 },
			                 {
			                   "id": 2,
			                   "title": "J\u00F3ga",
			                   "artist": "Bj\u00F6rk"
			                 }
			               ]
			             """);
	}
}
