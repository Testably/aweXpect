using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WithoutName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutMethodsWithName()
			{
				Filtered.Methods methods = In.Type<ConcreteMethodClass>()
					.Methods().WithoutName("ConcreteMethod");

				await That(methods).All().Satisfy(m => m!.Name != "ConcreteMethod").And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("methods without name equal to \"ConcreteMethod\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Methods methods = In.Type<ConcreteMethodClass>()
					.Methods().WithoutName("ConcreteMethod").AsSuffix();

				await That(methods).All().Satisfy(m => !m!.Name.EndsWith("ConcreteMethod")).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("methods without name ending with \"ConcreteMethod\" in").AsPrefix();
			}
		}
	}
}
