using aweXpect.Customization;

namespace aweXpect.Core.Tests.Customization;

public sealed class CustomizeReflectionTests
{
	[Fact]
	public async Task ExcludeAssemblies_ShouldChangeTheExcludedAssemblyPrefixes()
	{
		string additionalExcludedAssemblyNamespace = "foo";

		await That(Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get())
			.DoesNotContain(additionalExcludedAssemblyNamespace);

		using (IDisposable _ = Customize.aweXpect.Reflection().Update(p => p with
		       {
			       ExcludedAssemblyPrefixes = [..p.ExcludedAssemblyPrefixes, additionalExcludedAssemblyNamespace,],
		       }))
		{
			await That(Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get())
				.Contains(additionalExcludedAssemblyNamespace);
		}

		await That(Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get())
			.DoesNotContain(additionalExcludedAssemblyNamespace);
	}

	[Fact]
	public async Task ExcludedAssemblyPrefixes_ShouldBeInitializedCorrectly()
	{
		AwexpectCustomization.ReflectionCustomization reflection = Customize.aweXpect.Reflection();

		await That(reflection.ExcludedAssemblyPrefixes.Get()).IsEqualTo([
			"mscorlib",
			"System",
			"Microsoft",
			"netstandard",
			"WindowsBase",
			"JetBrains",
			"xunit",
			"Castle",
			"DynamicProxyGenAssembly2",
		]).InAnyOrder();
	}

	[Fact]
	public async Task Reflection_ShouldReturnSameInstance()
	{
		AwexpectCustomization.ReflectionCustomization reflection1 = Customize.aweXpect.Reflection();
		AwexpectCustomization.ReflectionCustomization reflection2 = Customize.aweXpect.Reflection();

		await That(reflection1).IsSameAs(reflection2);
	}
}
