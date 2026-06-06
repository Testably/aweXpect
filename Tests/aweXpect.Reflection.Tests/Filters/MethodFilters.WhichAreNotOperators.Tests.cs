using aweXpect.Customization;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotOperators
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonOperators()
			{
				Filtered.Methods methods = In.AssemblyContaining<MethodFilters>()
					.Types().Methods().WhichAreNotOperators();

				await That(methods).All().Satisfy(x => !x.IsReallyOperator()).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-operator methods in types in assembly").AsPrefix();
			}
		}

		public sealed class WithOperatorTests
		{
			[Fact]
			public async Task ShouldExcludeTheSpecificOperatorButKeepOthers()
			{
				using (Customize.aweXpect.Reflection().IncludedSpecialNameMembers()
					       .Set(SpecialNameMembers.Operators))
				{
					Filtered.Methods methods = In.Type<ClassWithOperators>()
						.Methods().WhichAreNotOperators(Operator.Addition);

					await That(methods).All().Satisfy(x => x!.Name != "op_Addition").And.IsNotEmpty();
					await That(methods).Contains(x => x.Name == "op_Subtraction");
					await That(methods.GetDescription())
						.IsEqualTo("non-Addition operator methods in").AsPrefix();
				}
			}

			[Fact]
			public async Task ShouldKeepOtherOperatorsWithoutCustomization()
			{
				Filtered.Methods methods = In.Type<ClassWithOperators>()
					.Methods().WhichAreNotOperators(Operator.Addition);

				await That(methods).All().Satisfy(x => x!.Name != "op_Addition").And.IsNotEmpty();
				await That(methods).Contains(x => x.Name == "op_Subtraction");
				await That(methods.GetDescription())
					.IsEqualTo("non-Addition operator methods in").AsPrefix();
			}
		}
	}
}
