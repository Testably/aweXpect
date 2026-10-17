using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class MemberOfTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task IgnoringMemberOf_ShouldOnlyLeaveOutTheMemberOfTheType()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();
				subject.Tracks.RemoveAt(1);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.MemberOf<RuleAlbum>(a => a.Price);

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
			public async Task IgnoringMemberOf_ShouldLeaveOutTheMemberOfADerivedType()
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
						.Ignoring.MemberOf<RuleAlbum>(a => a.Price)
						.Ignoring.MemberOf<RuleAlbum>(a => a.Artist)
						.Ignoring.MemberOf<RuleAlbum>(a => a.CoverUrl)
						.Ignoring.MemberOf<RuleAlbum>(a => a.Tracks);

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
			public async Task IgnoringMemberOf_WhenTheTypeIsAnotherOne_ShouldKeepTheMember()
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
						.Ignoring.MemberOf<RuleLiveAlbum>(a => a.Price)
						.Ignoring.Member("Artist")
						.Ignoring.Member("CoverUrl")
						.Ignoring.Member("Tracks");

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
			public async Task MaskingMemberOf_ShouldNumberThePlaceholderAfterTheMember()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.MemberOf<RuleTrack>(t => t.Id)
						.Ignoring.Member("CoverUrl")
						.Ignoring.Member("PreviewUrl")
						.Ignoring.Member("Price")
						.Ignoring.Member("Duration");

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
			public async Task MaskingMemberOf_ShouldShareTheNumbersWithMember()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();
				subject.Tracks[0].Id = 1;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.MemberOf<RuleArtist>(a => a.Id, "{artist}")
						.Masking.MemberOf<RuleTrack>(t => t.Id)
						.Masking.Member("Id")
						.Ignoring.Member(value => value.Name is "CoverUrl" or "PreviewUrl" or "Price" or "Duration");

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
			public async Task MaskingMemberOf_WithAField_ShouldWriteTheReplacementWithoutQuotes()
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
						.Masking.MemberOf<RuleArtist>(a => a.Name, "{artist}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleArtist {
				                                             Id = 3,
				                                             Name = {artist}
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenGlobalRules_ShouldApplyThemAfterTheLocalOnes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime ignorings = Customize.aweXpect.Snapshots().Ignorings.Set([
					SnapshotIgnoring.MemberOf<RuleTrack>(t => t.PreviewUrl),
				]);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.MemberOf<RuleTrack>(t => t.Title, "{global}"),
					SnapshotMasking.MemberOf<RuleTrack>(t => t.Id),
				]);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.MemberOf<RuleTrack>(t => t.Title, "{local}");

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
			public async Task WhenTheMemberIsNested_ShouldThrowArgumentExceptionWhereItIsWritten()
			{
				RuleAlbum subject = RuleAlbum.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.MemberOf<RuleAlbum>(a => a.Artist.Name);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("member").And
					.WithMessage(
						"The 'member' must access a field or property of the RuleAlbum itself, but 'a => a.Artist.Name' accesses a nested member. Select it by the type that declares it instead, e.g. 'MemberOf<RuleArtist>(x => x.Name)'.*")
					.AsWildcard()
					.Because("the formatter does not tell the type of the object that contains a nested member");
			}

			[Fact]
			public async Task WhenTheParameterIsCast_ShouldThrowArgumentExceptionNamingTheDeclaringType()
			{
				void Act()
					=> SnapshotIgnoring.MemberOf<RuleAlbum>(a => ((RuleLiveAlbum)a).Venue);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("member").And
					.WithMessage("*accesses a nested member*e.g. 'MemberOf<RuleLiveAlbum>(x => x.Venue)'.*")
					.AsWildcard();
			}

			[Fact]
			public async Task WhenTheExpressionIsNoMemberOfTheParameter_ShouldThrowArgumentException()
			{
				void MethodAct()
					=> SnapshotIgnoring.MemberOf<RuleAlbum>(a => a.ToString());

				void ParameterAct()
					=> SnapshotMasking.MemberOf<RuleAlbum>(a => a);

				void IndexerAct()
					=> SnapshotMasking.MemberOf<RuleAlbum>(a => a.Tracks[0], "{track}");

				void StaticAct()
					=> SnapshotIgnoring.MemberOf<RuleAlbum>(_ => string.Empty);

				RuleAlbum other = RuleAlbum.AbbeyRoad();

				void CapturedAct()
					=> SnapshotIgnoring.MemberOf<RuleAlbum>(_ => other.Price);

				await That(MethodAct).Throws<ArgumentException>().WithParamName("member").And
					.WithMessage(
						"The 'member' must access a field or property of the RuleAlbum itself, e.g. 'x => x.Id', but was 'a => a.ToString()'.*")
					.AsWildcard();
				await That(ParameterAct).Throws<ArgumentException>().WithParamName("member").And
					.WithMessage("*but was 'a => a'.*").AsWildcard();
				await That(IndexerAct).Throws<ArgumentException>().WithParamName("member").And
					.WithMessage("*e.g. 'x => x.Id', but was*").AsWildcard();
				await That(StaticAct).Throws<ArgumentException>().WithParamName("member").And
					.WithMessage("*e.g. 'x => x.Id', but was*").AsWildcard();
				await That(CapturedAct).Throws<ArgumentException>().WithParamName("member").And
					.WithMessage("*e.g. 'x => x.Id', but was*").AsWildcard()
					.Because("a member of another object is no member of the parameter");
			}

			[Fact]
			public async Task WhenTheMemberIsNull_ShouldThrowArgumentNullException()
			{
				void IgnoringAct()
					=> SnapshotIgnoring.MemberOf<RuleAlbum>(null!);

				void MaskingAct()
					=> SnapshotMasking.MemberOf<RuleAlbum>(null!);

				void MaskingWithReplacementAct()
					=> SnapshotMasking.MemberOf<RuleAlbum>(null!, "{x}");

				await That(IgnoringAct).Throws<ArgumentNullException>().WithParamName("member");
				await That(MaskingAct).Throws<ArgumentNullException>().WithParamName("member");
				await That(MaskingWithReplacementAct).Throws<ArgumentNullException>().WithParamName("member");
			}

			[Fact]
			public async Task WhenReplacementIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotMasking.MemberOf<RuleAlbum>(a => a.Id, null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("replacement");
			}

			[Fact]
			public async Task WhenSubjectIsAString_ShouldThrowInvalidOperationException()
			{
				string subject = "foo";

				async Task IgnoringAct()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.MemberOf<RuleAlbum>(a => a.Price);

				async Task MaskingAct()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.MemberOf<RuleAlbum>(a => a.Id);

				await That(IgnoringAct).Throws<InvalidOperationException>()
					.WithMessage("A member or type rule only applies to a snapshot of an object*").AsWildcard();
				await That(MaskingAct).Throws<InvalidOperationException>()
					.WithMessage("A member or type rule only applies to a snapshot of an object*").AsWildcard();
			}
		}
	}
}
