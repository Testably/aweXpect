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

	[Theory]
	[InlineData(null, "*Content with unknown length*")]
	[InlineData("application/my-type", "*Content (application/my-type) with unknown length*")]
	[InlineData("image/png", "*Content is binary (image/png) with unknown length*")]
	public async Task Format_HttpRequestMessage_WhenLengthIsUnknown_ShouldStateThatTheLengthIsUnknown(
		string? mediaType, string expectedContentLine)
	{
		NonSeekableStream stream = new("foo"u8.ToArray());
		StreamContent content = new(stream);
		if (mediaType != null)
		{
			content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
		}

		HttpRequestMessage sut = new(HttpMethod.Post, "https://aweXpect.com")
		{
			Content = content,
		};

		string first = await HttpFormatter.Format(sut, "  ", CancellationToken.None);
		string second = await HttpFormatter.Format(sut, "  ", CancellationToken.None);

		await That(first).EndsWith($"\n  {expectedContentLine}");
		await That(first).DoesNotContain("Content-Length");
		await That(second).IsEqualTo(first)
			.Because("the failure message shows equal contexts only once");
		await That(stream.Position).IsEqualTo(0L)
			.Because("the context must not read the content to find out its length");
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

	[Theory]
	[InlineData(null, "*Content with unknown length*")]
	[InlineData("application/my-type", "*Content (application/my-type) with unknown length*")]
	[InlineData("image/png", "*Content is binary (image/png) with unknown length*")]
	public async Task Format_HttpResponseMessage_WhenLengthIsUnknown_ShouldStateThatTheLengthIsUnknown(
		string? mediaType, string expectedContentLine)
	{
		NonSeekableStream stream = new("foo"u8.ToArray());
		StreamContent content = new(stream);
		if (mediaType != null)
		{
			content.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
		}

		HttpResponseMessage sut = new()
		{
			Content = content,
		};

		string first = await HttpFormatter.Format(sut, "  ", CancellationToken.None);
		string second = await HttpFormatter.Format(sut, "  ", CancellationToken.None);

		await That(first).EndsWith($"\n  {expectedContentLine}");
		await That(first).DoesNotContain("Content-Length");
		await That(second).IsEqualTo(first)
			.Because("the failure message shows equal contexts only once");
		await That(stream.Position).IsEqualTo(0L)
			.Because("the context must not read the content to find out its length");
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
