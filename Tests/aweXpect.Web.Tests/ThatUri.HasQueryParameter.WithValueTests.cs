namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed partial class HasQueryParameter
	{
		public sealed class WithValueTests
		{
			[Fact]
			public async Task WhenQueryParameterDoesNotExist_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("jazz");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "genre" whose value is equal to "jazz",
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Fact]
			public async Task WhenQueryParameterHasMultipleValues_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValue("live");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose value is equal to "live",
					             but it had 2 query parameter values ["live", "remastered"]
					             """);
			}

			[Fact]
			public async Task WhenValueDiffers_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WithValue("3");

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenValueDiffersOnlyInCaseAndIgnoringCase_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?genre=Jazz");

				async Task Act()
					=> await That(subject).HasQueryParameter("genre").WithValue("jazz").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Theory]
			[InlineData("https://awexpect.com/tracks?page=2", "page", "2")]
			[InlineData("https://awexpect.com/tracks?q=a%20b", "q", "a b")]
			[InlineData("https://awexpect.com/tracks?q=a+b", "q", "a b")]
			[InlineData("https://awexpect.com/tracks?q=a%3Db", "q", "a=b")]
			[InlineData("https://awexpect.com/tracks?q=a=b", "q", "a=b")]
			[InlineData("https://awexpect.com/tracks?explicit", "explicit", "")]
			[InlineData("https://awexpect.com/tracks?page=", "page", "")]
			public async Task WhenValueIsEqual_ShouldSucceed(string uri, string name, string value)
			{
				Uri subject = new(uri);

				async Task Act()
					=> await That(subject).HasQueryParameter(name).WithValue(value);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WithValue("2");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page" whose value is equal to "2",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?page=2", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WithValue("2");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "page" whose value is equal to "2",
					             but it was relative
					             """);
			}
		}

		public sealed class NegatedWithValueTests
		{
			[Fact]
			public async Task WhenQueryParameterDoesNotExist_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("genre").WithValue("jazz"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueDiffers_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("page").WithValue("3"));

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValueIsEqual_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("page").WithValue("2"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "page" whose value is equal to "2",
					             but it was https://awexpect.com/tracks?page=2 and had query parameter value "2"
					             """)
					.Because("the negation applies to the query parameter as a whole, like the negated text of a `Whose`");
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("page").WithValue("2"));

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it was relative").AsWildcard()
					.Because("a relative URI has no query to inspect, so it fails the expectation and its negation alike");
			}
		}
	}
}
