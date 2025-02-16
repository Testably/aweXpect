#if NET8_0_OR_GREATER
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed class HasContent
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

					             HTTP-Request:
					               HTTP/1.1 200 OK
					               some content
					               The originating request was <null>
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
	}
}
#endif
