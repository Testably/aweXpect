using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Which
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldFilterForTypesWhichSatisfyThePredicate()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Which(it => it.Name.Equals(nameof(SomeClassToVerifyViaAPredicate)));

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyViaAPredicate));
				await That(types.GetDescription())
					.IsEqualTo(
						"types matching it => it.Name.Equals(nameof(SomeClassToVerifyViaAPredicate)) in assembly")
					.AsPrefix();
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Which(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(types).AreNotAbstract();
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             are all not abstract,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

			[Test]
			public async Task WhenPredicateThrows_ShouldFailWhenNegated()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().Which(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(types).DoesNotComplyWith(they => they.AreNotAbstract());
				}

				await That(Act).Throws<FailException>()
					.WithMessage("""
					             Expected that types matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             also contain an abstract type,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("a predicate that throws answers neither the expectation nor its negation");
			}

			private class SomeClassToVerifyViaAPredicate;
		}
	}
}
