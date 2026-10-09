using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
	public async Task Format_HttpRequestMessage_WhenFormattedTwice_ShouldReturnSameText()
	{
		HttpRequestMessage sut = new(HttpMethod.Post, "https://aweXpect.com")
		{
			Content = new StringContent("foo"),
		};

		string first = await HttpFormatter.Format(sut, "  ", CancellationToken.None);
		string second = await HttpFormatter.Format(sut, "  ", CancellationToken.None);

		await That(second).IsEqualTo(first)
			.Because("the failure message shows equal contexts only once");
	}

	[Fact]
	public async Task Format_HttpResponseMessage_WhenFormattedTwice_ShouldReturnSameText()
	{
		HttpResponseMessage sut = new()
		{
			Content = new StringContent("foo"),
		};

		string first = await HttpFormatter.Format(sut, "  ", CancellationToken.None);
		string second = await HttpFormatter.Format(sut, "  ", CancellationToken.None);

		await That(second).IsEqualTo(first)
			.Because("the failure message shows equal contexts only once");
	}

	[Theory]
	[InlineData("text/plain", "foo")]
	[InlineData("application/json", "{\"foo\":1}")]
	public async Task Format_HttpResponseMessage_WhenLengthIsInitiallyUnknown_ShouldReturnSameText(
		string mediaType, string body)
	{
		StreamContent content = new(new NonSeekableStream(Encoding.UTF8.GetBytes(body)));
		content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
		HttpResponseMessage sut = new()
		{
			Content = content,
		};

		string first = await HttpFormatter.Format(sut, "  ", CancellationToken.None);
		string second = await HttpFormatter.Format(sut, "  ", CancellationToken.None);

		await That(first).Contains($"Content-Length: {body.Length}");
		await That(second).IsEqualTo(first)
			.Because("the failure message shows equal contexts only once");
	}

	[Fact]
	public async Task Format_HttpResponseMessage_WhenNull_ShouldReturnNullString()
	{
		HttpResponseMessage? sut = null;

		string result = await HttpFormatter.Format(sut, "  ", CancellationToken.None);

		await That(result).IsEqualTo("<null>");
	}

	private sealed class NonSeekableStream(byte[] bytes) : MemoryStream(bytes)
	{
		public override bool CanSeek => false;
	}
}
