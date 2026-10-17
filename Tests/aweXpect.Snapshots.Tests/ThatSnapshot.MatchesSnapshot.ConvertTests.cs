using System.Collections.Generic;
using System.Globalization;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class ConvertTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task Convert_WithAType_ShouldWriteTheConvertedValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleRelease>().WithName(nameof(RuleRelease.Released)).OfType<DateTime>()
						.Convert(released => released.Year)
						.For.Members().DeclaredIn<RuleSong>().WithName(nameof(RuleSong.Duration)).OfType<TimeSpan>()
						.Convert(duration => Math.Round(duration.TotalMinutes));

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = 1969,
				                                             Songs = [
				                                               RuleSong {
				                                                 Duration = 4.0,
				                                                 Title = "Come Together"
				                                               },
				                                               RuleSong {
				                                                 Duration = 3.0,
				                                                 Title = "Something"
				                                               }
				                                             ],
				                                             Title = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task Convert_WithoutAType_ShouldPassTheValueAsObject()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleRelease.Released)).Convert(released => ((DateTime)released).Year)
						.For.Members().WithName(nameof(RuleRelease.Songs)).Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = 1969,
				                                             Title = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task ConvertValues_ShouldAlsoConvertTheItems()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<TimeSpan> subject = [TimeSpan.FromSeconds(259), TimeSpan.FromSeconds(182),];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Values().OfType<TimeSpan>().Convert(duration => duration.TotalSeconds);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           [
				                                             259.0,
				                                             182.0
				                                           ]
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task Convert_WhenTheConvertedValueIsAStringOrANumber_ShouldWriteItLikeAnyOtherValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				CultureInfo previousCulture = CultureInfo.CurrentCulture;
				CultureInfo.CurrentCulture = new CultureInfo("de-DE");
				try
				{
					RuleRelease subject = RuleRelease.AbbeyRoad();

					async Task Act()
						=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
							.For.Members().WithName(nameof(RuleRelease.Released)).OfType<DateTime>()
							.Convert(released => released.ToString("MMMM yyyy", CultureInfo.InvariantCulture))
							.For.Members().WithName(nameof(RuleSong.Duration)).OfType<TimeSpan>()
							.Convert(duration => duration.TotalSeconds / 2)
							.For.Members().WithName("Title").Ignore();

					await That(Act).Throws<XunitException>();
				}
				finally
				{
					CultureInfo.CurrentCulture = previousCulture;
				}

				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = "September 1969",
				                                             Songs = [
				                                               RuleSong {
				                                                 Duration = 129.5
				                                               },
				                                               RuleSong {
				                                                 Duration = 91.0
				                                               }
				                                             ]
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a string is written in quotes and a number in the invariant culture, like any other value");
			}

			[Fact]
			public async Task Convert_WhenTheConvertedValueIsAnObject_ShouldIndentItAtTheDepthOfTheMember()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();
				subject.Songs.RemoveAt(1);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleSong.Duration)).OfType<TimeSpan>().Convert(duration => new RuleArtist
						{
							Id = duration.Minutes,
							Name = $"{duration.Seconds} s",
						})
						.For.Members().WithName(nameof(RuleRelease.Released)).OfType<DateTime>()
						.Convert(released => new[] { released.Year, released.Month, });

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = [
				                                               1969,
				                                               9
				                                             ],
				                                             Songs = [
				                                               RuleSong {
				                                                 Duration = RuleArtist {
				                                                   Id = 4,
				                                                   Name = "19 s"
				                                                 },
				                                                 Title = "Come Together"
				                                               }
				                                             ],
				                                             Title = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task Convert_WhenTheConvertedValueIsAnObject_ShouldNotApplyTheMemberRulesToIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleRelease.Released)).OfType<DateTime>().Convert(released =>
							new RuleArtist
							{
								Id = released.Year,
								Name = "The Beatles",
							})
						.For.Members().WithName("Id").Ignore()
						.For.Members().DeclaredIn<RuleArtist>().WithName(nameof(RuleArtist.Name)).OfType<string>()
						.Convert(name => name.ToUpperInvariant())
						.For.Members().WithName("Songs").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = RuleArtist {
				                                               Id = 1969,
				                                               Name = "The Beatles"
				                                             },
				                                             Title = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the converter decides what is written, so its result is not converted or left out again");
			}

			[Fact]
			public async Task Convert_ShouldApplyTheTextRulesToTheConvertedValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleRelease.Released)).OfType<DateTime>()
						.Convert(released => released.Date)
						.For.Dates().Mask()
						.For.Members().WithName("Songs").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = {Date_1},
				                                             Title = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the member rules act while the object is written, before the text rules");
			}

			[Fact]
			public async Task Convert_WhenTheConverterReturnsNull_ShouldWriteNull()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleRelease.Title)).OfType<string>().Convert(_ => (string?)null)
						.For.Members().WithName("Songs").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = 1969-09-26T00:00:00.0000000,
				                                             Title = <null>
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task Convert_WhenTheValueIsNull_ShouldNotCallTheConverter()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();
				int calls = 0;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleRelease.Label)).OfType<string>().Convert(label =>
						{
							calls++;
							return label.Length;
						})
						.For.Members().WithName(nameof(RuleRelease.Label)).Mask("{label}")
						.For.Members().WithName("Songs").Ignore();

				await That(Act).Throws<XunitException>();
				await That(calls).IsEqualTo(0);
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = 1969-09-26T00:00:00.0000000,
				                                             Title = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a null value is not masked, like by the other maskings");
			}

			[Fact]
			public async Task Convert_ShouldReadTheValueOnce()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				int reads = 0;
				RuleCountingRelease subject = new(() => reads++);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleCountingRelease.Released)).OfType<DateTime>()
						.Convert(released => released.Year);

				await That(Act).Throws<XunitException>();
				await That(reads).IsEqualTo(1);
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleCountingRelease {
				                                             Released = 1969
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task Convert_WhenTheGetterThrows_ShouldWriteTheExceptionWithoutCallingTheConverter()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleThrowing subject = new();
				int calls = 0;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleThrowing.Value)).Convert(value =>
						{
							calls++;
							return value;
						});

				await That(Act).Throws<XunitException>();
				await That(calls).IsEqualTo(0);
				await That(store.Mismatches[Id]).IsEqualTo("*Value = *boom*").AsWildcard()
					.Because("a value that cannot be read is written with its exception by the formatter");
			}

			[Fact]
			public async Task Convert_WhenTheConverterThrows_ShouldThrowTheException()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleRelease.Released)).OfType<DateTime>()
						.Convert(int (_) => throw new InvalidOperationException("converter failed"));

				await That(Act).Throws<InvalidOperationException>().WithMessage("converter failed")
					.Because("an exception of the converter is a mistake in the test, which a placeholder would hide");
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task Convert_WhenTheValueIsOfAnotherType_ShouldNotConvertIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleReissue subject = new()
				{
					Released = "Remastered",
					Title = "Abbey Road",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleRelease>().WithName(nameof(RuleRelease.Released)).OfType<DateTime>()
						.Convert(released => released.Year)
						.For.Members().WithName("Songs").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("*Released = \"Remastered\"*").AsWildcard()
					.Because("the reissue hides the release date with a member of another type of the same name");
			}

			[Fact]
			public async Task Convert_WhenAnotherMaskingComesFirst_ShouldUseTheFirstOne()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleRelease.Title)).Mask("{title}")
						.For.Members().WithName(nameof(RuleRelease.Title)).OfType<string>().Convert(title => title.Length)
						.For.Members().WithName(nameof(RuleRelease.Released)).OfType<DateTime>()
						.Convert(released => released.Year)
						.For.Members().WithName(nameof(RuleRelease.Released)).Mask()
						.For.Members().WithName("Songs").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = 1969,
				                                             Title = {title}
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the first masking or converter that matches a value replaces it");
			}

			[Fact]
			public async Task Convert_WhenTheMemberIsIgnored_ShouldLeaveItOut()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleRelease subject = RuleRelease.AbbeyRoad();
				int calls = 0;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().WithName(nameof(RuleRelease.Released)).OfType<DateTime>().Convert(released =>
						{
							calls++;
							return released.Year;
						})
						.For.Members().DeclaredIn<RuleRelease>().WithName(nameof(RuleRelease.Released)).Ignore()
						.For.Members().WithName("Songs").Ignore();

				await That(Act).Throws<XunitException>();
				await That(calls).IsEqualTo(0);
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Title = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("ignoring wins over converting");
			}

			[Fact]
			public async Task WhenGlobalConverters_ShouldApplyThemAfterTheLocalOnes()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Members().DeclaredIn<RuleRelease>().WithName(nameof(RuleRelease.Title))
						.OfType<string>().Convert(title => title.ToUpperInvariant()),
					SnapshotRule.For.Values().OfType<DateTime>().Convert(released => released.Year),
				]);
				RuleRelease subject = RuleRelease.AbbeyRoad();

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().DeclaredIn<RuleRelease>().WithName(nameof(RuleRelease.Title)).OfType<string>()
						.Convert(title => title.Length)
						.For.Members().WithName("Songs").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleRelease {
				                                             Label = <null>,
				                                             Released = 1969,
				                                             Title = 10
				                                           }
				                                           """).IgnoringNewlineStyle();
			}

			[Fact]
			public async Task WhenGlobalConverterAndStringSubject_ShouldIgnoreIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Values().OfType<string>().Convert(title => title.Length),
				]);

				async Task Act()
					=> await That("Abbey Road").MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road");
			}

			[Fact]
			public async Task WhenSubjectIsAString_ShouldThrowInvalidOperationException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Members().OfType<DateTime>().Convert(released => released.Year);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A member or value rule only applies to a snapshot of an object*").AsWildcard();
			}

			[Fact]
			public async Task WhenTheConverterIsNull_ShouldThrowArgumentNullException()
			{
				void MembersAct()
					=> SnapshotRule.For.Members().OfType<DateTime>().Convert<int>(null!);

				void ValuesAct()
					=> SnapshotRule.For.Values().Convert<int>(null!);

				await That(MembersAct).Throws<ArgumentNullException>().WithParamName("convert");
				await That(ValuesAct).Throws<ArgumentNullException>().WithParamName("convert");
			}
		}
	}
}

public class RuleRelease
{
	public string? Label { get; set; }
	public DateTime Released { get; set; }
	public List<RuleSong> Songs { get; set; } = [];
	public string Title { get; set; } = "";

	public static RuleRelease AbbeyRoad()
		=> new()
		{
			Released = new DateTime(1969, 9, 26),
			Songs =
			[
				new RuleSong
				{
					Duration = TimeSpan.FromSeconds(259),
					Title = "Come Together",
				},
				new RuleSong
				{
					Duration = TimeSpan.FromSeconds(182),
					Title = "Something",
				},
			],
			Title = "Abbey Road",
		};
}

public sealed class RuleReissue : RuleRelease
{
	public new string Released { get; set; } = "";
}

public sealed class RuleSong
{
	public TimeSpan Duration { get; set; }
	public string Title { get; set; } = "";
}

public sealed class RuleCountingRelease(Action onRead)
{
	public DateTime Released
	{
		get
		{
			onRead();
			return new DateTime(1969, 9, 26);
		}
	}
}
