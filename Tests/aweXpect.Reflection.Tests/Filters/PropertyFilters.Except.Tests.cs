using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutPropertiesThatSatisfyThePredicate()
			{
				Filtered.Properties properties = In.Type<ConcretePropertyClass>()
					.Properties().Except(property => property.Name == "ConcreteProperty");

				await That(properties).All().Satisfy(p => p!.Name != "ConcreteProperty").And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("properties except property => property.Name == \"ConcreteProperty\" in").AsPrefix();
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>().Properties()
					.Except(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(properties).IsEmpty();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that properties except _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
