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
