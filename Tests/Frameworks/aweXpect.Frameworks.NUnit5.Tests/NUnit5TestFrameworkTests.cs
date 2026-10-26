using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Adapters;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace aweXpect.Frameworks.NUnit5.Tests;

public sealed class NUnit5TestFrameworkTests
{
	private static int _repeatedRuns;

	[Test]
	public async Task OnFail_WhenUsingNUnit5AsTestFramework_ShouldThrowAssertionException()
	{
		void Act()
			=> Fail.Test("my message");

		await Expect.That(Act).Throws<AssertionException>()
			.WithMessage("my message");
	}

	[Test]
	public async Task OnFailWithCause_WhenUsingNUnit5AsTestFramework_ShouldForwardTheCauseAsInnerException()
	{
		Exception cause = new InvalidOperationException("my cause");

		void Act()
			=> Fail.Test("my message", cause);

		await Expect.That(Act).Throws<AssertionException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(cause));
	}

	[Test]
	public async Task OnInconclusive_WhenUsingNUnit5AsTestFramework_ShouldThrowAssertionException()
	{
		void Act()
			=> Fail.Inconclusive("my message");

		await Expect.That(Act).Throws<NUnit.Framework.InconclusiveException>()
			.WithMessage("my message");
	}

	[Test]
	public async Task OnSkip_WhenUsingNUnit5AsTestFramework_ShouldThrowIgnoreException()
	{
		void Act()
			=> Skip.Test("my message");

		await Expect.That(Act).Throws<IgnoreException>()
			.WithMessage("my message");
	}

	[Test]
	public async Task TestFramework_ShouldHaveMajorVersion5()
	{
		Version? version = typeof(TestAttribute).Assembly.GetName().Version;

		await Expect.That(version?.Major).IsEqualTo(5)
			.Because("this project tests the adapter against NUnit 5");
	}

	[Test]
	public async Task TryAddAttachment_ShouldAttachTheFile()
	{
		string filePath = Path.Combine(TestContext.CurrentContext.WorkDirectory,
			$"{nameof(TryAddAttachment_ShouldAttachTheFile)}.txt");
		File.WriteAllText(filePath, "received");

		bool isAttached = TestFrameworkRegistry.TryAddAttachment(filePath, "the received snapshot");

		await Expect.That(isAttached).IsTrue();
		await Expect.That(TestExecutionContext.CurrentContext.CurrentResult.TestAttachments
				.Any(x => x.FilePath == filePath && x.Description == "the received snapshot")).IsTrue()
			.Because("NUnit 5 attaches files to the result of the test");
	}

	[Test]
	public async Task TryAddAttachment_WhenFileDoesNotExist_ShouldReturnFalse()
	{
		string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");

		bool isAttached = TestFrameworkRegistry.TryAddAttachment(filePath, "the received snapshot");

		await Expect.That(isAttached).IsFalse()
			.Because("NUnit throws for a missing file, which the adapter catches");
	}

	[Test]
	public async Task TryGetCancellationToken_ShouldReturnTheTokenOfTheTest()
	{
		bool hasToken = TestFrameworkRegistry.TryGetCancellationToken(out CancellationToken cancellationToken);

		await Expect.That(hasToken).IsTrue();
		await Expect.That(cancellationToken).IsEqualTo(TestContext.CurrentContext.CancellationToken);
	}

	[TestCase("Abbey Road", 1969)]
	[TestCase("Revolver", 1966)]
	public async Task TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestCase(string title, int year)
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue()
			.Because("NUnit 5 exposes the running test");
		await Expect.That(currentTest!.Identity.ClassName).IsEqualTo(typeof(NUnit5TestFrameworkTests).FullName);
		await Expect.That(currentTest.Identity.MethodName).IsEqualTo(nameof(TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestCase));
		await Expect.That(currentTest.Identity.Arguments).IsEqualTo(new object?[] { title, year, });
	}

	[Test]
	public async Task TryGetCurrentTest_ShouldReturnTheIdAndTheDisplayName()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.Id).IsEqualTo(TestContext.CurrentContext.Test.ID);
		await Expect.That(currentTest.DisplayName).IsEqualTo(TestContext.CurrentContext.Test.Name);
		await Expect.That(currentTest.RetryAttempt).IsEqualTo(0);
	}

	[Test]
	[Repeat(2)]
	public async Task TryGetCurrentTest_WhenRepeated_ShouldCountTheRuns()
	{
		int run = Interlocked.Increment(ref _repeatedRuns) - 1;

		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.RetryAttempt).IsEqualTo(run)
			.Because("NUnit counts the repetitions of a test like its retries");
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
