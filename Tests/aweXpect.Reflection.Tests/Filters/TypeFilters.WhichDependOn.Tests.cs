using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichDependOn
	{
		private const string ConsumersNamespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers";
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";

		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesDependingOnNamespace()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace).WhichDependOn(Layer1Namespace);

				await That(types).Contains(typeof(ViaField));
				await That(types).Contains(typeof(ViaSubNamespace));
				await That(types).DoesNotContain(typeof(FrameworkConsumer));
			}

			[Test]
			public async Task WhenExcludingSubNamespaces_ShouldNotMatchSubNamespace()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(Layer1Namespace).ExcludingSubNamespaces();

				await That(types).Contains(typeof(ViaField));
				await That(types).DoesNotContain(typeof(ViaSubNamespace));
			}

			[Test]
			public async Task WhenNoNamespaceIsSpecified_ShouldThrowArgumentException()
			{
				void Act()
				{
					_ = Types.InNamespace(ConsumersNamespace).WhichDependOn();
				}

				await That(Act).Throws<ArgumentException>()
					.WithParamName("namespaces").And
					.WithMessage("The 'namespaces' collection cannot be empty.").AsPrefix();
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldMatchEither()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn("Non.Existent.Namespace").OrOn(Layer1Namespace);

				await That(types).Contains(typeof(ViaField));
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Test]
			public async Task OrOn_ShouldNotAffectOriginalFilter()
			{
				Filtered.Types.TypeSetDependencyFilterResult original = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(Types.InNamespace("Non.Existent.Namespace"));
				_ = original.OrOn(Types.InNamespace(Layer1Namespace));

				await That(original).DoesNotContain(typeof(ViaField));
			}

			[Test]
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

			[Test]
			public async Task ShouldFilterForTypesDependingOnTargetCollection()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(Types.InNamespace(Layer1Namespace));

				await That(types).Contains(typeof(ViaField));
				await That(types).Contains(typeof(ViaSubNamespace));
				await That(types).DoesNotContain(typeof(FrameworkConsumer));
			}

			[Test]
			public async Task WhenTargetIsSingleRuntimeType_ShouldFilterForTypesDependingOnIt()
			{
				Type target = typeof(TargetA);

				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(In.Type(target));

				await That(types).Contains(typeof(ViaField));
				await That(types).DoesNotContain(typeof(OnlyLayer2))
					.Because("OnlyLayer2 depends on TargetB, not on TargetA");
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldMatchEither()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDependOn(Types.InNamespace("Non.Existent.Namespace"))
					.OrOn(Types.InNamespace(Layer1Namespace));

				await That(types).Contains(typeof(ViaField));
			}
		}

		public sealed class SpecificTypeTests
		{
			[Test]
			public async Task OrOn_ShouldNotAffectOriginalFilter()
			{
				Filtered.Types.TypeDependencyFilterResult original = In.Types<ViaField, OnlyLayer2, FrameworkConsumer>()
					.WhichDependOn<TargetA>();
				_ = original.OrOn<TargetB>();

				await That(original).IsEqualTo([typeof(ViaField),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldFilterForTypesDependingOnType()
			{
				Filtered.Types types = In.Types<ViaField, OnlyLayer2, FrameworkConsumer>()
					.WhichDependOn<TargetA>();

				await That(types).IsEqualTo([typeof(ViaField),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldIncludeDescriptionInFailureMessage()
			{
				async Task Act()
				{
					await That(Types.InNamespace(ConsumersNamespace).WhichDependOn<TargetA>())
						.DoesNotContain(typeof(ViaField));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types within namespace "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers" which depend on type TargetA in all loaded assemblies
					             does not contain an item equal to ViaField,
					             but it contained ViaField at least once
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenTypeIsNull_ShouldThrowArgumentNullException()
			{
				void Act()
				{
					_ = In.Type<ViaField>().WhichDependOn((Type)null!);
				}

				await That(Act).Throws<ArgumentNullException>()
					.WithParamName("type").And
					.WithMessage("The 'type' cannot be null.").AsPrefix();
			}

			[Test]
			public async Task WhenTypeIsSpecifiedAtRuntime_ShouldFilterForTypesDependingOnIt()
			{
				Type target = typeof(TargetB);

				Filtered.Types types = In.Types<ViaField, OnlyLayer2, FrameworkConsumer>()
					.WhichDependOn(target);

				await That(types).IsEqualTo([typeof(OnlyLayer2),]).InAnyOrder();
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldMatchEither()
			{
				Filtered.Types types = In.Types<ViaField, OnlyLayer2, FrameworkConsumer>()
					.WhichDependOn<TargetA>().OrOn<TargetB>();

				await That(types).IsEqualTo([typeof(ViaField), typeof(OnlyLayer2),]).InAnyOrder();
			}
		}
	}
}
