using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutMethodsThatSatisfyThePredicate()
			{
				Filtered.Methods methods = In.Type<ConcreteMethodClass>()
					.Methods().Except(method => method.Name == "ConcreteMethod");

				await That(methods).All().Satisfy(m => m!.Name != "ConcreteMethod").And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("methods except method => method.Name == \"ConcreteMethod\" in").AsPrefix();
			}
		}
	}
}
