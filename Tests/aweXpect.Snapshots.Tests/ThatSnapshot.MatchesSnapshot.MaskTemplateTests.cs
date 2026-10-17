using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class MaskTemplateTests
		{
			private const string FirstGuid = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
			private const string SecondGuid = "7c9e6679-7425-40de-944b-e07fc1f90ae7";

			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task WithoutNumber_ShouldBeAFixedText()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=1, id=2";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"id=\d+").AsRegex().Mask("id=<id>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("id=<id>, id=<id>");
			}

			[Fact]
			public async Task WithNumber_ShouldNumberTheValuesInTheOrderOfAppearance()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=7, id=3, id=7";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"id=\d+").AsRegex().Mask("<id-{0}>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<id-1>, <id-2>, <id-1>")
					.Because("the same value keeps the same number");
			}

			[Fact]
			public async Task WithNumberSeveralTimes_ShouldReplaceEachOccurrence()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Ann";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Ann").Mask("{0}-{0}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("1-1");
			}

			[Fact]
			public async Task WithTheSameTemplate_ShouldShareTheCounterAcrossRules()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Ann, Bob, Ann";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Ann").Mask("{Customer_{0}}")
						.For.Text("Bob").Mask("{Customer_{0}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Customer_1}, {Customer_2}, {Customer_1}")
					.Because("every value masked with the same template shares one counter");
			}

			[Fact]
			public async Task WithDifferentTemplates_ShouldCountSeparately()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Ann, Bob";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Ann").Mask("{Customer_{0}}")
						.For.Text("Bob").Mask("{Artist_{0}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Customer_1}, {Artist_1}");
			}

			[Fact]
			public async Task WhenTheTextDoesNotOccur_ShouldNotUseUpANumber()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Bob";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Ann").Mask("{Customer_{0}}")
						.For.Text("Bob").Mask("{Customer_{0}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Customer_1}");
			}

			[Fact]
			public async Task WhenGlobalAndLocalRulesUseTheSameTemplate_ShouldShareTheCounter()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Text("Bob").Mask("{Customer_{0}}"),
				]);
				string subject = "Bob, Ann";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Ann").Mask("{Customer_{0}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Customer_2}, {Customer_1}")
					.Because("the rule of the expectation is applied first, so it numbers its value first");
			}

			[Fact]
			public async Task WithBracesAroundTheTemplate_ShouldKeepThemAsTheyAre()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Ann";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Ann").Mask("{{Customer_{0}}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{{Customer_1}}")
					.Because("'{0}' is replaced as plain text, not as a composite format, so braces are not escaped");
			}

			[Fact]
			public async Task WhenIgnoringCase_ShouldNumberEveryCaseTheSame()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road, ABBEY ROAD, Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("abbey road").IgnoringCase().Mask("<album-{0}>")
						.For.Text("let it be").IgnoringCase().Mask("<album-{0}>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<album-1>, <album-1>, <album-2>");
			}

			[Fact]
			public async Task WithWildcard_ShouldNumberTheMatches()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Track 1, Track 2, Track 1";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Track ?").AsWildcard().Mask("{Track_{0}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Track_1}, {Track_2}, {Track_1}");
			}

			[Fact]
			public async Task WithRegexSubstitution_ShouldSubstituteFirstAndThenNumber()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "api:5001 api:5002 web:5001 api:5001";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"(\w+):\d+").AsRegex().Mask("$1:{Port_{0}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("api:{Port_1} api:{Port_2} web:{Port_1} api:{Port_1}")
					.Because("the substitutions are applied first, so each resulting template, e.g. 'web:{Port_{0}}', counts on its own");
			}

			[Fact]
			public async Task MaskOfGuids_ShouldShareTheCounterWithTheSameTemplate()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"order {FirstGuid}, customer {SecondGuid}, again {FirstGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(SecondGuid).Mask("{Guid_{0}}")
						.For.Guids().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("order {Guid_2}, customer {Guid_1}, again {Guid_2}")
					.Because("'Mask()' is the shorthand for the template '{Guid_{0}}'");
			}

			[Fact]
			public async Task MaskOfDates_ShouldShareTheCounterWithTheSameTemplate()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "released 1969-09-26, recorded 17.10.2026, again 1969-09-26";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"\d{2}\.\d{2}\.\d{4}").AsRegex().Mask("{Date_{0}}")
						.For.Dates().Mask();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("released {Date_2}, recorded {Date_1}, again {Date_2}")
					.Because("'Mask()' is the shorthand for the template '{Date_{0}}'");
			}

			[Fact]
			public async Task GuidsWithFixedText_ShouldReplaceEachGuidWithIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{FirstGuid} {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask("<guid>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<guid> <guid>");
			}

			[Fact]
			public async Task GuidsWithTemplate_ShouldNumberThemIgnoringTheCase()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{FirstGuid} {SecondGuid} {FirstGuid.ToUpperInvariant()}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask("<guid-{0}>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<guid-1> <guid-2> <guid-1>");
			}

			[Fact]
			public async Task DatesWithTemplate_ShouldNumberThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "2026-10-17 2026-10-18 2026-10-17";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Dates().Mask("<day {0}>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<day 1> <day 2> <day 1>");
			}

			[Fact]
			public async Task LinesWithTemplate_ShouldNumberEqualLinesTheSame()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Now playing: Help!\nNow playing: Let It Be\nNow playing: Help!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("Now playing").Mask("Now playing: {Track_{0}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id])
					.IsEqualTo("Now playing: {Track_1}\nNow playing: {Track_2}\nNow playing: {Track_1}");
			}

			[Fact]
			public async Task LinesAndTextWithTheSameTemplate_ShouldShareTheCounter()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Ann\nBob and Ann";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Bob").Mask("{Customer_{0}}")
						.For.Text("Ann").Mask("{Customer_{0}}")
						.For.Lines().Which(line => line == "Ann").Mask("{Customer_{0}}");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Customer_1}\n{Customer_2} and {Customer_1}")
					.Because("the lines are masked before the text, and the line and the text 'Ann' are the same value");
			}
		}
	}
}
