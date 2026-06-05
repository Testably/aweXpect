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
				Filtered.Types types = In.Namespace(ConsumersNamespace).WhichDependOnlyOn(Layer1Namespace);

				await That(types).Contains(typeof(OnlyLayer1));
				await That(types).Contains(typeof(FrameworkConsumer));
				await That(types).DoesNotContain(typeof(Layer1AndLayer2));
			}
		}
	}
}
