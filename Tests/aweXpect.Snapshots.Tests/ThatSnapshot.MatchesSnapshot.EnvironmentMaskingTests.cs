using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using aweXpect.Customization;
using aweXpect.Snapshots.Tests.TestHelpers;

namespace aweXpect.Snapshots.Tests;

public sealed partial class ThatSnapshot
{
	public sealed partial class MatchesSnapshot
	{
		public sealed class EnvironmentMaskingTests
		{
			private const string FirstGuid = "3f2504e0-4f89-11d3-9a0c-0305e82c3301";
			private static readonly char Separator = Path.DirectorySeparatorChar;
			private static readonly bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
			private static readonly string TempPath = Trim(Path.GetTempPath());

			private static readonly string UserProfile =
				Trim(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

			private SnapshotId Id { get; } = new($"/src/{Guid.NewGuid():N}/MyTests.cs", "MyTest");

			[Test]
			public async Task WhenSubjectContainsTheTempPath_ShouldMaskIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Cache: {Path.Combine(Path.GetTempPath(), "abbey.tmp")}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($"Cache: {{TempPath}}{Separator}abbey.tmp");
			}

			[Test]
			public async Task WhenSubjectContainsTheUserProfile_ShouldMaskIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = Path.Combine(UserProfile, "Music", "Let It Be.mp3");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($"{{UserProfile}}{Separator}Music{Separator}Let It Be.mp3");
			}

			[Test]
			public async Task WhenSubjectContainsTheOutputDirectory_ShouldMaskItBeforeTheSourceRoot()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = Path.Combine(AppContext.BaseDirectory, "albums.json");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($"{{OutputDirectory}}{Separator}albums.json")
					.Because("the output directory lies in the source root, so the longest path is masked first");
			}

