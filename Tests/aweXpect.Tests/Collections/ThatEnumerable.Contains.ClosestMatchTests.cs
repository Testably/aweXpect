using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed partial class Contains
	{
		public sealed class ClosestMatchTests
		{
			[Test]
			public async Task WhenAPatternIsExpected_ShouldNotSuggestAnItem()
			{
				string[] subject = ["Username", "Password",];

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
			public async Task WhenItemIsContainedTooOften_ShouldNotSuggestAnItem()
			{
				string[] subject = ["Username", "Username", "Usernames",];

				async Task Act()
					=> await That(subject).Contains("Username").Once();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "Username" exactly once,
					             but it contained "Username" at least twice

					             Collection:
					             [
					               "Username",
					               "Username",
					               "Usernames"
					             ]
					             """)
					.Because("the item was found, only not as often as expected");
			}

			[Test]
			public async Task WhenItemIsMissing_ShouldSuggestTheClosestItem()
			{
				IEnumerable<string> subject = ToEnumerable(["Username", "Password", "Email",]);

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
			public async Task WhenItemIsMissing_WithACustomComparer_ShouldSuggestTheClosestItem()
			{
				string[] subject = ["Username", "Password",];

				async Task Act()
					=> await That(subject).Contains("Usrname").Using(new IgnoreCaseForVocalsComparer());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "Usrname" using IgnoreCaseForVocalsComparer at least once,
					             but it did not contain it (did you mean "Username"?)

					             Collection:
					             [
					               "Username",
					               "Password"
					             ]
					             """)
					.Because("an item that the comparer considers equal would have been found");
			}

			[Test]
			public async Task WhenItemIsMissing_WithAQuantifier_ShouldSuggestTheClosestItem()
			{
				string[] subject = ["Username", "Password",];

				async Task Act()
					=> await That(subject).Contains("Usrname").AtLeast(2);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "Usrname" at least twice,
					             but it did not contain it (did you mean "Username"?)

					             Collection:
					             [
					               "Username",
					               "Password"
					             ]
					             """)
					.Because("the item was not found at all");
			}

			[Test]
			public async Task WhenItemIsMissing_WithIgnoringCase_ShouldSuggestTheClosestItem()
			{
				string[] subject = ["Username", "Password",];

				async Task Act()
					=> await That(subject).Contains("USRNAME").IgnoringCase();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "USRNAME" ignoring case at least once,
					             but it did not contain it (did you mean "Username"?)

					             Collection:
					             [
					               "Username",
					               "Password"
					             ]
					             """);
			}

			[Test]
			public async Task WhenNegated_ShouldNotSuggestAnItem()
			{
				string[] subject = ["Username", "Password",];

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
			public async Task WhenNoItemIsClose_ShouldNotSuggestAnItem()
			{
				string[] subject = ["Username", "Password",];

				async Task Act()
					=> await That(subject).Contains("Phone");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "Phone" at least once,
					             but it did not contain it

					             Collection:
					             [
					               "Username",
					               "Password"
					             ]
					             """);
			}

			[Test]
			public async Task WhenSubjectCanOnlyBeEnumeratedOnce_ShouldSuggestTheClosestItem()
			{
				IEnumerable<string> subject = Factory.GetSingleUseEnumerable("Username", "Password");

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
					               "Password"
					             ]
					             """)
					.Because("the suggestion compares the items that were already read");
			}

			[Test]
			public async Task WhenSubjectHasMoreItemsThanListed_ShouldSuggestAnUnlistedItem()
			{
				IEnumerable<string> subject = ToEnumerable(
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
					.Because("all items are compared, not only the listed ones");
			}

			[Test]
			public async Task WhenSubjectHasNullItems_ShouldSuggestTheClosestItem()
			{
				string?[] subject = [null, "Username",];

				async Task Act()
					=> await That(subject).Contains("Usrname");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             contains "Usrname" at least once,
					             but it did not contain it (did you mean "Username"?)

					             Collection:
					             [
					               <null>,
					               "Username"
					             ]
					             """);
			}
		}
	}
}
