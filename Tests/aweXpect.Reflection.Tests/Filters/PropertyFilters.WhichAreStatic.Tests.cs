using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class PropertyFilters
{
	public sealed class WhichAreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForStaticProperties()
			{
				Filtered.Properties properties = In.AssemblyContaining<AssemblyFilters>()
					.Properties().WhichAreStatic();

				await That(properties)
					.All().Satisfy(p => (p.GetMethod?.IsStatic ?? false) || (p.SetMethod?.IsStatic ?? false))
					.And.IsNotEmpty();
				await That(properties.GetDescription())
					.IsEqualTo("static properties in assembly").AsPrefix();
			}

			// ReSharper disable once UnusedType.Local
			private static class WithStaticProperty
			{
				// ReSharper disable once UnusedMember.Local
				public static int? Foo { get; set; }
			}
		}
	}
}
