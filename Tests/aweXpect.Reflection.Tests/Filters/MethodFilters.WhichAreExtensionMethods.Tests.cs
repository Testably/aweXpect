using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
#if NET10_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers.Types;
#endif

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreExtensionMethods
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForExtensionMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<MethodFilters>()
					.Types().Methods().WhichAreExtensionMethods();

				await That(methods).All().Satisfy(x => x.IsReallyExtensionMethod()).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("extension methods in types in assembly").AsPrefix();
			}
		}

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Fact]
			public async Task ShouldIncludeStaticExtensionMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<MethodFilters>()
					.Types().Methods().WhichAreExtensionMethods()
					.WithName(nameof(StaticClassWithNewExtensionMethods.Create));

				await That(methods).All().Satisfy(x =>
						x.IsStatic && x.DeclaringType == typeof(StaticClassWithNewExtensionMethods))
					.And.IsNotEmpty();
			}
		}
#endif
	}
}
