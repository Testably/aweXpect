using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreWithinNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldExcludeNamespacesThatOnlyShareThePrefix()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreWithinNamespace(
						"aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				await That(types).DoesNotContain(typeof(ClassInSiblingNamespaceScope));
			}

			[Test]
			public async Task ShouldFilterForTypesWithinNamespaceIncludingSubNamespaces()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreWithinNamespace(
						"aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				await That(types).Contains(typeof(ClassInNamespaceScope));
				await That(types).Contains(typeof(ClassInNestedNamespaceScope));
				await That(types).DoesNotContain(typeof(ClassInSiblingNamespaceScope));
				await That(types.GetDescription())
					.IsEqualTo(
						"types which are within namespace \"aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope\" in assembly")
					.AsPrefix();
			}
		}
	}
}
