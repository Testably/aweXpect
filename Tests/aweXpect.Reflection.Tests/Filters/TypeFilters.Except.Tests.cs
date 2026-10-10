using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutTheGenericType()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Except<TypeToExclude>();

				await That(types).DoesNotContain(typeof(TypeToExclude)).And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("types except TypeFilters.Except.Tests.TypeToExclude in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldFilterOutTypesThatSatisfyThePredicate()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Except(type => type.Name == "TypeToExclude");

				await That(types).All().Satisfy(t => t!.Name != "TypeToExclude").And.IsNotEmpty();
				await That(types).DoesNotContain(typeof(TypeToExclude));
				await That(types.GetDescription())
					.IsEqualTo("types except type => type.Name == \"TypeToExclude\" in assembly").AsPrefix();
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>().Types()
					.Except(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(types).IsEmpty();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types except _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFailWhenNegated()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>().Types()
					.Except(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(types).DoesNotComplyWith(they => they.IsEmpty());
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that types except _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is not empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("a predicate that throws answers neither the expectation nor its negation");
			}

			private class TypeToExclude;
		}
	}
}
