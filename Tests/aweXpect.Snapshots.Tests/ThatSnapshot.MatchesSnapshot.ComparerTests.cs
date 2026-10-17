using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class ComparerTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task ComparedWith_WhenComparerConsidersEqual_ShouldSucceedAndKeepTheSnapshot()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "ABBEY ROAD";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(IgnoringCase());

				await That(Act).DoesNotThrow();
				await That(store.Matches).IsEqualTo([Id,])
					.Because("an equal result records a match, which deletes a leftover pending snapshot");
				await That(store.Mismatches).IsEmpty();
				await That(store.Snapshots[Id]).IsEqualTo("Abbey Road");
			}

			[Fact]
			public async Task ComparedWith_WhenComparerConsidersEqual_AndAcceptingAll_ShouldNotRewriteTheSnapshot()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "ABBEY ROAD";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(IgnoringCase())
						.Accepting(SnapshotAcceptance.All);

				await That(Act).DoesNotThrow();
				await That(store.Accepted).IsEmpty()
					.Because("an equal result keeps the old snapshot");
				await That(store.Snapshots[Id]).IsEqualTo("Abbey Road");
			}

			[Fact]
			public async Task ComparedWith_WhenComparerReportsDifference_ShouldFailWithTheDescription()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(FakeSnapshotComparer.Different("the tempo changed"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it differed: the tempo changed

					             Actual:
					             Abbey Road

					             Snapshot:
					             Abbey Road
					             """)
					.Because("the description of the comparer replaces the built-in difference");
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = "Abbey Road",
				}).Because("a difference is recorded for review like any other");
			}

			[Fact]
			public async Task ComparedWith_WhenDescriptionHasSeveralLines_ShouldShowThemAll()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";
				string newLine = Environment.NewLine;

				async Task Act()
					=> await That(subject).IsNotEmpty().And.MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(FakeSnapshotComparer.Different(
							$"2 tracks differ:{newLine}- Something{newLine}- Because"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             is not empty and matches the snapshot "MyTests.MyTest",
					             but it differed: 2 tracks differ:
					             - Something
					             - Because

					             Actual:
					             Abbey Road

					             Snapshot:
					             Abbey Road
					             """);
			}

			[Fact]
			public async Task ComparedWith_WhenSnapshotIsMissing_ShouldNotCallTheComparer()
			{
				InMemorySnapshotStore store = new();
				FakeSnapshotComparer comparer = FakeSnapshotComparer.Equal();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(comparer);

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot did not exist*").AsWildcard();
				await That(comparer.Contexts).IsEmpty()
					.Because("there is nothing to compare with");
				await That(store.Mismatches.Keys).IsEqualTo([Id,]);
			}

			[Fact]
			public async Task ComparedWith_ShouldGetTheIdTheSnapshotAndTheSubjectAfterTheRules()
			{
				SnapshotId id = Id with
				{
					Extension = "json",
				};
				InMemorySnapshotStore store = new();
				store.Snapshots[id] = "id {Guid_1}";
				FakeSnapshotComparer comparer = FakeSnapshotComparer.Equal();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "id 2c1b7a3e-4a2f-4d4e-9a63-0e2b1b9d6f11";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("json")
						.For.Guids().Mask()
						.ComparedWith(comparer);

				await That(Act).DoesNotThrow();
				SnapshotComparisonContext context = await That(comparer.Contexts).HasSingle();
				await That(context.Id).IsEqualTo(id);
				await That(context.Snapshot.Text).IsEqualTo("id {Guid_1}");
				await That(context.Snapshot.Bytes).IsSameAs(store.SnapshotBytes[id]);
				await That(context.Actual.Text).IsEqualTo("id {Guid_1}")
					.Because("the rules are applied before the comparer");
				await That(context.Actual.IsText).IsTrue();
			}

			[Fact]
			public async Task ComparedWith_OnAnObject_ShouldCompareTheWrittenText()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "something else";
				FakeSnapshotComparer comparer = FakeSnapshotComparer.Equal();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				int[] subject = [1, 2,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(comparer);

				await That(Act).DoesNotThrow();
				SnapshotComparisonContext context = await That(comparer.Contexts).HasSingle();
				await That(context.Actual.Text).IsEqualTo($"[{Environment.NewLine}  1,{Environment.NewLine}  2{Environment.NewLine}]");
			}

			[Fact]
			public async Task ComparedWith_OnBinaryData_WhenComparerConsidersEqual_ShouldSucceed()
			{
				SnapshotId id = Id with
				{
					Extension = "png",
				};
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[id] = [1, 2, 3,];
				FakeSnapshotComparer comparer = FakeSnapshotComparer.Equal();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [1, 2, 4,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png")
						.ComparedWith(comparer);

				await That(Act).DoesNotThrow();
				SnapshotComparisonContext context = await That(comparer.Contexts).HasSingle();
				await That(context.Actual.IsText).IsFalse();
				await That(context.Actual.Bytes).IsEqualTo([1, 2, 4,]);
				await That(context.Snapshot.IsText).IsFalse();
				await That(context.Snapshot.Bytes).IsEqualTo([1, 2, 3,]);
				await That(store.Matches).IsEqualTo([id,]);
			}

			[Fact]
			public async Task ComparedWith_OnBinaryData_WhenComparerReportsDifference_ShouldFailWithTheDescription()
			{
				SnapshotId id = Id with
				{
					Extension = "png",
				};
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[id] = [1, 2, 3,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				byte[] subject = [1, 2, 3,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("png")
						.ComparedWith(FakeSnapshotComparer.Different("2.3 % of the pixels differ"));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it differed: 2.3 % of the pixels differ
					             """);
				await That(store.MismatchBytes[id]).IsEqualTo([1, 2, 3,]);
			}

			[Fact]
			public async Task ComparedWith_OnAFile_ShouldUseTheComparer()
			{
				using SnapshotDirectory directory = new();
				string file = Path.Combine(directory.Path, "Cover.PNG");
				File.WriteAllBytes(file, [1, 2, 4,]);
				SnapshotId id = Id with
				{
					Extension = "PNG",
				};
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[id] = [1, 2, 3,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.ForExtension("png", FakeSnapshotComparer.Equal()),
				]);

				async Task Act()
					=> await That(new FileInfo(file)).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow()
					.Because("the extension of the file is compared ignoring its case");
			}

			[Fact]
			public async Task ComparedWith_ShouldGoBeforeTheComparersForAllSnapshots()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				FakeSnapshotComparer global = FakeSnapshotComparer.Different("global");
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.When(_ => true, global),
				]);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(FakeSnapshotComparer.Different("expectation"));

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed: expectation*").AsWildcard();
				await That(global.Contexts).IsEmpty();
			}

			[Fact]
			public async Task ComparedWith_WhenSetTwice_ShouldUseTheLastOne()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(FakeSnapshotComparer.Different("first"))
						.ComparedWith(FakeSnapshotComparer.Different("second"));

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed: second*").AsWildcard();
			}

			[Fact]
			public async Task ComparedWith_WhenNull_ShouldThrowArgumentNullException()
			{
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("comparer");
			}

			[Fact]
			public async Task Comparers_ShouldUseTheFirstMatchingComparerInListOrder()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				FakeSnapshotComparer skipped = FakeSnapshotComparer.Different("skipped");
				FakeSnapshotComparer later = FakeSnapshotComparer.Different("later");
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.When(_ => false, skipped),
					SnapshotComparer.When(_ => true, FakeSnapshotComparer.Different("first match")),
					SnapshotComparer.When(_ => true, later),
				]);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed: first match*").AsWildcard();
				await That(skipped.Contexts).IsEmpty();
				await That(later.Contexts).IsEmpty();
			}

			[Fact]
			public async Task Comparers_When_ShouldSeeTheIdTheSnapshotAndTheSubject()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				List<SnapshotComparisonContext> contexts = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.When(context =>
					{
						contexts.Add(context);
						return context.Actual.Bytes.Length > 1_000;
					}, FakeSnapshotComparer.Equal()),
				]);
				string subject = "Let It Be";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*which differs at index 0*").AsWildcard()
					.Because("the predicate did not select the comparer, so the built-in comparison applies");
				SnapshotComparisonContext context = await That(contexts).HasSingle();
				await That(context.Id).IsEqualTo(Id);
				await That(context.Snapshot.Text).IsEqualTo("Abbey Road");
				await That(context.Actual.Text).IsEqualTo("Let It Be");
			}

			[Theory]
			[InlineData("json")]
			[InlineData("JSON")]
			public async Task Comparers_ForExtension_ShouldMatchTheExtensionIgnoringCase(string extension)
			{
				SnapshotId id = Id with
				{
					Extension = "json",
				};
				InMemorySnapshotStore store = new();
				store.Snapshots[id] = "{ \"title\": \"Abbey Road\" }";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.ForExtension("txt", FakeSnapshotComparer.Different("txt")),
					SnapshotComparer.ForExtension(extension, FakeSnapshotComparer.Different("json")),
				]);
				string subject = "{\"title\":\"Abbey Road\"}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("json");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed: json*").AsWildcard();
			}

			[Fact]
			public async Task Comparers_WhenNoneApplies_ShouldKeepTheBuiltInTextFailure()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "foo bar";
				FakeSnapshotComparer comparer = FakeSnapshotComparer.Equal();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.ForExtension("png", comparer),
				]);
				string subject = "foo baz";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it was "foo baz", which differs at index 6:
					                      ↓ (actual)
					               "foo baz"
					               "foo bar"
					                      ↑ (expected)
					             """);
				await That(comparer.Contexts).IsEmpty();
			}

			[Fact]
			public async Task Comparers_WhenNoneApplies_ShouldKeepTheBuiltInBinaryFailure()
			{
				SnapshotId id = Id with
				{
					Extension = "bin",
				};
				InMemorySnapshotStore store = new();
				store.SnapshotBytes[id] = [1, 2, 3, 4,];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.ForExtension("png", FakeSnapshotComparer.Equal()),
				]);
				byte[] subject = [1, 2, 9, 4,];

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("bin");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but it differed at byte 2 (both have 4 bytes)
					             """);
			}

			[Fact]
			public async Task Comparers_ShouldDefaultToNone()
			{
				IReadOnlyList<SnapshotComparerRegistration> comparers = Customize.aweXpect.Snapshots().Comparers.Get();

				await That(comparers).IsEmpty();
			}

			[Fact]
			public async Task Comparers_ShouldCopyTheList()
			{
				List<SnapshotComparerRegistration> comparers =
					[SnapshotComparer.ForExtension("png", FakeSnapshotComparer.Equal()),];
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Comparers.Set(comparers);

				comparers.Add(SnapshotComparer.ForExtension("svg", FakeSnapshotComparer.Equal()));

				await That(Customize.aweXpect.Snapshots().Comparers.Get()).HasCount(1)
					.Because("changing the list afterwards must not change the setting");
			}

			[Fact]
			public async Task Comparers_WhenSetToNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Comparers.Set(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("value");
			}

			[Fact]
			public async Task Comparers_WhenContainingNull_ShouldThrowArgumentException()
			{
				void Act()
					=> Customize.aweXpect.Snapshots().Comparers.Set([null!,]);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("value");
			}

			[Fact]
			public async Task When_WithNullPredicate_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotComparer.When(null!, FakeSnapshotComparer.Equal());

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("predicate");
			}

			[Fact]
			public async Task When_WithNullComparer_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotComparer.When(_ => true, null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("comparer");
			}

			[Theory]
			[InlineData("")]
			[InlineData(".png")]
			[InlineData("pending")]
			public async Task ForExtension_WithInvalidExtension_ShouldThrowArgumentException(string extension)
			{
				void Act()
					=> SnapshotComparer.ForExtension(extension, FakeSnapshotComparer.Equal());

				await That(Act).Throws<ArgumentException>()
					.WithParamName("extension")
					.Because("no snapshot could have the extension, so the comparer would never be used");
			}

			[Fact]
			public async Task ForExtension_WithNullComparer_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotComparer.ForExtension("png", null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("comparer");
			}

			[Fact]
			public async Task Different_WithNullDescription_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotComparison.Different(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("description");
			}

			[Theory]
			[InlineData("")]
			[InlineData(" ")]
			public async Task Different_WithEmptyDescription_ShouldThrowArgumentException(string description)
			{
				void Act()
					=> SnapshotComparison.Different(description);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("description");
			}

			[Fact]
			public async Task SnapshotComparison_ShouldTellEqualAndDifferentApart()
			{
				SnapshotComparison different = SnapshotComparison.Different("louder");

				await That(SnapshotComparison.Equal.IsEqual).IsTrue();
				await That(SnapshotComparison.Equal.Description).IsNull();
				await That(different.IsEqual).IsFalse();
				await That(different.Description).IsEqualTo("louder");
			}

			[Fact]
			public async Task WhenComparerReportsDifference_AndAcceptingAll_ShouldAcceptTheSubject()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road (Remastered)";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(FakeSnapshotComparer.Different("the title changed"))
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>()
					.WithMessage("*but it differed: the title changed*Accepted snapshot:*").AsWildcard();
				await That(store.Accepted[Id]).IsEqualTo("Abbey Road (Remastered)");
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenComparerReportsDifference_AndAcceptingNew_ShouldRecordAMismatch()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road (Remastered)";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(FakeSnapshotComparer.Different("the title changed"))
						.Accepting(SnapshotAcceptance.New);

				await That(Act).Throws<XunitException>();
				await That(store.Accepted).IsEmpty()
					.Because("accepting new snapshots never replaces an existing one");
				await That(store.Mismatches[Id]).IsEqualTo("Abbey Road (Remastered)");
			}

			[Fact]
			public async Task WhenNegated_AndComparerConsidersEqual_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "ABBEY ROAD";

				async Task Act()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(IgnoringCase()));

				await That(Act).Throws<XunitException>()
					.WithMessage("*but a snapshot cannot be negated").AsWildcard();
			}

			[Fact]
			public async Task WhenComparerThrows_ShouldFailBothWaysWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				InvalidOperationException exception = new("broken comparer");
				FakeSnapshotComparer comparer = new(_ => throw exception);
				string subject = "Abbey Road (Remastered)";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(comparer)
						.Accepting(SnapshotAcceptance.All);

				async Task ActNegated()
					=> await That(subject).DoesNotComplyWith(it
						=> it.MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(comparer));

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot could not be compared: broken comparer
					             """)
					.And.WithInner<InvalidOperationException>();
				await That(ActNegated).Throws<XunitException>()
					.WithMessage("*but a snapshot cannot be negated").AsWildcard();
				await That(store.Mismatches).IsEmpty()
					.Because("a broken comparer says nothing about the subject");
				await That(store.Accepted).IsEmpty();
				await That(store.Snapshots[Id]).IsEqualTo("Abbey Road");
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.When(_ => throw new FormatException("broken predicate"),
						FakeSnapshotComparer.Equal()),
				]);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot could not be compared: broken predicate").AsWildcard()
					.And.WithInner<FormatException>();
				await That(store.Matches).IsEmpty();
			}

			[Fact]
			public async Task WhenComparerReturnsNull_ShouldFailWithAnInvalidOperationException()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "Abbey Road";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(new FakeSnapshotComparer(_ => null!));

				await That(Act).Throws<XunitException>()
					.WithMessage(
						"*but the snapshot could not be compared: The comparer FakeSnapshotComparer returned null.")
					.AsWildcard()
					.And.WithInner<InvalidOperationException>()
					.Because("falling back to the built-in comparison would hide the broken comparer");
			}

			[Fact]
			public async Task WhenSnapshotIsUsedAgain_AndComparerConsidersTheValuesEqual_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				FakeSnapshotComparer comparer = IgnoringCase();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act(string subject)
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(comparer);

				await That(() => Act("abbey road")).DoesNotThrow();
				await That(() => Act("ABBEY ROAD")).DoesNotThrow()
					.Because("two slightly different values of the same test are no conflict for the comparer");
				await That(comparer.Contexts.Select(context => $"{context.Snapshot.Text} ~ {context.Actual.Text}"))
					.IsEqualTo(["Abbey Road ~ abbey road", "abbey road ~ ABBEY ROAD", "Abbey Road ~ ABBEY ROAD",])
					.Because("a reused snapshot is first compared with the first value of the test run");
			}

			[Fact]
			public async Task WhenSnapshotIsUsedAgain_AndComparerConsidersTheValuesDifferent_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				FakeSnapshotComparer comparer = IgnoringCase();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act(string subject)
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(comparer);

				await That(() => Act("Abbey Road")).DoesNotThrow();
				await That(() => Act("Let It Be")).Throws<XunitException>()
					.WithMessage(
						"*but the snapshot was already used for another value in this test run, so name each one with .Named(…) or .NamedAfter(…)")
					.AsWildcard();
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenSnapshotIsUsedAgain_AndComparerForAllSnapshotsConsidersTheValuesEqual_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime __ = Customize.aweXpect.Snapshots().Comparers.Set([
					SnapshotComparer.When(_ => true, IgnoringCase()),
				]);

				async Task Act(string subject)
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(() => Act("abbey road")).DoesNotThrow();
				await That(() => Act("ABBEY ROAD")).DoesNotThrow();
			}

			[Fact]
			public async Task WhenSnapshotIsUsedAgain_AndNoComparerWasSetForTheFirstValue_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That("Abbey Road").MatchesSnapshot(Id.SourceFile, Id.MemberName);

				async Task Act()
					=> await That("ABBEY ROAD").MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.ComparedWith(IgnoringCase());

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot was already used for another value in this test run*").AsWildcard()
					.Because("the first value is only kept while a comparer is set");
			}

			[Fact]
			public async Task WhenSnapshotIsUsedAgain_AndComparerThrows_ShouldFailWithTheExceptionAsCause()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				int calls = 0;
				FakeSnapshotComparer comparer = new(_ => Interlocked.Increment(ref calls) == 1
					? SnapshotComparison.Equal
					: throw new InvalidOperationException("broken comparer"));
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act(string subject)
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(comparer);

				await That(() => Act("Abbey Road")).DoesNotThrow();
				await That(() => Act("ABBEY ROAD")).Throws<XunitException>()
					.WithMessage("*but the snapshot could not be compared: broken comparer").AsWildcard()
					.And.WithInner<InvalidOperationException>();
				await That(store.Mismatches).IsEmpty();
			}

			[Fact]
			public async Task WhenSnapshotIsUsedConcurrently_ShouldCompareEachValueWithTheSameFirstValue()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "Abbey Road";
				FakeSnapshotComparer comparer = IgnoringCase();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string[] subjects = Enumerable.Range(0, 20)
					.Select(index => index % 2 == 0 ? "abbey road" : "ABBEY ROAD").ToArray();

				async Task Act()
					=> await Task.WhenAll(subjects.Select(async subject
						=> await Task.Run(async () => await That(subject)
							.MatchesSnapshot(Id.SourceFile, Id.MemberName).ComparedWith(comparer))));

				await That(Act).DoesNotThrow();
				await That(comparer.Contexts
						.Where(context => context.Snapshot.Text != "Abbey Road")
						.Select(context => context.Snapshot.Text).Distinct())
					.HasSingle()
					.Because("every reused value is compared with the one first value of the test run");
			}

			[Fact]
			public async Task WithDiskStore_WhenComparerConsidersEqual_ShouldKeepTheSnapshotAndDeleteThePendingFile()
			{
				using SnapshotDirectory directory = new();
				directory.WriteSnapshot("Abbey Road");
				directory.WritePending("ABBEY ROAD!");
				string subject = "ABBEY ROAD";

				async Task Act()
					=> await That(subject).MatchesSnapshot(directory.TestFilePath, SnapshotDirectory.MemberName)
						.ComparedWith(IgnoringCase())
						.Accepting(SnapshotAcceptance.All);

				await That(Act).DoesNotThrow();
				await That(File.ReadAllText(directory.SnapshotPath)).IsEqualTo("Abbey Road")
					.Because("an equal result keeps the old snapshot");
				await That(File.Exists(directory.PendingPath)).IsFalse()
					.Because("a leftover pending file is deleted as on a match");
			}

			private static FakeSnapshotComparer IgnoringCase()
				=> new(context => string.Equals(context.Snapshot.Text, context.Actual.Text,
					StringComparison.OrdinalIgnoreCase)
					? SnapshotComparison.Equal
					: SnapshotComparison.Different("the text differs in more than its case"));
		}
	}
}
