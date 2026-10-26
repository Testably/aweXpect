using aweXpect.Core.Adapters;

namespace aweXpect.Core.Tests.Core.Adapters;

public sealed class TestIdentityTests
{
	[Test]
	public async Task ShouldKeepTheClassTheMethodAndTheArguments()
	{
		object?[] classArguments = ["de-DE",];
		object?[] arguments = ["Abbey Road", 1969, null,];

		TestIdentity identity = new("MyTests.AlbumTests", classArguments, "Album_MatchesSnapshot", arguments);

		await That(identity.ClassName).IsEqualTo("MyTests.AlbumTests");
		await That(identity.ClassArguments).IsSameAs(classArguments);
		await That(identity.MethodName).IsEqualTo("Album_MatchesSnapshot");
		await That(identity.Arguments).IsSameAs(arguments);
	}

	[Test]
	public async Task WithNullArguments_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new TestIdentity("MyTests.AlbumTests", [], "Album_MatchesSnapshot", null!);

		await That(Act).Throws<ArgumentNullException>().WithParamName("arguments");
	}

	[Test]
	public async Task WithNullClassArguments_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new TestIdentity("MyTests.AlbumTests", null!, "Album_MatchesSnapshot", []);

		await That(Act).Throws<ArgumentNullException>().WithParamName("classArguments");
	}

	[Test]
	public async Task WithNullClassName_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new TestIdentity(null!, [], "Album_MatchesSnapshot", []);

		await That(Act).Throws<ArgumentNullException>().WithParamName("className");
	}

	[Test]
	public async Task WithNullMethodName_ShouldThrowArgumentNullException()
	{
		void Act() => _ = new TestIdentity("MyTests.AlbumTests", [], null!, []);

		await That(Act).Throws<ArgumentNullException>().WithParamName("methodName");
	}
}
