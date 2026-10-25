using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Adapters;
using TUnit.Core.Exceptions;

namespace aweXpect.Frameworks.Tunit.Core.Tests;

public sealed class TunitCoreTestFrameworkTests
{
	private static int _retriedRuns;

	[Test]
	public async Task OnFail_WhenUsingTUnitWithoutItsAssertions_ShouldThrowFailException()
	{
		void Act()
			=> Fail.Test("my message");

		await Expect.That(Act).ThrowsExactly<FailException>()
			.WithMessage("my message");
	}

	[Test]
	public async Task OnFailWithCause_WhenUsingTUnitWithoutItsAssertions_ShouldForwardTheCauseAsInnerException()
	{
		Exception cause = new InvalidOperationException("my cause");

		void Act()
			=> Fail.Test("my message", cause);

		await Expect.That(Act).ThrowsExactly<FailException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(cause));
	}

	[Test]
	public async Task OnInconclusive_WhenUsingTUnitWithoutItsAssertions_ShouldThrowInconclusiveTestException()
	{
		void Act()
			=> Fail.Inconclusive("my message");

		await Expect.That(Act).Throws<InconclusiveTestException>()
			.WithMessage("my message");
	}

	[Test]
	public async Task OnSkip_WhenUsingTUnitWithoutItsAssertions_ShouldThrowSkipTestException()
	{
		void Act()
			=> Skip.Test("my message");

		await Expect.That(Act).Throws<SkipTestException>()
			.Whose(e => e.Reason, r => r.IsEqualTo("my message"));
	}

	[Test]
	public async Task TryAddAttachment_ShouldAttachTheFile()
	{
		string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
		File.WriteAllText(filePath, "received");

		bool isAttached = TestFrameworkRegistry.TryAddAttachment(filePath, "the received snapshot");

		await Expect.That(isAttached).IsTrue();
		await Expect.That(TestContext.Current!.Output.Artifacts
				.Any(x => x.File.FullName == filePath && x.Description == "the received snapshot")).IsTrue()
			.Because("TUnit attaches the file as an artifact of the test");
	}

	[Test]
	public async Task TryGetCancellationToken_ShouldReturnTheTokenOfTheTest()
	{
		bool hasToken = TestFrameworkRegistry.TryGetCancellationToken(out CancellationToken cancellationToken);

		await Expect.That(hasToken).IsTrue();
		await Expect.That(cancellationToken).IsEqualTo(TestContext.Current!.Execution.CancellationToken);
	}

	[Test]
	[Arguments("Abbey Road", 1969)]
	[Arguments("Revolver", 1966)]
	public async Task TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestCase(string title, int year)
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue()
			.Because("TUnit without its assertions exposes the running test");
		await Expect.That(currentTest!.Identity.ClassName).IsEqualTo(typeof(TunitCoreTestFrameworkTests).FullName);
		await Expect.That(currentTest.Identity.MethodName).IsEqualTo(nameof(TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestCase));
		await Expect.That(currentTest.Identity.Arguments).IsEqualTo(new object?[] { title, year, });
	}

	[Test]
	public async Task TryGetCurrentTest_ShouldReturnTheIdAndTheDisplayName()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.Id).IsEqualTo(TestContext.Current!.Metadata.TestDetails.TestId);
		await Expect.That(currentTest.DisplayName).IsEqualTo(TestContext.Current.Metadata.DisplayName);
		await Expect.That(currentTest.RetryAttempt).IsEqualTo(0);
	}

	[Test]
	[Retry(1)]
	public async Task TryGetCurrentTest_WhenRetried_ShouldCountTheAttempts()
	{
		int attempt = Interlocked.Increment(ref _retriedRuns) - 1;

		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.RetryAttempt).IsEqualTo(attempt);
		if (attempt == 0)
		{
			throw new InvalidOperationException("The first attempt fails, so that TUnit retries the test.");
		}
	}

	[Test]
	public async Task TryGetCurrentTest_WithoutParameters_ShouldReturnNoArguments()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.Identity.MethodName).IsEqualTo(nameof(TryGetCurrentTest_WithoutParameters_ShouldReturnNoArguments));
		await Expect.That(currentTest.Identity.Arguments).IsEmpty();
	}
}
