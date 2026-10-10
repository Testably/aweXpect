using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class Except
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutFieldsThatSatisfyThePredicate()
			{
				Filtered.Fields fields = In.Type<ClassWithFields>()
					.Fields().Except(field => field.Name == "ExcludedField");

				await That(fields).All().Satisfy(f => f!.Name != "ExcludedField").And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("fields except field => field.Name == \"ExcludedField\" in").AsPrefix();
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>().Fields()
					.Except(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(fields).IsEmpty();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fields except _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			private class ClassWithFields
			{
				public int? ExcludedField;
				public int? KeptField;
			}
#pragma warning restore CS0649
		}
	}
}
