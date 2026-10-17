using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class EmptyCollectionsTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task IgnoringEmptyCollections_ShouldLeaveOutEmptyCollectionMembers()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleShelf subject = new();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Ignoring.EmptyCollections();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleShelf {
				                                             Empty = "",
				                                             Missing = <null>,
				                                             Tracks = [
				                                               "Let It Be"
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("empty arrays, lists, sets and dictionaries are left out, also in an object member, while null and an empty string are kept");
			}

			[Fact]
			public async Task WithoutIgnoringEmptyCollections_ShouldKeepThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = new()
				{
					Title = "Let It Be",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.Member("Artist");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("*Tracks = []*").AsWildcard()
					.Because("leaving out empty collections is opt-in");
			}

			[Fact]
			public async Task IgnoringEmptyCollections_ShouldKeepEmptyItemsAndDictionaryValues()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleBoxSet subject = new();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Ignoring.EmptyCollections();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleBoxSet {
				                                             Bonus = {
				                                               ["Abbey Road"] = []
				                                             },
				                                             Discs = [
				                                               [],
				                                               [
				                                                 "Let It Be"
				                                               ]
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("only members are left out, so that the items keep their place");
			}

			[Fact]
			public async Task IgnoringEmptyCollections_WhenASequenceDoesNotKnowItsCount_ShouldKeepItAndEnumerateItOnce()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				OneShotSequence sequence = new();
				RuleSequences subject = new()
				{
					Filtered = new List<string>().Where(title => title.Length > 0),
					OneShot = sequence,
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Ignoring.EmptyCollections();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleSequences {
				                                             Filtered = [],
				                                             OneShot = []
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a sequence might compute its items or only be enumerable once, so it is not enumerated to check whether it is empty");
				await That(sequence.Enumerations).IsEqualTo(1);
			}

			[Fact]
			public async Task IgnoringEmptyCollections_WhenAGetterThrows_ShouldWriteTheException()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleThrowing subject = new();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Ignoring.EmptyCollections();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("*Value = *boom*").AsWildcard()
					.Because("a value that cannot be read is written with its exception by the formatter");
			}

			[Fact]
			public async Task WhenGlobalIgnorings_ShouldLeaveOutEmptyCollections()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime ignorings = Customize.aweXpect.Snapshots().Ignorings.Set([
					SnapshotIgnoring.EmptyCollections(),
					SnapshotIgnoring.MemberOf<RuleAlbum>(a => a.Artist),
				]);
				RuleAlbum subject = new()
				{
					Id = 2,
					Price = 9.99m,
					Title = "Let It Be",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleAlbum {
				                                             CoverUrl = "",
				                                             Id = 2,
				                                             Price = 9.99,
				                                             Title = "Let It Be"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenSubjectIsAString_ShouldThrowInvalidOperationException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Ignoring.EmptyCollections();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A member or type rule only applies to a snapshot of an object*").AsWildcard();
			}

			private sealed class OneShotSequence : IEnumerable<string>
			{
				public int Enumerations { get; private set; }

				public IEnumerator<string> GetEnumerator()
				{
					if (++Enumerations > 1)
					{
						throw new InvalidOperationException("The sequence can only be enumerated once.");
					}

					return Enumerable.Empty<string>().GetEnumerator();
				}

				IEnumerator IEnumerable.GetEnumerator()
					=> GetEnumerator();
			}
		}
	}
}

public sealed class RuleShelf
{
	public string[] Array { get; set; } = [];
	public object? Boxed { get; set; } = new List<int>();
	public Dictionary<string, int> Dictionary { get; set; } = new();
	public string Empty { get; set; } = "";
	public List<string> List { get; set; } = [];
	public List<string>? Missing { get; set; }
	public HashSet<int> Numbers { get; set; } = [];
	public HashSet<string> Titles { get; set; } = [];
	public List<string> Tracks { get; set; } = ["Let It Be",];
}

public sealed class RuleBoxSet
{
	public Dictionary<string, List<string>> Bonus { get; set; } = new()
	{
		["Abbey Road"] = [],
	};

	public List<List<string>> Discs { get; set; } = [[], ["Let It Be",],];
}

public sealed class RuleSequences
{
	public IEnumerable<string> Filtered { get; set; } = [];
	public IEnumerable<string> OneShot { get; set; } = [];
}
