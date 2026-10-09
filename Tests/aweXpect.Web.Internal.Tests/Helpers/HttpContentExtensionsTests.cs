using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using aweXpect.Helpers;

namespace aweXpect.Web.Internal.Tests;

public sealed class HttpContentExtensionsTests
{
	[Fact]
	public async Task IsNullOrDisposed_WhenDisposed_ShouldReturnTrue()
	{
		HttpContent content = new StringContent("");
		content.Dispose();

		bool result = content.IsNullOrDisposed();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task IsNullOrDisposed_WhenNull_ShouldReturnTrue()
	{
		HttpContent? content = null;
		bool result = content.IsNullOrDisposed();

		await That(result).IsTrue();
	}

	[Fact]
	public async Task TryGetContentLength_WhenDisposed_ShouldReturnFalse()
	{
		HttpContent content = new ByteArrayContent([]);
		content.Dispose();

		bool result = content.TryGetContentLength(out long length);

		await That(result).IsFalse();
		await That(length).IsEqualTo(0);
	}

	[Fact]
	public async Task TryGetContentLength_WhenEmpty_ShouldReturnTrue()
	{
		HttpContent content = new ByteArrayContent([]);

		bool result = content.TryGetContentLength(out long length);

		await That(result).IsTrue();
		await That(length).IsEqualTo(0);
	}

	[Fact]
	public async Task TryGetContentLength_WhenUnknown_ShouldReturnFalse()
	{
		HttpContent content = new StreamContent(new NonSeekableStream([0x1,]));

		bool result = content.TryGetContentLength(out long length);

		await That(result).IsFalse()
			.Because("a non-seekable stream that was not read yet has no known length");
		await That(length).IsEqualTo(0);
	}

	[Fact]
	public async Task TryGetMediaType_WhenDisposed_ShouldReturnMediaType()
	{
		HttpContent content = new StringContent("");
		content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
		content.Dispose();

		bool result = content.TryGetMediaType(out string? mediaType);

		await That(result).IsTrue();
		await That(mediaType).IsEqualTo("text/plain");
	}

	private sealed class NonSeekableStream(byte[] bytes) : MemoryStream(bytes)
	{
		public override bool CanSeek => false;
	}
}
