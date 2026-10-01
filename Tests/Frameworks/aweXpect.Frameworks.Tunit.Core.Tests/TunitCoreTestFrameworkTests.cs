using System;
using System.Threading.Tasks;
using TUnit.Core.Exceptions;

namespace aweXpect.Frameworks.Tunit.Core.Tests;

public sealed class TunitCoreTestFrameworkTests
{
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
}
