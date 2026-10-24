using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed class AccessModifiersExtensionsTests
{
	public sealed class GetStringTests
	{
		[Test]
		public async Task Any_ShouldNotPrependAnyAccessModifierToDescription()
		{
			Reflection.Collections.Filtered.Types types =
				In.AllLoadedAssemblies().Types();

			await That(types.GetDescription()).IsEqualTo("types in all loaded assemblies").AsPrefix();
		}

		[Test]
		public async Task CombinationIncludingPrivate_ShouldListPrivate()
		{
			Reflection.Collections.Filtered.Types types =
				In.AllLoadedAssemblies().Types().WhichAre(AccessModifiers.Private | AccessModifiers.Internal);

			await That(types.GetDescription()).IsEqualTo("private or internal types in all loaded assemblies")
				.AsPrefix();
		}

		[Test]
		public async Task CombinationIncludingPrivateProtected_ShouldListPrivateProtected()
		{
			Reflection.Collections.Filtered.Types types =
				In.AllLoadedAssemblies().Types().WhichAre(AccessModifiers.PrivateProtected | AccessModifiers.Internal);

			await That(types.GetDescription())
				.IsEqualTo("internal or private protected types in all loaded assemblies").AsPrefix();
		}

		[Test]
		public async Task CombinationIncludingProtectedInternal_ShouldListProtectedInternal()
		{
			Reflection.Collections.Filtered.Types types =
				In.AllLoadedAssemblies().Types().WhichAre(AccessModifiers.ProtectedInternal | AccessModifiers.Internal);

			await That(types.GetDescription())
				.IsEqualTo("internal or protected internal types in all loaded assemblies").AsPrefix();
		}
	}
}
