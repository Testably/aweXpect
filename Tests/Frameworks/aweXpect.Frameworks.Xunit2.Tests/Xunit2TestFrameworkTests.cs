using System;
using System.Threading.Tasks;
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
}
