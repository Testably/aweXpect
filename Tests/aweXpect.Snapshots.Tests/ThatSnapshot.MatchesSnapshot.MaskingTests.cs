using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using aweXpect.Core;
using aweXpect.Customization;
using aweXpect.Results;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed partial class MaskingTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

#if NET8_0_OR_GREATER
			[GeneratedRegex("(token)=\\w+", RegexOptions.IgnoreCase)]
			private static partial Regex TokenRegex();

			[GeneratedRegex(@"^(\w+\s?)*$", RegexOptions.None, 1)]
			private static partial Regex BacktrackingRegex();
#else
			private static Regex TokenRegex() => new("(token)=\\w+", RegexOptions.IgnoreCase);

			private static Regex BacktrackingRegex() => new(@"^(\w+\s?)*$", RegexOptions.None, TimeSpan.FromMilliseconds(1));
#endif

			[Test]
			public async Task WhenAccepting_ShouldAcceptTheMaskedValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("42").Mask("<id>").Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<FailException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "id=<id>",
				});
			}

			[Test]
			public async Task WhenMaskingIsDeclaredAfterAGeneralOne_ShouldNotApply()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "on 2026-10-17 and 2026-10-18";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"\d{4}-\d{2}-\d{2}").AsRegex().Mask("<date>")
						.For.Text("2026-10-17").Mask("<today>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("on <date> and <date>")
					.Because("the general masking already replaced the specific value");
			}

			[Test]
			public async Task WhenMaskingIsDeclaredBeforeAGeneralOne_ShouldApplyFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "on 2026-10-17 and 2026-10-18";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("2026-10-17").Mask("<today>")
						.For.Text(@"\d{4}-\d{2}-\d{2}").AsRegex().Mask("<date>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("on <today> and <date>");
			}

			[Test]
			public async Task WhenMaskingLiteralValue_ShouldInsertTheReplacementLiterally()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "price 5";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("5").Mask("$1");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("price $1");
			}

			[Test]
			public async Task WhenMaskingLiteralValue_ShouldNotInterpretItAsAPattern()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "a.b axb";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("a.b").Mask("<x>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("<x> axb")
					.Because("a text is compared literally unless it is followed by '.AsRegex()'");
			}

			[Test]
			public async Task WhenMaskingLiteralValue_ShouldCompareTheCase()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road and ABBEY ROAD";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Abbey Road").Mask("<album>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("<album> and ABBEY ROAD")
					.Because("a text is compared ordinally by default");
			}

			[Test]
			public async Task WhenMaskingLiteralValue_IgnoringCase_ShouldMaskEveryCase()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road and ABBEY ROAD (abbey road)";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("abbey road").IgnoringCase().Mask("<album>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("<album> and <album> (<album>)");
			}

			[Test]
			public async Task WhenMaskingLiteralValue_IgnoringCase_ShouldStillInsertTheReplacementLiterally()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "price: 5 EUR";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("eur").IgnoringCase().Mask("$1");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("price: 5 $1")
					.Because("only a regex pattern uses the substitutions of Regex.Replace");
			}

			[Test]
			public async Task WhenMaskingLiteralValue_ShouldReplaceEveryOccurrence()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42, ref=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("42").Mask("<id>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("id=<id>, ref=<id>");
			}

			[Test]
			public async Task WhenMaskingWithEmptyReplacement_ShouldRemoveTheValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo (debug)";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text(" (debug)").Mask("");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("foo");
			}

			[Test]
			public async Task WhenMaskingAsWildcard_ShouldMatchThePattern()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Track 1: Help!\nTrack 2: Let It Be\nAbbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Track ?:*").AsWildcard().Mask("<track>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("<track>\n<track>\nAbbey Road")
					.Because("'*' does not match a line break, so it cannot mask the rest of the snapshot");
			}

			[Test]
			public async Task WhenMaskingAsWildcard_ShouldKeepTheLineBreaksOfEveryNewlineStyle()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Track 1: Help!\r\nTrack 2: Let It Be\rAbbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Track ?:*").AsWildcard().Mask("<track>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("<track>\r\n<track>\rAbbey Road")
					.Because("a wildcard matches within a line, which also ends with '\\r'");
			}

			[Test]
			public async Task WhenMaskingAsWildcard_ShouldInsertTheReplacementLiterally()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("id=*").AsWildcard().Mask("$0");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("$0");
			}

			[Test]
			public async Task WhenMaskingAsWildcard_IgnoringCase_ShouldMatchEveryCase()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "TRACK 1, track 2";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("Track ?").AsWildcard().IgnoringCase().Mask("<track>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("<track>, <track>");
			}

			[Test]
			public async Task WhenMaskingWithRegex_ShouldUseItsOptions()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Token=abc, TOKEN=def";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Regex(TokenRegex()).Mask("$1=<token>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("Token=<token>, TOKEN=<token>");
			}

			[Test]
			public async Task WhenMaskingAsRegex_IgnoringCase_ShouldIgnoreTheCase()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Token=abc, TOKEN=def";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"(token)=\w+").AsRegex().IgnoringCase().Mask("$1=<token>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("Token=<token>, TOKEN=<token>");
			}

			[Test]
			public async Task WhenNegated_ShouldStillFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id=<id>";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("42").Mask("<id>"));

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Test]
			public async Task WhenNoMaskingMatches_ShouldKeepTheValue()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("bar").Mask("<bar>")
						.For.Text(@"\d+").AsRegex().Mask("<n>");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenPatternIsInvalid_ShouldThrowArgumentException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("(").AsRegex().Mask("x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("text").And
					.WithMessage("The 'text' is not a valid regex pattern: *").AsWildcard()
					.Because("an invalid pattern is reported where the rule is written");
			}

			[Test]
			public async Task WhenTextWithRegexCharactersIsNotARegex_ShouldNotThrow()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "f(oo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("*(").AsWildcard().Mask("y")
						.For.Text("(").IgnoringCase().Mask("x");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("yoo")
					.Because("a literal text and a wildcard pattern are escaped, so they are always valid");
			}

			[Test]
			public async Task WhenTextIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("").Mask("x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("text").And
					.WithMessage("The 'text' cannot be empty.*").AsWildcard();
			}

			[Test]
			public async Task WhenTextIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text(null!).Mask("x");

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("text");
			}

			[Test]
			public async Task WhenNamedBeforeMasking_ShouldApplyBoth()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Named("before").For.Text("42").Mask("<id>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id with { Name = "before", }]).IsEqualTo("id=<id>");
			}

			[Test]
			public async Task WhenNamedAfterMasking_ShouldApplyBoth()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("42").Mask("<id>").Named("after");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id with { Name = "after", }]).IsEqualTo("id=<id>");
			}

			[Test]
			public async Task WhenPatternIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Regex(null!).Mask("x");

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("pattern");
			}

			[Test]
			public async Task WhenPatternIsAString_ShouldUseTheDefaultRegexTimeoutOfTheSettings()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime timeout =
					Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromMilliseconds(1));
				string subject = new string('a', 40) + "!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"^(\w+\s?)*$").AsRegex().Mask("<x>");

				FailException exception = await That(Act).Throws<FailException>();
				await That(exception.InnerException).Is<RegexMatchTimeoutException>()
					.Whose(timeoutException => timeoutException.MatchTimeout,
						matchTimeout => matchTimeout.IsEqualTo(TimeSpan.FromMilliseconds(1)))
					.Because("the setting is read when the masking is applied, so a scoped value applies");
			}

			[Test]
			public async Task WhenMaskingBuiltIn_ShouldUseTheDefaultRegexTimeoutOfTheSettings()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime timeout =
					Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromMilliseconds(1));
				string subject = string.Concat(Enumerable.Repeat("0123456789abcdef ", 200_000));

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();

				FailException exception = await That(Act).Throws<FailException>();
				await That(exception.InnerException).Is<RegexMatchTimeoutException>()
					.Whose(timeoutException => timeoutException.MatchTimeout,
						matchTimeout => matchTimeout.IsEqualTo(TimeSpan.FromMilliseconds(1)))
					.Because("the built-in maskings use the regex timeout of the settings as well");
			}

			[Test]
			public async Task WhenPatternIsARegex_ShouldKeepItsOwnTimeout()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime timeout =
					Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromHours(1));
				string subject = new string('a', 40) + "!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Regex(BacktrackingRegex()).Mask("<x>");

				FailException exception = await That(Act).Throws<FailException>();
				await That(exception.InnerException).Is<RegexMatchTimeoutException>()
					.Whose(timeoutException => timeoutException.MatchTimeout,
						matchTimeout => matchTimeout.IsEqualTo(TimeSpan.FromMilliseconds(1)))
					.Because("a regex of the caller comes with its own timeout");
			}

			[Test]
			public async Task WhenRegexMaskingTimesOut_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = new string('a', 40) + "!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Regex(BacktrackingRegex()).Mask("<x>");

				FailException exception = await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the masking with the regex "^(\\w+\\s?)*$" did not complete within *. Simplify the pattern to avoid catastrophic backtracking
					             """).AsWildcard();
				await That(exception.InnerException).Is<RegexMatchTimeoutException>();
				await That(store.Mismatches).IsEmpty()
					.Because("the subject could not be masked");
			}

			[Test]
			public async Task WhenRegexMaskingUsesDoubleDollar_ShouldInsertADollar()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "price 5";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"\d+").AsRegex().Mask("$$<n>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("price $<n>");
			}

			[Test]
			public async Task WhenRegexMaskingUsesGroup_ShouldSubstituteIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "api.local:5001 and web.local:5002";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"(\w+\.local):\d+").AsRegex().Mask("$1:<port>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("api.local:<port> and web.local:<port>");
			}

			[Test]
			public async Task WhenRegexMaskingIsFollowedByOptions_ShouldApplyThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"\d+").AsRegex().Mask("<n>")
						.Named("first")
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<FailException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id with { Name = "first", }] = "id=<n>",
				});
			}

			[Test]
			public async Task WhenTemplateIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("foo").Mask(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("template");
			}

			[Test]
			public async Task WhenTemplateOfRegexIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Regex(TokenRegex()).Mask(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("template");
			}

			[Test]
			public async Task WhenSnapshotContainsThePlaceholder_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id=<id>";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=17";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("17").Mask("<id>");

				await That(Act).DoesNotThrow();
			}

			[Test]
			public async Task WhenSnapshotDiffers_ShouldShowTheMaskedValue()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id=<id>, name=foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=17, name=bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("17").Mask("<id>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was "id=<id>, name=bar", which differs at index 14:
					                      ↓ (actual)
					               "…name=bar"
					               "…name=foo"
					                      ↑ (expected)

					             Snapshot:
					             id=<id>, name=foo
					             """)
					.Because("the masked value is what was compared and what would be stored");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFailWithoutMasking()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text("foo").Mask("<foo>");

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
			}

			[Test]
			public async Task WhenSubjectIsAnObject_ShouldMaskTheWrittenText()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				RuleNote subject = new()
				{
					Text = "Abbey Road",
				};

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("\"Abbey Road\"").Mask("<album>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("""
				                                           RuleNote {
				                                             Text = <album>
				                                           }
				                                           """).IgnoringNewlineStyle()
					.Because("a text rule applies to the written text of every subject");
			}

			[Test]
			public async Task WhenValuesDifferOnlyInAMaskedPart_ShouldNotBeAConflict()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id=<id>";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
				{
					await That("id=1").MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text(@"\d+").AsRegex().Mask("<id>");
					await That("id=2").MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Text(@"\d+").AsRegex().Mask("<id>");
				}

				await That(Act).DoesNotThrow()
					.Because("the check for a snapshot used for another value compares the masked values");
			}
		}
	}
}
