using aweXpect.Customization;

namespace aweXpect.Core.Tests.Customization;

public sealed class CustomizeReflectionTests
{
	[Test]
	public async Task ExcludeAssemblies_ShouldChangeTheExcludedAssemblyPrefixes()
	{
		string additionalExcludedAssemblyNamespace = "foo";

		await That(Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get())
			.DoesNotContain(additionalExcludedAssemblyNamespace);

		using (IDisposable _ = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Set([
			       ..Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get(),
			       additionalExcludedAssemblyNamespace,
		       ]))
		{
			await That(Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get())
				.Contains(additionalExcludedAssemblyNamespace);
		}

		await That(Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get())
			.DoesNotContain(additionalExcludedAssemblyNamespace);
	}

	[Test]
	public async Task ExcludedAssemblyPrefixes_ChangingTheReturnedArray_ShouldNotChangeTheSetting()
	{
		string[] prefixes = Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get();
		string original = prefixes[0];
		prefixes[0] = "CHANGED";

		await That(Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Get()[0]).IsEqualTo(original)
			.Because("a change of the returned array would bypass the scoping of the setting");
	}

	[Test]
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

	[Test]
	public async Task ExcludedAssemblyPrefixes_WhenNull_ShouldThrowArgumentNullException()
	{
		void Act() => Customize.aweXpect.Reflection().ExcludedAssemblyPrefixes.Set(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithParamName("prefixes").And
			.WithMessage("The 'prefixes' cannot be null.").AsPrefix()
			.Because("a stored null would be returned as null instead of the default prefixes");
	}

	[Test]
	public async Task Reflection_ShouldReturnSameInstance()
	{
		AwexpectCustomization.ReflectionCustomization reflection1 = Customize.aweXpect.Reflection();
		AwexpectCustomization.ReflectionCustomization reflection2 = Customize.aweXpect.Reflection();

		await That(reflection1).IsSameAs(reflection2);
	}
}
