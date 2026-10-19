using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasStatusCode
	{
		public sealed class SuccessTests
		{
			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder.WithStatusCode(HttpStatusCode.NotFound),];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.HasStatusCode().Success()));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses have a success status code (2xx) for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}
			[Test]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(SuccessStatusCodes))]
			public async Task WhenStatusCodeIsExpected_ShouldSucceed(HttpStatusCode statusCode)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().Success();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(RedirectStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ClientErrorStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ServerErrorStatusCodes))]
			public async Task WhenStatusCodeIsUnexpected_ShouldFail(HttpStatusCode statusCode)
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().Success();

				await That(Act).Throws<FailException>()
					.WithMessage("*a success status code (2xx)*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasStatusCode().Success();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a success status code (2xx),
					             but it was <null>
					             """);
			}
		}
	}
}
