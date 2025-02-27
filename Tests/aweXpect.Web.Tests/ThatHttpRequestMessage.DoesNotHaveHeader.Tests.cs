using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed class DoesNotHaveHeader
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WhenHeaderDoesNotExist_ShouldSucceed()
			{
				string name = "x-my-header";
				HttpRequestMessage subject = RequestBuilder
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotHaveHeader(name);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenHeaderExists_ShouldFail()
			{
				string name = "x-my-header";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, "some header")
					.WithContent("some content");

				async Task Act()
					=> await That(subject).DoesNotHaveHeader(name);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `x-my-header` header,
					             but it did contain the `x-my-header` header: ["some header"]

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					               some content
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotHaveHeader("x-my-header");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a `x-my-header` header,
					             but it was <null>
					             """);
			}
		}
	}
}
