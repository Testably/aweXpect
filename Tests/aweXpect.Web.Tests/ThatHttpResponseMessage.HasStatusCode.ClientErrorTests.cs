using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasStatusCode
	{
		public sealed class ClientErrorTests
		{
			[Test]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ClientErrorStatusCodes))]
			public async Task WhenStatusCodeIsExpected_ShouldSucceed(HttpStatusCode statusCode)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().ClientError();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(SuccessStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(RedirectStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ServerErrorStatusCodes))]
			public async Task WhenStatusCodeIsUnexpected_ShouldFail(HttpStatusCode statusCode)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().ClientError();

				await That(Act).Throws<FailException>()
					.WithMessage("*has a client error status code (4xx)*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasStatusCode().ClientError();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a client error status code (4xx),
					             but it was <null>
					             """);
			}
		}
	}
}
