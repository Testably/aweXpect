using System.Reflection;
using System.Runtime.Versioning;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public class AssemblyHelpersTests
{
	[Test]
	public async Task HasAttribute_WithAttributeType_WithoutPredicate_WhenAttributePresent_ShouldReturnTrue()
	{
		Assembly assembly = typeof(AssemblyHelpersTests).Assembly;

		bool result = assembly.HasAttribute(typeof(TargetFrameworkAttribute));

		await That(result).IsTrue();
	}

	[Test]
	public async Task HasAttribute_WithAttributeType_WithPredicate_ShouldReturnPredicateResult()
	{
		Assembly assembly = typeof(AssemblyHelpersTests).Assembly;

		bool result1 = assembly.HasAttribute(typeof(TargetFrameworkAttribute),
			a => ((TargetFrameworkAttribute)a).FrameworkName.Length > 0);
		bool result2 = assembly.HasAttribute(typeof(TargetFrameworkAttribute),
			a => ((TargetFrameworkAttribute)a).FrameworkName.Length < 0);

		await That(result1).IsTrue();
		await That(result2).IsFalse();
	}

	[Test]
	[Arguments(".NETCoreApp,Version=v5.0", "net5.0")]
	[Arguments(".NETCoreApp,Version=v6.0", "net6.0")]
	[Arguments(".NETCoreApp,Version=v7.0", "net7.0")]
	[Arguments(".NETCoreApp,Version=v8.0", "net8.0")]
	[Arguments(".NETCoreApp,Version=v9.0", "net9.0")]
	[Arguments(".NETCoreApp,Version=v10.0", "net10.0")]
	[Arguments(".NETCoreApp,Version=v3.1", "netcoreapp3.1")]
	[Arguments(".NETStandard,Version=v2.0", "netstandard2.0")]
	[Arguments(".NETStandard,Version=v2.1", "netstandard2.1")]
	[Arguments(".NETFramework,Version=v4.8", "net48")]
	[Arguments(".NETFramework,Version=v4.7.2", "net472")]
	[Arguments(".NETFramework,Version=v4.6.1", "net461")]
	[Arguments(".NETFramework,Version=v4.5.1", "net451")]
	[Arguments(".NETFramework,Version=v4.5", "net45")]
	[Arguments(".NETFramework,Version=v4.0", "net40")]
	[Arguments(".NETFramework,Version=v3.5", "net35")]
	[Arguments(".NETFramework,Version=v2.0", "net20")]
	[Arguments(".NETCoreApp,Version=v8.0,Profile=foo", "net8.0")]
	public async Task MapFrameworkName_ShouldMapToShortMoniker(string frameworkName, string expected)
	{
		string? result = AssemblyHelpers.MapFrameworkName(frameworkName);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(null)]
	[Arguments("")]
	[Arguments("   ")]
	public async Task MapFrameworkName_WhenBlank_ShouldReturnNull(string? frameworkName)
	{
		string? result = AssemblyHelpers.MapFrameworkName(frameworkName);

		await That(result).IsNull();
	}

	[Test]
	public async Task MapFrameworkName_WhenUnparsable_ShouldReturnInput()
	{
		string? result = AssemblyHelpers.MapFrameworkName("SomethingWithoutAVersion");

		await That(result).IsEqualTo("SomethingWithoutAVersion");
	}
}

#pragma warning restore CA2263 // Prefer generic overload when type is known
