namespace aweXpect.Architecture.Tests;

public sealed class DependencyTests
{
	[Test]
	public async Task ExtensionPackages_ShouldNotDependOnAweXpect()
		=> await That(In.Assemblies(Packages.Extensions)).DoNotDependOn("aweXpect")
			.Because("an extension builds only on aweXpect.Core, like a third-party extension would");

	[Test]
	public async Task Snapshots_ShouldNotDependOnRoslyn()
		=> await That(typeof(ThatSnapshot).Assembly).DoesNotDependOn("Microsoft.CodeAnalysis*").AsWildcard()
			.Because("Roslyn cannot be trimmed, so only aweXpect.Snapshots.PublicApi may use it");
}
