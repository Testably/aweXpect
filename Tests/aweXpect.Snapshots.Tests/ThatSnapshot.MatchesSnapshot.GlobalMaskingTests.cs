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
				IReadOnlyList<SnapshotRule> rules = Customize.aweXpect.Snapshots().Rules.Get();

				await That(rules).IsEmpty();
			}

			[Fact]
			public async Task WhenTheListIsChangedAfterSetting_ShouldKeepTheRulesOfTheSetting()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				List<SnapshotRule> rules = [SnapshotRule.For.Text("Bob").Mask("<name>"),];
				using CustomizationLifetime setting = Customize.aweXpect.Snapshots().Rules.Set(rules);
				rules.Add(SnapshotRule.For.Text("Alice").Mask("<name>"));
				string subject = "Bob Alice";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("<name> Alice")
					.Because("the setting keeps a copy of the list, so a later change of the list does not affect it");
			}

			[Fact]
			public async Task WhenExtendingAnOuterSetting_ShouldApplyBoth()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime outer = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Guids().Mask(),
				]);
				using CustomizationLifetime inner = Customize.aweXpect.Snapshots().Rules.Set([
					.. Customize.aweXpect.Snapshots().Rules.Get(),
					SnapshotRule.For.Text("Bob").Mask("<name>"),
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
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Guids().Mask(),
					SnapshotRule.For.Dates().Mask(),
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

				using (Customize.aweXpect.Snapshots().Rules.Set([SnapshotRule.For.Guids().Mask(),]))
				{
					await That(Customize.aweXpect.Snapshots().Rules.Get()).HasCount(1);
				}

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject)
					.Because("the global rules only apply while they are set");
			}

			[Fact]
			public async Task WhenGlobalRegexMaskingUsesASubstitution_ShouldApplyIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Regex(TokenRegex()).Mask("$1=<token>"),
				]);
				string subject = "Token=abc, TOKEN=def";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("Token=<token>, TOKEN=<token>");
			}

			[Fact]
			public async Task WhenGlobalTextMaskingAsRegex_ShouldApplyIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Text(@"localhost:(\d+)").AsRegex().Mask("localhost:<port>"),
				]);
				string subject = "http://localhost:5001/albums";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("http://localhost:<port>/albums");
			}

			[Fact]
			public async Task WhenGlobalTextMasking_ShouldInsertTheReplacementLiterally()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Text("42").Mask("$1"),
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
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Guids().Mask(),
				]);
				string subject = $"order {FirstGuid}, customer {SecondGuid}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(FirstGuid).Mask("<order-id>");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches[Id]).IsEqualTo("order <order-id>, customer {Guid_1}")
					.Because("a specific rule of the expectation goes before a general one for all snapshots");
			}

			[Fact]
			public async Task WhenSeveralGlobalMaskings_ShouldApplyThemInTheirOrder()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Text("foo").Mask("bar"),
					SnapshotRule.For.Text("bar").Mask("baz"),
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
					=> Customize.aweXpect.Snapshots().Rules.Set([null!,]);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("value").And
					.WithMessage("The 'value' cannot contain null.*").AsWildcard();
			}

			[Fact]
			public async Task WhenSetToNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Rules.Set(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("value");
			}

			[Fact]
			public async Task WhenNegated_AndSubjectIsNull_ShouldStillFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Guids().Mask(),
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
					=> SnapshotRule.For.Regex(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("pattern");
			}

			[Fact]
			public async Task WhenTextAsRegexIsInvalid_ShouldThrowArgumentException()
			{
				void Act()
					=> SnapshotRule.For.Text("(").AsRegex().Mask("x");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("text").And
					.WithMessage("The 'text' is not a valid regex pattern: *").AsWildcard();
			}

			[Fact]
			public async Task WhenRegexTemplateIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotRule.For.Regex(TokenRegex()).Mask(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("template");
			}

			[Fact]
			public async Task WhenTextTemplateIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotRule.For.Text("foo").Mask(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("template");
			}

			[Fact]
			public async Task WhenGuidsTemplateIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotRule.For.Guids().Mask(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("template");
			}

			[Fact]
			public async Task WhenTextIsEmpty_ShouldThrowArgumentException()
			{
				void Act()
					=> SnapshotRule.For.Text("");

				await That(Act).Throws<ArgumentException>()
					.WithParamName("text").And
					.WithMessage("The 'text' cannot be empty.*").AsWildcard();
			}

			[Fact]
			public async Task WhenTextIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotRule.For.Text(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("text");
			}
		}
	}
}
