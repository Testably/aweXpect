using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class WithNameTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task IgnoringCase_ShouldMatchTheNameInAnyCase()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("previewurl").IgnoringCase().Ignore()
						.For.Members().WithName("TITLE").IgnoringCase().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Duration = 259,
				                                             Id = 11,
				                                             Price = 1.29
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task AsPrefix_ShouldMatchTheStartOfTheName()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("Pr").AsPrefix().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Duration = 259,
				                                             Id = 11,
				                                             Title = "Come Together"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task AsSuffix_ShouldMatchTheEndOfTheName()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleAlbum subject = RuleAlbum.AbbeyRoad();
				subject.Tracks.RemoveAt(1);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("url").AsSuffix().IgnoringCase().Ignore()
						.For.Members().WithName("Price").Ignore();

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
				                                                 Duration = 259,
				                                                 Id = 11,
				                                                 Title = "Come Together"
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the options combine, and the name matches at any depth");
			}

			[Fact]
			public async Task AsWildcard_ShouldMatchTheNameAsWildcardPattern()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("*Url").AsWildcard().Ignore()
						.For.Members().WithName("?d").AsWildcard().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Duration = 259,
				                                             Id = {Id_1},
				                                             Price = 1.29,
				                                             Title = "Come Together"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task AsRegex_ShouldMatchTheNameAsRegexPattern()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleTrack subject = RuleTrack.ComeTogether();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName("^(Duration|Price)$").AsRegex().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleTrack {
				                                             Id = 11,
				                                             PreviewUrl = "https://example.com/come-together.mp3",
				                                             Title = "Come Together"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task OnValues_ShouldMatchTheNameOfTupleItemsAndDictionaryKeys()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				Dictionary<string, int> subject = new()
				{
					["Abbey Road"] = 1969,
					["Let It Be"] = 1970,
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().WithName("[Let It Be]").Mask("{year}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           {
				                                             ["Abbey Road"] = 1969,
				                                             ["Let It Be"] = {year}
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the name of a dictionary value is its key in brackets");
			}

			[Fact]
			public async Task WhenTheRegexIsInvalid_ShouldThrowArgumentExceptionAtTheAction()
			{
				SnapshotMembersWithName<SnapshotRule, object> selection =
					SnapshotRule.For.Members().WithName("(").AsRegex();

				void Act()
					=> selection.Ignore();

				await That(Act).Throws<ArgumentException>().WithParamName("name")
					.Because("the pattern is complete when the action is written, so it is checked there");
			}

			[Fact]
			public async Task WhenTheRegexTimesOut_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				Dictionary<string, int> subject = new()
				{
					[new string('a', 40) + "!"] = 1,
				};

				async Task Act()
				{
					using CustomizationLifetime timeout =
						Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromMilliseconds(1));
					await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().WithName(@"^\[(\w+\s?)*\]$").AsRegex().Ignore();
				}

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the member rule with the pattern "^\\[(\\w+\\s?)*\\]$" did not complete within *. Simplify the pattern to avoid catastrophic backtracking
					             """).AsWildcard()
					.Because("the short timeout only applies to the expectation, not to this wildcard match of its message");
				await That(exception.InnerException).Is<RegexMatchTimeoutException>()
					.Because("a pattern that times out fails both ways, like a line rule or a masking");
				await That(store.Mismatches).IsEmpty();
			}
		}
	}
}
