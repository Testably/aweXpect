using System.Net.Http;
using System.Threading;
using aweXpect.Core;
using aweXpect.Web.Tests.TestHelpers;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasContent
	{
		public sealed class ExpectationsTests
		{
			[Test]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasContent((Action<IThat<string?>>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenContentDiffersFromExpected_ShouldFail()
			{
				string expected = "other content";
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasContent(which => which.IsEqualTo(expected));

				await That(Act).Throws<FailException>()
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

			[Test]
			public async Task WhenContentEqualsExpected_ShouldSucceed()
			{
				string expected = "some content";
				HttpResponseMessage subject = ResponseBuilder
					.WithContent(expected);

				async Task Act()
					=> await That(subject).HasContent(which => which.IsEqualTo(expected));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenContentHasDifferentLineCount_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("foo");

				async Task Act()
					=> await That(subject).HasContent(which => which.HasLineCount().EqualTo(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a string content which has line count equal to 2,
					             but the string content had line count 1

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 3
					               foo
					             """);
			}

			[Test]
			public async Task WhenContentIsNotSet_ShouldTreatItAsEmpty()
			{
				HttpResponseMessage subject = new();

				async Task Act()
					=> await That(subject).HasContent(which => which.IsEmpty());

				await That(Act).DoesNotThrow()
					.Because("on .NET Framework the content of a new response is null, but on .NET 5+ it is empty");
			}

			[Test]
			public async Task WhenSubjectIsNull_Negated_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasContent(which => which.IsEmpty()));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a string content which is not empty,
					             but it was <null>
					             """)
					.Because("a null subject has no content, so the negated expectation fails as well");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasContent(which => which.IsEmpty());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a string content which is empty,
					             but it was <null>
					             """);
			}

#if NET8_0_OR_GREATER
			[Test]
			public async Task WhenTheEvaluationIsCanceledWhileTheContentIsRead_ShouldCancelReadingTheContent()
			{
				using CancellationTokenSource cts = new();
				PendingContent content = new(cts);
				HttpResponseMessage subject = new()
				{
					Content = content,
				};

				async Task Act()
					=> await That(subject).HasContent(which => which.IsEmpty()).WithCancellation(cts.Token);

				await That(Act).Throws<TUnit.Core.Exceptions.InconclusiveTestException>()
					.WithMessage("""
					             Expected that subject
					             has a string content which is empty,
					             but it could not be verified, because the evaluation was already canceled
					             """);
				await That(content.ReadToken.IsCancellationRequested).IsTrue()
					.Because("the cancellation of the evaluation must also stop reading a slow content");
			}
#endif
		}
	}
}
