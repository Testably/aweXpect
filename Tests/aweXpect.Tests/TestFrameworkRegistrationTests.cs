using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace aweXpect.Tests;

public sealed class TestFrameworkRegistrationTests
{
#if NET8_0_OR_GREATER
	[Fact]
	public async Task ShouldRegisterTheDetectedAdapterFromAModuleInitializer()
	{
		MethodInfo[] registrations = GetRegistrationMethods();

		await That(registrations).HasCount(1)
			.Because("the generator emits a registration for the detected test framework");
		await That(registrations[0].GetCustomAttributes(typeof(ModuleInitializerAttribute), false)).IsNotEmpty()
			.Because("the adapter has to be registered before any other code of this assembly runs");
	}
#else
	[Fact]
	public async Task ShouldNotRegisterTheDetectedAdapter()
	{
		await That(GetRegistrationMethods()).IsEmpty()
			.Because("a target framework without `ModuleInitializerAttribute` cannot be trimmed or published with Native AOT");
	}
#endif

	private static MethodInfo[] GetRegistrationMethods()
		=> typeof(TestFrameworkRegistrationTests).Assembly
			.GetTypes()
			.Where(x => x.Namespace == "aweXpect.Frameworks" &&
			            x.Name.EndsWith("Registration", StringComparison.Ordinal))
			.Select(x => x.GetMethod("Register", BindingFlags.Static | BindingFlags.NonPublic))
			.Where(x => x is not null)
			.Select(x => x!)
			.ToArray();
}
