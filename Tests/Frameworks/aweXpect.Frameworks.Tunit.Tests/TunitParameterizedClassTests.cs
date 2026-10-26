using System.Threading.Tasks;
using aweXpect.Core.Adapters;

namespace aweXpect.Frameworks.Tunit.Tests;

[Arguments("de-DE")]
[Arguments("en-US")]
public sealed class TunitParameterizedClassTests(string culture)
{
	[Test]
	[Arguments("Abbey Road")]
	public async Task TryGetCurrentTest_ShouldReturnTheArgumentsOfTheTestClass(string title)
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.Identity.ClassArguments).IsEqualTo(new object?[] { culture, });
		await Expect.That(currentTest.Identity.Arguments).IsEqualTo(new object?[] { title, });
	}
}
