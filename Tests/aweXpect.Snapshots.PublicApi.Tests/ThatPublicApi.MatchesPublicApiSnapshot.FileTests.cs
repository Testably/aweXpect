using System.IO;
using aweXpect.Customization;
using aweXpect.Snapshots.PublicApi.Tests.TestHelpers;
using static aweXpect.Snapshots.PublicApi.Tests.TestHelpers.Compiler;

namespace aweXpect.Snapshots.PublicApi.Tests;

public sealed partial class ThatPublicApi
{
	public sealed partial class MatchesPublicApiSnapshot
	{
		public sealed class FileTests
		{
			[Test]
			public async Task ShouldDescribeThePublicApiOfTheAssemblyInTheFile()
			{
				FileInfo subject = WriteFile("Library.dll", Compile("Library", "1.0.0.0", """
					namespace Library;
					public class Api
					{
					    public string Name { get; set; } = "";
					    public void Play(int volume = 5) { }
					}
					"""));

				await That(subject).MatchesPublicApiSnapshot();
			}

			[Test]
			public async Task ShouldIncludeForwardedTypes()
				=> await That(WriteForwardingLibrary()).MatchesPublicApiSnapshot();

			[Test]
			public async Task WhenIgnoringForwardedTypes_ShouldNotIncludeThem()
				=> await That(WriteForwardingLibrary()).MatchesPublicApiSnapshot(o => o.Ignoring.ForwardedTypes());

			[Test]
			public async Task WhenOptionIsInvalid_ShouldThrowWhenTheExpectationIsWritten()
			{
				FileInfo subject = new("Library.dll");

				object Act()
					=> That(subject).MatchesPublicApiSnapshot(o => o.Within.Type("Library.Api+Nested"));

				await That(Act).ThrowsExactly<ArgumentException>()
					.WithParamName("fullName").And
					.WithMessage(
						"The type 'Library.Api+Nested' is nested, so it is written with its containing type: restrict the public API to the containing type instead.*")
					.AsWildcard()
					.Because("an invalid option is reported where it is written, like the other argument checks");
			}

			[Test]
			public async Task WhenOptionsAreNull_ShouldUseTheDefaults()
				=> await That(WriteForwardingLibrary()).MatchesPublicApiSnapshot(null);

			[Test]
			public async Task WhenOptionsArePassedByName_ShouldApplyThem()
				=> await That(WriteForwardingLibrary())
					.MatchesPublicApiSnapshot(options: o => o.Ignoring.ForwardedTypes());

			[Test]
			public async Task WhenOptionsAreSet_ShouldListThemInTheFailure()
			{
				RecordingSnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = WriteFile("Library.dll", Compile("Library", "1.0.0.0", """
					namespace Library;
					public class Api
					{
					    public void Play(int volume, bool loop) { }
					}
					"""));

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot(o => o
						.Within.Type("Library.Api").Within.Namespace("Library")
						.Ignoring.Type("Library.Legacy").Ignoring.Namespace("Library.Internal")
						.Ignoring.Attribute<ObsoleteAttribute>().Ignoring.Attribute("Library.BetaAttribute")
						.Ignoring.CompilerGeneratedMembers().Ignoring.ForwardedTypes()
						.Formatting.ParametersOnSeparateLines());

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a public API which matches the snapshot "ThatPublicApi.MatchesPublicApiSnapshot.FileTests.WhenOptionsAreSet_ShouldListThemInTheFailure",
					             but the snapshot did not exist

					             Actual (public API):
					             namespace Library
					             {
					                 public class Api
					                 {
					                     public Api() { }
					                     public void Play(
					                         int volume,
					                         bool loop) { }
					                 }
					             }


					             Public API options (public API):
					              - ignore attributes: ["Library.BetaAttribute", "System.ObsoleteAttribute"]
					              - ignore compiler-generated members
					              - ignore forwarded types
					              - ignore namespaces: ["Library.Internal"]
					              - ignore types: ["Library.Legacy"]
					              - restrict to namespaces: ["Library"]
					              - restrict to types: ["Library.Api"]
					              - write parameters on separate lines
					             """).IgnoringNewlineStyle()
					.Because("the options are usually set once for all tests of the public API");
			}

			[Test]
			public async Task WhenOptionsAreTheDefaults_ShouldNotListThemInTheFailure()
			{
				RecordingSnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = WriteFile("Library.dll", Compile("Library", "1.0.0.0", """
					namespace Library;
					public class Api { }
					"""));

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot(o => o);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a public API which matches the snapshot "ThatPublicApi.MatchesPublicApiSnapshot.FileTests.WhenOptionsAreTheDefaults_ShouldNotListThemInTheFailure",
					             but the snapshot did not exist

					             Actual (public API):
					             namespace Library
					             {
					                 public class Api
					                 {
					                     public Api() { }
					                 }
					             }

					             """).IgnoringNewlineStyle();
			}

			[Test]
			public async Task WhenSubjectIsNull_AndOptionIsInvalid_ShouldThrowWhenTheExpectationIsWritten()
			{
				FileInfo? subject = null;

				object Act()
					=> That(subject).MatchesPublicApiSnapshot(o => o.Ignoring.Type(null!));

				await That(Act).ThrowsExactly<ArgumentNullException>()
					.WithParamName("fullName").And
					.WithMessage("The 'fullName' cannot be null.*").AsWildcard()
					.Because("the options are applied even though no public API is generated for a null subject");
			}

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				FileInfo? subject = null;

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot();

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a public API which matches the snapshot "*",
					             but it was <null>
					             """).AsWildcard().IgnoringNewlineStyle();
			}

			[Test]
			public async Task WithExtension_ShouldPassItToTheStore()
			{
				RecordingSnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = WriteForwardingLibrary();

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot().WithExtension("txt")
						.Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a public API which matches the snapshot "ThatPublicApi.MatchesPublicApiSnapshot.FileTests.WithExtension_ShouldPassItToTheStore",
					             but the snapshot did not exist
					             *
					             """).AsWildcard().IgnoringNewlineStyle();
				await That(store.Mismatch?.Extension).IsEqualTo("txt")
					.Because("the extension replaces the default 'cs'");
			}

			[Test]
			public async Task WithoutExtension_ShouldStoreTheSnapshotAsCSharp()
			{
				RecordingSnapshotStore store = new();
				using CustomizationLifetime _ = Customize.aweXpect.Snapshots().Store.Set(store);
				FileInfo subject = WriteForwardingLibrary();

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot().Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<FailException>()
					.WithMessage("*but the snapshot did not exist*").AsWildcard();
				await That(store.Mismatch?.Extension).IsEqualTo("cs")
					.Because("the public API is written as C# declarations, which editors and diff tools then highlight");
			}
		}
	}
}
