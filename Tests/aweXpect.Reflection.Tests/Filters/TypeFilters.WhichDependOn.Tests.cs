using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichDependOn
	{
		private const string ConsumersNamespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers";
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";

		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForTypesDependingOnNamespace()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace).WhichDependOn(Layer1Namespace);

				await That(types).Contains(typeof(ViaField));
				await That(types).Contains(typeof(ViaSubNamespace));
				await That(types).DoesNotContain(typeof(FrameworkConsumer));
			}

			[Fact]
			public async Task WhenExcludingSubNamespaces_ShouldNotMatchSubNamespace()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(Layer1Namespace).ExcludingSubNamespaces();

				await That(types).Contains(typeof(ViaField));
				await That(types).DoesNotContain(typeof(ViaSubNamespace));
			}

			[Fact]
			public async Task WhenNoNamespaceIsSpecified_ShouldThrowArgumentException()
			{
				void Act()
				{
					_ = Types.InNamespace(ConsumersNamespace).WhichDependOn();
				}

				await That(Act).Throws<ArgumentException>()
					.WithMessage("At least one namespace must be specified.");
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldMatchEither()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn("Non.Existent.Namespace").OrOn(Layer1Namespace);

				await That(types).Contains(typeof(ViaField));
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Fact]
			public async Task OrOn_ShouldNotAffectOriginalFilter()
			{
				Filtered.Types.TypeSetDependencyFilterResult original = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(Types.InNamespace("Non.Existent.Namespace"));
				_ = original.OrOn(Types.InNamespace(Layer1Namespace));

				await That(original).DoesNotContain(typeof(ViaField));
			}

			[Fact]
			public async Task ShouldDelimitTargetDescriptionFromSourceSuffix()
			{
				// The parentheses keep the target's trailing source scope apart from the subject collection's
				// own source suffix, so the two "in all loaded assemblies" do not run into each other.
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(Types.InNamespace(Layer1Namespace));

				await That(types.GetDescription()).IsEqualTo(
					$"types within namespace \"{ConsumersNamespace}\" which depend on " +
					$"(types within namespace \"{Layer1Namespace}\" in all loaded assemblies) " +
					"in all loaded assemblies");
			}

			[Fact]
			public async Task ShouldFilterForTypesDependingOnTargetCollection()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(Types.InNamespace(Layer1Namespace));

				await That(types).Contains(typeof(ViaField));
				await That(types).Contains(typeof(ViaSubNamespace));
				await That(types).DoesNotContain(typeof(FrameworkConsumer));
			}

			[Fact]
			public async Task WhenWidenedWithOrOn_ShouldMatchEither()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(Types.InNamespace("Non.Existent.Namespace"))
					.OrOn(Types.InNamespace(Layer1Namespace));

				await That(types).Contains(typeof(ViaField));
			}
		}
	}
}
