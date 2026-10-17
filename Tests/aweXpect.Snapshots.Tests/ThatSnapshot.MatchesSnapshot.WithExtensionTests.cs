using System.Collections.Generic;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class WithExtensionTests
		{
			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Fact]
			public async Task WithoutExtension_ShouldPassTxtToTheStore()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That("Abbey Road").MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches.Keys).IsEqualTo(new[] { Id with { Extension = "txt", }, }).InAnyOrder();
				await That(Id.Extension).IsEqualTo("txt");
			}

			[Fact]
			public async Task WithExtension_ShouldPassItToTheStore()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "{\"title\": \"Abbey Road\"}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("json");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot did not exist

					             Actual:
					             {"title": "Abbey Road"}
					             """);
				await That(store.Mismatches.Keys).IsEqualTo(new[] { Id with { Extension = "json", }, }).InAnyOrder();
			}

			[Fact]
			public async Task WithExtension_ShouldKeepItsCase()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That("<album />").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("XML");

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches.Keys).IsEqualTo(new[] { Id with { Extension = "XML", }, }).InAnyOrder()
					.Because("the extension is kept as given, like the name of the snapshot");
			}

			[Fact]
			public async Task WithExtension_WhenTheSnapshotWithTheExtensionMatches_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Extension = "json", }] = "[1969]";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That("[1969]").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("json");

				await That(Act).DoesNotThrow();
				await That(store.Matches).IsEqualTo(new[] { Id with { Extension = "json", }, });
			}

			[Fact]
			public async Task WithExtension_WhenOnlyTheSnapshotWithoutTheExtensionExists_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "[1969]";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That("[1969]").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("json");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot did not exist*").AsWildcard()
					.Because("the extension is part of the snapshot that the store reads");
			}

			[Fact]
			public async Task WithExtension_WhenAccepting_ShouldAcceptTheSnapshotWithTheExtension()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That("<h1>Abbey Road</h1>").MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.WithExtension("html").Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<XunitException>();
				await That(store.Accepted.Keys).IsEqualTo(new[] { Id with { Extension = "html", }, }).InAnyOrder();
			}

			[Fact]
			public async Task WithExtension_ShouldCombineWithTheOtherOptions()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string title = "Abbey Road";

				async Task Act()
					=> await That($"<h1>{title}</h1><p>{Guid.NewGuid()}</p>")
						.MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.WithExtension("html")
						.For.Guids().Mask()
						.NamedAfter(title)
						.UniqueFor.OperatingSystem()
						.Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id with
					{
						Name = "title=Abbey Road",
						Variant = CurrentEnvironment.OperatingSystem,
						Extension = "html",
					}] = "<h1>Abbey Road</h1><p>{Guid_1}</p>",
				});
			}

			[Fact]
			public async Task WithExtension_WhenTheSnapshotWasUsedWithAnotherExtension_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "{}";
				store.Snapshots[Id with { Extension = "json", }] = "{}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = "{}";
				await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("json");

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             matches the snapshot "MyTests.MyTest",
					             but the snapshot was already used with the extension "txt" in this test run, so use one extension or name each one with .Named(…) or .NamedAfter(…)
					             """)
					.Because("two files for the same snapshot would leave one of them unchecked");
				await That(store.Matches).IsEqualTo(new[] { Id, })
					.Because("the conflicting snapshot is not read");
			}

			[Fact]
			public async Task WithExtension_WhenTheSnapshotWasUsedWithTheExtensionInAnotherCase_ShouldFail()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That(async () => await That("{}").MatchesSnapshot(Id.SourceFile, Id.MemberName)
					.WithExtension("json")).Throws<XunitException>();

				async Task Act()
					=> await That("{}").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("JSON");

				await That(Act).Throws<XunitException>()
					.WithMessage("*but the snapshot was already used with the extension \"json\" in this test run*")
					.AsWildcard()
					.Because("the files are the same on a file system that ignores the case, but not on others");
			}

			[Fact]
			public async Task WithExtension_WhenTheSnapshotIsUsedAgainWithTheSameExtension_ShouldSucceed()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Extension = "json", }] = "{}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That("{}").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("json");

				await That(Act).DoesNotThrow();
				await That(Act).DoesNotThrow()
					.Because("a retried or repeated test uses the same extension again");
			}

			[Fact]
			public async Task WithExtension_WhenNamedDifferently_ShouldNotConflict()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Name = "Abbey Road", }] = "Abbey Road";
				store.Snapshots[Id with { Name = "Let It Be", Extension = "json", }] = "\"Let It Be\"";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
				{
					await That("Abbey Road").MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("Abbey Road");
					await That("\"Let It Be\"").MatchesSnapshot(Id.SourceFile, Id.MemberName).Named("Let It Be")
						.WithExtension("json");
				}

				await That(Act).DoesNotThrow();
			}

			[Fact]
			public async Task WithExtension_WhenNull_ShouldThrowArgumentNullException()
			{
				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension(null!);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("extension").And
					.WithMessage("The 'extension' cannot be null.*").AsWildcard();
			}

			[Theory]
			[InlineData("")]
			[InlineData(" ")]
			[InlineData("\t")]
			public async Task WithExtension_WhenEmpty_ShouldThrowArgumentException(string extension)
			{
				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension(extension);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("extension").And
					.WithMessage("The 'extension' cannot be empty.*").AsWildcard();
			}

			[Theory]
			[InlineData(".json")]
			[InlineData("g.cs")]
			[InlineData("pending.json")]
			[InlineData("json.")]
			public async Task WithExtension_WhenItContainsADot_ShouldThrowArgumentException(string extension)
			{
				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension(extension);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("extension").And
					.WithMessage("The 'extension' cannot contain a '.', e.g. use \"json\" instead of \".json\".*")
					.AsWildcard();
			}

			[Theory]
			[InlineData("<")]
			[InlineData(">")]
			[InlineData(":")]
			[InlineData("\"")]
			[InlineData("/")]
			[InlineData("\\")]
			[InlineData("|")]
			[InlineData("?")]
			[InlineData("*")]
			[InlineData("%")]
			[InlineData("@")]
			[InlineData("js on")]
			[InlineData("json ")]
			[InlineData("js\ton")]
			[InlineData("js\u0001on")]
			[InlineData("js on")]
			public async Task WithExtension_WhenItContainsAnInvalidCharacter_ShouldThrowArgumentException(
				string extension)
			{
				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension(extension);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("extension").And
					.WithMessage(
						"The 'extension' cannot contain whitespace, control characters or any of < > : \" / \\ | ? * % @, as it is part of the file name.*")
					.AsWildcard()
					.Because("the extension is not escaped, and '%' and '@' have a meaning in the other parts of the file name");
			}

			[Theory]
			[InlineData("pending")]
			[InlineData("Pending")]
			public async Task WithExtension_WhenPending_ShouldThrowArgumentException(string extension)
			{
				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension(extension);

				await That(Act).Throws<ArgumentException>()
					.WithParamName("extension").And
					.WithMessage(
						"The 'extension' cannot be \"pending\", as it marks the pending file of a snapshot.*")
					.AsWildcard()
					.Because("'<Class>.<Method>.awexpect-snapshot.pending' would look like a pending file");
			}

			[Theory]
			[InlineData("json")]
			[InlineData("cs")]
			[InlineData("c++")]
			[InlineData("code-workspace")]
			[InlineData("snapshot")]
			[InlineData("äöü")]
			public async Task WithExtension_WhenValid_ShouldPassItToTheStore(string extension)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);

				async Task Act()
					=> await That("foo").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension(extension);

				await That(Act).Throws<XunitException>();
				await That(store.Mismatches.Keys).IsEqualTo(new[] { Id with { Extension = extension, }, }).InAnyOrder();
			}

			[Fact]
			public async Task WithExtension_WhenTheSnapshotWithAnotherExtensionIsListed_ShouldReportItAsOrphaned()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = "{}";
				store.Snapshots[Id with { Extension = "json", }] = "{}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That("{}").MatchesSnapshot(Id.SourceFile, Id.MemberName).WithExtension("json");

				async Task Act() => await OrphanedSnapshots.CheckAsync(true);

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that every snapshot is used by a test of this run, but 1 snapshot was not used by any test:
					               MyTests.MyTest
					             """)
					.Because("the snapshot with the extension 'txt' is left over from before the test used 'json'");
			}

			[Fact]
			public async Task WithExtension_WhenTheSnapshotIsListedWithTheExtensionInAnotherCase_ShouldNotReportIt()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id with { Extension = "JSON", }] = "{}";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				await That(async () => await That("{}").MatchesSnapshot(Id.SourceFile, Id.MemberName)
					.WithExtension("json")).Throws<XunitException>();

				async Task Act() => await OrphanedSnapshots.CheckAsync(true);

				await That(Act).DoesNotThrow()
					.Because("on a file system that ignores the case, the file is the snapshot of the test");
			}
		}
	}
}
