using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class ConstructorFilters
{
	public sealed class WhichSatisfy
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForConstructorsWhichSatisfyThePredicate()
			{
				Filtered.Constructors constructors = In.AssemblyContaining<AssemblyFilters>()
					.Constructors().WhichSatisfy(it
						=> it.DeclaringType == typeof(SomeClassToVerifyTheConstructorNameOfIt));

				await That(constructors).HasSingle().Which.IsEqualTo(ExpectedConstructorInfo());
				await That(constructors.GetDescription())
					.IsEqualTo(
						"constructors matching it => it.DeclaringType == typeof(SomeClassToVerifyTheConstructorNameOfIt) in assembly")
					.AsPrefix();
			}
		}
	}
}
