using aweXpect.Reflection.Collections;
using Xunit.Sdk;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class Which
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForFieldsWhichSatisfyThePredicate()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().Which(it => it.Name.Equals("SomeFieldToVerifyTheNameOfIt"));

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields matching it => it.Name.Equals(\"SomeFieldToVerifyTheNameOfIt\") in assembly")
					.AsPrefix();
			}

			[Fact]
			public async Task WhenPredicateThrows_ShouldFail()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>().Fields()
					.Which(_ => throw new InvalidOperationException("boom"));

				async Task Act()
				{
					await That(fields).IsEmpty();
				}

				await That(Act).Throws<XunitException>()
					.WithMessage("""
					             Expected that fields matching _ => throw new InvalidOperationException("boom") in assembly containing type AssemblyFilters
					             is empty,
					             but the predicate did throw an InvalidOperationException:
					               boom
					             """)
					.Because("an exception of the predicate must fail the expectation instead of aborting the evaluation");
			}
		}
	}
}
