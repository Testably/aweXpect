using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace aweXpect.Frameworks.NUnit4.Tests;

public sealed class NUnit4TestFrameworkTests
{
	[Test]
	public async Task OnFail_WhenUsingNUnit4AsTestFramework_ShouldThrowAssertionException()
	{
		void Act()
			=> Fail.Test("my message");

		await Expect.That(Act).Throws<AssertionException>()
			.WithMessage("my message");
	}

	[Test]
	public async Task OnFailWithCause_WhenUsingNUnit4AsTestFramework_ShouldForwardTheCauseAsInnerException()
	{
		Exception cause = new InvalidOperationException("my cause");

		void Act()
			=> Fail.Test("my message", cause);

		await Expect.That(Act).Throws<AssertionException>()
			.Whose(e => e.InnerException, i => i.IsSameAs(cause));
	}

	[Test]
	public async Task OnInconclusive_WhenUsingNUnit3AsTestFramework_ShouldThrowAssertionException()
	{
		void Act()
			=> Fail.Inconclusive("my message");

		await Expect.That(Act).Throws<NUnit.Framework.InconclusiveException>()
			.WithMessage("my message");
	}

	[Test]
	public async Task OnSkip_WhenUsingNUnit4AsTestFramework_ShouldThrowIgnoreException()
	{
		void Act()
			=> Skip.Test("my message");

		await Expect.That(Act).Throws<IgnoreException>()
			.WithMessage("my message");
	}

	[Test]
	public async Task TestFramework_ShouldHaveMajorVersion4()
	{
		Version? version = typeof(TestAttribute).Assembly.GetName().Version;

		await Expect.That(version?.Major).IsEqualTo(4)
			.Because("this project tests the adapter against NUnit 4");
	}
}
