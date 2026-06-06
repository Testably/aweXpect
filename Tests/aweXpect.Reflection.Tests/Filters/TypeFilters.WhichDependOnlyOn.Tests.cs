using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichDependOnlyOn
	{
		private const string ConsumersNamespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers";
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";

		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForTypesDependingOnlyOnAllowedNamespaces()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace).WhichDependOnlyOn(Layer1Namespace);

				await That(types).Contains(typeof(OnlyLayer1));
				await That(types).Contains(typeof(FrameworkConsumer));
				await That(types).DoesNotContain(typeof(Layer1AndLayer2));
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Fact]
			public async Task ShouldFilterForTypesDependingOnlyOnTargetCollection()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOnlyOn(Types.InNamespace(Layer1Namespace));

				await That(types).Contains(typeof(OnlyLayer1));
				await That(types).Contains(typeof(FrameworkConsumer));
				await That(types).Contains(typeof(ReferencesOwnNamespace));
				await That(types).DoesNotContain(typeof(Layer1AndLayer2));
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldAllowEither()
			{
				const string layer2Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2";
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOnlyOn(Types.InNamespace(Layer1Namespace))
					.OrOn(Types.InNamespace(layer2Namespace));

				await That(types).Contains(typeof(Layer1AndLayer2));
			}

			[Fact]
			public async Task WhenExcludingOwnSubNamespaces_ShouldFilterOutTypesReferencingOwnSubNamespace()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOnlyOn(Types.InNamespace(Layer1Namespace))
					.ExcludingOwnSubNamespaces();

				await That(types).Contains(typeof(OnlyLayer1));
				await That(types).DoesNotContain(typeof(ReferencesOwnSubNamespace));
			}

			[Fact]
			public async Task ExcludingOwnSubNamespaces_ShouldNotAffectOriginalFilter()
			{
				Filtered.Types.TypeSetDependencyOnlyOnFilterResult original = Types.InNamespace(ConsumersNamespace)
					.WhichDependOnlyOn(Types.InNamespace(Layer1Namespace));
				_ = original.ExcludingOwnSubNamespaces();

				await That(original).Contains(typeof(ReferencesOwnSubNamespace));
			}
		}
	}
}
