using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed partial class HasContent
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ContentLengthHeader_ShouldBeLastHeader()
			{
				string expected = "other content";
				StringContent content = new("some content");
				content.Headers.Add("x-foo", "bar");
				HttpRequestMessage subject = RequestBuilder
					.WithContent(content);

				async Task Act()
					=> await That(subject).HasContent(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a string content equal to "other content",
					             but it was "some content" which differs at index 0:
					                ↓ (actual)
					               "some content"
					               "other content"
					                ↑ (expected)

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 x-foo: bar
					                 Content-Length: 12
					               some content
					             """);
			}

			[Fact]
			public async Task WhenContentDiffersFromExpected_ShouldFail()
			{
				string expected = "other content";
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasContent(expected);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a string content equal to "other content",
					             but it was "some content" which differs at index 0:
					                ↓ (actual)
					               "some content"
					               "other content"
					                ↑ (expected)

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Fact]
			public async Task WhenContentEqualsExpected_ShouldSucceed()
			{
				string expected = "some content";
				HttpRequestMessage subject = RequestBuilder
					.WithContent(expected);

				async Task Act()
					=> await That(subject).HasContent(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenContentIsNull_ShouldFail()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://awexpect.com");

				async Task Act()
					=> await That(subject).HasContent("foo");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a string content equal to "foo",
					             but it had a <null> content

					             HTTP-Request:
					               GET https://awexpect.com/ HTTP/1.1
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasContent("some content");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a string content equal to "some content",
					             but it was <null>
					             """);
			}
		}
	}
}
