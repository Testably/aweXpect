namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed partial class HasQueryParameter
	{
		public sealed class WithValueTests
		{
			[Test]
			public async Task WhenCombiningMatchTypes_ShouldThrowInvalidOperationException()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("prog*").AsWildcard().AsRegex();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("AsRegex cannot be combined with AsWildcard.");
			}

			[Test]
			public async Task WhenCombiningRegexWithUsing_ShouldThrowInvalidOperationException()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("^prog").AsRegex()
						.Using(StringComparer.Ordinal);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("Using cannot be combined with AsRegex.");
			}

			[Test]
			public async Task WhenQueryParameterDoesNotExist_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("jazz");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "genre" whose value is equal to "jazz",
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Test]
			public async Task WhenQueryParameterHasMultipleValues_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValue("live");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose value is equal to "live",
					             but it had 2 query parameter values ["live", "remastered"]
					             """);
			}

			[Test]
			public async Task WhenValueDiffers_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WithValue("3");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page" whose value is equal to "3",
					             but it had query parameter value "2", which differs at index 0:
					                ↓ (actual)
					               "2"
					               "3"
					                ↑ (expected)
					             """);
			}

			[Test]
			public async Task WhenValueDiffersOnlyInCaseAndIgnoringCase_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=Jazz");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("jazz").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueDoesNotEndWithSuffix_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("jazz").AsSuffix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "genre" whose value ends with "jazz",
					             but it had query parameter value "progressive-rock", which differs at index 15:
					                               ↓ (actual)
					               "progressive-rock"
					                           "jazz"
					                               ↑ (expected suffix)
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotMatchRegex_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("^jazz").AsRegex();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "genre" whose value matches regex "^jazz",
					             but it had query parameter value "progressive-rock", which did not match:
					               ↓ (actual)
					               "progressive-rock"
					               "^jazz"
					               ↑ (regex pattern)
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotMatchWildcard_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("jazz*").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "genre" whose value matches "jazz*",
					             but it had query parameter value "progressive-rock", which did not match:
					               ↓ (actual)
					               "progressive-rock"
					               "jazz*"
					               ↑ (wildcard pattern)
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotStartWithPrefix_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("jazz").AsPrefix();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "genre" whose value starts with "jazz",
					             but it had query parameter value "progressive-rock", which differs at index 0:
					                ↓ (actual)
					               "progressive-rock"
					               "jazz"
					                ↑ (expected prefix)
					             """);
			}

			[Test]
			public async Task WhenValueDoesNotStartWithPrefixIgnoringCase_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("JAZZ").AsPrefix().IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "genre" whose value starts with "JAZZ" ignoring case,
					             but it had query parameter value "progressive-rock", which differs at index 0:
					                ↓ (actual)
					               "progressive-rock"
					               "JAZZ"
					                ↑ (expected prefix)
					             """);
			}

			[Test]
			public async Task WhenValueEndsWithSuffix_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("rock").AsSuffix();

				await That(Act).DoesNotThrow();
			}

			[Test]
			[Arguments("https://awexpect.com/tracks?page=2", "page", "2")]
			[Arguments("https://awexpect.com/tracks?q=a%20b", "q", "a b")]
			[Arguments("https://awexpect.com/tracks?q=a+b", "q", "a b")]
			[Arguments("https://awexpect.com/tracks?q=a%3Db", "q", "a=b")]
			[Arguments("https://awexpect.com/tracks?q=a=b", "q", "a=b")]
			[Arguments("https://awexpect.com/tracks?explicit", "explicit", "")]
			[Arguments("https://awexpect.com/tracks?page=", "page", "")]
			public async Task WhenValueIsEqual_ShouldSucceed(string uri, string name, string value)
			{
				Uri subject = new(uri);

				async Task Act()
					=> await That(subject).HasQueryParameter(name).WithValue(value);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueMatchesRegex_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("^[a-z]+-rock$").AsRegex();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueMatchesWildcard_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("prog*").AsWildcard();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueStartsWithPrefix_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("progressive").AsPrefix();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueStartsWithPrefixIgnoringCase_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("PROGRESSIVE").AsPrefix().IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WithValue("2");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page" whose value is equal to "2",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?page=2", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WithValue("2");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page" whose value is equal to "2",
					             but it was relative
					             """);
			}
		}

		public sealed class NegatedWithValueTests
		{
			[Test]
			public async Task WhenQueryParameterDoesNotExist_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("genre").WithValue("jazz"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueDiffers_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("page").WithValue("3"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueDoesNotMatchWildcard_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("genre").WithValue("jazz*").AsWildcard());

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValueIsEqual_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("page").WithValue("2"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "page" whose value is equal to "2",
					             but it was https://awexpect.com/tracks?page=2 and had query parameter value "2"
					             """)
					.Because("the negation applies to the query parameter as a whole, like the negated text of a `Whose`");
			}

			[Test]
			public async Task WhenValueMatchesWildcard_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=progressive-rock");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("genre").WithValue("prog*").AsWildcard());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "genre" whose value matches "prog*",
					             but it was https://awexpect.com/tracks?genre=progressive-rock and had query parameter value "progressive-rock"
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("page").WithValue("2"));

				await That(Act).Throws<FailException>()
					.WithMessage("*but it was relative").AsWildcard()
					.Because("a relative URI has no query to inspect, so it fails the expectation and its negation alike");
			}
		}
	}
}
