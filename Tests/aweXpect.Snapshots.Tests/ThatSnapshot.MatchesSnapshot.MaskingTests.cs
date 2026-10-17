using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

			[Fact]
			public async Task WhenAccepting_ShouldAcceptTheMaskedValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Text("42", "<id>").AcceptingChanges();

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "id=<id>",
				});
			}

			[Fact]
			public async Task WhenMaskingIsDeclaredAfterAGeneralOne_ShouldNotApply()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "on 2026-10-17 and 2026-10-18";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(@"\d{4}-\d{2}-\d{2}", "<date>")
						.Masking.Text("2026-10-17", "<today>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("on <date> and <date>")
					.Because("the general masking already replaced the specific value");
			}

			[Fact]
			public async Task WhenMaskingIsDeclaredBeforeAGeneralOne_ShouldApplyFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "on 2026-10-17 and 2026-10-18";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Text("2026-10-17", "<today>")
						.Masking.Regex(@"\d{4}-\d{2}-\d{2}", "<date>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("on <today> and <date>");
			}

			[Fact]
			public async Task WhenMaskingLiteralValue_ShouldInsertTheReplacementLiterally()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "price 5";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text("5", "$1");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("price $1");
			}

			[Fact]
			public async Task WhenMaskingLiteralValue_ShouldReplaceEveryOccurrence()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42, ref=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text("42", "<id>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("id=<id>, ref=<id>");
			}

			[Fact]
			public async Task WhenMaskingWithEmptyReplacement_ShouldRemoveTheValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo (debug)";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text(" (debug)", "");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("foo");
			}

			[Fact]
			public async Task WhenMaskingWithRegex_ShouldUseItsOptions()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Token=abc, TOKEN=def";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(TokenRegex(), "$1=<token>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Token=<token>, TOKEN=<token>");
			}

			[Fact]
			public async Task WhenNegated_ShouldStillFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id=<id>";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text("42", "<id>"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but a snapshot cannot be negated
					             """);
			}

			[Fact]
			public async Task WhenNoMaskingMatches_ShouldKeepTheValue()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Text("bar", "<bar>")
						.Masking.Regex(@"\d+", "<n>");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPatternIsInvalid_ShouldThrowArgumentException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Regex("(", "x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("pattern").And
					.WithMessage("The 'pattern' is not a valid regex pattern: *").AsWildcard()
					.Because("an invalid pattern is reported where it is written");
			}

			[Fact]
			public async Task WhenPatternStringIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Regex("", "x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("pattern").And
					.WithMessage("The 'pattern' cannot be empty.*").AsWildcard();
			}

			[Fact]
			public async Task WhenPatternStringIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Regex((string)null!, "x");

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("pattern");
			}

			[Fact]
			public async Task WhenNamedBeforeMasking_ShouldApplyBoth()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Named("before").Masking.Text("42", "<id>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id with { Name = "before", }]).IsEqualTo("id=<id>");
			}

			[Fact]
			public async Task WhenNamedAfterMasking_ShouldApplyBoth()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Text("42", "<id>").Named("after");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id with { Name = "after", }]).IsEqualTo("id=<id>");
			}

			[Fact]
			public async Task MaskingProperty_ShouldOfferEveryFactoryOfSnapshotMasking()
			{
				string[] factories = typeof(SnapshotMasking)
					.GetMethods(BindingFlags.Public | BindingFlags.Static)
					.Select(GetSignature)
					.OrderBy(signature => signature, StringComparer.Ordinal)
					.ToArray();
				string[] maskings = typeof(SnapshotMaskings<string, IThat<string>>)
					.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
					.Select(GetSignature)
					.OrderBy(signature => signature, StringComparer.Ordinal)
					.ToArray();

				await That(maskings).IsEqualTo(factories)
					.Because("a masking of the expectation has the same name and parameters as for all snapshots");
			}

			private static string GetSignature(MethodInfo method)
				=> $"{method.Name}({string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.FullName} {p.Name}"))})";

			[Fact]
			public async Task WhenPatternIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Regex((Regex)null!, "x");

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("pattern");
			}

			[Fact]
			public async Task WhenPatternIsAString_ShouldUseTheDefaultRegexTimeoutOfTheSettings()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime timeout =
					Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromMilliseconds(1));
				string subject = new string('a', 40) + "!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(@"^(\w+\s?)*$", "<x>");

				XunitException exception = await That(Act).Throws<XunitException>();
				await That(exception.InnerException).Is<RegexMatchTimeoutException>()
					.Whose(timeoutException => timeoutException.MatchTimeout,
						matchTimeout => matchTimeout.IsEqualTo(TimeSpan.FromMilliseconds(1)))
					.Because("the setting is read when the masking is applied, so a scoped value applies");
			}

			[Fact]
			public async Task WhenMaskingBuiltIn_ShouldUseTheDefaultRegexTimeoutOfTheSettings()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime timeout =
					Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromMilliseconds(1));
				string subject = string.Concat(Enumerable.Repeat("0123456789abcdef ", 200_000));

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Guids();

				XunitException exception = await That(Act).Throws<XunitException>();
				await That(exception.InnerException).Is<RegexMatchTimeoutException>()
					.Whose(timeoutException => timeoutException.MatchTimeout,
						matchTimeout => matchTimeout.IsEqualTo(TimeSpan.FromMilliseconds(1)))
					.Because("the built-in maskings use the regex timeout of the settings as well");
			}

			[Fact]
			public async Task WhenPatternIsARegex_ShouldKeepItsOwnTimeout()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime timeout =
					Customize.aweXpect.Settings().DefaultRegexTimeout.Set(TimeSpan.FromHours(1));
				string subject = new string('a', 40) + "!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(BacktrackingRegex(), "<x>");

				XunitException exception = await That(Act).Throws<XunitException>();
				await That(exception.InnerException).Is<RegexMatchTimeoutException>()
					.Whose(timeoutException => timeoutException.MatchTimeout,
						matchTimeout => matchTimeout.IsEqualTo(TimeSpan.FromMilliseconds(1)))
					.Because("a regex of the caller comes with its own timeout");
			}

			[Fact]
			public async Task WhenRegexMaskingTimesOut_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = new string('a', 40) + "!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(BacktrackingRegex(), "<x>");

				XunitException exception = await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the masking with the regex "^(\\w+\\s?)*$" did not complete within *. Simplify the pattern to avoid catastrophic backtracking
					             """).AsWildcard();
				await That(exception.InnerException).Is<RegexMatchTimeoutException>();
				await That(store.Mismatches).IsEmpty()
					.Because("the subject could not be masked");
			}

			[Fact]
			public async Task WhenRegexMaskingUsesDoubleDollar_ShouldInsertADollar()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "price 5";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(@"\d+", "$$<n>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("price $<n>");
			}

			[Fact]
			public async Task WhenRegexMaskingUsesGroup_ShouldSubstituteIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "api.local:5001 and web.local:5002";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(@"(\w+\.local):\d+", "$1:<port>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("api.local:<port> and web.local:<port>");
			}

			[Fact]
			public async Task WhenRegexMaskingIsFollowedByOptions_ShouldApplyThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(@"\d+", "<n>")
						.Named("first")
						.AcceptingChanges();

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id with { Name = "first", }] = "id=<n>",
				});
			}

			[Fact]
			public async Task WhenReplacementIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("replacement");
			}

			[Fact]
			public async Task WhenReplacementOfRegexIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Regex(TokenRegex(), null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("replacement");
			}

			[Fact]
			public async Task WhenSnapshotContainsThePlaceholder_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id=<id>";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=17";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text("17", "<id>");

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSnapshotDiffers_ShouldShowTheMaskedValue()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id=<id>, name=foo";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=17, name=bar";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text("17", "<id>");

				await That(Act).Throws<XunitException>()
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

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFailWithoutMasking()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string? subject = null;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text("foo", "<foo>");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenValueIsEmpty_ShouldThrowArgumentException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text("", "x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("value").And
					.WithMessage("The 'value' cannot be empty.*").AsWildcard();
			}

			[Fact]
			public async Task WhenValueIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Text(null!, "x");

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("value");
			}

			[Fact]
			public async Task WhenValuesDifferOnlyInAMaskedPart_ShouldNotBeAConflict()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "id=<id>";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
				{
					await That("id=1").MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Regex(@"\d+", "<id>");
					await That("id=2").MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking.Regex(@"\d+", "<id>");
				}

				await That(Act).DoesNotThrow()
					.Because("the check for a snapshot used for another value compares the masked values");
			}
		}
	}
}
