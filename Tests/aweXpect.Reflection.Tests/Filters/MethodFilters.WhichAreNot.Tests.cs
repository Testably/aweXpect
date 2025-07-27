using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class MethodFilters
{
	public sealed class WhichAreNot
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForInternalMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNot(AccessModifiers.Internal);

				await That(methods).AreNotInternal();
				await That(methods.GetDescription())
					.IsEqualTo("non-internal methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPrivateMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNot(AccessModifiers.Private);

				await That(methods).AreNotPrivate();
				await That(methods.GetDescription())
					.IsEqualTo("non-private methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForProtectedMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNot(AccessModifiers.Protected);

				await That(methods).AreNotProtected();
				await That(methods.GetDescription())
					.IsEqualTo("non-protected methods in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldAllowFilteringForPublicMethods()
			{
				Filtered.Methods methods = In.AssemblyContaining<AssemblyFilters>()
					.Methods().WhichAreNot(AccessModifiers.Public);

				await That(methods).AreNotPublic();
				await That(methods.GetDescription())
					.IsEqualTo("non-public methods in assembly").AsPrefix();
			}
		}
	}
}
