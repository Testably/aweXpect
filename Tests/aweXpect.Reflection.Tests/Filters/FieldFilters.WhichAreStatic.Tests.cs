using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class FieldFilters
{
	public sealed class WhichAreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForStaticFields()
			{
				Filtered.Fields fields = In.AssemblyContaining<AssemblyFilters>()
					.Fields().WhichAreStatic();

				await That(fields).All().Satisfy(f => f.IsStatic).And.IsNotEmpty();
				await That(fields.GetDescription())
					.IsEqualTo("static fields in assembly").AsPrefix();
			}

			private class WithStaticField
			{
#pragma warning disable CS0169 // Field is never used
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
				public static int Foo;
#pragma warning restore CS0649
#pragma warning restore CS0169
			}
		}
	}
}
