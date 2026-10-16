using System.Collections.Generic;
using aweXpect.Equivalency;

// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace aweXpect.Tests;

public sealed partial class ThatObject
{
	public sealed partial class IsEquivalentTo
	{
		public sealed class MappingTests
		{
			[Test]
			public async Task WhenActualMemberIsMissing_ShouldFail()
			{
				Dto subject = new(1, "a");
				Entity expected = new(1, "a");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("Missing", "Key")
						.Mapping("Name", "FullName"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Key, mapped to Missing, was missing on the actual object

					             Equivalency options:
					              - include public fields and properties
					              - map "Missing" to "Key"
					              - map "Name" to "FullName"
					             """);
			}

			[Test]
			public async Task WhenActualPathIsNested_ShouldReadThroughIt()
			{
				NestedTrack subject = new(new Artist("Wings", "UK"));
				FlatTrack expected = new("Wings", "UK");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping<NestedTrack, FlatTrack>(track => track.Artist!.Name, track => track.ArtistName)
						.Mapping<NestedTrack, FlatTrack>(track => track.Artist!.Country, track => track.ArtistCountry));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenActualPathRunsThroughNull_ShouldReadNull()
			{
				NestedTrack subject = new(null);
				FlatTrack expected = new("Wings", null);

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("Artist.Name", "ArtistName")
						.Mapping("Artist.Country", "ArtistCountry"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property ArtistName differed:
					                   Actual: <null>
					                 Expected: "Wings"

					             Equivalency options:
					              - include public fields and properties
					              - map "Artist.Name" to "ArtistName"
					              - map "Artist.Country" to "ArtistCountry"
					             """);
			}

			[Test]
			public async Task WhenExpectedMemberDoesNotExist_ShouldHintAtTheClosestMember()
			{
				Dto subject = new(1, "a");
				Entity expected = new(1, "a");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("Id", "Kee")
						.Mapping("Name", "FullName"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Key was missing on the actual object

					             Equivalency options:
					              - include public fields and properties
					              - map "Id" to "Kee"
					                "Kee" matched no member (did you mean "Key"?)
					              - map "Name" to "FullName"
					             """);
			}

			[Test]
			public async Task WhenExpectedPathIsNested_AndActualHasTheMember_ShouldOnlyRedirectTheMappedMember()
			{
				FlatTrackWithArtist subject = new("Wings", new Artist("Paul McCartney", "UK"));
				NestedTrack expected = new(new Artist("Wings", "UK"));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping<FlatTrackWithArtist, NestedTrack>(track => track.ArtistName,
							track => track.Artist!.Name));

				await That(Act).DoesNotThrow()
					.Because("only the mapped member below Artist reads another actual member");
			}

			[Test]
			public async Task WhenExpectedPathIsNested_AndAMemberBelowIsNotMapped_ShouldReportItAsMissing()
			{
				FlatTrack subject = new("Wings", "UK");
				NestedTrack expected = new(new Artist("Wings", "UK"));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping<FlatTrack, NestedTrack>(track => track.ArtistName, track => track.Artist!.Name));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Artist.Country was missing on the actual object

					             Equivalency options:
					              - include public fields and properties
					              - map ThatObject.IsEquivalentTo.MappingTests.FlatTrack.ArtistName to ThatObject.IsEquivalentTo.MappingTests.NestedTrack.Artist.Name
					             """)
					.Because("nothing below a member that the actual object does not have may be left unverified");
			}

			[Test]
			public async Task WhenExpectedPathIsNested_ShouldFlattenIt()
			{
				FlatTrack subject = new("Wings", "UK");
				NestedTrack expected = new(new Artist("Wings", "UK"));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping<FlatTrack, NestedTrack>(track => track.ArtistName, track => track.Artist!.Name)
						.Mapping<FlatTrack, NestedTrack>(track => track.ArtistCountry, track => track.Artist!.Country));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedPathIsNested_TwoLevelsDeep_ShouldFlattenIt()
			{
				FlatLabelTrack subject = new("Apple");
				LabelTrack expected = new(new LabelArtist(new Label("Apple")));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("LabelName", "Artist.Label.Name"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenExpectedPathRunsThroughACollection_ShouldReportTheCollectionAsMissing()
			{
				AlbumSummary subject = new(2);
				AlbumWithTracks expected = new(["Jet", "Bluebird",]);

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("TrackCount", "Tracks.Count"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Tracks was missing on the actual object

					             Equivalency options:
					              - include public fields and properties
					              - map "TrackCount" to "Tracks.Count"
					                "Tracks.Count" matched no member
					             """)
					.Because("a collection is compared by its items, and its count is no compared member");
			}

			[Test]
			public async Task WhenExpectedPathRunsThroughNull_ShouldCompareWithNull()
			{
				FlatTrack subject = new("Wings", null);
				NestedTrack expected = new(null);

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("ArtistName", "Artist.Name")
						.Mapping("ArtistCountry", "Artist.Country"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Artist.Name differed:
					                   Actual: "Wings"
					                 Expected: <null>

					             Equivalency options:
					              - include public fields and properties
					              - map "ArtistName" to "Artist.Name"
					              - map "ArtistCountry" to "Artist.Country"
					             """);
			}

			[Test]
			public async Task WhenMappedByName_ShouldApplyAtAnyDepth()
			{
				Order<Dto> subject = new(7, new Dto(1, "a"));
				Order<Entity> expected = new(7, new Entity(1, "a"));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("Id", "Key")
						.Mapping("Name", "FullName"));

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenMappedByNameWithAPath_ShouldReadRelativeToTheObjectWithItsFirstMember()
			{
				Order<Dto> subject = new(7, new Dto(1, "a"));
				Order<Entity> expected = new(7, new Entity(1, "a"));

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("Customer.Id", "Customer.Key")
						.Mapping("Customer.Name", "Customer.FullName"));

				await That(Act).DoesNotThrow()
					.Because("both paths start at the order, whose customer is compared");
			}

			[Test]
			public async Task WhenMappedValuesDiffer_ShouldNameTheExpectedMember()
			{
				Dto subject = new(2, "a");
				Entity expected = new(3, "a");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping<Dto, Entity>(dto => dto.Id, entity => entity.Key)
						.Mapping<Dto, Entity>(dto => dto.Name, entity => entity.FullName));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property Key differed:
					                   Actual: 2
					                 Expected: 3

					             Equivalency options:
					              - include public fields and properties
					              - map ThatObject.IsEquivalentTo.MappingTests.Dto.Id to ThatObject.IsEquivalentTo.MappingTests.Entity.Key
					              - map ThatObject.IsEquivalentTo.MappingTests.Dto.Name to ThatObject.IsEquivalentTo.MappingTests.Entity.FullName
					             """);
			}

			[Test]
			public async Task WhenMemberIsIgnored_ShouldNotCompareIt()
			{
				Dto subject = new(2, "a");
				Entity expected = new(3, "a");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("Id", "Key")
						.Mapping("Name", "FullName")
						.IgnoringMember("Key"));

				await That(Act).DoesNotThrow()
					.Because("members to ignore refer to the expected member");
			}

			[Test]
			public async Task WhenMembersAreFields_ShouldMapThem()
			{
				FieldDto subject = new()
				{
					Id = 2,
				};
				FieldEntity expected = new()
				{
					Key = 3,
				};

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping<FieldDto, FieldEntity>(dto => dto.Id, entity => entity.Key));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Field Key differed:
					                   Actual: 2
					                 Expected: 3

					             Equivalency options:
					              - include public fields and properties
					              - map ThatObject.IsEquivalentTo.MappingTests.FieldDto.Id to ThatObject.IsEquivalentTo.MappingTests.FieldEntity.Key
					             """);
			}

