using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichDoNotDependOn
	{
		private const string ConsumersNamespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Consumers";
		private const string Layer1Namespace = "aweXpect.Reflection.Tests.TestHelpers.Dependencies.Layer1";

		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForTypesNotDependingOnNamespace()
			{
				Filtered.Types types = Types.InNamespace(ConsumersNamespace).WhichDoNotDependOn(Layer1Namespace);

				await That(types).Contains(typeof(FrameworkConsumer));
				await That(types).DoesNotContain(typeof(ViaField));
			}
		}
	}
}
