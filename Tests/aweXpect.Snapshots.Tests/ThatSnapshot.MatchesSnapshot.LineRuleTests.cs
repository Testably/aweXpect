using System.Collections.Generic;
using System.Linq;
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
			public async Task Lines_ShouldRemoveTheLinesMatchingThePredicate()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\n\nLet It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.Lines(line => line.Length == 0);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\nLet It Be");
			}

			[Fact]
			public async Task Lines_ShouldPassEachLineWithoutItsLineBreak()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<string> lines = [];
				string subject = "Abbey Road\r\nLet It Be\nHelp!\rBack in Black\n";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.Lines(line =>
						{
							lines.Add(line);
							return false;
						});

				await That(Act).Throws<XunitException>();
				await That(lines).IsEqualTo(["Abbey Road", "Let It Be", "Help!", "Back in Black", "",])
					.Because("a line ends with \\r\\n, \\n or \\r, and a final line break is followed by an empty line");
				await That(store.Mismatches[Id]).IsEqualTo(subject);
			}

			[Fact]
			public async Task LinesContaining_ShouldRemoveTheLinesContainingTheTextAnywhere()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nloaded, elapsed 3 ms\nLet It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("elapsed");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\nLet It Be");
			}

			[Fact]
			public async Task LinesContaining_ShouldCompareOrdinallyByDefault()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nElapsed 3 ms\nelapsed 4 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("elapsed");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\nElapsed 3 ms")
					.Because("the case of the text is compared like in Contains");
			}

			[Fact]
			public async Task LinesContaining_IgnoringCase_ShouldIgnoreTheCaseOfTheText()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road\nElapsed 3 ms\nelapsed 4 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("ELAPSED").IgnoringCase();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road");
			}

			[Fact]
			public async Task LinesContaining_IgnoringCase_ShouldNotIgnoreTheCaseWhenComparingWithTheSnapshot()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "ABBEY ROAD\nelapsed 3 ms";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("ELAPSED").IgnoringCase();

				await That(Act).Throws<XunitException>()
					.Because("the option only applies to the line rule");
				await That(store.Mismatches[Id]).IsEqualTo("ABBEY ROAD");
			}

			[Fact]
			public async Task LinesContaining_AsWildcard_ShouldMatchThePattern()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Track 1: Help!\nTrack 2: Help! (Live)\nTrack 3: Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("Track ?: *Live*").AsWildcard();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Track 1: Help!\nTrack 3: Let It Be");
			}

			[Fact]
			public async Task LinesContaining_AsRegex_ShouldMatchThePattern()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "09:30 started\nAbbey Road\nat 09:31 Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining(@"^\d{2}:\d{2} ").AsRegex();

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\nat 09:31 Let It Be")
					.Because("the anchor of the pattern applies to each line");
			}

			[Fact]
			public async Task LinesContaining_Options_ShouldOnlyApplyToTheirOwnRule()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Track 1: Help!\nlive at Shea Stadium\nLIVE at Wembley\ntrack 2: Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("TRACK").IgnoringCase()
						.Ignoring.LinesContaining("LIVE");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("live at Shea Stadium")
					.Because("only the first rule ignores the case");
			}

			[Fact]
			public async Task LinesContaining_WhenFollowedByAnotherOption_ShouldNoLongerOfferTheOptionsOfTheRule()
			{
				Type afterLineRule = typeof(SnapshotLinesContainingResult<string?, IThat<string?>>);
				Type afterOtherOption = typeof(SnapshotLinesContainingResult<string?, IThat<string?>>)
					.GetMethod(nameof(SnapshotResult<string?, IThat<string?>>.Named))!.ReturnType;

				await That(typeof(IOptionsProvider<StringEqualityOptions>).IsAssignableFrom(afterLineRule)).IsTrue();
				await That(typeof(IOptionsProvider<StringEqualityOptions>).IsAssignableFrom(afterOtherOption)).IsFalse()
					.Because("the options of the line rule must not be mistaken for options of the snapshot comparison");
			}

			[Fact]
			public async Task LinesContaining_WhenFollowedByOtherOptions_ShouldApplyThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"album {FirstGuid}\nelapsed 3 ms\nalbum {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("ELAPSED").IgnoringCase()
						.Named("tracks")
						.Masking.Guids()
						.AcceptingChanges();

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id with { Name = "tracks", }] = "album {Guid_1}\nalbum {Guid_2}",
				});
			}

			[Fact]
			public async Task LinesContaining_WhenRegexIsInvalid_ShouldThrowArgumentException()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.Lines(_ => true)
						.Ignoring.LinesContaining("(").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithParamName("text").And
					.WithMessage("The 'text' regex pattern is invalid: *").AsWildcard()
					.Because("the pattern is validated before any line is checked, also when another rule removes every line");
			}

			[Fact]
			public async Task LinesContaining_WhenRegexTimesOut_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime timeout =
					Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromMilliseconds(1));
				string subject = new string('a', 40) + "!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining(@"^(\w+\s?)*$").AsRegex();

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the line rule with the pattern "^(\\w+\\s?)*$" did not complete within *. Simplify the pattern to avoid catastrophic backtracking
					             """).AsWildcard();
				await That(exception.InnerException).Is<RegexMatchTimeoutException>()
					.Whose(timeoutException => timeoutException.MatchTimeout,
						matchTimeout => matchTimeout.IsEqualTo(TimeSpan.FromMilliseconds(1)))
					.Because("the regex timeout of the settings applies, like for a masking");
				await That(store.Mismatches).IsEmpty()
					.Because("the lines could not be removed");
			}

			[Fact]
			public async Task LinesContaining_WhenTextIsEmpty_ShouldThrowArgumentException()
			{
				void Act()
					=> SnapshotIgnoring.LinesContaining("");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("text").And
					.WithMessage("The 'text' cannot be empty.*").AsWildcard();
			}

			[Fact]
			public async Task LinesContaining_WhenTextIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining(null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("text");
			}

			[Fact]
			public async Task Lines_WhenPredicateIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Ignoring.Lines(null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("predicate");
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
						.Ignoring.LinesContaining("x");

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
						.Ignoring.Lines(line => line.Length == 0);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road\r\nLet It Be");
			}

			[Fact]
			public async Task WhenMaskingGuids_ShouldNotNumberTheRemovedLines()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime ignorings = Customize.aweXpect.Snapshots().Ignorings.Set([
					SnapshotIgnoring.LinesContaining("session"),
				]);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Guids(),
				]);
				string subject = $"session {Guid.NewGuid()}\nAbbey Road {FirstGuid}\ndebug {Guid.NewGuid()}\nLet It Be {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Guids()
						.Ignoring.LinesContaining("debug");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road {Guid_1}\nLet It Be {Guid_2}")
					.Because("the lines are removed before the maskings of the expectation and the global ones");
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
						.Ignoring.LinesContaining("elapsed");

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
						.Ignoring.LinesContaining("elapsed");

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
						.Ignoring.LinesContaining("elapsed");
					await That("Abbey Road\nelapsed 4 ms").MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("elapsed");
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
						.Ignoring.LinesContaining("(").AsRegex();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenSubjectIsAnObject_LinesRule_ShouldThrowInvalidOperationException()
			{
				RuleNote subject = new()
				{
					Text = "Abbey Road",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Ignoring.Lines(_ => true);

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A line rule only applies to a snapshot of a string, but an object is written by the formatter.*")
					.AsWildcard();
			}

			[Fact]
			public async Task WhenSubjectIsAnObject_LinesContainingRule_ShouldThrowInvalidOperationException()
			{
				RuleNote subject = new()
				{
					Text = "Abbey Road",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("Abbey");

				await That(Act).Throws<InvalidOperationException>()
					.WithMessage("A line rule only applies to a snapshot of a string*").AsWildcard();
			}

			[Fact]
			public async Task WhenGlobalLineRules_ShouldApplyThemTogetherWithTheRulesOfTheExpectation()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime ignorings = Customize.aweXpect.Snapshots().Ignorings.Set([
					SnapshotIgnoring.LinesContaining("BUILD DATE").IgnoringCase(),
					SnapshotIgnoring.Lines(line => line.Length > 0 && line[0] == '#'),
				]);
				string subject = "Build date: 2026-10-17\n# AC/DC\nBack in Black\nelapsed 3 ms\nHighway to Hell";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Ignoring.LinesContaining("elapsed");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Back in Black\nHighway to Hell");
			}

			[Fact]
			public async Task WhenGlobalLineRulesAndTheSubjectIsAnObject_ShouldIgnoreThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime ignorings = Customize.aweXpect.Snapshots().Ignorings.Set([
					SnapshotIgnoring.LinesContaining("Text"),
					SnapshotIgnoring.Lines(_ => true),
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
				using CustomizationLifetime timeout =
					Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromMilliseconds(1));
				using CustomizationLifetime ignorings = Customize.aweXpect.Snapshots().Ignorings.Set([
					SnapshotIgnoring.LinesContaining(@"^(\w+\s?)*$").AsRegex(),
				]);
				string subject = new string('a', 40) + "!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("*the line rule with the pattern \"^(\\\\w+\\\\s?)*$\" did not complete*").AsWildcard();
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
					using CustomizationLifetime ignorings = Customize.aweXpect.Snapshots().Ignorings.Set([
						WithOption(SnapshotIgnoring.LinesContaining(text), option),
					]);
					await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("global");
				}

				async Task ActWithExpectationRule()
					=> await WithOption(That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("expectation")
						.Ignoring.LinesContaining(text), option);

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

			[Fact]
			public async Task LinesContaining_ShouldOfferTheSameOptionsForAllSnapshotsAsForTheExpectation()
			{
				Type[] global = typeof(SnapshotIgnoringLinesContaining).GetInterfaces()
					.OrderBy(type => type.FullName, StringComparer.Ordinal).ToArray();
				Type[] expectation = typeof(SnapshotLinesContainingResult<string?, IThat<string?>>).GetInterfaces()
					.Except(typeof(SnapshotResult<string?, IThat<string?>>).GetInterfaces())
					.OrderBy(type => type.FullName, StringComparer.Ordinal).ToArray();

				await That(expectation).IsEqualTo(global)
					.Because("the global line rule supports the same options as the one of the expectation");
				await That(global).Contains(typeof(IOptionsProvider<StringEqualityOptions>)).And
					.Contains(typeof(IStringPatternMatchTypeOptions));
			}
		}
	}
}
