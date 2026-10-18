using System.Reflection;
using aweXpect.Snapshots.PublicApi.Fixture;
using static aweXpect.Snapshots.PublicApi.Tests.TestHelpers.Compiler;

namespace aweXpect.Snapshots.PublicApi.Tests;

public sealed partial class ThatPublicApi
{
	public sealed partial class MatchesPublicApiSnapshot
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldDescribeThePublicApiOfTheFixture()
				=> await That(typeof(Album).Assembly).MatchesPublicApiSnapshot().UniqueFor.TargetFramework();

			[Fact]
			public async Task WhenIgnoringForwardedTypes_ShouldNotIncludeThem()
				=> await That(Assembly.LoadFile(WriteForwardingLibrary().FullName))
					.MatchesPublicApiSnapshot(o => o.Ignoring.ForwardedTypes());

			[Fact]
			public async Task WhenOptionsAreNull_ShouldThrowWhenCalled()
			{
				Assembly subject = typeof(Album).Assembly;
				Func<PublicApiOptions, PublicApiOptions> options = null!;

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot(options);

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("options").And
					.WithMessage("The 'options' cannot be null.*").AsWildcard()
					.Because("a check while the expectation is evaluated would fail it instead of throwing");
			}

			[Fact]
			public async Task WhenSubjectIsNull_ShouldFail()
			{
				Assembly? subject = null;

				async Task Act()
					=> await That(subject).MatchesPublicApiSnapshot();

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that subject
					             has a public API which matches the snapshot "*",
					             but it was <null>
					             """).AsWildcard().IgnoringNewlineStyle();
			}
		}
	}
}
