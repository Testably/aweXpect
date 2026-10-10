using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutConstructorsThatSatisfyThePredicate()
			{
				Filtered.Constructors constructors = In.Type<ClassWithConstructors>()
					.Constructors().Except(constructor => constructor.GetParameters().Length == 0);

				await That(constructors).All().Satisfy(c => c!.GetParameters().Length != 0).And.IsNotEmpty();
				await That(constructors.GetDescription())
					.IsEqualTo("constructors except constructor => constructor.GetParameters().Length == 0 in")
					.AsPrefix();
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>().Constructors()
					.Except(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(constructors).IsEmpty();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that constructors except _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			private class ClassWithConstructors
			{
				public ClassWithConstructors()
				{
				}

				// ReSharper disable once UnusedParameter.Local
				public ClassWithConstructors(int value)
				{
				}
			}
		}
	}
}
