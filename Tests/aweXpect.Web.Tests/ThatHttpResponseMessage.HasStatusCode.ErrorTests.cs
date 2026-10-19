using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasStatusCode
	{
		public sealed class ErrorTests
		{
			[Test]
			public async Task WhenStatusCodeIs600_ShouldFail()
			{
				HttpStatusCode statusCode = (HttpStatusCode)600;
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().Error();

				await That(Act).Throws<FailException>()
					.WithMessage("*has an error status code (4xx or 5xx)*")
					.AsWildcard();
			}

			[Test]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ClientErrorStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ServerErrorStatusCodes))]
			public async Task WhenStatusCodeIsExpected_ShouldSucceed(HttpStatusCode statusCode)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().Error();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(SuccessStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(RedirectStatusCodes))]
			public async Task WhenStatusCodeIsUnexpected_ShouldFail(HttpStatusCode statusCode)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().Error();

				await That(Act).Throws<FailException>()
					.WithMessage("*has an error status code (4xx or 5xx)*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasStatusCode().Error();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an error status code (4xx or 5xx),
					             but it was <null>
					             """);
			}
		}
	}
}
