using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer2;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichDoNotDependOn
	{
		private const string ConsumersNamespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers";
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";

		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesNotDependingOnNamespace()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace).WhichDoNotDependOn(Layer1Namespace);

				await That(types).Contains(typeof(FrameworkConsumer));
				await That(types).DoesNotContain(typeof(ViaField));
			}
		}

		public sealed class FilteredTypesTargetTests
		{
			[Test]
			public async Task ShouldFilterForTypesNotDependingOnTargetCollection()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDoNotDependOn(Types.InNamespace(Layer1Namespace));

				await That(types).Contains(typeof(FrameworkConsumer));
				await That(types).DoesNotContain(typeof(ViaField));
			}

			[Test]
			public async Task WhenTargetCollectionIsEmpty_ShouldKeepAllTypes()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace)
					.WhichDoNotDependOn(Types.InNamespace("Non.Existent.Namespace"));

				await That(types).Contains(typeof(ViaField));
				await That(types).Contains(typeof(FrameworkConsumer));
			}
		}

		public sealed class SpecificTypeTests
		{
			[Test]
			public async Task ShouldFilterForTypesNotDependingOnType()
			{
				Filtered.Types types = In.Types<ViaField, OnlyLayer2, FrameworkConsumer>()
					.WhichDoNotDependOn<TargetA>();

				await That(types).IsEqualTo([typeof(OnlyLayer2), typeof(FrameworkConsumer),]).InAnyOrder();
			}

			[Test]
			public async Task ShouldIncludeDescriptionInFailureMessage()
			{
				async Task Act()
				{
					await That(Types.InNamespace(ConsumersNamespace).WhichDoNotDependOn<TargetA>())
						.DoesNotContain(typeof(FrameworkConsumer));
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types within namespace "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers" which do not depend on type TargetA in all loaded assemblies
					             does not contain an item equal to FrameworkConsumer,
					             but it contained FrameworkConsumer at least once
					             """).AsPrefix();
			}

			[Test]
			public async Task WhenTypeIsSpecifiedAtRuntime_ShouldFilterForTypesNotDependingOnIt()
			{
				Filtered.Types types = In.Types<ViaField, OnlyLayer2, FrameworkConsumer>()
					.WhichDoNotDependOn(typeof(TargetB));

				await That(types).IsEqualTo([typeof(ViaField), typeof(FrameworkConsumer),]).InAnyOrder();
			}

			[Test]
			public async Task WhenWidenedWithOrOn_ShouldExcludeTypesDependingOnEither()
			{
				Filtered.Types types = In.Types<ViaField, OnlyLayer2, FrameworkConsumer>()
					.WhichDoNotDependOn<TargetA>().OrOn<TargetB>();

				await That(types).IsEqualTo([typeof(FrameworkConsumer),]).InAnyOrder();
			}
		}
	}
}
