using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aweXpect.Frameworks.MsTest3.Tests;

[TestClass]
public sealed class MsTestFrameworkTests
{
	[TestMethod]
	public async Task OnFail_WhenUsingMsTestAsTestFramework_ShouldThrowAssertFailedException()
	{
		void Act()
			=> Fail.Test("my message");

		await Expect.That(Act).Throws<AssertFailedException>()
			.WithMessage("my message");
	}

	[TestMethod]
	public async Task OnFailWithCause_WhenUsingMsTestAsTestFramework_ShouldForwardTheCauseAsInnerException()
	{
		Exception cause = new InvalidOperationException("my cause");

		void Act()
			=> Fail.Test("my message", cause);

		await Expect.That(Act).Throws<AssertFailedException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(cause));
	}

	[TestMethod]
	public async Task OnInconclusive_WhenUsingMsTestAsTestFramework_ShouldThrowAssertInconclusiveException()
	{
		void Act()
			=> Fail.Inconclusive("my message");

		await Expect.That(Act).Throws<AssertInconclusiveException>()
			.WithMessage("my message");
	}

	[TestMethod]
	public async Task OnSkip_WhenUsingMsTestAsTestFramework_ShouldThrowAssertInconclusiveException()
	{
		void Act()
			=> Skip.Test("my message");

		await Expect.That(Act).Throws<AssertInconclusiveException>()
			.WithMessage("my message");
	}

	[TestMethod]
	public async Task TestFramework_ShouldHaveMajorVersion3()
	{
		// The assembly version of MSTest does not follow the package version (MSTest 3 is fixed at 14.0.0.0).
		string? version = typeof(TestClassAttribute).Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

		await Expect.That(version).StartsWith("3.")
			.Because("this project tests the adapter against MSTest 3");
	}

	[TestMethod]
	public async Task TryAddAttachment_ShouldNotBeSupported()
	{
		bool isAttached = TestFrameworkRegistry.TryAddAttachment("snapshot.txt", "the received snapshot");

		await Expect.That(isAttached).IsFalse()
			.Because("MSTest 3 has no ambient context of the running test");
	}

	[TestMethod]
	public async Task TryGetCancellationToken_ShouldNotBeSupported()
	{
		bool hasToken = TestFrameworkRegistry.TryGetCancellationToken(out CancellationToken cancellationToken);

		await Expect.That(hasToken).IsFalse()
			.Because("MSTest 3 has no ambient context of the running test");
		await Expect.That(cancellationToken).IsEqualTo(CancellationToken.None);
	}

	[TestMethod]
	public async Task TryGetCurrentTest_ShouldNotFindTheRunningTest()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out TestIdentity? currentTest);

		await Expect.That(isRunning).IsFalse()
			.Because("MSTest 3 has no TestContext.Current, so the running test is only known to a TestContext that is injected into the test class");
		await Expect.That(currentTest).IsNull();
	}
}
