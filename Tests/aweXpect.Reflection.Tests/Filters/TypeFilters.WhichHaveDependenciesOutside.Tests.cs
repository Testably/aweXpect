using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichHaveDependenciesOutside
	{
		private const string ConsumersNamespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers";
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";
		private const string Layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";

		public sealed class Tests
		{
			[Fact]
			public async Task ExcludingOwnSubNamespaces_ShouldNotAffectOriginalFilter()
			{
				Filtered.Types.NamespaceDependencyOutsideFilterResult original =
					Types.InNamespace(ConsumersNamespace).WhichHaveDependenciesOutside(Layer1Namespace);
				_ = original.ExcludingOwnSubNamespaces();

				await That(original).DoesNotContain(typeof(ReferencesOwnSubNamespace));
			}

			[Fact]
			public async Task ShouldFilterForTypesWithDependenciesOutsideAllowedNamespaces()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Layer1Namespace);

				await That(types).Contains(typeof(Layer1AndLayer2));
				await That(types).Contains(typeof(OnlyLayer2));
				await That(types).DoesNotContain(typeof(OnlyLayer1));
				await That(types).DoesNotContain(typeof(FrameworkConsumer));
				await That(types).DoesNotContain(typeof(ReferencesOwnNamespace));
			}

			[Fact]
			public async Task ShouldIncludeFilterInDescription()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Layer1Namespace);

				await That(types.GetDescription())
					.Contains($"which have dependencies outside namespace \"{Layer1Namespace}\"");
			}

			[Fact]
			public async Task WhenExcludingOwnSubNamespaces_ShouldSelectTypesReferencingOwnSubNamespace()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Layer1Namespace)
					.ExcludingOwnSubNamespaces();

				await That(types).Contains(typeof(ReferencesOwnSubNamespace));
			}

			[Fact]
			public async Task WhenExcludingSubNamespaces_SubNamespaceDependencyCountsAsOutside()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Layer1Namespace)
					.ExcludingSubNamespaces();

				await That(types).Contains(typeof(ViaSubNamespace));
			}

			[Fact]
			public async Task WhenNotExcludingOwnSubNamespaces_OwnSubNamespaceStaysInside()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Layer1Namespace);

				await That(types).DoesNotContain(typeof(ReferencesOwnSubNamespace));
			}

			[Fact]
			public async Task WhenNotExcludingSubNamespaces_SubNamespaceDependencyStaysInside()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Layer1Namespace);

				await That(types).DoesNotContain(typeof(ViaSubNamespace));
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldFilterOutTypesDependingOnEither()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Layer1Namespace)
					.OrOn(Layer2Namespace);

				await That(types).DoesNotContain(typeof(Layer1AndLayer2));
				await That(types).DoesNotContain(typeof(OnlyLayer2));
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Fact]
			public async Task ExcludingOwnSubNamespaces_ShouldNotAffectOriginalFilter()
			{
				Filtered.Types.TypeSetDependencyOutsideFilterResult original =
					Types.InNamespace(ConsumersNamespace)
						.WhichHaveDependenciesOutside(Types.InNamespace(Layer1Namespace));
				_ = original.ExcludingOwnSubNamespaces();

				await That(original).DoesNotContain(typeof(ReferencesOwnSubNamespace));
			}

			[Fact]
			public async Task ShouldFilterForTypesWithDependenciesOutsideTargetCollection()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Types.InNamespace(Layer1Namespace));

				await That(types).Contains(typeof(Layer1AndLayer2));
				await That(types).Contains(typeof(OnlyLayer2));
				await That(types).DoesNotContain(typeof(OnlyLayer1));
				await That(types).DoesNotContain(typeof(FrameworkConsumer));
				await That(types).DoesNotContain(typeof(ReferencesOwnNamespace));
			}

			[Fact]
			public async Task ShouldIncludeFilterInDescription()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Types.InNamespace(Layer1Namespace));

				await That(types.GetDescription())
					.Contains(
						$"which have dependencies outside (types within namespace \"{Layer1Namespace}\" in all loaded assemblies)");
			}

			[Fact]
			public async Task WhenExcludingOwnSubNamespaces_ShouldSelectTypesReferencingOwnSubNamespace()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Types.InNamespace(Layer1Namespace))
					.ExcludingOwnSubNamespaces();

				await That(types).Contains(typeof(ReferencesOwnSubNamespace));
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldFilterOutTypesDependingOnEither()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichHaveDependenciesOutside(Types.InNamespace(Layer1Namespace))
					.OrOn(Types.InNamespace(Layer2Namespace));

				await That(types).DoesNotContain(typeof(Layer1AndLayer2));
				await That(types).DoesNotContain(typeof(OnlyLayer2));
			}
		}
	}
}
