using System.Reflection;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed class Tests
		{
			[Fact]
			public async Task Assembly_FromTypes_ShouldHaveExpectedDescription()
			{
				Assembly assembly = typeof(Tests).Assembly;

				Reflection.Collections.Filtered.Assemblies assemblies =
					new Reflection.Collections.Filtered.Assemblies(assembly, "in my assembly").Types().Assemblies();
				string description = assemblies.GetDescription();

				await That(assemblies).IsNotEmpty();
				await That(description).IsEqualTo("assemblies containing types in my assembly");
			}

			[Fact]
			public async Task CanCombineAbstractAndGeneric()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Abstract.Generic.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("abstract generic classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineAbstractAndNested()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Abstract.Nested.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("abstract nested classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineGenericAndAbstract()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Generic.Abstract.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("generic abstract classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineGenericAndSealed()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Generic.Sealed.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("generic sealed classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineGenericAndStatic()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Generic.Static.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("generic static classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineGenericNestedAndAbstract()
			{
				Reflection.Collections.Filtered.Types
					types = In.AllLoadedAssemblies().Generic.Nested.Abstract.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("generic nested abstract classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineGenericStaticAndNested()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Generic.Static.Nested.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("generic static nested classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineNestedAndAbstract()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Abstract.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("nested abstract classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineNestedAndSealed()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Sealed.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("nested sealed classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineNestedAndStatic()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Static.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("nested static classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineNestedGenericAndSealed()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Generic.Sealed.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("nested generic sealed classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineSealedAndGeneric()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Sealed.Generic.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("sealed generic classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineSealedAndNested()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Sealed.Nested.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("sealed nested classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineStaticAndGeneric()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Static.Generic.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("static generic classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanCombineStaticAndNested()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Static.Nested.Classes();
				string description = types.GetDescription();

				await That(description).IsEqualTo("static nested classes in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForInternalTypes()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Internal.Types();
				string description = types.GetDescription();

				await That(types).AreInternal();
				await That(description).IsEqualTo("internal types in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateProtectedTypes()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Private.Protected.Types();
				string description = types.GetDescription();

				await That(types).ArePrivate();
				await That(description).IsEqualTo("private protected types in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateTypes()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Private.Types();
				string description = types.GetDescription();

				await That(types).ArePrivate();
				await That(description).IsEqualTo("private types in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedInternalTypes()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Protected.Internal.Types();
				string description = types.GetDescription();

				await That(types).AreProtected();
				await That(description).IsEqualTo("protected internal types in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedTypes()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Protected.Types();
				string description = types.GetDescription();

				await That(types).AreProtected();
				await That(description).IsEqualTo("protected types in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPublicTypes()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Public.Types();
				string description = types.GetDescription();

				await That(types).ArePublic();
				await That(description).IsEqualTo("public types in all loaded assemblies");
			}

			[Fact]
			public async Task NullAssembly_ShouldBeIgnored()
			{
				Assembly? assembly = null;

				Reflection.Collections.Filtered.Assemblies assemblies = new(assembly, "in my assembly");
				string description = assemblies.GetDescription();

				await That(assemblies).IsEmpty();
				await That(description).IsEqualTo("in my assembly");
			}
		}
	}
}
