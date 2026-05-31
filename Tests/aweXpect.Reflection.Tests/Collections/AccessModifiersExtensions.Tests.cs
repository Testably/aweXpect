using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed class AccessModifiersExtensionsTests
{
	public sealed class GetStringTests
	{
		[Fact]
		public async Task Any_ShouldNotPrependAnyAccessModifierToDescription()
		{
			Reflection.Collections.Filtered.Types types =
				In.AllLoadedAssemblies().Types(AccessModifiers.Any);

			await That(types.GetDescription()).IsEqualTo("types in all loaded assemblies").AsPrefix();
		}

		[Fact]
		public async Task CombinationIncludingPrivate_ShouldListPrivate()
		{
			Reflection.Collections.Filtered.Types types =
				In.AllLoadedAssemblies().Types(AccessModifiers.Private | AccessModifiers.Internal);

			await That(types.GetDescription()).IsEqualTo("private or internal types in all loaded assemblies")
				.AsPrefix();
		}

		[Fact]
		public async Task CombinationIncludingPrivateProtected_ShouldListPrivateProtected()
		{
			Reflection.Collections.Filtered.Types types =
				In.AllLoadedAssemblies().Types(AccessModifiers.PrivateProtected | AccessModifiers.Internal);

			await That(types.GetDescription())
				.IsEqualTo("internal or private protected types in all loaded assemblies").AsPrefix();
		}

		[Fact]
		public async Task CombinationIncludingProtectedInternal_ShouldListProtectedInternal()
		{
			Reflection.Collections.Filtered.Types types =
				In.AllLoadedAssemblies().Types(AccessModifiers.ProtectedInternal | AccessModifiers.Internal);

			await That(types.GetDescription())
				.IsEqualTo("internal or protected internal types in all loaded assemblies").AsPrefix();
		}
	}
}
