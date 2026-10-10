using System.Net.Http;
using aweXpect.Core;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasContent
	{
		public sealed class ExpectationsTests
		{
			[Fact]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasContent((Action<IThat<string?>>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

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
					             but the string content was "some content", which differs at index 0:
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
					             """);
			}
		}
	}
}
