using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed partial class HasHeader
	{
		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenSubjectIsNull_Negated_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHeader("x-my-key").WithValue("foo"));

				await That(Act).Throws<FailException>()
					.WithMessage("*but it was <null>").AsWildcard()
					.Because("a null subject fails the expectation and its negation alike");
			}

			[Test]
			public async Task WhenContentHeaderHasADifferentValue_ShouldFail()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://awexpect.com/")
				{
					Content = new StringContent("some content"),
				};

				async Task Act()
					=> await That(subject).HasHeader("Content-Type").WithValue("application/json");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a "Content-Type" header whose value is equal to "application/json",
					             but it had header value "text/plain; charset=utf-8", which differs at index 0:
					                ↓ (actual)
					               "text/plain; charset=utf-8"
					               "application/json"
					                ↑ (expected)

					             HTTP-Request:
					               GET https://awexpect.com/ HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenContentHeaderHasTheValue_ShouldSucceed()
			{
				HttpRequestMessage subject = new(HttpMethod.Get, "https://awexpect.com/")
				{
					Content = new StringContent("some content"),
				};

				async Task Act()
					=> await That(subject).HasHeader("Content-Type").WithValue("text/plain; charset=utf-8");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderDoesNotExist_ShouldFail()
			{
				string name = "x-my-header";
				string value = "some header";
				string otherKey = "x-some-other-key";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(otherKey).WithValue(value);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-some-other-key" header whose value is equal to "some header",
					             but it did not contain the expected header

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: some header
					             """);
			}

			[Test]
			public async Task WhenHeaderExistsAndValueDoesNotSatisfyTheExpectations_ShouldFail()
			{
				string name = "x-my-header";
				string value = "some header";
				string expectedValue = "some other header";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(name).WithValue(expectedValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose value is equal to "some other header",
					             but it had header value "some header", which differs at index 5:
					                     ↓ (actual)
					               "some header"
					               "some other header"
					                     ↑ (expected)

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: some header
					             """);
			}

			[Test]
			public async Task WhenHeaderExistsAndValueSatisfiesTheExpectations_ShouldSucceed()
			{
				string name = "x-my-header";
				string value = "some header";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(name).WithValue(value);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderValueIsNull_ShouldFail()
			{
				string name = "x-my-header";
				string otherKey = "x-some-other-key";
				HttpRequestMessage subject = RequestBuilder
					.WithHeader(name, null!);

				async Task Act()
					=> await That(subject).HasHeader(otherKey).WithValue("some header");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-some-other-key" header whose value is equal to "some header",
					             but it did not contain the expected header

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header:
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader("x-my-key").WithValue("foo");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-key" header whose value is equal to "foo",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotMatchWildcard_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("other*").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose value matches "other*",
					             but it had header value "some header", which did not match:
					               ↓ (actual)
					               "some header"
					               "other*"
					               ↑ (wildcard pattern)

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: some header
					             """);
			}

			[Test]
			public async Task WhenValueMatchesWildcard_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("some*").AsWildcard();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedWithValueTests
		{
			[Test]
			public async Task WhenHeaderExistsAndValueDiffers_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasHeader("x-my-header").WithValue("some other header"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderExistsAndValueIsEqual_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHeader("x-my-header").WithValue("some header"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an "x-my-header" header whose value is equal to "some header",
					             but it did contain the "x-my-header" header: ["some header"] and had header value "some header"

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: some header
					             """)
					.Because("the negation applies to the header as a whole, like the negated text of a `Whose`");
			}

			[Test]
			public async Task WhenValueMatchesWildcard_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasHeader("x-my-header").WithValue("some*").AsWildcard());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an "x-my-header" header whose value matches "some*",
					             but it did contain the "x-my-header" header: ["some header"] and had header value "some header"

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: some header
					             """);
			}
		}
	}
}
