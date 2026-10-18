using System.Collections.Generic;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpRequestMessage
{
	public sealed partial class HasHeader
	{
		public sealed class WithValuesTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenHeaderDoesNotExist_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "foo", "bar");

				async Task Act()
					=> await That(subject).HasHeader("x-some-other-key").WithValues("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-some-other-key" header whose values are equal to collection ["foo", "bar"] in order,
					             but it did not contain the expected header

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: foo
					                 x-my-header: bar
					             """);
			}

			[Test]
			public async Task WhenHeaderHasAdditionalValues_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "foo", "bar", "baz");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose values are equal to collection ["foo", "bar"] in order,
					             but it had header values ["foo", "bar", "baz"]

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: foo
					                 x-my-header: bar
					                 x-my-header: baz
					             """);
			}

			[Test]
			public async Task WhenHeaderHasFewerValues_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "foo");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose values are equal to collection ["foo", "bar"] in order,
					             but it had header values ["foo"]

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: foo
					             """);
			}

			[Test]
			public async Task WhenHeaderHasSameValuesInDifferentOrder_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "bar", "foo");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose values are equal to collection ["foo", "bar"] in order,
					             but it had header values ["bar", "foo"]

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: bar
					                 x-my-header: foo
					             """)
					.Because("the values have to match in the order of the header");
			}

			[Test]
			public async Task InAnyOrder_WhenHeaderHasSameValuesInDifferentOrder_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "bar", "foo");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues("foo", "bar").InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringCase_WhenHeaderValuesDifferInCasing_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "Foo", "Bar");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues("foo", "bar").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringCaseInAnyOrder_WhenHeaderValuesDiffer_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "Bar", "Foo");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues("foo", "baz").IgnoringCase().InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose values are equal to collection ["foo", "baz"] ignoring case in any order,
					             but it had header values ["Bar", "Foo"]

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: Bar
					                 x-my-header: Foo
					             """);
			}

			[Test]
			public async Task WhenHeaderHasSameValuesInSameOrder_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "foo", "bar");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues("foo", "bar");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderHasSingleExpectedValue_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeader("x-my-header", "foo");

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues("foo");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedIsPassedAsCollection_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "foo", "bar");
				List<string> expected = ["foo", "bar",];

				async Task Act()
					=> await That(subject).HasHeader("x-my-header").WithValues(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenKnownHeaderHasCommaSeparatedValues_ShouldCompareTheParsedValues()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeader("Accept", "application/json, text/plain");

				async Task Act()
					=> await That(subject).HasHeader("Accept").WithValues("application/json", "text/plain");

				await That(Act).DoesNotThrow()
					.Because("HttpClient parses the comma-separated values of known headers into separate values");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader("x-my-key").WithValues("foo", "bar");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-key" header whose values are equal to collection ["foo", "bar"] in order,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_Negated_ShouldFail()
			{
				HttpRequestMessage? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasHeader("x-my-key").WithValues("foo", "bar"));

				await That(Act).Throws<FailException>()
					.WithMessage("*but it was <null>").AsWildcard()
					.Because("a null subject fails the expectation and its negation alike");
			}
		}

		public sealed class NegatedWithValuesTests
		{
			[Test]
			public async Task WhenHeaderValuesDiffer_ShouldSucceed()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "foo", "bar");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasHeader("x-my-header").WithValues("foo", "baz"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenHeaderValuesAreEqual_ShouldFail()
			{
				HttpRequestMessage subject = RequestBuilder
					.WithHeaders("x-my-header", "foo", "bar");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasHeader("x-my-header").WithValues("foo", "bar"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have an "x-my-header" header whose values are equal to collection ["foo", "bar"] in order,
					             but it did contain the "x-my-header" header: ["foo", "bar"] and had header values ["foo", "bar"]

					             HTTP-Request:
					               HEAD https://awexpect.com/ HTTP/1.1
					                 x-my-header: foo
					                 x-my-header: bar
					             """)
					.Because("the negation applies to the header as a whole, like the negated text of a `Whose`");
			}
		}
	}
}
