using aweXpect.Core.Adapters;

namespace aweXpect.Core.Tests.Core.Adapters;

public sealed class TestIdentityTests
{
	[Test]
	public async Task ShouldKeepTheClassTheMethodAndTheArguments()
	{
		object?[] arguments = ["Abbey Road", 1969, null,];

		TestIdentity identity = new("MyTests.AlbumTests", "Album_MatchesSnapshot", arguments);

		await That(identity.ClassName).IsEqualTo("MyTests.AlbumTests");
		await That(identity.MethodName).IsEqualTo("Album_MatchesSnapshot");
		await That(identity.Arguments).IsSameAs(arguments);
	}

	[Test]
	public async Task ShouldKeepTheIdTheDisplayNameAndTheRetryAttempt()
	{
		TestIdentity identity = new("MyTests.AlbumTests", "Album_MatchesSnapshot", ["Abbey Road",], "1001",
			"Album_MatchesSnapshot(\"Abbey Road\")", 2);

		await That(identity.Id).IsEqualTo("1001");
		await That(identity.DisplayName).IsEqualTo("Album_MatchesSnapshot(\"Abbey Road\")");
		await That(identity.RetryAttempt).IsEqualTo(2);
	}

	[Test]
	public async Task WithNegativeRetryAttempt_ShouldThrowArgumentOutOfRangeException()
	{
		void Act() => _ = new TestIdentity("MyTests.AlbumTests", "Album_MatchesSnapshot", [], null, null, -1);

		await That(Act).Throws<ArgumentOutOfRangeException>()
			.WithMessage("The retry attempt must not be negative.*").AsWildcard().And
			.WithParamName("retryAttempt");
	}

	[Test]
	public async Task WithNullArguments_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new TestIdentity("MyTests.AlbumTests", "Album_MatchesSnapshot", null!);

		await That(Act).Throws<ArgumentNullException>().WithParamName("arguments");
	}

	[Test]
	public async Task WithNullClassName_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new TestIdentity(null!, "Album_MatchesSnapshot", []);

		await That(Act).Throws<ArgumentNullException>().WithParamName("className");
	}

	[Test]
	public async Task WithNullMethodName_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new TestIdentity("MyTests.AlbumTests", null!, []);

		await That(Act).Throws<ArgumentNullException>().WithParamName("methodName");
	}

	[Test]
	public async Task WithoutIdDisplayNameAndRetryAttempt_ShouldBeTheFirstRunWithoutIdAndDisplayName()
	{
		TestIdentity identity = new("MyTests.AlbumTests", "Album_MatchesSnapshot", []);

		await That(identity.Id).IsNull();
		await That(identity.DisplayName).IsNull();
		await That(identity.RetryAttempt).IsEqualTo(0);
	}
}
