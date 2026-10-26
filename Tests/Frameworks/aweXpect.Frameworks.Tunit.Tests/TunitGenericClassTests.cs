using System.Threading.Tasks;
using aweXpect.Core.Adapters;

namespace aweXpect.Frameworks.Tunit.Tests;

[GenerateGenericTest(typeof(int))]
public sealed class TunitGenericClassTests<T>
{
	[Test]
	public async Task TryGetCurrentTest_ShouldReturnTheClassNameWithoutAssemblies()
	{
		bool isRunning = TestFrameworkRegistry.TryGetCurrentTest(out RunningTest? currentTest);

		await Expect.That(isRunning).IsTrue();
		await Expect.That(currentTest!.Identity.ClassName).IsEqualTo(typeof(TunitGenericClassTests<T>).ToString())
			.Because($"the full name of a generic test class names the assembly and the version of the runtime of {typeof(T).Name}");
	}
}
