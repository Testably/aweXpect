using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Sdk;

namespace aweXpect.Web.Samples.Tests;

public class CommentsTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
	[Fact]
	public async Task GetComments_ShouldReturn200Ok()
	{
		HttpClient client = factory.CreateClient();

		HttpResponseMessage response = await client.GetAsync("/comments");

		await Expect.That(response).HasStatusCode().EqualTo(HttpStatusCode.OK);
	}

	[Fact]
	public async Task GetComments_WhenCheckingForInvalidStatusCode_ShouldHaveCorrectFailureMessage()
	{
		HttpClient client = factory.CreateClient();

		HttpResponseMessage response = await client.GetAsync("/comments");

		async Task Act() => await Expect.That(response).HasStatusCode().EqualTo(HttpStatusCode.NotFound);

		await Expect.That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that response
			             has status code 404 NotFound,
			             but it had status code 200 OK

			             HTTP-Request:
			               HTTP/1.1 200 OK
			                 Content-Type: application/json; charset=utf-8
			               [
			                 {
			                   "id": 1,
			                   "author": "Valentin",
			                   "body": "This is my first example comment"
			                 },
			                 {
			                   "id": 2,
			                   "author": "Breu\u00DF",
			                   "body": "Another comment (my second)"
			                 }
			               ]
			               The originating request was:
			                 GET http://localhost/comments HTTP 1.1
			             """);
	}
}
