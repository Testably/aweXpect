using System.Collections.Generic;
using System.Linq;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class OrOfTypeTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task IgnoringMembers_OrOfType_ShouldMatchAnyOfTheTypes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleListener subject = RuleListener.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<int>().OrOfType<long>().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleListener {
				                                             Extra = "Abbey Road",
				                                             Name = "Ringo",
				                                             Plays = 3
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the int, the long, the long? with and without a value are left out, while a short and a string in an object member are kept");
			}

			[Fact]
			public async Task MaskingMembers_OrOfType_ShouldNameThePlaceholderAfterTheMatchingType()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleListener subject = RuleListener.Create();
				subject.Extra = 7L;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<int>().OrOfType<long>().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleListener {
				                                             Extra = {Int64_1},
				                                             FavoriteSongId = {Int64_2},
				                                             Id = {Int32_1},
				                                             LastSongId = <null>,
				                                             Name = "Ringo",
				                                             PlaylistId = {Int64_1},
				                                             Plays = 3
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("each value is named after the type it matches, also by its runtime type or as a long?, and a null value is not masked");
			}

			[Fact]
			public async Task MaskingValues_OrOfType_ShouldShareTheNumbersWithASingleType()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleSession subject = new()
				{
					Ids = [3, 3L, 4, "Let It Be",],
					Track = 3,
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleSession>().OfType<int>().Mask()
						.For.Values().OfType<int>().OrOfType<long>().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleSession {
				                                             Ids = [
				                                               {Int32_1},
				                                               {Int64_1},
				                                               {Int32_2},
				                                               "Let It Be"
				                                             ],
				                                             Track = {Int32_1}
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the items are masked as well, and an int gets the placeholder of OfType<int>() alone, so they share the numbers");
			}

			[Fact]
			public async Task MaskingValues_WhenAValueMatchesSeveralTypes_ShouldNameThePlaceholderAfterTheFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<object> subject = [1, "Help!",];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().OfType<int>().OrOfType<object>().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             {Int32_1},
				                                             {Object_1}
				                                           ]
				                                           """).IgnoringNewlineStyle()
					.Because("the int is also an object, but the int comes first");
			}

			[Fact]
			public async Task OrOfExactType_ShouldNotMatchDerivedTypes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleFailures subject = new()
				{
					First = new Exception("first"),
					Second = new ArgumentException("second"),
					Third = new ArgumentNullException("third"),
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfExactType<Exception>().OrOfExactType<ArgumentException>().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleFailures {
				                                             Third = *third*
				                                           }
				                                           """).AsWildcard().IgnoringNewlineStyle()
					.Because("an ArgumentNullException is neither exactly an Exception nor exactly an ArgumentException");
			}

			[Fact]
			public async Task OrOfType_AfterOfExactType_ShouldMatchEither()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleFailures subject = new()
				{
					First = new Exception("first"),
					Second = new InvalidOperationException("second"),
					Third = new ArgumentNullException("third"),
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfExactType<Exception>().OrOfType<ArgumentException>().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleFailures {
				                                             Second = *second*
				                                           }
				                                           """).AsWildcard().IgnoringNewlineStyle()
					.Because("each alternative keeps its own semantics, so the ArgumentNullException matches as derived type, but the InvalidOperationException matches neither");
			}

			[Fact]
			public async Task OrOfType_WithName_ShouldLinkTheIds()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTour subject = RuleTour.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<int>().OrOfType<long>().WithName("Id").AsSuffix()
						.Mask(value => "{" + (value.Name == "Id"
							? value.DeclaringType.Name.Substring("Rule".Length)
							: value.Name.Substring(0, value.Name.Length - 2)) + "_{0}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTour {
				                                             Bands = [
				                                               RuleBand {
				                                                 CatalogId = "B-7",
				                                                 Id = {Band_1},
				                                                 Name = "The Beatles"
				                                               }
				                                             ],
				                                             Concerts = [
				                                               RuleConcert {
				                                                 BandId = {Band_1},
				                                                 Id = {Concert_1},
				                                                 VenueId = <null>
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the int and long ids are masked, linked by the template, while a string id is no number and a null id stays visible");
			}

			[Fact]
			public async Task OrOfType_WithOtherFilters_ShouldCombineThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTour subject = RuleTour.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<int>().OrOfType<long>().DeclaredIn<RuleConcert>()
						.Except(value => value.Name == nameof(RuleConcert.BandId)).Mask("{number}")
						.For.Values().OfType<string>().OrOfType<long>().WithPath("Bands[0].CatalogId").Ignore()
						.For.Members().OfType<int>().OrOfType<long>()
						.Which(value => value.Path.StartsWith("Bands", StringComparison.Ordinal)).Mask("{band}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTour {
				                                             Bands = [
				                                               RuleBand {
				                                                 Id = {band},
				                                                 Name = "The Beatles"
				                                               }
				                                             ],
				                                             Concerts = [
				                                               RuleConcert {
				                                                 BandId = 7,
				                                                 Id = {number},
				                                                 VenueId = <null>
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the filters after the alternatives narrow the selection down like after a single type");
			}

			[Fact]
			public async Task Convert_AfterOrOfType_ShouldGetTheValueAsObject()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleListener subject = RuleListener.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<int>().OrOfType<long>().Convert(id => id.GetType().Name + " " + id);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleListener {
				                                             Extra = "Abbey Road",
				                                             FavoriteSongId = "Int64 42",
				                                             Id = "Int32 1",
				                                             LastSongId = <null>,
				                                             Name = "Ringo",
				                                             PlaylistId = "Int64 7",
				                                             Plays = 3
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the converter gets each value as an object of its own type");
			}

			[Fact]
			public async Task WhenGlobalRule_ShouldApplyTheAlternatives()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().OfExactType<int>().OrOfExactType<long>().WithName("Id").AsSuffix().Mask(),
					SnapshotRule.For.Values().OfType<string>().OrOfType<short>().Ignore(),
				]);
				RuleListener subject = RuleListener.Create();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleListener {
				                                             FavoriteSongId = {FavoriteSongId_1},
				                                             Id = {Id_1},
				                                             LastSongId = <null>,
				                                             PlaylistId = {PlaylistId_1}
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a selection that is also filtered by its name is named after the member");
			}

			[Fact]
			public async Task OrOfType_ShouldOnlyFollowATypeFilter()
			{
				string[] surface = ApiParity.GetSurface(
					typeof(SnapshotResult<object, IThat<object>>)
						.GetProperty(nameof(SnapshotResult<object, IThat<object>>.For))!.PropertyType,
					typeof(SnapshotResult<object, IThat<object>>));

				string[] alternatives = surface
					.Where(signature => signature.Contains(".OrOfType`1(") || signature.Contains(".OrOfExactType`1("))
					.ToArray();

				await That(alternatives.Select(signature => signature.Substring(0, signature.IndexOf(".Or", StringComparison.Ordinal))).Distinct())
					.IsEqualTo([
						"aweXpect.Snapshots.SnapshotMembersOfType`2[TReturn,System.Object]",
						"aweXpect.Snapshots.SnapshotMembersOfType`2[TReturn,T]",
						"aweXpect.Snapshots.SnapshotValuesOfType`2[TReturn,System.Object]",
						"aweXpect.Snapshots.SnapshotValuesOfType`2[TReturn,T]",
					]).InAnyOrder()
					.Because("an alternative type only exists directly after OfType or OfExactType");
				await That(alternatives.All(signature => signature.EndsWith(",System.Object]", StringComparison.Ordinal)))
					.IsTrue()
					.Because("a selection with alternative types is no longer typed");
			}
		}
	}
}

public sealed class RuleListener
{
	public object? Extra { get; set; }
	public long? FavoriteSongId { get; set; }
	public int Id { get; set; }
	public long? LastSongId { get; set; }
	public string Name { get; set; } = "";
	public long PlaylistId { get; set; }
	public short Plays { get; set; }

	public static RuleListener Create()
		=> new()
		{
			Extra = "Abbey Road",
			FavoriteSongId = 42,
			Id = 1,
			Name = "Ringo",
			PlaylistId = 7,
			Plays = 3,
		};
}

public sealed class RuleSession
{
	public List<object> Ids { get; set; } = [];
	public int Track { get; set; }
}

public sealed class RuleFailures
{
	public Exception? First { get; set; }
	public Exception? Second { get; set; }
	public Exception? Third { get; set; }
}

public sealed class RuleTour
{
	public List<RuleBand> Bands { get; set; } = [];
	public List<RuleConcert> Concerts { get; set; } = [];

	public static RuleTour Create()
		=> new()
		{
			Bands = [new RuleBand { CatalogId = "B-7", Id = 7, Name = "The Beatles", },],
			Concerts = [new RuleConcert { BandId = 7, Id = 1969, },],
		};
}

public sealed class RuleBand
{
	public string CatalogId { get; set; } = "";
	public int Id { get; set; }
	public string Name { get; set; } = "";
}

public sealed class RuleConcert
{
	public int BandId { get; set; }
	public long Id { get; set; }
	public long? VenueId { get; set; }
}
