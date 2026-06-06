using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNotNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonNullableFields()
			{
				Filtered.Fields fields = In.Type<ClassWithMixedNullableMembers>()
					.Fields().WhichAreNotNullable();

				await That(fields).AreNotNullable().And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("non-nullable fields in type").AsPrefix();
			}

			[Fact]
			public async Task ShouldOnlyKeepNonNullableFields()
			{
				Filtered.Fields fields = In.Type<ClassWithMixedNullableMembers>()
					.Fields().WhichAreNotNullable();

				await That(fields).IsEqualTo([
					typeof(ClassWithMixedNullableMembers)
						.GetField(nameof(ClassWithMixedNullableMembers.NonNullableField))!,
					typeof(ClassWithMixedNullableMembers)
						.GetField(nameof(ClassWithMixedNullableMembers.NonNullableValueField))!,
				]).InAnyOrder();
			}
		}
	}
}
