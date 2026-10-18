using System.Collections.Generic;

namespace aweXpect.Tests;

public sealed partial class ThatUri
{
	public sealed partial class HasQueryParameter
	{
		public sealed class WithValuesTests
		{
			[Test]
			public async Task WhenExpectedIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues((IEnumerable<string>)null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenExpectedArrayIsNull_ShouldThrowArgumentNullException()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues((string[])null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("expected").And
					.WithMessage("The 'expected' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenQueryParameterDoesNotExist_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live", "remastered");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to collection ["live", "remastered"] in order,
					             but it was https://awexpect.com/tracks?page=2
					             """);
			}

			[Test]
			public async Task WhenValuesAreEqual_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&page=2&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live", "remastered");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValuesAreEqualAsCollection_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");
				List<string> expected = ["live", "remastered",];

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues(expected);

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSingleValueIsEqual_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?page=2");

				async Task Act()
					=> await That(subject).HasQueryParameter("page").WithValues("2");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenValuesAreInDifferentOrder_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("remastered", "live");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to collection ["remastered", "live"] in order,
					             but it had query parameter values ["live", "remastered"]
					             """)
					.Because("the values are compared in the order in which they appear in the URI");
			}

			[Test]
			public async Task InAnyOrder_WhenValuesAreInDifferentOrder_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("remastered", "live").InAnyOrder();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringCase_WhenValuesDifferInCasing_ShouldSucceed()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=Live&tag=Remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live", "remastered").IgnoringCase();

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task IgnoringCaseInAnyOrder_WhenValuesDiffer_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=Remastered&tag=Live");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live", "acoustic").IgnoringCase()
						.InAnyOrder();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to collection ["live", "acoustic"] ignoring case in any order,
					             but it had query parameter values ["Remastered", "Live"]
					             """);
			}

			[Test]
			public async Task WhenValuesContainAdditionalItems_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to collection ["live"] in order,
					             but it had query parameter values ["live", "remastered"]
					             """);
			}

			[Test]
			public async Task WhenValuesAreMissingItems_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live");

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live", "remastered");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to collection ["live", "remastered"] in order,
					             but it had query parameter values ["live"]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Uri? subject = null;

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to collection ["live"] in order,
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?tag=live", UriKind.Relative);

				async Task Act()
					=> await That(subject).HasQueryParameter("tag").WithValues("live");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a query parameter "tag" whose values are equal to collection ["live"] in order,
					             but it was relative
					             """);
			}
		}

		public sealed class NegatedWithValuesTests
		{
			[Test]
			public async Task WhenValuesAreEqual_ShouldFail()
			{
				Uri subject = new("https://awexpect.com/tracks?tag=live&tag=remastered");

				async Task Act()
					=> await That(subject)
						.DoesNotComplyWith(it => it.HasQueryParameter("tag").WithValues("live", "remastered"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not have a query parameter "tag" whose values are equal to collection ["live", "remastered"] in order,
					             but it was https://awexpect.com/tracks?tag=live&tag=remastered and had query parameter values ["live", "remastered"]
					             """);
			}

			[Test]
			public async Task WhenSubjectIsRelative_ShouldFail()
			{
				Uri subject = new("tracks?tag=live", UriKind.Relative);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.HasQueryParameter("tag").WithValues("live"));

				await That(Act).Throws<FailException>()
					.WithMessage("*but it was relative").AsWildcard()
					.Because("a relative URI has no query to inspect, so it fails the expectation and its negation alike");
			}

			[Test]
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
