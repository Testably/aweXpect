using aweXpect.Reflection.Collections;

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
