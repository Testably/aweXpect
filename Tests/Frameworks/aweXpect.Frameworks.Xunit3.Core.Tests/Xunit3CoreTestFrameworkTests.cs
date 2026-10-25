using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Adapters;
using Xunit;

namespace aweXpect.Frameworks.Xunit3.Core.Tests;

public class Xunit3TestFrameworkTests
{
	[Fact]
	public async Task OnFail_WhenUsingXunit3AsTestFramework_ShouldThrowXunitException()
	{
		void Act()
			=> Fail.Test("my message");

		Exception exception = await Expect.That(Act).Throws()
			.WithMessage("my message");
		await Expect.That(exception.GetType().GetInterfaces().Select(e => e.Name))
			.Contains("IAssertionException");
	}

	[Fact]
	public async Task OnFailWithCause_WhenUsingXunit3AsTestFramework_ShouldForwardTheCauseAsInnerException()
	{
		Exception cause = new InvalidOperationException("my cause");

		void Act()
			=> Fail.Test("my message", cause);

		await Expect.That(Act).Throws()
			.Whose(e => e.InnerException, i => i.IsSameAs(cause));
	}

	[Fact]
	public async Task OnInconclusive_WhenUsingXunit3AsTestFramework_ShouldThrowXunitException()
	{
		void Act()
			=> Fail.Inconclusive("my message");

		Exception exception = await Expect.That(Act).Throws()
			.WithMessage("my message");
		await Expect.That(exception.GetType().GetInterfaces().Select(e => e.Name))
			.Contains("ITestTimeoutException");
	}

	[Fact]
	public async Task OnSkip_WhenUsingXunit3AsTestFramework_ShouldThrowSkipException()
	{
		void Act()
			=> Skip.Test("my message");

		await Expect.That(Act).Throws()
			.WithMessage("$XunitDynamicSkip$my message");
	}

	[Fact]
	public async Task TryAddAttachment_ShouldAttachTheFile()
	{
		string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
		File.WriteAllText(filePath, "received");

		bool isAttached = TestFrameworkRegistry.TryAddAttachment(filePath, "the received snapshot");

		await Expect.That(isAttached).IsTrue();
		await Expect.That(TestContext.Current.Attachments!.ContainsKey(Path.GetFileName(filePath))).IsTrue()
			.Because("xunit v3 attaches the content of the file under its name");
	}

	[Fact]
	public async Task TryGetCancellationToken_ShouldReturnTheTokenOfTheTest()
	{
		bool hasToken = TestFrameworkRegistry.TryGetCancellationToken(out CancellationToken cancellationToken);

		await Expect.That(hasToken).IsTrue();
		await Expect.That(cancellationToken).IsEqualTo(TestContext.Current.CancellationToken);
	}

	[Theory]
	[InlineData("Abbey Road", 1969)]
	[InlineData("Revolver", 1966)]
	public async Task TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestCase(string title, int year)
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out TestIdentity? currentTest);

		await Expect.That(isRunning).IsTrue()
			.Because("xunit v3 without its assertions exposes the running test");
		await Expect.That(currentTest!.ClassName).IsEqualTo(typeof(Xunit3TestFrameworkTests).FullName);
		await Expect.That(currentTest.MethodName).IsEqualTo(nameof(TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestCase));
		await Expect.That(currentTest.Arguments).IsEqualTo(new object?[] { title, year, });
	}

	[Fact]
	public async Task TryGetCurrentTest_ShouldReturnTheIdAndTheDisplayName()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out TestIdentity? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.Id).IsEqualTo(TestContext.Current.Test!.UniqueID);
		await Expect.That(currentTest.DisplayName).IsEqualTo(TestContext.Current.Test.TestDisplayName);
		await Expect.That(currentTest.RetryAttempt).IsEqualTo(0)
			.Because("xunit v3 does not retry tests");
	}

	[Fact]
	public async Task TryGetCurrentTest_WithoutParameters_ShouldReturnNoArguments()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out TestIdentity? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.MethodName).IsEqualTo(nameof(TryGetCurrentTest_WithoutParameters_ShouldReturnNoArguments));
		await Expect.That(currentTest.Arguments).IsEmpty();
	}
}