			[Test]
			public async Task WhenSeveralMappingsApply_ShouldUseTheLastOne()
			{
				Dto subject = new(1, "a");
				Entity expected = new(1, "a");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("Name", "Key")
						.Mapping("Id", "Key")
						.Mapping("Name", "FullName"));

				await That(Act).DoesNotThrow()
					.Because("a later mapping overrides an earlier one for the same expected member");
			}

			[Test]
			public async Task WhenTypesDoNotMeet_ShouldNotApply()
			{
				Dto subject = new(1, "a");
				Entity expected = new(1, "a");

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping<Entity, Dto>(entity => entity.Key, dto => dto.Id));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             is equivalent to expected,
					             but it was not:
					               Property FullName was missing on the actual object and
					               Property Key was missing on the actual object

					             Equivalency options:
					              - include public fields and properties
					              - map ThatObject.IsEquivalentTo.MappingTests.Entity.Key to ThatObject.IsEquivalentTo.MappingTests.Dto.Id
					             """)
					.Because("the mapping is for the reverse pair of types");
			}

			[Test]
			public async Task WithCollectionOrderIgnored_ShouldPairTheElementsByTheMappedMembers()
			{
				Dto[] subject = [new(1, "a"), new(2, "b"), new(3, "c"),];
				Entity[] expected = [new(3, "c"), new(1, "a"), new(2, "b"),];

				async Task Act()
					=> await That(subject).IsEquivalentTo(expected, o => o
						.Mapping("Id", "Key")
						.Mapping("Name", "FullName")
						.IgnoringCollectionOrder());

				await That(Act).DoesNotThrow();
			}

			private sealed class AlbumSummary(int trackCount)
			{
				public int TrackCount { get; } = trackCount;
			}

			private sealed class AlbumWithTracks(List<string> tracks)
			{
				public List<string> Tracks { get; } = tracks;
			}

			private sealed class Artist(string name, string? country)
			{
				public string? Country { get; } = country;
				public string Name { get; } = name;
			}

			private sealed class Dto(int id, string name)
			{
				public int Id { get; } = id;
				public string Name { get; } = name;
			}

			private sealed class Entity(int key, string fullName)
			{
				public string FullName { get; } = fullName;
				public int Key { get; } = key;
			}

			private sealed class FieldDto
			{
				public int Id;
			}

			private sealed class FieldEntity
			{
				public int Key;
			}

			private sealed class FlatLabelTrack(string labelName)
			{
				public string LabelName { get; } = labelName;
			}

			private sealed class FlatTrack(string? artistName, string? artistCountry)
			{
				public string? ArtistCountry { get; } = artistCountry;
				public string? ArtistName { get; } = artistName;
			}

			private sealed class FlatTrackWithArtist(string artistName, Artist artist)
			{
				public Artist Artist { get; } = artist;
				public string ArtistName { get; } = artistName;
			}

			private sealed class Label(string name)
			{
				public string Name { get; } = name;
			}

			private sealed class LabelArtist(Label label)
			{
				public Label Label { get; } = label;
			}

			private sealed class LabelTrack(LabelArtist artist)
			{
				public LabelArtist Artist { get; } = artist;
			}

			private sealed class NestedTrack(Artist? artist)
			{
				public Artist? Artist { get; } = artist;
			}

			private sealed class Order<TCustomer>(int number, TCustomer customer)
			{
				public TCustomer Customer { get; } = customer;
				public int Number { get; } = number;
			}
		}
	}
}
