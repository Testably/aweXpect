using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Adapters;
using Xunit;
using Xunit.Sdk;

namespace aweXpect.Frameworks.Xunit2.Tests;

public sealed class Xunit2TestFrameworkTests
{
	[Fact]
	public async Task OnFail_WhenUsingXunit2AsTestFramework_ShouldThrowXunitException()
	{
		void Act()
			=> Fail.Test("my message");

		await Expect.That(Act).Throws<XunitException>();
	}

	[Fact]
	public async Task OnFailWithCause_WhenUsingXunit2AsTestFramework_ShouldForwardTheCauseAsInnerException()
	{
		Exception cause = new InvalidOperationException("my cause");

		void Act()
			=> Fail.Test("my message", cause);

		await Expect.That(Act).Throws<XunitException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(cause));
	}

	[Fact]
	public async Task OnInconclusive_WhenUsingXunit2AsTestFramework_ShouldThrowInconclusiveException()
	{
		void Act()
			=> Fail.Inconclusive("my message");

		await Expect.That(Act).Throws<InconclusiveException>()
			.WithMessage("my message");
	}

	[Fact]
	public async Task OnSkip_WhenUsingXunit2AsTestFramework_ShouldThrowSkipException()
	{
		void Act()
			=> Skip.Test("my message");

		await Expect.That(Act).Throws<SkipException>()
			.WithMessage("SKIPPED: my message (xunit v2 does not support skipping test)");
	}

	[Fact]
	public async Task TestFramework_ShouldHaveMajorVersion2()
	{
		Version? version = typeof(FactAttribute).Assembly.GetName().Version;

		await Expect.That(version?.Major).IsEqualTo(2)
			.Because("this project tests the adapter against xunit v2");
	}

	[Fact]
	public async Task TryAddAttachment_ShouldNotBeSupported()
	{
		bool isAttached = TestFrameworkRegistry.TryAddAttachment("snapshot.txt", "the received snapshot");

		await Expect.That(isAttached).IsFalse()
			.Because("xunit v2 has no ambient context of the running test");
	}

	[Fact]
	public async Task TryGetCancellationToken_ShouldNotBeSupported()
	{
		bool hasToken = TestFrameworkRegistry.TryGetCancellationToken(out CancellationToken cancellationToken);

		await Expect.That(hasToken).IsFalse()
			.Because("xunit v2 has no ambient context of the running test");
		await Expect.That(cancellationToken).IsEqualTo(CancellationToken.None);
	}

	[Fact]
	public async Task TryGetCurrentTest_ShouldNotFindTheRunningTest()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out TestIdentity? currentTest);

		await Expect.That(isRunning).IsFalse()
			.Because("xunit v2 has no ambient context of the running test");
		await Expect.That(currentTest).IsNull();
	}
}
