using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace aweXpect.Frameworks.Xunit3.Tests;

public class Xunit3TestFrameworkTests
{
	[Fact]
	public async Task OnFail_WhenUsingXunit3AsTestFramework_ShouldThrowXunitException()
	{
		void Act()
			=> Fail.Test("my message");

		await Expect.That(Act).Throws<XunitException>();
	}

	[Fact]
	public async Task OnFailWithCause_WhenUsingXunit3AsTestFramework_ShouldForwardTheCauseAsInnerException()
	{
		Exception cause = new InvalidOperationException("my cause");

		void Act()
			=> Fail.Test("my message", cause);

		await Expect.That(Act).Throws<XunitException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(cause));
	}

	[Fact]
	public async Task OnInconclusive_WhenUsingXunit3AsTestFramework_ShouldThrowXunitException()
	{
		void Act()
			=> Fail.Inconclusive("my message");

		InconclusiveException exception = await Expect.That(Act).Throws<InconclusiveException>()
			.WithMessage("my message");
		await Expect.That(exception.GetType().GetInterfaces().Select(e => e.Name))
			.Contains("ITestTimeoutException");
	}

	[Fact]
	public async Task OnSkip_WhenUsingXunit3AsTestFramework_ShouldThrowSkipException()
	{
		void Act()
			=> Skip.Test("my message");

		await Expect.That(Act).Throws<SkipException>()
			.WithMessage("$XunitDynamicSkip$my message");
	}
}
