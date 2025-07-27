using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichSatisfy
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForFieldsWhichSatisfyThePredicate()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichSatisfy(it => it.Name.Equals("SomeFieldToVerifyTheNameOfIt"));

				await That(fields).HasSingle().Which.IsEqualTo(ExpectedFieldInfo());
				await That(fields.GetDescription())
					.IsEqualTo(
						"fields matching it => it.Name.Equals(\"SomeFieldToVerifyTheNameOfIt\") in assembly")
					.AsPrefix();
			}
		}
	}
}
