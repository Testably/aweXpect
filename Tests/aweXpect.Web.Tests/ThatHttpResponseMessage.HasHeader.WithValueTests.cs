using System.Net;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasHeader
	{
		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenCombiningMatchTypes_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("some*").AsWildcard().AsPrefix();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("AsPrefix cannot be combined with AsWildcard.");
			}

			[Test]
			public async Task WhenCombiningWildcardWithUsing_ShouldThrowInvalidOperationException()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("some*").AsWildcard()
						.Using(StringComparer.Ordinal);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Using cannot be combined with AsWildcard.");
			}

			[Test]
			public async Task WhenContentHeaderHasADifferentValue_ShouldFail()
			{
				HttpResponseMessage subject = new(HttpStatusCode.OK)
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

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 12
					               some content
					             """);
			}

			[Test]
			public async Task WhenContentHeaderHasTheValue_ShouldSucceed()
			{
				HttpResponseMessage subject = new(HttpStatusCode.OK)
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
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(otherKey).WithValue(value);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-some-other-key" header whose value is equal to "some header",
					             but it did not contain the expected header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenHeaderExistsAndValueDoesNotSatisfyTheExpectations_ShouldFail()
			{
				string name = "x-my-header";
				string value = "some header";
				string expectedValue = "some other header";
				HttpResponseMessage subject = ResponseBuilder
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

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenHeaderExistsAndValueSatisfiesTheExpectations_ShouldSucceed()
			{
				string name = "x-my-header";
				string value = "some header";
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader(name, value);

				async Task Act()
					=> await That(subject).HasHeader(name).WithValue(value);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderExistsButContainsAdditionalValues_ShouldFail()
			{
				string name = "x-my-header";
				string value = "some header";
				string expectedValue = "some other header";
				HttpResponseMessage subject = ResponseBuilder
					.WithHeaders(name, value, "some other value");

				async Task Act()
					=> await That(subject).HasHeader(name).WithValue(expectedValue);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose value is equal to "some other header",
					             but it had 2 header values ["some header", "some other value"]

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 x-my-header: some other value
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenPrefixIsEmpty_ShouldThrowArgumentException()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("").AsPrefix();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

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
			public async Task WhenValueDoesNotEndWithSuffix_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("value").AsSuffix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose value ends with "value",
					             but it had header value "some header", which differs at index 10:
					                          ↓ (actual)
					               "some header"
					                     "value"
					                          ↑ (expected suffix)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotMatchRegex_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("^some [0-9]+$").AsRegex();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose value matches regex "^some [0-9]+$",
					             but it had header value "some header", which did not match:
					               ↓ (actual)
					               "some header"
					               "^some [0-9]+$"
					               ↑ (regex pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotMatchWildcard_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
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

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotMatchWildcardIgnoringCase_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("OTHER*").AsWildcard().IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose value matches "OTHER*" ignoring case,
					             but it had header value "some header", which did not match:
					               ↓ (actual)
					               "some header"
					               "OTHER*"
					               ↑ (wildcard pattern)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotStartWithPrefix_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("other").AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose value starts with "other",
					             but it had header value "some header", which differs at index 0:
					                ↓ (actual)
					               "some header"
					               "other"
					                ↑ (expected prefix)

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenValueEndsWithSuffix_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("header").AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueMatchesRegex_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("^some [a-z]+$").AsRegex();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueMatchesWildcard_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("some*").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueMatchesWildcardIgnoringCase_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("SOME*").AsWildcard().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueStartsWithPrefix_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValue("some").AsPrefix();

				await That(Act).DoesNotThrow();
			}
		}

		public sealed class NegatedWithValueTests
		{
			[Test]
			public async Task WhenHeaderExistsAndValueDiffers_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasHeader("x-my-header").WithValue("some other header"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderExistsAndValueIsEqual_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHeader("x-my-header").WithValue("some header"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an "x-my-header" header whose value is equal to "some header",
					             but it did contain the "x-my-header" header: ["some header"] and had header value "some header"

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """)
					.Because("the negation applies to the header as a whole, like the negated text of a `Whose`");
			}

			[Test]
			public async Task WhenValueDoesNotMatchWildcard_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasHeader("x-my-header").WithValue("other*").AsWildcard());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueMatchesWildcard_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "some header");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasHeader("x-my-header").WithValue("some*").AsWildcard());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an "x-my-header" header whose value matches "some*",
					             but it did contain the "x-my-header" header: ["some header"] and had header value "some header"

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}
		}
	}
}
