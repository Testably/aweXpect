using System.Collections.Generic;
using System.Text;
using aweXpect.Equivalency;

namespace aweXpect.Core.Tests.Equivalency;

public sealed partial class EquivalencyComparisonTests
{
	[Test]
	public async Task Mapping_WhenActualMemberIsMissing_ShouldNameTheMapping()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedDto(1, "a"), new MappedEntity(1, "a"),
			new EquivalencyOptions().Mapping("Missing", "Key").Mapping("Name", "FullName"), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Key, mapped to Missing, was missing on the actual object
		                                                """);
	}

	[Test]
	public async Task Mapping_WhenActualPathRunsThroughNull_ShouldReadNull()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedNestedTrack(null), new MappedFlatTrack("Wings", null),
			new EquivalencyOptions().Mapping("Artist.Name", "ArtistName").Mapping("Artist.Country", "ArtistCountry"),
			failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property ArtistName differed:
		                                                      Actual: <null>
		                                                    Expected: "Wings"
		                                                """);
	}

	[Test]
	public async Task Mapping_WhenCollectionOrderIsIgnored_ShouldPairTheElementsByTheMappedMembers()
	{
		MappedDto[] actual = [new(1, "a"), new(2, "b"),];
		MappedEntity[] expected = [new(2, "b"), new(1, "a"),];

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions()
			.Mapping("Id", "Key").Mapping("Name", "FullName").IgnoringCollectionOrder(), new StringBuilder());

		await That(result).IsTrue();
	}

	[Test]
	public async Task Mapping_WhenExpectedPathIsNested_AndActualHasTheMember_ShouldOnlyRedirectTheMappedMember()
	{
		MappedFlatTrackWithArtist actual = new("Wings", new MappedArtist("Paul McCartney", "UK"));
		MappedNestedTrack expected = new(new MappedArtist("Wings", "UK"));

		bool result = await EquivalencyComparison.Compare(actual, expected, new EquivalencyOptions()
				.Mapping<MappedFlatTrackWithArtist, MappedNestedTrack>(track => track.ArtistName,
					track => track.Artist!.Name),
			new StringBuilder());

		await That(result).IsTrue();
	}

	[Test]
	public async Task Mapping_WhenExpectedPathIsNested_AndAMemberBelowIsNotMapped_ShouldReportItAsMissing()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedFlatTrack("Wings", "UK"),
			new MappedNestedTrack(new MappedArtist("Wings", "UK")),
			new EquivalencyOptions().Mapping("ArtistName", "Artist.Name"), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Artist.Country was missing on the actual object
		                                                """);
	}

	[Test]
	public async Task Mapping_WhenExpectedPathIsNested_ShouldFlattenIt()
	{
		bool result = await EquivalencyComparison.Compare(new MappedFlatTrack("Wings", "UK"),
			new MappedNestedTrack(new MappedArtist("Wings", "UK")),
			new EquivalencyOptions()
				.Mapping<MappedFlatTrack, MappedNestedTrack>(track => track.ArtistName, track => track.Artist!.Name)
				.Mapping<MappedFlatTrack, MappedNestedTrack>(track => track.ArtistCountry,
					track => track.Artist!.Country),
			new StringBuilder());

		await That(result).IsTrue();
	}

	[Test]
	public async Task Mapping_WhenExpectedPathRunsThroughACollection_ShouldReportTheCollectionAsMissing()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedAlbumSummary(2),
			new MappedAlbum(["Jet", "Bluebird",]),
			new EquivalencyOptions().Mapping("TrackCount", "Tracks.Count"), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Tracks was missing on the actual object
		                                                """);
	}

	[Test]
	public async Task Mapping_WhenExpectedPathRunsThroughAValueComparedByValue_ShouldReportTheValueAsMissing()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedAlbumSummary(4),
			new MappedTitle("Jet"),
			new EquivalencyOptions().Mapping("TrackCount", "Title.Length"), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Title was missing on the actual object
		                                                """);
	}

	[Test]
	public async Task Mapping_WhenExpectedPathRunsThroughNull_AndTheActualMemberIsMissing_ShouldNameTheMapping()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedFlatTrack("Wings", null),
			new MappedNestedTrack(null),
			new EquivalencyOptions().Mapping("Missing", "Artist.Name"), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Artist.Name, mapped to Missing, was missing on the actual object
		                                                """);
	}

	[Test]
	public async Task Mapping_WhenExpectedPathRunsThroughNull_ShouldCompareWithNull()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedFlatTrack("Wings", null),
			new MappedNestedTrack(null),
			new EquivalencyOptions().Mapping("ArtistName", "Artist.Name").Mapping("ArtistCountry", "Artist.Country"),
			failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property Artist.Name differed:
		                                                      Actual: "Wings"
		                                                    Expected: <null>
		                                                """);
	}

	[Test]
	public async Task Mapping_WhenFieldsAreExcluded_ShouldOnlyReadProperties()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedFieldDto
			{
				Id = 1,
			}, new MappedEntity(1, "a"),
			new EquivalencyOptions
			{
				Fields = IncludeMembers.None,
			}.Mapping("Id", "Key").Mapping("Id", "FullName"), failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property FullName, mapped to Id, was missing on the actual object and
		                                                  Property Key, mapped to Id, was missing on the actual object
		                                                """)
			.Because("the field Id of the actual object is excluded");
	}

	[Test]
	public async Task Mapping_WhenMemberIsIgnored_ShouldNotCompareIt()
	{
		bool result = await EquivalencyComparison.Compare(new MappedDto(2, "a"), new MappedEntity(3, "a"),
			new EquivalencyOptions
			{
				MembersToIgnore = [new MemberToIgnore.ByName("Key"),],
			}.Mapping("Id", "Key").Mapping("Name", "FullName"), new StringBuilder());

		await That(result).IsTrue();
	}

	[Test]
	public async Task Mapping_WhenMembersAreFields_ShouldMapThem()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedFieldDto
			{
				Id = 2,
			}, new MappedFieldEntity
			{
				Key = 3,
			},
			new EquivalencyOptions().Mapping<MappedFieldDto, MappedFieldEntity>(dto => dto.Id, entity => entity.Key),
			failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Field Key differed:
		                                                      Actual: 2
		                                                    Expected: 3
		                                                """);
	}

	[Test]
	public async Task Mapping_WhenPropertiesAreExcluded_ShouldOnlyReadFields()
	{
		bool result = await EquivalencyComparison.Compare(new MappedFieldDto
			{
				Id = 3,
			}, new MappedFieldEntity
			{
				Key = 3,
			},
			new EquivalencyOptions
			{
				Properties = IncludeMembers.None,
			}.Mapping("Id", "Key"), new StringBuilder());

		await That(result).IsTrue();
	}

	[Test]
	public async Task Mapping_WhenSeveralMappingsApply_ShouldUseTheLastOne()
	{
		bool result = await EquivalencyComparison.Compare(new MappedDto(1, "a"), new MappedEntity(1, "a"),
			new EquivalencyOptions().Mapping("Name", "Key").Mapping("Id", "Key").Mapping("Name", "FullName"),
			new StringBuilder());

		await That(result).IsTrue();
	}

	[Test]
	public async Task Mapping_WhenTypesDoNotMeet_ShouldNotApply()
	{
		StringBuilder failureBuilder = new();

		bool result = await EquivalencyComparison.Compare(new MappedDto(1, "a"), new MappedEntity(1, "a"),
			new EquivalencyOptions().Mapping<MappedEntity, MappedDto>(entity => entity.Key, dto => dto.Id),
			failureBuilder);

		await That(result).IsFalse();
		await That(failureBuilder.ToString()).IsEqualTo("""

		                                                  Property FullName was missing on the actual object and
		                                                  Property Key was missing on the actual object
		                                                """);
	}

	[Test]
	public async Task Mapping_WhenTwoLevelsDeep_ShouldFlattenIt()
	{
		bool result = await EquivalencyComparison.Compare(new MappedFlatLabelTrack("Apple"),
			new MappedLabelTrack(new MappedLabelArtist(new MappedLabel("Apple"))),
			new EquivalencyOptions().Mapping("LabelName", "Artist.Label.Name"), new StringBuilder());

		await That(result).IsTrue();
	}

	private sealed class MappedAlbum(List<string> tracks)
	{
		public List<string> Tracks { get; } = tracks;
	}

	private sealed class MappedAlbumSummary(int trackCount)
	{
		public int TrackCount { get; } = trackCount;
	}

	private sealed class MappedArtist(string name, string? country)
	{
		public string? Country { get; } = country;
		public string Name { get; } = name;
	}

	private sealed class MappedDto(int id, string name)
	{
		public int Id { get; } = id;
		public string Name { get; } = name;
	}

	private sealed class MappedEntity(int key, string fullName)
	{
		public string FullName { get; } = fullName;
		public int Key { get; } = key;
	}

	private sealed class MappedFieldDto
	{
		public int Id;
	}

	private sealed class MappedFieldEntity
	{
		public int Key;
	}

	private sealed class MappedFlatLabelTrack(string labelName)
	{
		public string LabelName { get; } = labelName;
	}

	private sealed class MappedFlatTrack(string? artistName, string? artistCountry)
	{
		public string? ArtistCountry { get; } = artistCountry;
		public string? ArtistName { get; } = artistName;
	}

	private sealed class MappedFlatTrackWithArtist(string artistName, MappedArtist artist)
	{
		public MappedArtist Artist { get; } = artist;
		public string ArtistName { get; } = artistName;
	}

	private sealed class MappedLabel(string name)
	{
		public string Name { get; } = name;
	}

	private sealed class MappedLabelArtist(MappedLabel label)
	{
		public MappedLabel Label { get; } = label;
	}

	private sealed class MappedLabelTrack(MappedLabelArtist artist)
	{
		public MappedLabelArtist Artist { get; } = artist;
	}

	private sealed class MappedNestedTrack(MappedArtist? artist)
	{
		public MappedArtist? Artist { get; } = artist;
	}

	private sealed class MappedTitle(string title)
	{
		public string Title { get; } = title;
	}
}
