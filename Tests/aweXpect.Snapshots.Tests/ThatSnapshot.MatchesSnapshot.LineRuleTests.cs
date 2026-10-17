using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class LineRuleTests
		{
			private const string FirstGuid = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
			private const string SecondGuid = "7c9e6679-7425-40de-944b-e07fc1f90ae7";

			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task Which_ShouldRemoveTheLinesMatchingThePredicate()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\n\nLet It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Which(line => line.Length == 0).Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\nLet It Be");
			}

			[Fact]
			public async Task Which_ShouldPassEachLineWithoutItsLineBreak()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<string> lines = [];
				string subject = "Abbey Road\r\nLet It Be\nHelp!\rBack in Black\n";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Which(line =>
						{
							lines.Add(line);
							return false;
						}).Ignore();

				await That(Act).Throws<XunitException>();
				await That(lines).IsEqualTo(["Abbey Road", "Let It Be", "Help!", "Back in Black", "",])
					.Because("a line ends with \\r\\n, \\n or \\r, and a final line break is followed by an empty line");
				await That(store.Mismatches[Id]).IsEqualTo(subject);
			}

			[Fact]
			public async Task Except_ShouldRemoveTheLinesNotMatchingThePredicate()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Track 1: Help!\nloaded\nTrack 2: Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Except(line => line.StartsWith("Track", StringComparison.Ordinal)).Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Track 1: Help!\nTrack 2: Let It Be");
			}

			[Fact]
			public async Task Filters_ShouldAllHaveToMatch()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Track 1: Help! (Live)\nTrack 2: Let It Be\nLive at Wembley";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("Live").Which(line => line.StartsWith("Track", StringComparison.Ordinal))
						.Except(line => line.EndsWith("Be", StringComparison.Ordinal))
						.Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Track 2: Let It Be\nLive at Wembley")
					.Because("the filters of a rule are combined, so a line has to match all of them");
			}

			[Fact]
			public async Task WithoutFilter_ShouldApplyToEveryLine()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nLet It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Mask("<line>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<line>\n<line>");
			}

			[Fact]
			public async Task Containing_ShouldRemoveTheLinesContainingTheTextAnywhere()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nloaded, elapsed 3 ms\nLet It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\nLet It Be");
			}

			[Fact]
			public async Task Containing_ShouldCompareOrdinallyByDefault()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nElapsed 3 ms\nelapsed 4 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\nElapsed 3 ms")
					.Because("the case of the text is compared like in Contains");
			}

			[Fact]
			public async Task Containing_IgnoringCase_ShouldIgnoreTheCaseOfTheText()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nElapsed 3 ms\nelapsed 4 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("ELAPSED").IgnoringCase().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road");
			}

			[Fact]
			public async Task Containing_IgnoringCase_ShouldNotIgnoreTheCaseWhenComparingWithTheSnapshot()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "ABBEY ROAD\nelapsed 3 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("ELAPSED").IgnoringCase().Ignore();

				await That(Act).Throws<XunitException>()
					.Because("the option only applies to the line rule");
				await That(store.Mismatches[Id]).IsEqualTo("ABBEY ROAD");
			}

			[Fact]
			public async Task Containing_AsWildcard_ShouldMatchThePattern()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Track 1: Help!\nTrack 2: Help! (Live)\nTrack 3: Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("Track ?: *Live*").AsWildcard().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Track 1: Help!\nTrack 3: Let It Be");
			}

			[Fact]
			public async Task Containing_AsRegex_ShouldMatchThePattern()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "09:30 started\nAbbey Road\nat 09:31 Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing(@"^\d{2}:\d{2} ").AsRegex().Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\nat 09:31 Let It Be")
					.Because("the anchor of the pattern applies to each line");
			}

			[Fact]
			public async Task Containing_Options_ShouldOnlyApplyToTheirOwnRule()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Track 1: Help!\nlive at Shea Stadium\nLIVE at Wembley\ntrack 2: Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("TRACK").IgnoringCase().Ignore()
						.For.Lines().Containing("LIVE").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("live at Shea Stadium")
					.Because("only the first rule ignores the case");
			}

			[Fact]
			public async Task Containing_AfterTheAction_ShouldNoLongerOfferTheOptionsOfTheRule()
			{
				Type containing = typeof(SnapshotLinesContaining<SnapshotResult<string?, IThat<string?>>>);
				Type afterAction = containing.GetMethod(nameof(SnapshotLines<SnapshotRule>.Ignore))!.ReturnType;

				await That(typeof(IOptionsProvider<StringEqualityOptions>).IsAssignableFrom(containing)).IsTrue();
				await That(typeof(IOptionsProvider<StringEqualityOptions>).IsAssignableFrom(afterAction)).IsFalse()
					.Because("the options of the line rule must not be mistaken for options of the snapshot comparison");
			}

			[Fact]
			public async Task Containing_ShouldOnlyOfferThePatternOptions()
			{
				Type containing = typeof(SnapshotLinesContaining<SnapshotRule>);

				await That(typeof(IStringPatternMatchTypeOptions).IsAssignableFrom(containing)).IsTrue();
				await That(typeof(IStringMatchTypeOptions).IsAssignableFrom(containing)).IsFalse()
					.Because("the text is searched within the line, where a prefix or suffix has no meaning");
			}

			[Fact]
			public async Task Containing_WhenFollowedByOtherOptions_ShouldApplyThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"album {FirstGuid}\nelapsed 3 ms\nalbum {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("ELAPSED").IgnoringCase().Ignore()
						.Named("tracks")
						.For.Guids().Mask()
						.AcceptingChanges();

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id with { Name = "tracks", }] = "album {Guid_1}\nalbum {Guid_2}",
				});
			}

			[Fact]
			public async Task Containing_WhenRegexIsInvalid_ShouldThrowArgumentExceptionAtTheAction()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Which(_ => true).Ignore()
						.For.Lines().Containing("(").AsRegex().Ignore();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("text").And
					.WithMessage("The 'text' regex pattern is invalid: *").AsWildcard()
					.Because("the pattern is validated where the rule is written, also when another rule removes every line");
			}

			[Fact]
			public async Task Containing_WhenGlobalRegexIsInvalid_ShouldThrowArgumentExceptionAtTheAction()
			{
				void Act()
					=> SnapshotRule.For.Lines().Containing("(").AsRegex().Mask("x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("text").And
					.WithMessage("The 'text' regex pattern is invalid: *").AsWildcard();
			}

			[Fact]
			public async Task Containing_WhenRegexTimesOut_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = new string('a', 40) + "!";

				async Task Act()
				{
					using CustomizationLifetime timeout =
						Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromMilliseconds(1));
					await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing(@"^(\w+\s?)*$").AsRegex().Ignore();
				}

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the line rule with the pattern "^(\\w+\\s?)*$" did not complete within *. Simplify the pattern to avoid catastrophic backtracking
					             """).AsWildcard()
					.Because("the short timeout only applies to the expectation, not to this wildcard match of its message");
				await That(exception.InnerException).Is<RegexMatchTimeoutException>()
					.Whose(timeoutException => timeoutException.MatchTimeout,
						matchTimeout => matchTimeout.IsEqualTo(TimeSpan.FromMilliseconds(1)))
					.Because("the regex timeout of the settings applies, like for a masking");
				await That(store.Mismatches).IsEmpty()
					.Because("the lines could not be removed");
			}

			[Fact]
			public async Task Containing_WhenTextIsEmpty_ShouldThrowArgumentException()
			{
				void Act()
					=> SnapshotRule.For.Lines().Containing("");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("text").And
					.WithMessage("The 'text' cannot be empty.*").AsWildcard();
			}

			[Fact]
			public async Task Containing_WhenTextIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing(null!).Ignore();

				await That(Act).Throws<ArgumentNullException>().WithParamName("text");
			}

			[Fact]
			public async Task Which_WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Lines().Which(null!).Ignore();

				await That(Act).Throws<ArgumentNullException>().WithParamName("predicate");
			}

			[Fact]
			public async Task Except_WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotRule.For.Lines().Except(null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("predicate");
			}

			[Fact]
			public async Task Which_WhenPredicateThrows_ShouldThrowTheException()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Which(_ => throw new InvalidOperationException("boom")).Ignore();

				await That(Act).Throws<InvalidOperationException>().WithMessage("boom")
					.Because("an exception of the predicate is a mistake in the test that must not be hidden");
			}

			[Theory]
			[InlineData("Abbey Road\r\nx\r\nLet It Be", "Abbey Road\r\nLet It Be")]
			[InlineData("Abbey Road\nx\nLet It Be", "Abbey Road\nLet It Be")]
			[InlineData("x\r\nAbbey Road\nLet It Be", "Abbey Road\nLet It Be")]
			[InlineData("Abbey Road\r\nLet It Be\nx", "Abbey Road\r\nLet It Be")]
			[InlineData("Abbey Road\nLet It Be\r\nx", "Abbey Road\nLet It Be")]
			[InlineData("Abbey Road\r\nx\nLet It Be\nx\r\nHelp!", "Abbey Road\r\nLet It Be\nHelp!")]
			[InlineData("Abbey Road\rx\rLet It Be", "Abbey Road\rLet It Be")]
			[InlineData("Abbey Road\nx\n", "Abbey Road\n")]
			[InlineData("Abbey Road\nx\nx", "Abbey Road")]
			[InlineData("x\r\nx", "")]
			[InlineData("x", "")]
			[InlineData("", "")]
			public async Task LineBreaks_ShouldBeRemovedWithTheirLineAndKeptOtherwise(string subject, string expected)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("x").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo(expected)
					.Because("a removed line takes its own line break, the last one also the line break before it");
			}

			[Fact]
			public async Task LineBreaks_WhenTheLastLineIsEmptyAndRemoved_ShouldRemoveTheFinalLineBreak()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\r\n\r\nLet It Be\r\n";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Which(line => line.Length == 0).Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\r\nLet It Be");
			}

			[Theory]
			[InlineData("Abbey Road\r\nNow playing: Help!\r\nLet It Be", "Abbey Road\r\nNow playing: <track>\r\nLet It Be")]
			[InlineData("Now playing: Help!\nLet It Be", "Now playing: <track>\nLet It Be")]
			[InlineData("Abbey Road\rNow playing: Help!", "Abbey Road\rNow playing: <track>")]
			[InlineData("Now playing: Help!\n", "Now playing: <track>\n")]
			public async Task Mask_ShouldReplaceTheWholeLineAndKeepItsLineBreak(string subject, string expected)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("Now playing").Mask("Now playing: <track>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo(expected);
			}

			[Fact]
			public async Task Mask_WhenTemplateIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotRule.For.Lines().Mask(null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("template");
			}

			[Fact]
			public async Task Mask_ShouldInsertTheTemplateLiterally()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "elapsed 3 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing(@"(\d+) ms").AsRegex().Mask("$1 ms");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("$1 ms")
					.Because("a line is replaced as a whole, so a pattern of the filter has no groups to substitute");
			}

			[Fact]
			public async Task Convert_ShouldReplaceTheLineWithTheConvertedText()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\r\nelapsed 3 ms\r\nLet It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Convert(line => line.Substring(0, "elapsed".Length));

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\r\nelapsed\r\nLet It Be")
					.Because("the converted line keeps its line break");
			}

			[Fact]
			public async Task Convert_WhenItReturnsNull_ShouldRemoveTheLine()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nelapsed 3 ms\nLet It Be\nelapsed 4 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Convert(line => line.StartsWith("elapsed", StringComparison.Ordinal)
							? null
							: line.ToUpperInvariant());

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("ABBEY ROAD\nLET IT BE")
					.Because("a line converted to null is removed like an ignored line, also its line break");
			}

			[Fact]
			public async Task Convert_WhenConverterIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotRule.For.Lines().Convert(null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("convert");
			}

			[Fact]
			public async Task Convert_WhenConverterThrows_ShouldThrowTheException()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Convert(_ => throw new InvalidOperationException("boom"));

				await That(Act).Throws<InvalidOperationException>().WithMessage("boom");
			}

			[Fact]
			public async Task IgnoringWinsOverMasking()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nelapsed 3 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Mask("<elapsed>")
						.For.Lines().Containing("ms").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road")
					.Because("a rule that leaves lines out wins, also when it is added after a masking");
			}

			[Fact]
			public async Task TheFirstMatchingMaskingWins()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "elapsed 3 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Mask("<first>")
						.For.Lines().Containing("ms").Convert(_ => "<second>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<first>")
					.Because("a masked line is not masked again by a later rule");
			}

			[Fact]
			public async Task WhenGlobalAndLocalMaskings_ShouldApplyTheLocalOneFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Lines().Containing("elapsed").Mask("<global>"),
				]);
				string subject = "Abbey Road\nelapsed 3 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Mask("<local>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\n<local>")
					.Because("the rules of the expectation come before those for all snapshots");
			}

			[Fact]
			public async Task WhenMaskingLines_ShouldApplyTheTextRulesAfterwards()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Now playing: Help!\nalbum {FirstGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Now playing").Mask("<text>")
						.For.Guids().Mask()
						.For.Lines().Containing("Now playing").Mask("Now playing: <track>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<text>: <track>\nalbum {Guid_1}")
					.Because("the line rules are applied before the text rules, whatever the order in which they are added");
			}

			[Fact]
			public async Task WhenMaskingGuids_ShouldNotNumberTheRemovedLines()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Lines().Containing("session").Ignore(),
					SnapshotRule.For.Guids().Mask(),
				]);
				string subject = $"session {Guid.NewGuid()}\nAbbey Road {FirstGuid}\ndebug {Guid.NewGuid()}\nLet It Be {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Guids().Mask()
						.For.Lines().Containing("debug").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road {Guid_1}\nLet It Be {Guid_2}")
					.Because("the lines are removed before the text rules of the expectation and the global ones");
			}

			[Fact]
			public async Task WhenSnapshotDiffersOnlyInARemovedLine_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road\nLet It Be";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nelapsed 3 ms\nLet It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Ignore();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSnapshotDiffers_ShouldShowTheValueWithoutTheRemovedLines()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Let It Be";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "elapsed 3 ms\nAbbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Ignore();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was "Abbey Road", which differs at index 0:
					                ↓ (actual)
					               "Abbey Road"
					               "Let It Be"
					                ↑ (expected)
					             """)
					.Because("the value without the removed lines is what was compared and what would be stored");
			}

			[Fact]
			public async Task WhenValuesDifferOnlyInARemovedLine_ShouldNotBeAConflict()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
				{
					await That("Abbey Road\nelapsed 3 ms").MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Ignore();
					await That("Abbey Road\nelapsed 4 ms").MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Ignore();
				}

				await That(Act).DoesNotThrow()
					.Because("the check for a snapshot used for another value compares the values without the removed lines");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFailWithoutApplyingTheRules()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Which(_ => throw new InvalidOperationException("boom")).Ignore();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsAnObject_IgnoringLines_ShouldThrowInvalidOperationException()
			{
				RuleNote subject = new()
				{
					Text = "Abbey Road",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Lines().Which(_ => true).Ignore();

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A line rule only applies to a snapshot of a string, but an object is written by the formatter.*")
					.AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsAnObject_MaskingLines_ShouldThrowInvalidOperationException()
			{
				RuleNote subject = new()
				{
					Text = "Abbey Road",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("Abbey").Mask("<album>");

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A line rule only applies to a snapshot of a string*").AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsAnObject_ConvertingLines_ShouldThrowInvalidOperationException()
			{
				RuleNote subject = new()
				{
					Text = "Abbey Road",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Convert(line => line);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A line rule only applies to a snapshot of a string*").AsWildcard();
			}

			[Fact]
			public async Task WhenGlobalLineRules_ShouldApplyThemTogetherWithTheRulesOfTheExpectation()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Lines().Containing("BUILD DATE").IgnoringCase().Ignore(),
					SnapshotRule.For.Lines().Which(line => line.Length > 0 && line[0] == '#').Ignore(),
				]);
				string subject = "Build date: 2026-10-17\n# AC/DC\nBack in Black\nelapsed 3 ms\nHighway to Hell";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Lines().Containing("elapsed").Ignore();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Back in Black\nHighway to Hell");
			}

			[Fact]
			public async Task WhenGlobalLineRulesAndTheSubjectIsAnObject_ShouldIgnoreThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Lines().Containing("Text").Ignore(),
					SnapshotRule.For.Lines().Which(_ => true).Ignore(),
					SnapshotRule.For.Lines().Mask("<line>"),
				]);
				RuleNote subject = new()
				{
					Text = "Abbey Road",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleNote {
				                                             Text = "Abbey Road"
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("the lines of an object are written by the formatter");
			}

			[Fact]
			public async Task WhenGlobalLineRuleTimesOut_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = new string('a', 40) + "!";

				async Task Act()
				{
					using CustomizationLifetime timeout =
						Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromMilliseconds(1));
					using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
						SnapshotRule.For.Lines().Containing(@"^(\w+\s?)*$").AsRegex().Ignore(),
					]);
					await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);
				}

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("*the line rule with the pattern \"^(\\\\w+\\\\s?)*$\" did not complete*").AsWildcard()
					.Because("the short timeout only applies to the expectation, not to this wildcard match of its message");
				await That(exception.InnerException).Is<RegexMatchTimeoutException>();
			}

			[Theory]
			[InlineData("Track 1", "")]
			[InlineData("TRACK", "IgnoringCase")]
			[InlineData("track ?:", "AsWildcard")]
			[InlineData("^T.*k", "AsRegex")]
			public async Task GlobalAndExpectationRules_WithTheSameOptions_ShouldRemoveTheSameLines(string text,
				string option)
			{
				string subject = "Track 1: Help!\ntrack 2: Let It Be\nAbbey Road";
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task ActWithGlobalRule()
				{
					using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
						WithOption(SnapshotRule.For.Lines().Containing(text), option).Ignore(),
					]);
					await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("global");
				}

				async Task ActWithExpectationRule()
					=> await WithOption(That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("expectation")
						.For.Lines().Containing(text), option).Ignore();

				await That(ActWithGlobalRule).Throws<XunitException>();
				await That(ActWithExpectationRule).Throws<XunitException>();
				await That(store.Mismatches[Id with { Name = "expectation", }])
					.IsEqualTo(store.Mismatches[Id with { Name = "global", }])
					.Because("a line rule for all snapshots has the same semantics as one of the expectation");
			}

			private static TResult WithOption<TResult>(TResult result, string option)
				where TResult : IOptionsProvider<StringEqualityOptions>, IStringPatternMatchTypeOptions
				=> option switch
				{
					"IgnoringCase" => result.IgnoringCase(),
					"AsWildcard" => result.AsWildcard(),
					"AsRegex" => result.AsRegex(),
					_ => result,
				};
		}
	}
}
