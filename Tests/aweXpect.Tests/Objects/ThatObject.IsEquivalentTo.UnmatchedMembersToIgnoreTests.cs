using aweXpect.Equivalency;

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class UnmatchedMembersToIgnoreTests
		{
			[Test]
			public async Task ForType_WhenANameMatchesNoMember_ShouldNameItBelowTheType()
			{
				Album subject = new("Abbey Road", new Artist("The Beatles", 1960));
				Album expected = new("Abbey Road", new Artist("The Beatles", 1962));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.For<Artist>(x => x.IgnoringMember("Foundd")));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Artist.Founded differed:
					                   Actual: 1960
					                 Expected: 1962

					             Equivalency options:
					              - include public fields and properties
					              - for ThatObject.IsEquivalentTo.UnmatchedMembersToIgnoreTests.Artist:
					                - include public fields and properties
					                - ignore members: ["Foundd"]
					                  "Foundd" matched no member (did you mean "Founded"?)
					             """);
			}

			[Test]
			public async Task WhenANameMatchesAMember_ShouldNotNameIt()
			{
				Album subject = new("Abbey Road", new Artist("The Beatles", 1960));
				Album expected = new("Let It Be", new Artist("The Beatles", 1962));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IgnoringMember("Founded"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Title differed:
					                   Actual: "Abbey Road"
					                 Expected: "Let It Be"

					             Equivalency options:
					              - include public fields and properties
					              - ignore members: ["Founded"]
					             """);
			}

			[Test]
			public async Task WhenANameMatchesNoMember_ShouldSuggestTheClosestMember()
			{
				Album subject = new("Abbey Road", new Artist("The Beatles", 1960));
				Album expected = new("Let It Be", new Artist("The Beatles", 1960));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IgnoringMember("Titel"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Title differed:
					                   Actual: "Abbey Road"
					                 Expected: "Let It Be"

					             Equivalency options:
					              - include public fields and properties
					              - ignore members: ["Titel"]
					                "Titel" matched no member (did you mean "Title"?)
					             """);
			}

			[Test]
			public async Task WhenANameMatchesNoMemberAndNoneIsClose_ShouldOnlyNameIt()
			{
				Album subject = new("Abbey Road", new Artist("The Beatles", 1960));
				Album expected = new("Let It Be", new Artist("The Beatles", 1960));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IgnoringMember("Label"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Title differed:
					                   Actual: "Abbey Road"
					                 Expected: "Let It Be"

					             Equivalency options:
					              - include public fields and properties
					              - ignore members: ["Label"]
					                "Label" matched no member
					             """);
			}

			[Test]
			public async Task WhenANestedPathMatchesNoMember_ShouldSuggestTheClosestPath()
			{
				Album subject = new("Abbey Road", new Artist("The Beatles", 1960));
				Album expected = new("Abbey Road", new Artist("The Beatles", 1962));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IgnoringMember("Artst.Founded"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Artist.Founded differed:
					                   Actual: 1960
					                 Expected: 1962

					             Equivalency options:
					              - include public fields and properties
					              - ignore members: ["Artst.Founded"]
					                "Artst.Founded" matched no member (did you mean "Artist.Founded"?)
					             """);
			}

			[Test]
			public async Task WhenEquivalent_ShouldSucceedDespiteANameThatMatchesNoMember()
			{
				Album subject = new("Abbey Road", new Artist("The Beatles", 1960));
				Album expected = new("Abbey Road", new Artist("The Beatles", 1960));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o.IgnoringMember("Titel"));

				await That(Act).DoesNotThrow()
					.Because("a name that matches no member is only named to explain a failure");
			}

			private sealed class Album(string title, Artist artist)
			{
				public Artist Artist { get; } = artist;
				public string Title { get; } = title;
			}

			private sealed class Artist(string name, int founded)
			{
				public int Founded { get; } = founded;
				public string Name { get; } = name;
			}
		}
	}
}
