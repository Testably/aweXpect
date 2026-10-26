using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize]

namespace aweXpect.Frameworks.MsTest4.Tests;

[TestClass]
public sealed class MsTestFrameworkTests
{
	public TestContext TestContext { get; set; } = null!;

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
	public async Task TestFramework_ShouldHaveMajorVersion4()
	{
		// The assembly version of MSTest does not follow the package version (MSTest 3 is fixed at 14.0.0.0).
		string? version = typeof(TestClassAttribute).Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

		await Expect.That(version).StartsWith("4.")
			.Because("this project tests the adapter against MSTest 4");
	}

	[TestMethod]
	public async Task TryAddAttachment_ShouldAttachTheFile()
	{
		string filePath = Path.Combine(AppContext.BaseDirectory, "mstest4-attachment.txt");
		File.WriteAllText(filePath, "received");

		bool isAttached = TestFrameworkRegistry.TryAddAttachment(filePath, "the received snapshot");

		await Expect.That(isAttached).IsTrue()
			.Because("the project opts in to the experimental TestContext.Current with AweXpectExperimentalMSTest");
	}

	[TestMethod]
	public async Task TryAddAttachment_WhenFileDoesNotExist_ShouldReturnFalse()
	{
		string filePath = Path.Combine(AppContext.BaseDirectory, $"{Guid.NewGuid()}.txt");

		bool isAttached = TestFrameworkRegistry.TryAddAttachment(filePath, "the received snapshot");

		await Expect.That(isAttached).IsFalse()
			.Because("MSTest would only fail for the missing file when the test ends");
	}

	[TestMethod]
	public async Task TryGetCancellationToken_ShouldReturnTheTokenOfTheTest()
	{
		bool hasToken = TestFrameworkRegistry.TryGetCancellationToken(out CancellationToken cancellationToken);

		await Expect.That(hasToken).IsTrue();
		await Expect.That(cancellationToken).IsEqualTo(TestContext.CancellationToken);
	}

	[TestMethod]
	[DataRow("Abbey Road", 1969)]
	[DataRow("Revolver", 1966)]
	public async Task TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestCase(string title, int year)
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue()
			.Because("the project opts in to the experimental TestContext.Current with AweXpectExperimentalMSTest");
		await Expect.That(currentTest!.Identity.ClassName).IsEqualTo(typeof(MsTestFrameworkTests).FullName);
		await Expect.That(currentTest.Identity.MethodName)
			.IsEqualTo(nameof(TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestCase));
		await Expect.That(currentTest.Identity.Arguments).IsEqualTo(new object?[] { title, year, });
		await Expect.That(currentTest.Identity.ClassArguments).IsEmpty();
	}
}
