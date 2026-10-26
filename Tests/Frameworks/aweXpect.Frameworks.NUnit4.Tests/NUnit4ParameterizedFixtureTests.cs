using System.Threading.Tasks;
using aweXpect.Core.Adapters;
using NUnit.Framework;

namespace aweXpect.Frameworks.NUnit4.Tests;

[TestFixture("de-DE")]
[TestFixture("en-US")]
public sealed class NUnit4ParameterizedFixtureTests(string culture)
{
	[TestCase("Abbey Road")]
	public async Task TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestClass(string title)
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.Identity.ClassArguments).IsEqualTo(new object?[] { culture, })
			.Because("the fixture above the suite of the parameterized test method holds the arguments of the test class");
		await Expect.That(currentTest.Identity.Arguments).IsEqualTo(new object?[] { title, });
	}

	[Test]
	public async Task TryGetCurrentTest_WithoutParameters_ShouldReturnTheArgumentsOfTheTestClass()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.Identity.ClassArguments).IsEqualTo(new object?[] { culture, })
			.Because("the fixture is the parent of a test method without parameters");
		await Expect.That(currentTest.Identity.Arguments).IsEmpty();
	}
}
