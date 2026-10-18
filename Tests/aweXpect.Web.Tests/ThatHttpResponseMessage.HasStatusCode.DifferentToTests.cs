using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasStatusCode
	{
		public sealed class DifferentToTests
		{
			[Test]
			public async Task WhenMemberIsACollection_Negated_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder,];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.DoesNotComplyWith(response => response.HasStatusCode().DifferentTo(HttpStatusCode.NotFound))));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses have status code 404 NotFound for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder,];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.HasStatusCode().DifferentTo(HttpStatusCode.OK)));

				await That(Act).Throws<FailException>()
					.WithMessage("""*whose Responses have status code different to 200 OK for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Test]
			public async Task WhenStatusCodeDiffersFromExpected_ShouldSucceed()
			{
				HttpStatusCode unexpected = HttpStatusCode.OK;
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(HttpStatusCode.BadRequest);

				async Task Act()
					=> await That(subject).HasStatusCode().DifferentTo(unexpected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(SuccessStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(RedirectStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ClientErrorStatusCodes))]
			[MethodDataSource(typeof(ThatHttpResponseMessage), nameof(ServerErrorStatusCodes))]
			public async Task WhenStatusCodeIsUnexpected_ShouldFail(HttpStatusCode statusCode)
			{
				HttpStatusCode unexpected = statusCode;
				HttpResponseMessage subject = ResponseBuilder
					.WithStatusCode(statusCode);

				async Task Act()
					=> await That(subject).HasStatusCode().DifferentTo(unexpected);

				await That(Act).Throws<FailException>()
					.WithMessage("*status code different to*")
					.AsWildcard();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasStatusCode().DifferentTo(HttpStatusCode.Accepted);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has status code different to 202 Accepted,
					             but it was <null>
					             """);
			}
		}
	}
}
