using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

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
}
