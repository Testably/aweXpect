#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

namespace aweXpect.Tests;

public sealed partial class ThatAsyncEnumerable
{
	public sealed partial class Contains
	{
		public sealed class ClosestMatchTests
		{
			[Test]
			public async Task WhenAPatternIsExpected_ShouldNotSuggestAnItem()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["Username", "Password",]);

				async Task Act()
					=> await That(subject).Contains("Usrname*").AsWildcard();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains an item matching "Usrname*" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "Username",
					               "Password"
					             ]
					             """)
					.Because("a pattern is not a value that could have been mistyped");
			}

			[Test]
			public async Task WhenItemIsMissing_ShouldSuggestTheClosestItem()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["Username", "Password", "Email",]);

				async Task Act()
					=> await That(subject).Contains("Usrname");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "Usrname" at least once,
					             but it did not contain it (did you mean "Username"?)

					             Collection:
					             [
					               "Username",
					               "Password",
					               "Email"
					             ]
					             """);
			}

			[Test]
			public async Task WhenNegated_ShouldNotSuggestAnItem()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(["Username", "Password",]);

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.Contains("Usrname").Never());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "Usrname" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "Username",
					               "Password"
					             ]
					             """)
					.Because("a negated expectation never suggests an item");
			}

			[Test]
			public async Task WhenSubjectHasMoreItemsThanListed_ShouldSuggestAnUnlistedItem()
			{
				IAsyncEnumerable<string> subject = ToAsyncEnumerable(
					Enumerable.Range(1, 20).Select(index => $"Item {index:00}").Append("Username").ToArray());

				async Task Act()
					=> await That(subject).Contains("Usrname");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "Usrname" at least once,
					             but it did not contain it (did you mean "Username"?)

					             Collection:
					             [
					               "Item 01",
					               "Item 02",
					               "Item 03",
					               "Item 04",
					               "Item 05",
					               "Item 06",
					               "Item 07",
					               "Item 08",
					               "Item 09",
					               "Item 10",
					               (… and 11 more)
					             ]
					             """)
					.Because("all received items are compared, not only the listed ones");
			}
		}
	}
}
#endif
