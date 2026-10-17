using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Customization;
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
#else
			private static Regex TokenRegex() => new("(token)=\\w+", RegexOptions.IgnoreCase);
#endif

			[Fact]
			public async Task WhenAccepting_ShouldAcceptTheMaskedValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking("42", "<id>").AcceptingChanges();

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
						.Masking(@"\d{4}-\d{2}-\d{2}", "<date>").AsRegex()
						.Masking("2026-10-17", "<today>");

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
						.Masking("2026-10-17", "<today>")
						.Masking(@"\d{4}-\d{2}-\d{2}", "<date>").AsRegex();

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
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking("5", "$1");

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
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking("42", "<id>");

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
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking(" (debug)", "");

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
						.Masking(TokenRegex(), "$1=<token>");

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
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking("42", "<id>"));

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
						.Masking("bar", "<bar>")
						.Masking(@"\d+", "<n>").AsRegex();

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WhenPatternIsInvalid_ShouldThrowArgumentException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking("(", "x").AsRegex();

				await That(Act).Throws<ArgumentException>()
					.WithMessage("The 'value' is not a valid regex pattern: *").AsWildcard();
			}

			[Fact]
			public async Task WhenPatternIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking((Regex)null!, "x");

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("pattern");
			}

			[Fact]
			public async Task WhenRegexMaskingTimesOut_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = new string('a', 40) + "!";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking(new Regex(@"^(\w+\s?)*$", RegexOptions.None, TimeSpan.FromMilliseconds(1)), "<x>");

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
						.Masking(@"\d+", "$$<n>").AsRegex();

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
						.Masking(@"(\w+\.local):\d+", "$1:<port>").AsRegex();

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
						.Masking(@"\d+", "<n>").AsRegex()
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
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("replacement");
			}

			[Fact]
			public async Task WhenReplacementOfRegexIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking(TokenRegex(), null!);

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
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking("17", "<id>");

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
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking("17", "<id>");

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
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking("foo", "<foo>");

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
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking("", "x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("value").And
					.WithMessage("The 'value' cannot be empty.*").AsWildcard();
			}

			[Fact]
			public async Task WhenValueIsNull_ShouldThrowArgumentNullException()
			{
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking((string)null!, "x");

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
					await That("id=1").MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking(@"\d+", "<id>").AsRegex();
					await That("id=2").MatchesSnapshot(Id.SourceFile, Id.MemberName).Masking(@"\d+", "<id>").AsRegex();
				}

				await That(Act).DoesNotThrow()
					.Because("the check for a snapshot used for another value compares the masked values");
			}
		}
	}
}