			[Test]
			public async Task WhenSubjectContainsTheSourceRoot_ShouldMaskIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = Path.Combine(RecordedPaths.SourceRoot, "README.md");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($"{{SourceRoot}}{Separator}README.md");
			}

			[Test]
			public async Task WhenSubjectContainsTheProjectDirectoryOfTheTestFile_ShouldMaskIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Loaded {Path.Combine(RecordedPaths.ProjectDirectory, "albums.json")}";

				async Task Act()
					=> await That(subject).MatchesSnapshot();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches.Values.Single()).IsEqualTo($"Loaded {{ProjectDirectory}}{Separator}albums.json");
			}

			[Test]
			public async Task WhenTheTestFileIsNotInTheProjectDirectory_ShouldMaskItAsPartOfTheSourceRoot()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string projectDirectory = Trim(RecordedPaths.ProjectDirectory);
				string subject = Path.Combine(projectDirectory, "albums.json");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(
						"{SourceRoot}" + projectDirectory.Substring(Trim(RecordedPaths.SourceRoot).Length) +
						Separator + "albums.json")
					.Because("only the directory of the project that contains the test file is the project directory");
			}

			[Test]
			public async Task WhenPathUsesForwardSlashes_ShouldMaskIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{TempPath.Replace('\\', '/')}/abbey.tmp";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("{TempPath}/abbey.tmp")
					.Because("the rest of the path keeps its own separators");
			}

			[Test]
			public async Task WhenPathUsesBackslashes_ShouldOnlyMaskItOnWindows()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{TempPath.Replace('/', '\\')}\\let-it-be.tmp";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(IsWindows ? "{TempPath}\\let-it-be.tmp" : subject)
					.Because("a backslash is a separator on Windows, but part of a file name elsewhere");
			}

			[Test]
			public async Task WhenPathHasEscapedBackslashes_ShouldOnlyMaskItOnWindows()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string escapedTempPath = TempPath.Replace('/', '\\').Replace("\\", "\\\\");
				string subject = $"{{\"cache\": \"{escapedTempPath}\\\\abbey.tmp\"}}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(IsWindows ? "{\"cache\": \"{TempPath}\\\\abbey.tmp\"}" : subject)
					.Because("a path in JSON has escaped backslashes on Windows");
			}

			[Test]
			public async Task WhenPathEndsWithASeparator_ShouldKeepIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = TempPath + Separator;

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($"{{TempPath}}{Separator}");
			}

			[Test]
			public async Task WhenPathEndsASentence_ShouldMaskIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Cleaned {TempPath}.";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("Cleaned {TempPath}.");
			}

			[Test]
			[Arguments("s")]
			[Arguments("-backup")]
			[Arguments(".old")]
			public async Task WhenLastSegmentContinues_ShouldNotMaskIt(string continuation)
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{TempPath}{continuation}{Separator}abbey.tmp";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).DoesNotContain("{TempPath}")
					.Because("only whole path segments match, so the temp path does not match inside a longer segment");
			}

			[Test]
			public async Task WhenPathIsPartOfALongerPath_ShouldNotMaskIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"/mnt{TempPath}{Separator}abbey.tmp";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).DoesNotContain("{TempPath}")
					.Because("a path that does not start at a segment boundary is another path");
			}

			[Test]
			public async Task WhenPathHasAnotherCase_ShouldOnlyMaskItOnWindowsAndMacOs()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string otherCase = InvertCase(TempPath);
				string subject = $"{otherCase}{Separator}abbey.tmp";
				bool ignoresCase = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ||
				                   RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(ignoresCase ? $"{{TempPath}}{Separator}abbey.tmp" : subject)
					.Because("the file systems of Windows and macOS ignore the case by default, those of Linux do not");
			}

			[Test]
			public async Task WhenObjectHasAPathInAString_ShouldMaskTheEscapedPath()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotTrack subject = new("Abbey Road", Path.Combine(TempPath, "abbey.tmp"));
				string separator = Separator == '\\' ? "\\\\" : "/";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($$"""
				                                            SnapshotTrack {
				                                              File = "{TempPath}{{separator}}abbey.tmp",
				                                              Title = "Abbey Road"
				                                            }
				                                            """).IgnoringNewlineStyle()
					.Because("the formatter escapes the backslashes of a string");
			}

			[Test]
			public async Task WhenObjectHasAPathAfterAnEscapedLineBreak_ShouldMaskIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				SnapshotTrack subject = new("Let It Be", $"cached\n{Path.Combine(TempPath, "let-it-be.tmp")}");
				string separator = Separator == '\\' ? "\\\\" : "/";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).Contains($$"""
				                                           File = "cached\n{TempPath}{{separator}}let-it-be.tmp",
				                                           """)
					.Because("the 'n' of the escaped line break does not belong to the path");
			}

			[Test]
			public async Task WhenPathsAreKeptForTheExpectation_ShouldKeepThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = Path.Combine(TempPath, "abbey.tmp");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Paths().Keep();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject)
					.Because("the expectation keeps the paths, which are masked by default");
			}

			[Test]
			public async Task WhenPathsAreKeptForTheExpectation_ShouldStillApplyTheOtherRules()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = Path.Combine(TempPath, $"{FirstGuid}.tmp");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Paths().Keep()
						.For.Guids().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(Path.Combine(TempPath, "{Guid_1}.tmp"));
			}

			[Test]
			public async Task WhenPathsAreKeptGlobally_ShouldKeepThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Paths().Keep(),
				]);
				string subject = Path.Combine(TempPath, "abbey.tmp");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject);
			}

			[Test]
			public async Task WhenPathsAreKeptGlobally_AndAScopeSetsTheRulesWithoutIt_ShouldMaskThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.Paths().Keep(),
				]);
				string subject = Path.Combine(TempPath, "abbey.tmp");

				async Task Act()
				{
					using CustomizationLifetime inner = Customize.aweXpect.Snapshots().Rules.Set([]);
					await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);
				}

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($"{{TempPath}}{Separator}abbey.tmp")
					.Because("an expectation cannot mask the paths again, but a scope can set the rules without keeping them");
			}

			[Test]
			public async Task WhenOtherMaskingsFollow_ShouldSeeThePlaceholders()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Cache: {Path.Combine(TempPath, "abbey.tmp")}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text(@"\{TempPath\}[\\/]\w+\.tmp").AsRegex().Mask("{cache-file}");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("Cache: {cache-file}")
					.Because("the paths are masked before the other maskings");
			}

			[Test]
			public async Task WhenCombinedWithGuids_ShouldMaskBoth()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = Path.Combine(TempPath, $"{FirstGuid}.tmp");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.Guids().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($"{{TempPath}}{Separator}{{Guid_1}}.tmp");
			}

			[Test]
			public async Task WhenAccepting_ShouldAcceptTheMaskedValue()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = Path.Combine(TempPath, "abbey.tmp");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.Accepting(SnapshotAcceptance.All);

				await That(Act).Throws<FailException>();
				await That(store.Accepted).IsEqualTo(new Dictionary<SnapshotId, string>
				{
					[Id] = $"{{TempPath}}{Separator}abbey.tmp",
				});
			}

			[Test]
			public async Task WhenSnapshotHasThePlaceholder_ShouldMatchTheRealPath()
			{
				InMemorySnapshotStore store = new();
				store.Snapshots[Id] = $"Cache: {{TempPath}}{Separator}abbey.tmp";
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Cache: {Path.Combine(TempPath, "abbey.tmp")}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).DoesNotThrow()
					.Because("the snapshot no longer depends on the machine it was recorded on");
			}

			[Test]
			public async Task MachineName_ShouldNotBeMaskedByDefault()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Ripped on {Environment.MachineName}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject);
			}

			[Test]
			public async Task WhenMaskingMachineName_ShouldMaskItInAnyCase()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Ripped on {Environment.MachineName}, played on {InvertCase(Environment.MachineName)}.";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.MachineName().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("Ripped on {MachineName}, played on {MachineName}.")
					.Because("host names are case-insensitive");
			}

			[Test]
			public async Task WhenMaskingMachineName_ShouldOnlyMaskWholeWords()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"{Environment.MachineName}2 x{Environment.MachineName}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.MachineName().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo(subject);
			}

			[Test]
			public async Task WhenMaskingUserName_ShouldOnlyMaskWholeWords()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Playlist of {Environment.UserName}, not of {Environment.UserName}s";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.UserName().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($"Playlist of {{UserName}}, not of {Environment.UserName}s");
			}

			[Test]
			public async Task WhenMaskingUserName_ShouldMaskTheUserProfileFirst()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = Path.Combine(UserProfile, "Music");

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.UserName().Mask();

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo($"{{UserProfile}}{Separator}Music")
					.Because("the paths are masked before the user name");
			}

			[Test]
			public async Task WhenMachineAndUserNameAreMaskedGlobally_ShouldMaskThem()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.MachineName().Mask(),
					SnapshotRule.For.UserName().Mask(),
				]);
				string subject = $"{Environment.UserName} on {Environment.MachineName}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName);

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("{UserName} on {MachineName}");
			}

			[Test]
			public async Task WhenMachineNameIsMaskedGlobally_ShouldMaskItBeforeTheMaskingsOfTheExpectation()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				using CustomizationLifetime rules = Customize.aweXpect.Snapshots().Rules.Set([
					SnapshotRule.For.MachineName().Mask(),
				]);
				string subject = $"Ripped on {Environment.MachineName}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName)
						.For.Text("on {MachineName}").Mask("elsewhere");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("Ripped elsewhere")
					.Because("the maskings of the environment come before the others, also from the global setting");
			}

			[Test]
			public async Task WhenMaskingMachineNameWithATemplate_ShouldUseIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Ripped on {Environment.MachineName}, played on {InvertCase(Environment.MachineName)}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.MachineName().Mask("<host>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("Ripped on <host>, played on <host>");
			}

			[Test]
			public async Task WhenMaskingUserNameWithATemplate_ShouldUseIt()
			{
				InMemorySnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				string subject = $"Playlist of {Environment.UserName}";

				async Task Act()
					=> await That(subject).MatchesSnapshot(Id.SourceFile, Id.MemberName).For.UserName().Mask("<user>");

				await That(Act).Throws<FailException>();
				await That(store.Mismatches[Id]).IsEqualTo("Playlist of <user>");
			}

			[Test]
			public async Task WhenMachineNameTemplateIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
					=> SnapshotRule.For.MachineName().Mask(null!);

				await That(Act).Throws<ArgumentNullException>().WithParamName("template");
			}

			private static string InvertCase(string value)
				=> new(value.Select(character => char.IsUpper(character)
					? char.ToLowerInvariant(character)
					: char.ToUpperInvariant(character)).ToArray());

			private static string Trim(string path)
				=> path.TrimEnd('\\', '/');
		}
	}
}

public sealed record SnapshotTrack(string Title, string File);
