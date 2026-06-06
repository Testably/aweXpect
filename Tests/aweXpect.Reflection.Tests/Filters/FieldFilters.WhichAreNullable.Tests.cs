using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreNullable
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNullableFields()
			{
				Filtered.Fields fields = In.Type<ClassWithMixedNullableMembers>()
					.Fields().WhichAreNullable();

				await That(fields).AreNullable().And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("nullable fields in type").AsPrefix();
			}

			[Fact]
			public async Task ShouldOnlyKeepNullableFields()
			{
				Filtered.Fields fields = In.Type<ClassWithMixedNullableMembers>()
					.Fields().WhichAreNullable();

				await That(fields).IsEqualTo([
					typeof(ClassWithMixedNullableMembers)
						.GetField(nameof(ClassWithMixedNullableMembers.NullableField))!,
					typeof(ClassWithMixedNullableMembers)
						.GetField(nameof(ClassWithMixedNullableMembers.NullableValueField))!,
				]).InAnyOrder();
			}
		}
	}
}
