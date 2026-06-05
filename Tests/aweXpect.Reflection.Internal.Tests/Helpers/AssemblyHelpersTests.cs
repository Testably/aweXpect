using System.Reflection;
using System.Runtime.Versioning;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

#pragma warning disable CA2263 // Prefer generic overload when type is known

public class AssemblyHelpersTests
{
	[Fact]
	public async Task HasAttribute_WithAttributeType_WithoutPredicate_WhenAttributePresent_ShouldReturnTrue()
	{
		Assembly assembly = typeof(AssemblyHelpersTests).Assembly;

		bool result = assembly.HasAttribute(typeof(TargetFrameworkAttribute));

		await That(result).IsTrue();
	}

	[Fact]
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

	[Theory]
	[InlineData(".NETCoreApp,Version=v5.0", "net5.0")]
	[InlineData(".NETCoreApp,Version=v6.0", "net6.0")]
	[InlineData(".NETCoreApp,Version=v7.0", "net7.0")]
	[InlineData(".NETCoreApp,Version=v8.0", "net8.0")]
	[InlineData(".NETCoreApp,Version=v9.0", "net9.0")]
	[InlineData(".NETCoreApp,Version=v10.0", "net10.0")]
	[InlineData(".NETCoreApp,Version=v3.1", "netcoreapp3.1")]
	[InlineData(".NETStandard,Version=v2.0", "netstandard2.0")]
	[InlineData(".NETStandard,Version=v2.1", "netstandard2.1")]
	[InlineData(".NETFramework,Version=v4.8", "net48")]
	[InlineData(".NETFramework,Version=v4.7.2", "net472")]
	[InlineData(".NETFramework,Version=v4.6.1", "net461")]
	[InlineData(".NETFramework,Version=v4.5.1", "net451")]
	[InlineData(".NETFramework,Version=v4.5", "net45")]
	[InlineData(".NETFramework,Version=v4.0", "net40")]
	[InlineData(".NETFramework,Version=v3.5", "net35")]
	[InlineData(".NETFramework,Version=v2.0", "net20")]
	[InlineData(".NETCoreApp,Version=v8.0,Profile=foo", "net8.0")]
	public async Task MapFrameworkName_ShouldMapToShortMoniker(string frameworkName, string expected)
	{
		string? result = AssemblyHelpers.MapFrameworkName(frameworkName);

		await That(result).IsEqualTo(expected);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public async Task MapFrameworkName_WhenBlank_ShouldReturnNull(string? frameworkName)
	{
		string? result = AssemblyHelpers.MapFrameworkName(frameworkName);

		await That(result).IsNull();
	}

	[Fact]
	public async Task MapFrameworkName_WhenUnparsable_ShouldReturnInput()
	{
		string? result = AssemblyHelpers.MapFrameworkName("SomethingWithoutAVersion");

		await That(result).IsEqualTo("SomethingWithoutAVersion");
	}

	[Theory]
	[InlineData("System", "System", true)]
	[InlineData("System.Text.Json", "System", true)]
	[InlineData("SystemsBiology.Core", "System", false)]
	[InlineData("SystemsBiology", "System", false)]
	// A customized prefix written with a trailing dot is boundary-safe by construction.
	[InlineData("MyCompany.Domain", "MyCompany.", true)]
	[InlineData("MyCompany.", "MyCompany.", true)]
	[InlineData("MyCompany", "MyCompany.", false)]
	[InlineData("MyCompanyOther", "MyCompany.", false)]
	public async Task IsExcludedAssemblyName_ShouldMatchAtNameSegmentBoundary(
		string assemblyName, string prefix, bool expected)
	{
		bool result = assemblyName.IsExcludedAssemblyName([prefix,]);

		await That(result).IsEqualTo(expected);
	}

	[Fact]
	public async Task IsExcludedAssemblyName_WhenAssemblyNameIsNull_ShouldReturnFalse()
	{
		string? assemblyName = null;

		bool result = assemblyName.IsExcludedAssemblyName(["System",]);

		await That(result).IsFalse();
	}

	[Fact]
	public async Task IsExcludedAssemblyName_WhenPrefixIsEmpty_ShouldBeIgnored()
	{
		// An empty prefix cannot identify an assembly; it must neither exclude everything
		// (the old StartsWith behavior) nor be silently evaluated against the boundary check.
		bool result = "AnyAssembly".IsExcludedAssemblyName(["",]);

		await That(result).IsFalse();
	}
}

#pragma warning restore CA2263 // Prefer generic overload when type is known
