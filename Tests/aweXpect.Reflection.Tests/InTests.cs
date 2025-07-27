using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests;

// ReSharper disable PossibleMultipleEnumeration
public sealed class InTests
{
	[Fact]
	public async Task AllLoadedAssemblies_ShouldContainMultipleAssemblies()
	{
		Filtered.Assemblies sut = In.AllLoadedAssemblies();

		await That(sut).HasCount().AtLeast(5);
		await That(sut).All().Satisfy(x => !x.FullName!.StartsWith("System."));
		await That(sut.GetDescription()).IsEqualTo("in all loaded assemblies");
	}

	[Fact]
	public async Task AllLoadedAssemblies_WithPredicate_ShouldApplyPredicate()
	{
		Filtered.Assemblies sut = In.AllLoadedAssemblies()
			.WhichSatisfy(assembly => assembly.FullName!.StartsWith("aweXpect."));

		await That(sut).HasCount().Between(2).And(4);
		await That(sut.GetDescription())
			.IsEqualTo("in all loaded assemblies matching assembly => assembly.FullName!.StartsWith(\"aweXpect.\")");
	}

	[Fact]
	public async Task Assemblies_WithArray_ShouldContainProvidedAssemblies()
	{
		Assembly[] assemblies = [typeof(In).Assembly, typeof(InTests).Assembly,];

		Filtered.Assemblies sut = In.Assemblies(assemblies);

		await That(sut).IsEqualTo(assemblies);
		await That(sut.GetDescription())
			.IsEqualTo("in the assemblies [aweXpect.Reflection*, aweXpect.Reflection.Tests*]").AsWildcard();
	}

	[Fact]
	public async Task Assemblies_WithEnumerable_ShouldContainProvidedAssemblies()
	{
		IEnumerable<Assembly> assemblies = [typeof(In).Assembly, typeof(InTests).Assembly,];

		Filtered.Assemblies sut = In.Assemblies(assemblies);

		await That(sut).IsEqualTo(assemblies);
		await That(sut.GetDescription())
			.IsEqualTo("in the assemblies [aweXpect.Reflection*, aweXpect.Reflection.Tests*]").AsWildcard();
	}

	[Fact]
	public async Task AssemblyContaining_WithGeneric_ShouldContainAssemblyOfProvidedType()
	{
		Filtered.Assemblies sut = In.AssemblyContaining<InTests>();

		await That(sut).HasSingle().Which
			.IsEqualTo(typeof(InTests).Assembly);
		await That(sut.GetDescription()).IsEqualTo("in assembly containing type InTests");
	}

	[Fact]
	public async Task AssemblyContaining_WithType_ShouldContainAssemblyOfProvidedType()
	{
		Filtered.Assemblies sut = In.AssemblyContaining(typeof(In));

		await That(sut).HasSingle().Which
			.IsEqualTo(typeof(In).Assembly);
		await That(sut.GetDescription()).IsEqualTo("in assembly containing type In");
	}

	[Fact]
	public async Task EntryAssembly_ShouldContainExpectedAssembly()
	{
		Assembly? expectedAssembly = Assembly.GetEntryAssembly();

		Filtered.Assemblies sut = In.EntryAssembly();

#if NET8_0_OR_GREATER
		await That(sut).HasSingle().Which
			.IsEqualTo(expectedAssembly);
#else
		await That(sut).IsEmpty();
#endif
		await That(sut.GetDescription()).IsEqualTo("in entry assembly");
	}

	[Fact]
	public async Task ExecutingAssembly_ShouldContainExpectedAssembly()
	{
		Assembly expectedAssembly = typeof(In).Assembly;

		Filtered.Assemblies sut = In.ExecutingAssembly();

		await That(sut).HasSingle().Which
			.IsEqualTo(expectedAssembly);
		await That(sut.GetDescription()).IsEqualTo("in executing assembly");
	}
}
