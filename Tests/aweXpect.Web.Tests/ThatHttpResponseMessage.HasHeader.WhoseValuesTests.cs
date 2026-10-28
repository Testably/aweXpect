using System.Collections.Generic;
using System.Net.Http;

namespace aweXpect.Tests;

public sealed partial class ThatHttpResponseMessage
{
	public sealed partial class HasHeader
	{
		public sealed class WhoseValuesTests
		{
			[Test]
			public async Task WhenHeaderDoesNotExist_AndNoValuesWouldSatisfyTheExpectations_ShouldFail()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "foo");

				async Task Act()
					=> await That(subject).HasHeader("x-some-other-key").WhoseValues(v => v.IsEmpty());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-some-other-key" header whose values are empty,
					             but it did not contain the expected header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: foo
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenHeaderDoesNotExist_Negated_ShouldSucceed()
			{
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "foo");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasHeader("x-some-other-key").WhoseValues(v => v.IsEmpty()));

				await That(Act).DoesNotThrow()
					.Because("the missing header meets the negation, whatever the expectations on the values are");
			}

			[Test]
			public async Task WhenHeaderDoesNotExist_ShouldFail()
			{
				string name = "x-my-header";
				string[] value = ["foo", "bar", "baz",];
				string otherKey = "x-some-other-key";
				HttpResponseMessage subject = ResponseBuilder
					.WithHeaders(name, value);

				async Task Act()
					=> await That(subject).HasHeader(otherKey).WhoseValues(v => v.HasCount().EqualTo(3));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-some-other-key" header whose values have exactly 3 items,
					             but it did not contain the expected header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: foo
					                 x-my-header: bar
					                 x-my-header: baz
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
			}

			[Test]
			public async Task WhenHeaderDoesNotExist_ShouldNotPassNullToTheExpectations()
			{
				List<string[]> observedValues = [];
				HttpResponseMessage subject = ResponseBuilder
					.WithHeader("x-my-header", "foo");

				bool HasValues(string[] values)
				{
					observedValues.Add(values);
					return values.Length > 0;
				}

				async Task Act()
					=> await That(subject).HasHeader("x-some-other-key").WhoseValues(v => v.Satisfies(HasValues));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-some-other-key" header whose values satisfy HasValues,
					             but it did not contain the expected header

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: foo
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0
					             """);
				await That(observedValues).All().Satisfy(values => values is not null)
					.Because("the expectations on the values are also evaluated for a missing header");
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
					=> await That(subject).HasHeader(name).WhoseValues(v => v.Contains(expectedValue));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-header" header whose values contain "some other header" at least once,
					             but the values did not contain it

					             HTTP-Response:
					               200 OK HTTP/1.1
					                 x-my-header: some header
					                 Content-Type: text/plain; charset=utf-8
					                 Content-Length: 0

					             Collection (the values):
					             [
					               "some header"
					             ]
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
					=> await That(subject).HasHeader(name).WhoseValues(v => v.Contains(value));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				HttpResponseMessage? subject = null;

				async Task Act()
					=> await That(subject).HasHeader("x-my-key").WhoseValues(v => v.IsEmpty());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has an "x-my-key" header whose values are empty,
					             but it was <null>
					             """);
			}
		}
	}
}
