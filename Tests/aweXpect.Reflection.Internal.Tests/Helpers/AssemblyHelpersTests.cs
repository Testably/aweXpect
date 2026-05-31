using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Internal.Tests.Helpers;

public class AssemblyHelpersTests
{
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
}
