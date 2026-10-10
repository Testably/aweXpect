using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed partial class HasContent
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpRequestMessage[] subjects = [RequestBuilder.WithContent("foo"),];

				async Task Act()
					=> await That(new { Requests = subjects, })
						.Whose(x => x.Requests, items => items.All()
							.ComplyWith(item => item.HasContent("bar")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Requests have a string content equal to "bar" for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasContent((string)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

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
					             but it had string content "some content", which differs at index 0:
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
					             but it had string content "some content", which differs at index 0:
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

		public sealed class NegatedTests
		{
			[Fact]
			public async Task WhenContentDiffersFromExpected_ShouldSucceed()
			{
				string expected = "other content";
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContent(expected));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenContentEqualsExpected_ShouldFail()
			{
				string expected = "some content";
				HttpRequestMessage subject = RequestBuilder
					.WithContent(expected);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContent(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a string content equal to "some content",
					             but it had

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContent("some content"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a string content equal to "some content",
					             but it was <null>
					             """);
			}
		}
	}
}
