using System.Reflection;
using aweXpect.Customization;
using aweXpect.Snapshots.PublicApi.Fixture;
using aweXpect.Snapshots.PublicApi.Tests.TestHelpers;
using static aweXpect.Snapshots.PublicApi.Tests.TestHelpers.Compiler;

namespace aweXpect.Snapshots.PublicApi.Tests;

public sealed partial class ThatPublicApi
{
	public sealed partial class MatchesPublicApiSnapshot
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldDescribeThePublicApiOfTheFixture()
				=> await That(typeof(Album).Assembly).MatchesPublicApiSnapshot().UniqueFor.TargetFramework();

			[Test]
			public async Task WhenAwaited_ShouldReturnTheSubject()
			{
				Assembly subject = Assembly.LoadFile(WriteForwardingLibrary().FullName);

				Assembly result = await That(subject).MatchesPublicApiSnapshot();

				await That(result).IsSameAs(subject);
			}

			[Test]
			public async Task WhenChainedWithAnd_ShouldContinueOnTheSubject()
			{
				Assembly subject = Assembly.LoadFile(WriteForwardingLibrary().FullName);

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot().And.IsSameAs(subject);

				await That(Act).DoesNotThrow()
					.Because("the expectations after .And are on the assembly, not on the generated public API");
			}

			[Test]
			public async Task WhenIgnoringForwardedTypes_ShouldNotIncludeThem()
				=> await That(Assembly.LoadFile(WriteForwardingLibrary().FullName))
					.MatchesPublicApiSnapshot(o => o.Ignoring.ForwardedTypes());

			[Test]
			public async Task WhenOptionsAreNull_ShouldUseTheDefaults()
				=> await That(Assembly.LoadFile(WriteForwardingLibrary().FullName))
					.MatchesPublicApiSnapshot(null);

			[Test]
			public async Task WhenOptionsArePassedByName_ShouldApplyThem()
				=> await That(Assembly.LoadFile(WriteForwardingLibrary().FullName))
					.MatchesPublicApiSnapshot(options: o => o.Ignoring.ForwardedTypes());

			[Test]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Assembly? subject = null;

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
				Assembly subject = typeof(Album).Assembly;

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot().WithExtension("cs")
						.Accepting(SnapshotAcceptance.None);

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that subject
					             has a public API which matches the snapshot "ThatPublicApi.MatchesPublicApiSnapshot.Tests.WithExtension_ShouldPassItToTheStore",
					             but the snapshot did not exist
					             *
					             """).AsWildcard().IgnoringNewlineStyle();
				await That(store.Mismatch?.Extension).IsEqualTo("cs");
			}
		}
	}
}
