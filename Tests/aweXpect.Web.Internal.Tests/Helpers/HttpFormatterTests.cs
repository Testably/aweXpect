using System.Net.Http;
using System.Threading;
using aweXpect.Helpers;

namespace aweXpect.Web.Internal.Tests;

public sealed class HttpFormatterTests
{
	[Fact]
	public async Task Format_HttpRequestMessage_WhenNull_ShouldReturnNullString()
	{
		HttpRequestMessage? sut = null;

		string result = await HttpFormatter.Format(sut, "  ", CancellationToken.None);

		await That(result).IsEqualTo("<null>");
	}

	[Fact]
	public async Task Format_HttpResponseMessage_WhenNull_ShouldReturnNullString()
	{
		HttpResponseMessage? sut = null;

		string result = await HttpFormatter.Format(sut, "  ", CancellationToken.None);

		await That(result).IsEqualTo("<null>");
	}
}
