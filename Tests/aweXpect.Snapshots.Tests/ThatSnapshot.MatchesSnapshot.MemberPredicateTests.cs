using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class MemberPredicateTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			public async Task IgnoringMember_WithPredicate_ShouldLeaveOutTheMatchingMembers()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().Which(value => value.Name.EndsWith("Url", StringComparison.Ordinal)).Ignore()
						.For.Members().WithName("Tracks").Ignore();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleAlbum {
				                                             Artist = RuleArtist {
				                                               Id = 3,
				                                               Name = "The Beatles"
				                                             },
				                                             Id = 1,
				                                             Price = 12.99,
				                                             Title = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Test]
			public async Task IgnoringMember_WithMethodGroup_ShouldLeaveOutTheMatchingMembers()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().Which(IsUrl).Ignore();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Duration = 259,
				                                             Id = 11,
				                                             Price = 1.29,
				                                             Title = "Come Together"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a method group binds to the predicate, not to the member name");
			}

			[Test]
			public async Task IgnoringValues_WithPredicate_ShouldAlsoBeAskedForCollectionItems()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<string> subject = ["Come Together", "Something", "Octopus's Garden",];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().Which(value => value.Kind == FormattedValueKind.CollectionItem &&
						                              Equals(value.GetValue(), "Something")).Ignore();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             "Come Together",
				                                             "Octopus's Garden"
				                                           ]
				                                           """).IgnoringNewlineStyle()
					.Because("the predicate decides about every nested value, so it can check the kind itself");
			}

			[Test]
			public async Task IgnoringMember_WhenThePredicateReadsAThrowingGetter_ShouldWriteTheException()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleThrowing subject = new();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().Which(value => value.GetValue() is null).Ignore();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("*Value = *boom*").AsWildcard()
					.Because("a value that cannot be read is written with its exception by the formatter");
			}

			[Test]
			public async Task IgnoringMember_WhenThePredicateThrows_ShouldThrowTheException()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().Which(_ => throw new InvalidOperationException("predicate failed")).Ignore();

				await That(Act).Throws<InvalidOperationException>().WithMessage("predicate failed")
					.Because("an exception of the predicate is a mistake in the test, which a placeholder would hide");
			}

			[Test]
			public async Task MaskingMember_WithPredicate_ShouldNumberThePlaceholderAfterTheMember()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();
				subject.Tracks.Add(RuleTrack.ComeTogether());

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().Which(value => value.DeclaringType == typeof(RuleTrack) && value.Name == "Duration").Mask()
						.For.Members().Which(value => value.Name is "Artist" or "CoverUrl" or "PreviewUrl" or "Price").Ignore();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleAlbum {
				                                             Id = 1,
				                                             Title = "Abbey Road",
				                                             Tracks = [
				                                               RuleTrack {
				                                                 Duration = {Duration_1},
				                                                 Id = 11,
				                                                 Title = "Come Together"
				                                               },
				                                               RuleTrack {
				                                                 Duration = {Duration_2},
				                                                 Id = 12,
				                                                 Title = "Something"
				                                               },
				                                               RuleTrack {
				                                                 Duration = {Duration_1},
				                                                 Id = 11,
				                                                 Title = "Come Together"
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the same value keeps the same number");
			}

			[Test]
			public async Task MaskingValues_WithPredicateOnCollectionItems_ShouldNameThePlaceholderItem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<string> subject = ["Let It Be", "Abbey Road", "Let It Be",];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().Which(value => value.Kind == FormattedValueKind.CollectionItem).Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             {Item_1},
				                                             {Item_2},
				                                             {Item_1}
				                                           ]
				                                           """).IgnoringNewlineStyle()
					.Because("an item has no member name, so its placeholder is named 'Item'");
			}

			[Test]
			public async Task MaskingMember_WithPredicateAndReplacement_ShouldWriteItWithoutQuotes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().Which(value => value.Type == typeof(string)).Mask("{text}");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Duration = 259,
				                                             Id = 11,
				                                             PreviewUrl = {text},
				                                             Price = 1.29,
				                                             Title = {text}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Test]
			public async Task IgnoringWinsOverMasking()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().Which(value => value.Type == typeof(string)).Mask("{text}")
						.For.Members().Which(IsUrl).Ignore();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Duration = 259,
				                                             Id = 11,
				                                             Price = 1.29,
				                                             Title = {text}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Test]
			public async Task WhenGlobalAndLocal_ShouldApplyTheLocalRuleFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().Which(IsUrl).Ignore(),
					SnapshotRule.For.Members().Which(value => value.Type == typeof(string)).Mask("{global}"),
					SnapshotRule.For.Members().Which(value => value.Name == "Id").Mask(),
				]);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().Which(value => value.Name == "Title").Mask("{local}");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Duration = 259,
				                                             Id = {Id_1},
				                                             Price = 1.29,
				                                             Title = {local}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Test]
			public async Task ExceptOnValues_ShouldOnlySelectTheValuesForWhichThePredicateIsFalse()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<string> subject = ["Let It Be", "Abbey Road", "Let It Be",];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().Except(value => Equals(value.GetValue(), "Abbey Road")).Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             {Item_1},
				                                             "Abbey Road",
				                                             {Item_1}
				                                           ]
				                                           """).IgnoringNewlineStyle();
			}

			[Test]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				void MembersWhichAct()
					=> SnapshotRule.For.Members().Which(null!);

				void MembersExceptAct()
					=> SnapshotRule.For.Members().Except(null!);

				void ValuesWhichAct()
					=> SnapshotRule.For.Values().Which(null!);

				void ValuesExceptAct()
					=> SnapshotRule.For.Values().Except(null!);

				await That(MembersWhichAct).Throws<ArgumentNullException>().WithParamName("predicate");
				await That(MembersExceptAct).Throws<ArgumentNullException>().WithParamName("predicate");
				await That(ValuesWhichAct).Throws<ArgumentNullException>().WithParamName("predicate");
				await That(ValuesExceptAct).Throws<ArgumentNullException>().WithParamName("predicate");
			}

			[Test]
			public async Task WhenSubjectIsAString_ShouldThrowInvalidOperationException()
			{
				string subject = "foo";

				async Task IgnoringAct()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().Which(IsUrl).Ignore();

				async Task MaskingAct()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().Which(IsUrl).Mask("{url}");

				await That(IgnoringAct).Throws<InvalidOperationException>()
					.WithMessage("A member or value rule only applies to a snapshot of an object*").AsWildcard();
				await That(MaskingAct).Throws<InvalidOperationException>()
					.WithMessage("A member or value rule only applies to a snapshot of an object*").AsWildcard();
			}

			private static bool IsUrl(FormattedValue value)
				=> value.Name.EndsWith("Url", StringComparison.Ordinal);
		}
	}
}

public class RuleAlbum
{
	public RuleArtist Artist { get; set; } = new();
	public string CoverUrl { get; set; } = "";
	public int Id { get; set; }
	public decimal Price { get; set; }
	public string Title { get; set; } = "";
	public List<RuleTrack> Tracks { get; set; } = [];

	public static RuleAlbum AbbeyRoad()
		=> new()
		{
			Artist = new RuleArtist
			{
				Id = 3,
				Name = "The Beatles",
			},
			CoverUrl = "https://example.com/abbey-road.jpg",
			Id = 1,
			Price = 12.99m,
			Title = "Abbey Road",
			Tracks = [RuleTrack.ComeTogether(), RuleTrack.Something(),],
		};
}

public sealed class RuleLiveAlbum : RuleAlbum
{
	public string Venue { get; set; } = "";
}

public sealed class RuleArtist
{
	public string Name = "";
	public int Id { get; set; }
}

public sealed class RuleTrack
{
	public int Duration { get; set; }
	public int Id { get; set; }
	public string PreviewUrl { get; set; } = "";
	public decimal Price { get; set; }
	public string Title { get; set; } = "";

	public static RuleTrack ComeTogether()
		=> new()
		{
			Duration = 259,
			Id = 11,
			PreviewUrl = "https://example.com/come-together.mp3",
			Price = 1.29m,
			Title = "Come Together",
		};

	public static RuleTrack Something()
		=> new()
		{
			Duration = 182,
			Id = 12,
			PreviewUrl = "https://example.com/something.mp3",
			Price = 1.29m,
			Title = "Something",
		};
}
