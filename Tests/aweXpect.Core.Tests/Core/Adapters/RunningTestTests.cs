using aweXpect.Core.Adapters;

namespace aweXpect.Core.Tests.Core.Adapters;

public sealed class RunningTestTests
{
	[Test]
	public async Task ShouldKeepTheIdentityTheIdTheDisplayNameAndTheRetryAttempt()
	{
		TestIdentity identity = new("MyTests.AlbumTests", [], "Album_MatchesSnapshot", ["Abbey Road",]);

		RunningTest runningTest = new(identity, "1001", "Album_MatchesSnapshot(\"Abbey Road\")", 2);

		await That(runningTest.Identity).IsSameAs(identity);
		await That(runningTest.Id).IsEqualTo("1001");
		await That(runningTest.DisplayName).IsEqualTo("Album_MatchesSnapshot(\"Abbey Road\")");
		await That(runningTest.RetryAttempt).IsEqualTo(2);
	}

	[Test]
	public async Task WithNegativeRetryAttempt_ShouldThrowArgumentOutOfRangeException()
	{
		TestIdentity identity = new("MyTests.AlbumTests", [], "Album_MatchesSnapshot", []);

		void Act() => _ = new RunningTest(identity, null, null, -1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The retry attempt must not be negative.*").AsWildcard().And
			.WithParamName("retryAttempt");
	}

	[Test]
	public async Task WithNullIdentity_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new RunningTest(null!, null, null, 0);

		await That(Act).Throws<ArgumentNullException>().WithParamName("identity");
	}

	[Test]
	public async Task WithoutIdAndDisplayName_ShouldKeepThemNull()
	{
		RunningTest runningTest = new(new TestIdentity("MyTests.AlbumTests", [], "Album_MatchesSnapshot", []), null,
			null, 0);

		await That(runningTest.Id).IsNull();
		await That(runningTest.DisplayName).IsNull();
		await That(runningTest.RetryAttempt).IsEqualTo(0);
	}
}
