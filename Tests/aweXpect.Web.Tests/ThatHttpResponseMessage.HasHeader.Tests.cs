using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasHeader
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenMemberIsACollection_ShouldUsePluralVerb()
			{
				HttpResponseMessage[] subjects = [ResponseBuilder,];

				async Task Act()
					=> await That(new { Responses = subjects, })
						.Whose(x => x.Responses, items => items.All()
							.ComplyWith(item => item.HasHeader("x-my-header")));

				await That(Act).Throws<XunitException>()
					.WithMessage("""*whose Responses have an "x-my-header" header for all items,*""").AsWildcard()
					.Because("the verb agrees with the plural member");
			}

			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Theory]
			[InlineData("Accept-Ranges", "an")]
			[InlineData("ETag", "an")]
			[InlineData("Server", "a")]
			[InlineData("Upgrade", "an")]
			[InlineData("X-Request-Id", "an")]
			public async Task ShouldUseTheIndefiniteArticleOfTheHeaderName(string name, string article)
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader(name);

				await That(Act).Throws<XunitException>()
					.WithMessage($"""
					              Expected that subject
					              has {article} "{name}" header,
					              but it was <null>
					              """);
			}

			[Fact]
			public async Task WhenHeaderDoesNotExist_ShouldFail()
			{
				string name = "x-my-header";
				HttpResponseMessage subject = ResponseBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).HasHeader(name);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header,
					             but it did not contain the expected header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Fact]
			public async Task WhenHeaderExists_ShouldSucceed()
			{
				string name = "x-my-header";
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader(name, "some header");

				async Task Act()
					=> await That(subject).HasHeader(name);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader("x-my-header");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header,
					             but it was <null>
					             """);
			}
		}
	}
}
