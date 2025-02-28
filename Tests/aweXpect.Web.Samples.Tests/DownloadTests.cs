using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Sdk;

namespace aweXpect.Web.Samples.Tests;

public class DownloadTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
	[Fact]
	public async Task DownloadFile_ShouldReturnStatusCode200Ok()
	{
		HttpClient client = factory.CreateClient();

		HttpResponseMessage response = await client.GetAsync("/download");

		await Expect.That(response).HasStatusCode().EqualTo(HttpStatusCode.OK);
	}

	[Fact]
	public async Task DownloadFile_WhenContentTypeDoesNotMatch_ShouldFail()
	{
		HttpClient client = factory.CreateClient();

		HttpResponseMessage response = await client.GetAsync("/download");

		async Task Act() =>
			await Expect.That(response).HasContentType("image/jpg");

		await Expect.That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that response
			             has a `Content-Type` header equal to "image/jpg",
			             but it was "image/png" which differs at index 6:
			                      ↓ (actual)
			               "image/png"
			               "image/jpg"
			                      ↑ (expected)

			             HTTP-Request:
			               GET http://localhost/download HTTP/1.1

			             HTTP-Response:
			               200 OK HTTP/1.1
			                 Last-Modified: ???, ?? ??? ???? ??:??:?? ???
			                 Content-Type: image/png
			                 Content-Disposition: attachment; filename=failure.png; filename*=UTF-8''failure.png
			                 Content-Length: 141834
			               *Content is binary (image/png) with length 141834*
			             """).AsWildcard();
	}
}
