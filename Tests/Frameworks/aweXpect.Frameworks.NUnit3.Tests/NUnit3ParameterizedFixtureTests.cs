using System.Threading.Tasks;
using aweXpect.Core.Adapters;
using NUnit.Framework;

namespace aweXpect.Frameworks.NUnit3.Tests;

[TestFixture("de-DE")]
[TestFixture("en-US")]
public sealed class NUnit3ParameterizedFixtureTests(string culture)
{
	[TestCase("Abbey Road")]
	public async Task TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestClass(string title)
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.Identity.ClassArguments).IsEmpty()
			.Because($"NUnit 3 has no parent of the test in its context, so the fixture for {culture} cannot be found");
		await Expect.That(currentTest.Identity.Arguments).IsEqualTo(new object?[] { title, });
	}
}
