using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed partial class HasQueryParameter
	{
		public sealed class WhoseValuesTests
		{
			[Test]
			public async Task WhenExpectationsIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WhoseValues(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expectations").And
					.WithMessage("The 'expectations' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenQueryParameterDoesNotExist_AndNoValuesWouldSatisfyTheExpectations_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WhoseValues(v => v.IsEmpty());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are empty,
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Test]
			public async Task WhenQueryParameterDoesNotExist_Negated_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("tag").WhoseValues(v => v.IsEmpty()));

				await That(Act).DoesNotThrow()
					.Because("the missing query parameter meets the negation, whatever the expectations on the values are");
			}

			[Test]
			public async Task WhenQueryParameterDoesNotExist_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WhoseValues(v => v.HasCount().EqualTo(2));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values have exactly 2 items,
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Test]
			public async Task WhenQueryParameterDoesNotExist_ShouldNotPassNullToTheExpectations()
			{
				List<string[]> observedValues = [];
				Uri subject = new("https://awexpect.com/tracks?page=2");

				bool HasValues(string[] values)
				{
					observedValues.Add(values);
					return values.Length > 0;
				}

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WhoseValues(v => v.Satisfies(HasValues));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values satisfy HasValues,
					             but it was https://awexpect.com/tracks?page=2
					             """);
				await That(observedValues).All().Satisfy(values => values is not null)
					.Because("the expectations on the values are also evaluated for a missing query parameter");
			}

			[Test]
			public async Task WhenValuesDoNotSatisfyTheExpectations_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WhoseValues(v => v.Contains("studio"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values contain "studio" at least once,
					             but the values did not contain it

					             Collection (the values):
					             [
					               "live",
					               "remastered"
					             ]
					             """);
			}

			[Test]
			public async Task WhenValuesSatisfyTheExpectations_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WhoseValues(v => v.Contains("live"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSomeSubjectsDoNotHaveTheQueryParameter_ShouldNotPassNullToTheExpectations()
			{
				List<string[]> observedValues = [];
				Uri[] subjects =
				[
					new("https://awexpect.com/tracks?page=2"),
					new("https://awexpect.com/tracks?tag=live"),
				];

				bool HasValues(string[] values)
				{
					observedValues.Add(values);
					return values.Length > 0;
				}

				async Task Act()
					=> await That(subjects).None()
						.ComplyWith(it => it.HasQueryParameter("tag").WhoseValues(v => v.Satisfies(HasValues)));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subjects
					             has a query parameter "tag" whose values satisfy HasValues for no items,
					             but 1 of 2 did

					             Matching items:
					             [
					               https://awexpect.com/tracks?tag=live
					             ]

					             Collection:
					             [
					               https://awexpect.com/tracks?page=2,
					               https://awexpect.com/tracks?tag=live
					             ]
					             """);
				await That(observedValues).All().Satisfy(values => values is not null)
					.Because("the expectations on the values are also evaluated for a missing query parameter");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WhoseValues(v => v.IsEmpty());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are empty,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?tag=live", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WhoseValues(v => v.Contains("live"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values contain "live" at least once,
					             but it was relative
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_Negated_ShouldFail()
			{
				Uri subject = new("tracks?tag=live", UriKind.Relative);

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("tag").WhoseValues(v => v.Contains("live")));

				await That(Act).Throws<FailException>()
					.WithMessage("*but it was relative").AsWildcard()
					.Because("a relative URI has no query to inspect, so it fails the expectation and its negation alike");
			}
		}
	}
}
