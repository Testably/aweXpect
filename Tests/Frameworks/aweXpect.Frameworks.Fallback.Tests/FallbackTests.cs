using System;
using System.Threading;
using aweXpect.Core.Adapters;
using NUnit.Framework;

namespace aweXpect.Frameworks.Fallback.Tests;

public sealed class FallbackTests
{
	[Test]
	public void OnFail_WhenNotSpecifyingAnyTestFramework_ShouldFallbackToThrowingAFailException()
	{
		void Act()
			=> Fail.Test("my message");

		FailException? exception = Assert.Throws<FailException>(Act);
		Assert.That(exception!.Message, Is.EqualTo("my message"));
	}

	[Test]
	public void OnFailWithCause_WhenNotSpecifyingAnyTestFramework_ShouldForwardTheCauseAsInnerException()
	{
		Exception cause = new InvalidOperationException("my cause");

		void Act()
			=> Fail.Test("my message", cause);

		FailException? exception = Assert.Throws<FailException>(Act);
		Assert.That(exception!.Message, Is.EqualTo("my message"));
		Assert.That(exception.InnerException, Is.SameAs(cause));
	}

	[Test]
	public void OnInconclusive_WhenNotSpecifyingAnyTestFramework_ShouldFallbackToThrowingAnInconclusiveException()
	{
		void Act()
			=> Fail.Inconclusive("my message");

		InconclusiveException? exception = Assert.Throws<InconclusiveException>(Act);
		Assert.That(exception!.Message, Is.EqualTo("my message"));
	}

	[Test]
	public void OnSkip_WhenNotSpecifyingAnyTestFramework_ShouldFallbackToThrowingASkipException()
	{
		void Act()
			=> Skip.Test("my message");

		SkipException? exception = Assert.Throws<SkipException>(Act);
		Assert.That(exception!.Message, Is.EqualTo("my message"));
	}

	[Test]
	public void TryAddAttachment_WhenNotSpecifyingAnyTestFramework_ShouldNotAttachTheFile()
	{
		bool isAttached = TestFrameworkRegistry.TryAddAttachment("snapshot.txt", "the received snapshot");

		Assert.That(isAttached, Is.False);
	}

	[Test]
	public void TryGetCancellationToken_WhenNotSpecifyingAnyTestFramework_ShouldNotFindTheToken()
	{
		bool hasToken = TestFrameworkRegistry.TryGetCancellationToken(out CancellationToken cancellationToken);

		Assert.That(hasToken, Is.False);
		Assert.That(cancellationToken, Is.EqualTo(CancellationToken.None));
	}

	[Test]
	public void TryGetCurrentTest_WhenNotSpecifyingAnyTestFramework_ShouldNotFindTheRunningTest()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		Assert.That(isRunning, Is.False);
		Assert.That(currentTest, Is.Null);
	}
}
