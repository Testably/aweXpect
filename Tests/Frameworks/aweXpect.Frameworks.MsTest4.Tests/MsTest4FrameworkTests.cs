using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize]

namespace aweXpect.Frameworks.MsTest4.Tests;

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
	public async Task TestFramework_ShouldHaveMajorVersion4()
	{
		// The assembly version of MSTest does not follow the package version (MSTest 3 is fixed at 14.0.0.0).
		string? version = typeof(TestClassAttribute).Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

		await Expect.That(version).StartsWith("4.")
			.Because("this project tests the adapter against MSTest 4");
	}
}
