using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasContent
	{
		public sealed class ExpectationsTests
		{
			[Fact]
			public async Task WhenContentDiffersFromExpected_ShouldFail()
			{
				string expected = "other content";
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasContent(which => which.IsEqualTo(expected));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a string content which is equal to "other content",
					             but the string content was "some content" which differs at index 0:
					                ↓ (actual)
					               "some content"
					               "other content"
					                ↑ (expected)

					             HTTP-Request:
					               HTTP/1.1 200 OK
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
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
					=> await That(subject).HasContent(which => which.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasContent(which => which.IsEmpty());

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a string content which is empty,
					             but it was <null>
					             
					             HTTP-Request:
					             <null>
					             """);
			}
		}
	}
}
