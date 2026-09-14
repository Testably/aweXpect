using System.Reflection;
using System.Runtime.CompilerServices;

namespace aweXpect.Tests;

public sealed class TypeMetadataRegistrationTests
{
#if NET8_0_OR_GREATER
	[Fact]
	public async Task ShouldRegisterTheTypeMetadataFromAModuleInitializer()
	{
		MethodInfo? registration = GetRegistrationMethod();

		await That(registration).IsNotNull()
			.Because("the generator emits a registration for the types that reach an equivalency comparison");
		await That(registration!.GetCustomAttributes(typeof(ModuleInitializerAttribute), false)).IsNotEmpty()
			.Because("the metadata has to be registered before any other code of this assembly runs");
	}
#else
	[Fact]
	public async Task ShouldNotRegisterTheTypeMetadata()
	{
		await That(GetRegistrationMethod()).IsNull()
			.Because("a target framework without `ModuleInitializerAttribute` cannot be trimmed or published with Native AOT");
	}
#endif

	private static MethodInfo? GetRegistrationMethod()
		=> typeof(TypeMetadataRegistrationTests).Assembly
			.GetType("aweXpect.Generators.TypeMetadataRegistration")
			?.GetMethod("Register", BindingFlags.Static | BindingFlags.NonPublic);
}
