using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class Without
	{
		public sealed class AttributeTests
		{
			[Fact]
			public async Task ShouldFilterForFieldsWithoutAttribute()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().Without<BarAttribute>();

				await That(fields).IsNotEmpty()
					.And.Contains(typeof(Dummy).GetField(nameof(Dummy.PlainField)))
					.And.DoesNotContain(typeof(Dummy).GetField(nameof(Dummy.MyBarField)));
				await That(fields.GetDescription())
					.IsEqualTo("fields without FieldFilters.Without.AttributeTests.BarAttribute").AsPrefix();
			}

			[AttributeUsage(AttributeTargets.Field)]
			private class BarAttribute : Attribute
			{
			}

#pragma warning disable CS0649 // Field is never assigned to
			private class Dummy
			{
				[Bar] public string? MyBarField;

				public string? PlainField;
			}
#pragma warning restore CS0649
		}
	}
}
