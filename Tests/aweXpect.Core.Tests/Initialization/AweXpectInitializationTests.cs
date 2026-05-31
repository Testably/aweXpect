using aweXpect.Core.Initialization;

namespace aweXpect.Core.Tests.Initialization;

public sealed class AweXpectInitializationTests
{
	[Theory]
	[InlineData("System", "exact framework assembly name")]
	[InlineData("System.Net.Http", "sub-name of an excluded prefix")]
	[InlineData("Microsoft.Extensions.Logging")]
	[InlineData("netstandard")]
	[InlineData("WindowsBase")]
	[InlineData("xunit.core")]
	[InlineData("DynamicProxyGenAssembly2")]
	public async Task IsAssemblyNameIncluded_WhenExcludedByDefault_ShouldReturnFalse(string assemblyName, string? because = null)
	{
		bool included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);

		await That(included).IsEqualTo(false).Because(because);
	}

	[Theory]
	[InlineData("Systemics", "shares the \"System\" prefix, but not at a name boundary")]
	[InlineData("Microsoftish", "shares the \"Microsoft\" prefix, but not at a name boundary")]
	[InlineData("WindowsBaseExtensions", "shares the \"WindowsBase\" prefix, but not at a name boundary")]
	[InlineData("MyCompany.Product")]
	public async Task IsAssemblyNameIncluded_WhenNotExcluded_ShouldReturnTrue(string assemblyName, string? because = null)
	{
		bool included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);

		await That(included).IsEqualTo(true).Because(because);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public async Task IsAssemblyNameIncluded_WithoutName_ShouldReturnFalse(string? assemblyName)
	{
		bool included = AweXpectInitialization.IsAssemblyNameIncluded(assemblyName);

		await That(included).IsEqualTo(false);
	}
}
