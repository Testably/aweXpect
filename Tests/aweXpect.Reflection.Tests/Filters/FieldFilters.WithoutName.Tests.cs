using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WithoutName
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterOutFieldsWithName()
			{
				Filtered.Fields fields = In.Type<ClassWithTwoFields>()
					.Fields().WithoutName("ExcludedField");

				await That(fields).All().Satisfy(f => f!.Name != "ExcludedField").And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("fields without name equal to \"ExcludedField\" in").AsPrefix();
			}

			[Fact]
			public async Task ShouldSupportAsSuffix()
			{
				Filtered.Fields fields = In.Type<ClassWithTwoFields>()
					.Fields().WithoutName("ExcludedField").AsSuffix();

				await That(fields).All().Satisfy(f => !f!.Name.EndsWith("ExcludedField")).And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("fields without name ending with \"ExcludedField\" in").AsPrefix();
			}

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
			private class ClassWithTwoFields
			{
				public int? ExcludedField;
				public int? KeptField;
			}
#pragma warning restore CS0649
		}
	}
}
