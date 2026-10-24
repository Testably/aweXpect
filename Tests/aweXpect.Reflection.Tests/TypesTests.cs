using System.Collections.Generic;
using System.Reflection;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;

namespace aweXpect.Reflection.Tests;

// ReSharper disable PossibleMultipleEnumeration
public sealed class TypesTests
{
	[Test]
	public async Task InAllLoadedAssemblies_ShouldContainTypesFromMultipleAssemblies()
	{
		Filtered.Types sut = Types.InAllLoadedAssemblies();

		await That(sut).Contains(typeof(TypesTests));
		await That(sut).Contains(typeof(In));
		await That(sut.GetDescription()).IsEqualTo("types in all loaded assemblies");
	}

	[Test]
	public async Task InAssemblies_WithEnumerable_ShouldContainTypesFromProvidedAssemblies()
	{
		IEnumerable<Assembly?> assemblies = [typeof(TypesTests).Assembly,];

		Filtered.Types sut = Types.InAssemblies(assemblies);

		await That(sut).Contains(typeof(TypesTests));
		await That(sut).DoesNotContain(typeof(In));
	}

	[Test]
	public async Task InAssemblies_WithMultipleAssemblies_ShouldContainTypesFromProvidedAssemblies()
	{
		Filtered.Types sut = Types.InAssemblies(typeof(In).Assembly, typeof(TypesTests).Assembly);

		await That(sut).Contains(typeof(In));
		await That(sut).Contains(typeof(TypesTests));
		await That(sut.GetDescription())
			.IsEqualTo("types in the assemblies [aweXpect.Reflection*, aweXpect.Reflection.Tests*]").AsWildcard();
	}

	[Test]
	public async Task InAssemblyContaining_WithGeneric_ShouldContainTypesFromAssemblyOfProvidedType()
	{
		Filtered.Types sut = Types.InAssemblyContaining<TypesTests>();

		await That(sut).Contains(typeof(TypesTests));
		await That(sut).DoesNotContain(typeof(In));
		await That(sut.GetDescription()).IsEqualTo("types in assembly containing type TypesTests");
	}

	[Test]
	public async Task InAssemblyContaining_WithType_ShouldContainTypesFromAssemblyOfProvidedType()
	{
		Filtered.Types sut = Types.InAssemblyContaining(typeof(In));

		await That(sut).Contains(typeof(In));
		await That(sut).DoesNotContain(typeof(TypesTests));
		await That(sut.GetDescription()).IsEqualTo("types in assembly containing type In");
	}

	[Test]
	public async Task InNamespace_AfterClarification_OriginalShouldStillUseAllLoadedAssemblies()
	{
		Filtered.Types.InNamespaceResult sut = Types.InNamespace(NamespaceScope);
		_ = sut.InAssemblyContaining(typeof(In));

		await That(sut).Contains(typeof(ClassInNamespaceScope));
		await That(sut.GetDescription())
			.IsEqualTo($"types within namespace \"{NamespaceScope}\" in all loaded assemblies");
	}

	[Test]
	public async Task InNamespace_InAllLoadedAssemblies_ShouldMatchDefaultSource()
	{
		Filtered.Types sut = Types.InNamespace(NamespaceScope).InAllLoadedAssemblies();

		await That(sut).Contains(typeof(ClassInNamespaceScope));
		await That(sut.GetDescription())
			.IsEqualTo($"types within namespace \"{NamespaceScope}\" in all loaded assemblies");
	}

	[Test]
	public async Task InNamespace_InAssemblies_ShouldLimitSourceToProvidedAssemblies()
	{
		Filtered.Types sut = Types.InNamespace(NamespaceScope).InAssemblies(typeof(TypesTests).Assembly);

		await That(sut).Contains(typeof(ClassInNamespaceScope));
		await That(sut.GetDescription())
			.IsEqualTo($"types within namespace \"{NamespaceScope}\" in the assemblies [aweXpect.Reflection.Tests*]")
			.AsWildcard();
	}

	[Test]
	public async Task InNamespace_InAssemblyContaining_WithGeneric_ShouldLimitSourceToAssemblyOfProvidedType()
	{
		Filtered.Types sut = Types.InNamespace(NamespaceScope).InAssemblyContaining<ClassInNamespaceScope>();

		await That(sut).Contains(typeof(ClassInNamespaceScope));
		await That(sut.GetDescription())
			.IsEqualTo(
				$"types within namespace \"{NamespaceScope}\" in assembly containing type ClassInNamespaceScope");
	}

	[Test]
	public async Task InNamespace_InAssemblyContaining_WithType_WhenAssemblyDoesNotContainNamespace_ShouldBeEmpty()
	{
		Filtered.Types sut = Types.InNamespace(NamespaceScope).InAssemblyContaining(typeof(In));

		await That(sut).IsEmpty();
		await That(sut.GetDescription())
			.IsEqualTo($"types within namespace \"{NamespaceScope}\" in assembly containing type In");
	}

	[Test]
	public async Task InNamespace_ShouldContainTypesWithinNamespaceIncludingSubNamespaces()
	{
		Filtered.Types sut = Types.InNamespace(NamespaceScope);

		await That(sut).Contains(typeof(ClassInNamespaceScope));
		await That(sut).Contains(typeof(ClassInNestedNamespaceScope));
		await That(sut).DoesNotContain(typeof(ClassInSiblingNamespaceScope));
		await That(sut.GetDescription())
			.IsEqualTo($"types within namespace \"{NamespaceScope}\" in all loaded assemblies");
	}

	[Test]
	public async Task InNamespace_ShouldSelectTypesThatCanBeAsserted()
	{
		Filtered.Types sut = Types.InNamespace(NamespaceScope);

		async Task Act()
		{
			await That(sut).AreWithinNamespace(NamespaceScope);
		}

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task InNamespace_WhenAssertionFails_ShouldReportFullMessage()
	{
		Filtered.Types sut = Types.InNamespace(NamespaceScope);

		async Task Act()
		{
			await That(sut)
				.AreWithinNamespace("aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling");
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that types within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope" in all loaded assemblies
			             all are within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling",
			             but none of 2 were

			             Not matching items:
			             [
			               *
			             ]

			             Collection:
			             [
			               *
			             ]
			             """).AsWildcard();
	}

	[Test]
	public async Task InNamespace_WhenNegatingMatchingAssertion_ShouldReportFullMessage()
	{
		Filtered.Types sut = Types.InNamespace(NamespaceScope);

		async Task Act()
		{
			await That(sut).DoesNotComplyWith(they
				=> they.AreWithinNamespace(NamespaceScope));
		}

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that types within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope" in all loaded assemblies
			             not all are within namespace "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope",
			             but all 2 were

			             Collection:
			             [
			               *
			             ]
			             """).AsWildcard();
	}

	[Test]
	public async Task InNamespace_WhenNoTypesAreInNamespace_ShouldBeEmpty()
	{
		Filtered.Types sut = Types.InNamespace("Non.Existent.Namespace");

		await That(sut).IsEmpty();
		await That(sut.GetDescription())
			.IsEqualTo("types within namespace \"Non.Existent.Namespace\" in all loaded assemblies");
	}

	private const string NamespaceScope = "aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope";
}
