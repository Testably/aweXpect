using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed partial class HasQueryParameter
	{
		public sealed class WithValuesTests
		{
			[Fact]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues((IEnumerable<string>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenExpectedArrayIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues((string[])null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Fact]
			public async Task WhenQueryParameterDoesNotExist_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live", "remastered");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to ["live", "remastered"],
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Fact]
			public async Task WhenValuesAreEqual_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&page=2&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live", "remastered");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValuesAreEqualAsCollection_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");
				List<string> expected = ["live", "remastered",];

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues(expected);

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSingleValueIsEqual_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WithValues("2");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenValuesAreInDifferentOrder_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("remastered", "live");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to ["remastered", "live"],
					             but it had query parameter values ["live", "remastered"]
					             """)
					.Because("the values are compared in the order in which they appear in the URI");
			}

			[Fact]
			public async Task WhenValuesContainAdditionalItems_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to ["live"],
					             but it had query parameter values ["live", "remastered"]
					             """);
			}

			[Fact]
			public async Task WhenValuesAreMissingItems_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live", "remastered");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to ["live", "remastered"],
					             but it had query parameter values ["live"]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to ["live"],
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?tag=live", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to ["live"],
					             but it was relative
					             """);
			}
		}

		public sealed class NegatedWithValuesTests
		{
			[Fact]
			public async Task WhenValuesAreEqual_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("tag").WithValues("live", "remastered"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "tag" whose values are equal to ["live", "remastered"],
					             but it was https://awexpect.com/tracks?tag=live&tag=remastered and had query parameter values ["live", "remastered"]
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?tag=live", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("tag").WithValues("live"));

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it was relative").AsWildcard()
					.Because("a relative URI has no query to inspect, so it fails the expectation and its negation alike");
			}

			[Fact]
			public async Task WhenValuesDiffer_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("tag").WithValues("live"));

				await That(Act).DoesNotThrow();
			}
		}
	}
}
