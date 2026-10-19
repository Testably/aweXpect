using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope.Nested;
using aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScopeSibling;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAreNotWithinNamespace
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesNotWithinNamespaceIncludingSubNamespaces()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAreNotWithinNamespace(
						"aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope");

				await That(types).DoesNotContain(typeof(ClassInNamespaceScope));
				await That(types).DoesNotContain(typeof(ClassInNestedNamespaceScope));
				await That(types).Contains(typeof(ClassInSiblingNamespaceScope));
				await That(types.GetDescription())
					.IsEqualTo(
						"types which are not within namespace \"aweXpect.Reflection.Tests.TestHelpers.Types.NamespaceScope\" in assembly")
					.AsPrefix();
			}
		}
	}
}
