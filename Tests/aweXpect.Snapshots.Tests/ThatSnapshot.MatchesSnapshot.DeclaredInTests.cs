using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class DeclaredInTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task IgnoringMemberDeclaredIn_ShouldOnlyLeaveOutTheMemberOfTheType()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();
				subject.Tracks.RemoveAt(1);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleAlbum>().WithName(nameof(RuleAlbum.Price)).Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleAlbum {
				                                             Artist = RuleArtist {
				                                               Id = 3,
				                                               Name = "The Beatles"
				                                             },
				                                             CoverUrl = "https://example.com/abbey-road.jpg",
				                                             Id = 1,
				                                             Title = "Abbey Road",
				                                             Tracks = [
				                                               RuleTrack {
				                                                 Duration = 259,
				                                                 Id = 11,
				                                                 PreviewUrl = "https://example.com/come-together.mp3",
				                                                 Price = 1.29,
				                                                 Title = "Come Together"
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the price of a track is a member of another type");
			}

			[Fact]
			public async Task IgnoringMemberDeclaredIn_ShouldLeaveOutTheMemberOfADerivedType()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleLiveAlbum subject = new()
				{
					Id = 2,
					Price = 9.99m,
					Title = "Let It Be",
					Venue = "Apple Corps rooftop",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleAlbum>().WithName(nameof(RuleAlbum.Price)).Ignore()
						.For.Members().DeclaredIn<RuleAlbum>().WithName(nameof(RuleAlbum.Artist)).Ignore()
						.For.Members().DeclaredIn<RuleAlbum>().WithName(nameof(RuleAlbum.CoverUrl)).Ignore()
						.For.Members().DeclaredIn<RuleAlbum>().WithName(nameof(RuleAlbum.Tracks)).Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleLiveAlbum {
				                                             Id = 2,
				                                             Title = "Let It Be",
				                                             Venue = "Apple Corps rooftop"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task IgnoringMemberDeclaredIn_WhenTheTypeIsAnotherOne_ShouldKeepTheMember()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = new()
				{
					Id = 1,
					Price = 12.99m,
					Title = "Abbey Road",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleLiveAlbum>().WithName(nameof(RuleLiveAlbum.Price)).Ignore()
						.For.Members().WithName("Artist").Ignore()
						.For.Members().WithName("CoverUrl").Ignore()
						.For.Members().WithName("Tracks").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleAlbum {
				                                             Id = 1,
				                                             Price = 12.99,
				                                             Title = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("an album is no live album, even though it declares the price");
			}

			[Fact]
			public async Task MaskingMemberDeclaredIn_ShouldNumberThePlaceholderAfterTheMember()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleTrack>().WithName(nameof(RuleTrack.Id)).Mask()
						.For.Members().WithName("CoverUrl").Ignore()
						.For.Members().WithName("PreviewUrl").Ignore()
						.For.Members().WithName("Price").Ignore()
						.For.Members().WithName("Duration").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleAlbum {
				                                             Artist = RuleArtist {
				                                               Id = 3,
				                                               Name = "The Beatles"
				                                             },
				                                             Id = 1,
				                                             Title = "Abbey Road",
				                                             Tracks = [
				                                               RuleTrack {
				                                                 Id = {Id_1},
				                                                 Title = "Come Together"
				                                               },
				                                               RuleTrack {
				                                                 Id = {Id_2},
				                                                 Title = "Something"
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("only the ids of the tracks are masked, not those of the album or the artist");
			}

			[Fact]
			public async Task MaskingMemberDeclaredIn_ShouldShareTheNumbersWithOtherMemberRules()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();
				subject.Tracks[0].Id = 1;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleArtist>().WithName(nameof(RuleArtist.Id)).Mask("{artist}")
						.For.Members().DeclaredIn<RuleTrack>().WithName(nameof(RuleTrack.Id)).Mask()
						.For.Members().WithName("Id").Mask()
						.For.Members().Which(value => value.Name is "CoverUrl" or "PreviewUrl" or "Price" or "Duration")
						.Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleAlbum {
				                                             Artist = RuleArtist {
				                                               Id = {artist},
				                                               Name = "The Beatles"
				                                             },
				                                             Id = {Id_1},
				                                             Title = "Abbey Road",
				                                             Tracks = [
				                                               RuleTrack {
				                                                 Id = {Id_1},
				                                                 Title = "Come Together"
				                                               },
				                                               RuleTrack {
				                                                 Id = {Id_2},
				                                                 Title = "Something"
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the first matching masking wins, and the same id of the album and a track keeps its number");
			}

			[Fact]
			public async Task MaskingMemberDeclaredIn_WithAField_ShouldWriteTheReplacementWithoutQuotes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleArtist subject = new()
				{
					Id = 3,
					Name = "The Beatles",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleArtist>().WithName(nameof(RuleArtist.Name)).Mask("{artist}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleArtist {
				                                             Id = 3,
				                                             Name = {artist}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task Filters_ShouldAllHaveToMatch()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();
				subject.Tracks.RemoveAt(1);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleTrack>().OfType<decimal>().Ignore()
						.For.Members().WithName("Id").OfType<string>().Ignore()
						.For.Members().WithName("Url").AsSuffix().DeclaredIn<RuleAlbum>().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleAlbum {
				                                             Artist = RuleArtist {
				                                               Id = 3,
				                                               Name = "The Beatles"
				                                             },
				                                             Id = 1,
				                                             Price = 12.99,
				                                             Title = "Abbey Road",
				                                             Tracks = [
				                                               RuleTrack {
				                                                 Duration = 259,
				                                                 Id = 11,
				                                                 PreviewUrl = "https://example.com/come-together.mp3",
				                                                 Title = "Come Together"
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("only the price of a track is a decimal declared in a track, no id is a string, and only the cover url is declared in the album");
			}

			[Fact]
			public async Task Except_ShouldLeaveOutTheMembersForWhichThePredicateIsTrue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Id").Except(value => value.DeclaringType == typeof(RuleArtist)).Mask()
						.For.Members().Which(value => value.Name is "CoverUrl" or "PreviewUrl" or "Price" or "Duration")
						.Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleAlbum {
				                                             Artist = RuleArtist {
				                                               Id = 3,
				                                               Name = "The Beatles"
				                                             },
				                                             Id = {Id_1},
				                                             Title = "Abbey Road",
				                                             Tracks = [
				                                               RuleTrack {
				                                                 Id = {Id_2},
				                                                 Title = "Come Together"
				                                               },
				                                               RuleTrack {
				                                                 Id = {Id_3},
				                                                 Title = "Something"
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the id of the artist is excepted");
			}

			[Fact]
			public async Task WhenGlobalRules_ShouldApplyThemAfterTheLocalOnes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().DeclaredIn<RuleTrack>().WithName(nameof(RuleTrack.PreviewUrl)).Ignore(),
					SnapshotRule.For.Members().DeclaredIn<RuleTrack>().WithName(nameof(RuleTrack.Title)).Mask("{global}"),
					SnapshotRule.For.Members().DeclaredIn<RuleTrack>().WithName(nameof(RuleTrack.Id)).Mask(),
				]);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleTrack>().WithName(nameof(RuleTrack.Title)).Mask("{local}");

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
			public async Task WhenSubjectIsAString_ShouldThrowInvalidOperationException()
			{
				string subject = "foo";

				async Task IgnoringAct()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleAlbum>().WithName(nameof(RuleAlbum.Price)).Ignore();

				async Task MaskingAct()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleAlbum>().WithName(nameof(RuleAlbum.Id)).Mask();

				await That(IgnoringAct).Throws<InvalidOperationException>()
					.WithMessage("A member or value rule only applies to a snapshot of an object*").AsWildcard();
				await That(MaskingAct).Throws<InvalidOperationException>()
					.WithMessage("A member or value rule only applies to a snapshot of an object*").AsWildcard();
			}
		}
	}
}
