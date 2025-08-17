using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForStaticMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreStatic();

				await That(methods).All().Satisfy(m => m.IsStatic).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("static methods in assembly").AsPrefix();
			}

			// ReSharper disable once UnusedType.Local
			private class WithStaticMethod
			{
				// ReSharper disable once UnusedMember.Local
				public static void Foo()
				{
				}
			}
		}
	}
}
