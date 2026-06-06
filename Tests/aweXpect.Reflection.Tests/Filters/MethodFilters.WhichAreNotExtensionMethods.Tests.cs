using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;
#if NET10_0_OR_GREATER
using aweXpect.Reflection.Tests.TestHelpers.Types;
#endif

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNotExtensionMethods
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonExtensionMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<MethodFilters>()
					.Types().Methods().WhichAreNotExtensionMethods();

				await That(methods).All().Satisfy(x => !x.IsReallyExtensionMethod()).And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("non-extension methods in types in assembly").AsPrefix();
			}
		}

#if NET10_0_OR_GREATER
		public sealed class NewSyntaxTests
		{
			[Fact]
			public async Task ShouldExcludeStaticExtensionMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<MethodFilters>()
					.Types().Methods().WhichAreNotExtensionMethods()
					.WithName(nameof(StaticClassWithNewExtensionMethods.Create));

				// The static extension method Create() must be excluded; only the regular Create(int) overload
				// of the extension-bearing class may remain.
				await That(methods).All().Satisfy(x =>
						x.DeclaringType != typeof(StaticClassWithNewExtensionMethods) ||
						x.GetParameters().Length > 0)
					.And.IsNotEmpty();
			}
		}
#endif
	}
}
