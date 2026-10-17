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

			[Fact]
			public async Task IgnoringMember_WithPredicate_ShouldLeaveOutTheMatchingMembers()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.Member(value => value.Name.EndsWith("Url", StringComparison.Ordinal))
						.Ignoring.Member("Tracks");

				await That(Act).Throws<XunitException>();
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

			[Fact]
			public async Task IgnoringMember_WithMethodGroup_ShouldLeaveOutTheMatchingMembers()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Ignoring.Member(IsUrl);

				await That(Act).Throws<XunitException>();
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

			[Fact]
			public async Task IgnoringMember_WithPredicate_ShouldAlsoBeAskedForCollectionItems()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<string> subject = ["Come Together", "Something", "Octopus's Garden",];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.Member(value => value.Kind == FormattedValueKind.CollectionItem &&
						                          Equals(value.GetValue(), "Something"));

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             "Come Together",
				                                             "Octopus's Garden"
				                                           ]
				                                           """).IgnoringNewlineStyle()
					.Because("the predicate decides about every nested value, so it can check the kind itself");
			}

			[Fact]
			public async Task IgnoringMember_WhenThePredicateReadsAThrowingGetter_ShouldWriteTheException()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleThrowing subject = new();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.Member(value => value.GetValue() is null);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("*Value = *boom*").AsWildcard()
					.Because("a value that cannot be read is written with its exception by the formatter");
			}

			[Fact]
			public async Task IgnoringMember_WhenThePredicateThrows_ShouldThrowTheException()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.Member(_ => throw new InvalidOperationException("predicate failed"));

				await That(Act).Throws<InvalidOperationException>().WithMessage("predicate failed")
					.Because("an exception of the predicate is a mistake in the test, which a placeholder would hide");
			}

			[Fact]
			public async Task MaskingMember_WithPredicate_ShouldNumberThePlaceholderAfterTheMember()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();
				subject.Tracks.Add(RuleTrack.ComeTogether());

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Member(value => value.DeclaringType == typeof(RuleTrack) && value.Name == "Duration")
						.Ignoring.Member(value => value.Name is "Artist" or "CoverUrl" or "PreviewUrl" or "Price");

				await That(Act).Throws<XunitException>();
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

			[Fact]
			public async Task MaskingMember_WithPredicateOnCollectionItems_ShouldNameThePlaceholderItem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<string> subject = ["Let It Be", "Abbey Road", "Let It Be",];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Member(value => value.Kind == FormattedValueKind.CollectionItem);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             {Item_1},
				                                             {Item_2},
				                                             {Item_1}
				                                           ]
				                                           """).IgnoringNewlineStyle()
					.Because("an item has no member name, so its placeholder is named 'Item'");
			}

			[Fact]
			public async Task MaskingMember_WithPredicateAndReplacement_ShouldWriteItWithoutQuotes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Member(value => value.Type == typeof(string), "{text}");

				await That(Act).Throws<XunitException>();
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

			[Fact]
			public async Task IgnoringWinsOverMasking()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Member(value => value.Type == typeof(string), "{text}")
						.Ignoring.Member(IsUrl);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Duration = 259,
				                                             Id = 11,
				                                             Price = 1.29,
				                                             Title = {text}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenGlobalAndLocal_ShouldApplyTheLocalRuleFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime ignorings = Customize.aweXpect.Snapshots().Ignorings.Set([
					SnapshotIgnoring.Member(IsUrl),
				]);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Member(value => value.Type == typeof(string), "{global}"),
					SnapshotMasking.Member(value => value.Name == "Id"),
				]);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Member(value => value.Name == "Title", "{local}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Duration = 259,
				                                             Id = {Id_1},
				                                             Price = 1.29,
				                                             Title = {local}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				void IgnoringAct()
					=> SnapshotIgnoring.Member((Func<FormattedValue, bool>)null!);

				void MaskingAct()
					=> SnapshotMasking.Member((Func<FormattedValue, bool>)null!);

				void MaskingWithReplacementAct()
					=> SnapshotMasking.Member((Func<FormattedValue, bool>)null!, "{x}");

				await That(IgnoringAct).Throws<ArgumentNullException>().WithParamName("predicate");
				await That(MaskingAct).Throws<ArgumentNullException>().WithParamName("predicate");
				await That(MaskingWithReplacementAct).Throws<ArgumentNullException>().WithParamName("predicate");
			}

			[Fact]
			public async Task WhenReplacementIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotMasking.Member(_ => true, null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("replacement");
			}

			[Fact]
			public async Task WhenSubjectIsAString_ShouldThrowInvalidOperationException()
			{
				string subject = "foo";

				async Task IgnoringAct()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Ignoring.Member(IsUrl);

				async Task MaskingAct()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Member(IsUrl, "{url}");

				await That(IgnoringAct).Throws<InvalidOperationException>()
					.WithMessage("A member or type rule only applies to a snapshot of an object*").AsWildcard();
				await That(MaskingAct).Throws<InvalidOperationException>()
					.WithMessage("A member or type rule only applies to a snapshot of an object*").AsWildcard();
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
