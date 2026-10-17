using System.Collections.Generic;
using System.Text.RegularExpressions;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed partial class GlobalMaskingTests
		{
			private const string FirstGuid = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
			private const string SecondGuid = "7c9e6679-7425-40de-944b-e07fc1f90ae7";

			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

#if NET8_0_OR_GREATER
			[GeneratedRegex("(token)=\\w+", RegexOptions.IgnoreCase)]
			private static partial Regex TokenRegex();
#else
			private static Regex TokenRegex() => new("(token)=\\w+", RegexOptions.IgnoreCase);
#endif

			[Fact]
			public async Task ShouldBeEmptyByDefault()
			{
				IReadOnlyList<SnapshotMasking> maskings = Customize.aweXpect.Snapshots().Maskings.Get();

				await That(maskings).IsEmpty();
			}

			[Fact]
			public async Task WhenExtendingAnOuterSetting_ShouldApplyBoth()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime outer = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Guids(),
				]);
				using CustomizationLifetime inner = Customize.aweXpect.Snapshots().Maskings.Set([
					.. Customize.aweXpect.Snapshots().Maskings.Get(),
					SnapshotMasking.Text("Bob", "<name>"),
				]);
				string subject = $"Bob {FirstGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<name> {Guid_1}");
			}

			[Fact]
			public async Task WhenGlobalMaskingIsSet_ShouldApplyIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Guids(),
					SnapshotMasking.Dates(),
				]);
				string subject = $"{FirstGuid} 2026-10-17 {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("{Guid_1} {Date_1} {Guid_2}");
			}

			[Fact]
			public async Task WhenGlobalMaskingScopeHasEnded_ShouldNotApplyIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"id={FirstGuid}";

				using (Customize.aweXpect.Snapshots().Maskings.Set([SnapshotMasking.Guids(),]))
				{
					await That(Customize.aweXpect.Snapshots().Maskings.Get()).HasCount(1);
				}

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject)
					.Because("the global maskings only apply while they are set");
			}

			[Fact]
			public async Task WhenGlobalRegexMaskingUsesASubstitution_ShouldApplyIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Regex(TokenRegex(), "$1=<token>"),
				]);
				string subject = "Token=abc, TOKEN=def";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Token=<token>, TOKEN=<token>");
			}

			[Fact]
			public async Task WhenGlobalTextMasking_ShouldInsertTheReplacementLiterally()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Text("42", "$1"),
				]);
				string subject = "id=42";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("id=$1");
			}

			[Fact]
			public async Task WhenLocalAndGlobalMaskings_ShouldApplyTheLocalOnesFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Guids(),
				]);
				string subject = $"order {FirstGuid}, customer {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Masking.Text(FirstGuid, "<order-id>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("order <order-id>, customer {Guid_1}")
					.Because("a specific masking of the expectation goes before a general one for all snapshots");
			}

			[Fact]
			public async Task WhenSeveralGlobalMaskings_ShouldApplyThemInTheirOrder()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Text("foo", "bar"),
					SnapshotMasking.Text("bar", "baz"),
				]);
				string subject = "foo";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("baz");
			}

			[Fact]
			public async Task WhenSetToListWithNull_ShouldThrowArgumentException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Maskings.Set([null!,]);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("value").And
					.WithMessage("The 'value' cannot contain null.*").AsWildcard();
			}

			[Fact]
			public async Task WhenSetToNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Maskings.Set(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("value");
			}

			[Fact]
			public async Task WhenNegated_AndSubjectIsNull_ShouldStillFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime maskings = Customize.aweXpect.Snapshots().Maskings.Set([
					SnapshotMasking.Guids(),
				]);
				string? subject = null;

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it => it.MatchesSnapshot(Id.SourceFile, Id.MemberName));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             does not match the snapshot "MyTests.MyTest",
					             but it was <null>
					             """);
			}

			[Fact]
			public async Task WhenRegexPatternIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotMasking.Regex((Regex)null!, "x");

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("pattern");
			}

			[Fact]
			public async Task WhenRegexPatternStringIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotMasking.Regex((string)null!, "x");

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("pattern");
			}

			[Fact]
			public async Task WhenRegexPatternStringIsInvalid_ShouldThrowArgumentException()
			{
				void Act()
					=> SnapshotMasking.Regex("(", "x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("pattern").And
					.WithMessage("The 'pattern' is not a valid regex pattern: *").AsWildcard();
			}

			[Fact]
			public async Task WhenRegexReplacementIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotMasking.Regex(TokenRegex(), null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("replacement");
			}

			[Fact]
			public async Task WhenTextReplacementIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotMasking.Text("foo", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("replacement");
			}

			[Fact]
			public async Task WhenTextValueIsEmpty_ShouldThrowArgumentException()
			{
				void Act()
					=> SnapshotMasking.Text("", "x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("value").And
					.WithMessage("The 'value' cannot be empty.*").AsWildcard();
			}

			[Fact]
			public async Task WhenTextValueIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotMasking.Text(null!, "x");

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("value");
			}
		}
	}
}
