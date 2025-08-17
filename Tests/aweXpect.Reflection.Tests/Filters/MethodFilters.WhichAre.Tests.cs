using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAre
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAre(AccessModifiers.Internal);

				await That(methods).AreInternal().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("internal methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAre(AccessModifiers.Private);

				await That(methods).ArePrivate().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("private methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAre(AccessModifiers.Protected);

				await That(methods).AreProtected().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("protected methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAre(AccessModifiers.Public);

				await That(methods).ArePublic().And.IsNotEmpty();
				await That(methods.GetDescription())
					.IsEqualTo("public methods in assembly").AsPrefix();
			}
		}
	}
}
