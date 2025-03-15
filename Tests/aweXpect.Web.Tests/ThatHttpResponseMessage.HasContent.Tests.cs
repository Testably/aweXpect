using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasContent
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenContentDiffersFromExpected_ShouldFail()
			{
				string expected = "other content";
				HttpResponseMessage subject = ResponseBuilder
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

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Fact]
			public async Task WhenContentEqualsExpected_ShouldSucceed()
			{
				string expected = "some content";
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(expected);

				async Task Act()
					=> await That(subject).HasContent(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

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
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContent(expected));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenContentEqualsExpected_ShouldFail()
			{
				string expected = "some content";
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(expected);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContent(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a string content equal to "some content",
					             but it had
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

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
