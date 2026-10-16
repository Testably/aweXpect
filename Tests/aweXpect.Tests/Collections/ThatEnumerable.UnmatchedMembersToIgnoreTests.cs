using System.Collections.Generic;
using aweXpect.Equivalency;

namespace aweXpect.Tests;

public sealed partial class ThatEnumerable
{
	public sealed class UnmatchedMembersToIgnoreTests
	{
		[Test]
		public async Task AllAreEquivalentTo_WhenANameMatchesNoMember_ShouldSuggestTheClosestMember()
		{
			Track[] subject = [new("Something"), new("Let It Be"),];

			async Task Act()
				=> await That(subject).All().AreEquivalentTo(new Track("Something"), o => o.IgnoringMember("Titel"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equivalent to new Track("Something") for all items,
				             but only 1 of 2 were

				             Not matching items:
				             [
				               ThatEnumerable.UnmatchedMembersToIgnoreTests.Track {
				                 Title = "Let It Be"
				               }
				             ]

				             Collection:
				             [
				               ThatEnumerable.UnmatchedMembersToIgnoreTests.Track {
				                 Title = "Something"
				               },
				               ThatEnumerable.UnmatchedMembersToIgnoreTests.Track {
				                 Title = "Let It Be"
				               }
				             ]

				             Equivalency options:
				              - include public fields and properties
				              - ignore members: ["Titel"]
				                "Titel" matched no member (did you mean "Title"?)
				             """);
		}

		[Test]
		public async Task Contains_WhenANameMatchesNoMember_ShouldSuggestTheClosestMember()
		{
			Track[] subject = [new("Something"),];

			async Task Act()
				=> await That(subject).Contains(new Track("Let It Be")).Equivalent(o => o.IgnoringMember("Titel"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             contains an item equivalent to ThatEnumerable.UnmatchedMembersToIgnoreTests.Track {
				               Title = "Let It Be"
				             } at least once,
				             but it did not contain it

				             Collection:
				             [
				               ThatEnumerable.UnmatchedMembersToIgnoreTests.Track {
				                 Title = "Something"
				               }
				             ]

				             Equivalency options:
				              - include public fields and properties
				              - ignore members: ["Titel"]
				                "Titel" matched no member (did you mean "Title"?)
				             """);
		}

		[Test]
		public async Task IsEqualTo_WhenANameMatchesNoMember_ShouldSuggestTheClosestMember()
		{
			IEnumerable<Track> subject = [new("Something"),];

			async Task Act()
				=> await That(subject).IsEqualTo([new Track("Let It Be"),]).Equivalent(o => o.IgnoringMember("Titel"));

			await That(Act).Throws<FailException>()
				.WithMessage("""
				             Expected that subject
				             is equal to collection [new Track("Let It Be"),] using equivalency in order,
				             but it contained item ThatEnumerable.UnmatchedMembersToIgnoreTests.Track {
				               Title = "Something"
				             } at index 0 instead of ThatEnumerable.UnmatchedMembersToIgnoreTests.Track {
				               Title = "Let It Be"
				             }

				             Collection:
				             [
				               ThatEnumerable.UnmatchedMembersToIgnoreTests.Track {
				                 Title = "Something"
				               }
				             ]

				             Expected:
				             [
				               ThatEnumerable.UnmatchedMembersToIgnoreTests.Track {
				                 Title = "Let It Be"
				               }
				             ]

				             Equivalency options:
				              - include public fields and properties
				              - ignore members: ["Titel"]
				                "Titel" matched no member (did you mean "Title"?)
				             """);
		}

		[Test]
		public async Task WhenEquivalent_ShouldSucceedDespiteANameThatMatchesNoMember()
		{
			Track[] subject = [new("Something"),];

			async Task Act()
				=> await That(subject).Contains(new Track("Something")).Equivalent(o => o.IgnoringMember("Titel"));

			await That(Act).DoesNotThrow();
		}

		private sealed class Track(string title)
		{
			public string Title { get; } = title;
		}
	}
}
