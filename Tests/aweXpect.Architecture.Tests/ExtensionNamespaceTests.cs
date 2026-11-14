namespace aweXpect.Architecture.Tests;

public sealed class ExtensionNamespaceTests
{
	[Test]
	[MethodDataSource(typeof(Packages), nameof(Packages.ExtensionNames))]
	public async Task EntryPoints_ShouldBeInTheAweXpectNamespace(string package)
		=> await That(Packages.PublicTypes(Packages.Extension(package))
				.Which(type => type.Name.StartsWith("That", StringComparison.Ordinal)))
			.HaveNamespace("aweXpect")
			.Because("the That<Subject> classes need no extra using");

	[Test]
	[MethodDataSource(typeof(Packages), nameof(Packages.ExtensionNames))]
	public async Task OtherTypes_ShouldBeInTheNamespaceOfThePackage(string package)
		=> await That(Packages.PublicTypes(Packages.Extension(package))
				.Which(type => type.Namespace != "aweXpect"))
			.AreWithinNamespace(package)
			.Because("types outside the namespace of the package could clash with Core or another package");

	[Test]
	[MethodDataSource(typeof(Packages), nameof(Packages.ExtensionNames))]
	public async Task Results_ShouldBeInTheResultsNamespaceOfThePackage(string package)
		=> await That(Packages.PublicTypes(Packages.Extension(package)).Which(Packages.IsResult))
			.HaveNamespace($"{package}.Results")
			.Because("the results of a package have a namespace of their own");

	[Test]
	[MethodDataSource(typeof(Packages), nameof(Packages.ExtensionNames))]
	public async Task TypesInTheAweXpectNamespace_ShouldBeStaticClasses(string package)
		=> await That(Packages.PublicTypes(Packages.Extension(package))
				.Which(type => type.Namespace == "aweXpect"))
			.AreStatic()
			.Because("only the entry points and extension classes belong into the namespace aweXpect");
}
