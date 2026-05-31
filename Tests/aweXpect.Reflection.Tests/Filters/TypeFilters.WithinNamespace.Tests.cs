using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WithinNamespace
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldExcludeNamespacesThatOnlyShareThePrefix()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithinNamespace(
						"aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				await That(types).DoesNotContain(typeof(ClassInSiblingNamespaceScope));
			}

			[Fact]
			public async Task ShouldFilterForTypesWithinNamespaceIncludingSubNamespaces()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WithinNamespace(
						"aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				await That(types).Contains(typeof(ClassInNamespaceScope));
				await That(types).Contains(typeof(ClassInNestedNamespaceScope));
				await That(types).DoesNotContain(typeof(ClassInSiblingNamespaceScope));
				await That(types.GetDescription())
					.IsEqualTo(
						"types within namespace \"aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope\" in assembly")
					.AsPrefix();
			}
		}
	}
}
