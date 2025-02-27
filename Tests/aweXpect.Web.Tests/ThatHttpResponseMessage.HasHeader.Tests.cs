using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasHeader
	{
		public sealed class Tests
		{
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
					             has a `x-my-header` header,
					             but it did not contain the expected header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
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
					             has a `x-my-header` header,
					             but it was <null>
					             """);
			}
		}
	}
}
