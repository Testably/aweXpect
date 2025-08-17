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
			public async Task CanFilterForInternalConstructors()
			{
				Reflection.Collections.Filtered.Constructors constructors =
					In.AllLoadedAssemblies().Internal.Constructors();
				string description = constructors.GetDescription();

				await That(constructors).AreInternal();
				await That(description).IsEqualTo("internal constructors in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForInternalEvents()
			{
				Reflection.Collections.Filtered.Events events = In.AllLoadedAssemblies().Internal.Events();
				string description = events.GetDescription();

				await That(events).AreInternal();
				await That(description).IsEqualTo("internal events in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForInternalFields()
			{
				Reflection.Collections.Filtered.Fields fields = In.AllLoadedAssemblies().Internal.Fields();
				string description = fields.GetDescription();

				await That(fields).AreInternal();
				await That(description).IsEqualTo("internal fields in all loaded assemblies");
			}


			[Fact]
			public async Task CanFilterForInternalMethods()
			{
				Reflection.Collections.Filtered.Methods methods = In.AllLoadedAssemblies().Internal.Methods();
				string description = methods.GetDescription();

				await That(methods).AreInternal();
				await That(description).IsEqualTo("internal methods in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForInternalProperties()
			{
				Reflection.Collections.Filtered.Properties properties = In.AllLoadedAssemblies().Internal.Properties();
				string description = properties.GetDescription();

				await That(properties).AreInternal();
				await That(description).IsEqualTo("internal properties in all loaded assemblies");
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
			public async Task CanFilterForPrivateConstructors()
			{
				Reflection.Collections.Filtered.Constructors constructors =
					In.AllLoadedAssemblies().Private.Constructors();
				string description = constructors.GetDescription();

				await That(constructors).ArePrivate();
				await That(description).IsEqualTo("private constructors in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateEvents()
			{
				Reflection.Collections.Filtered.Events events = In.AllLoadedAssemblies().Private.Events();
				string description = events.GetDescription();

				await That(events).ArePrivate();
				await That(description).IsEqualTo("private events in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateFields()
			{
				Reflection.Collections.Filtered.Fields fields = In.AllLoadedAssemblies().Private.Fields();
				string description = fields.GetDescription();

				await That(fields).ArePrivate();
				await That(description).IsEqualTo("private fields in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateMethods()
			{
				Reflection.Collections.Filtered.Methods methods = In.AllLoadedAssemblies().Private.Methods();
				string description = methods.GetDescription();

				await That(methods).ArePrivate();
				await That(description).IsEqualTo("private methods in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateProperties()
			{
				Reflection.Collections.Filtered.Properties properties = In.AllLoadedAssemblies().Private.Properties();
				string description = properties.GetDescription();

				await That(properties).ArePrivate();
				await That(description).IsEqualTo("private properties in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateProtectedConstructors()
			{
				Reflection.Collections.Filtered.Constructors constructors =
					In.AllLoadedAssemblies().Private.Protected.Constructors();
				string description = constructors.GetDescription();

				await That(constructors).ArePrivate().And.AreProtected();
				await That(description).IsEqualTo("private protected constructors in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateProtectedEvents()
			{
				Reflection.Collections.Filtered.Events events = In.AllLoadedAssemblies().Private.Protected.Events();
				string description = events.GetDescription();

				await That(events).ArePrivate().And.AreProtected();
				await That(description).IsEqualTo("private protected events in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateProtectedFields()
			{
				Reflection.Collections.Filtered.Fields fields = In.AllLoadedAssemblies().Private.Protected.Fields();
				string description = fields.GetDescription();

				await That(fields).ArePrivate().And.AreProtected();
				await That(description).IsEqualTo("private protected fields in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateProtectedMethods()
			{
				Reflection.Collections.Filtered.Methods methods = In.AllLoadedAssemblies().Private.Protected.Methods();
				string description = methods.GetDescription();

				await That(methods).ArePrivate().And.AreProtected();
				await That(description).IsEqualTo("private protected methods in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateProtectedProperties()
			{
				Reflection.Collections.Filtered.Properties properties =
					In.AllLoadedAssemblies().Private.Protected.Properties();
				string description = properties.GetDescription();

				await That(properties).ArePrivate().And.AreProtected();
				await That(description).IsEqualTo("private protected properties in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPrivateProtectedTypes()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Private.Protected.Types();
				string description = types.GetDescription();

				await That(types).ArePrivate().And.AreProtected();
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
			public async Task CanFilterForProtectedConstructors()
			{
				Reflection.Collections.Filtered.Constructors constructors =
					In.AllLoadedAssemblies().Protected.Constructors();
				string description = constructors.GetDescription();

				await That(constructors).AreProtected();
				await That(description).IsEqualTo("protected constructors in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedEvents()
			{
				Reflection.Collections.Filtered.Events events = In.AllLoadedAssemblies().Protected.Events();
				string description = events.GetDescription();

				await That(events).AreProtected();
				await That(description).IsEqualTo("protected events in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedFields()
			{
				Reflection.Collections.Filtered.Fields fields = In.AllLoadedAssemblies().Protected.Fields();
				string description = fields.GetDescription();

				await That(fields).AreProtected();
				await That(description).IsEqualTo("protected fields in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedInternalConstructors()
			{
				Reflection.Collections.Filtered.Constructors constructors =
					In.AllLoadedAssemblies().Protected.Internal.Constructors();
				string description = constructors.GetDescription();

				await That(constructors).AreProtected().And.AreInternal();
				await That(description).IsEqualTo("protected internal constructors in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedInternalEvents()
			{
				Reflection.Collections.Filtered.Events events = In.AllLoadedAssemblies().Protected.Internal.Events();
				string description = events.GetDescription();

				await That(events).AreProtected().And.AreInternal();
				await That(description).IsEqualTo("protected internal events in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedInternalFields()
			{
				Reflection.Collections.Filtered.Fields fields = In.AllLoadedAssemblies().Protected.Internal.Fields();
				string description = fields.GetDescription();

				await That(fields).AreProtected().And.AreInternal();
				await That(description).IsEqualTo("protected internal fields in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedInternalMethods()
			{
				Reflection.Collections.Filtered.Methods methods = In.AllLoadedAssemblies().Protected.Internal.Methods();
				string description = methods.GetDescription();

				await That(methods).AreProtected().And.AreInternal();
				await That(description).IsEqualTo("protected internal methods in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedInternalProperties()
			{
				Reflection.Collections.Filtered.Properties properties =
					In.AllLoadedAssemblies().Protected.Internal.Properties();
				string description = properties.GetDescription();

				await That(properties).AreProtected().And.AreInternal();
				await That(description).IsEqualTo("protected internal properties in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedInternalTypes()
			{
				Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Protected.Internal.Types();
				string description = types.GetDescription();

				await That(types).AreProtected().And.AreInternal();
				await That(description).IsEqualTo("protected internal types in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedMethods()
			{
				Reflection.Collections.Filtered.Methods methods = In.AllLoadedAssemblies().Protected.Methods();
				string description = methods.GetDescription();

				await That(methods).AreProtected();
				await That(description).IsEqualTo("protected methods in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForProtectedProperties()
			{
				Reflection.Collections.Filtered.Properties properties = In.AllLoadedAssemblies().Protected.Properties();
				string description = properties.GetDescription();

				await That(properties).AreProtected();
				await That(description).IsEqualTo("protected properties in all loaded assemblies");
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
			public async Task CanFilterForPublicConstructors()
			{
				Reflection.Collections.Filtered.Constructors constructors =
					In.AllLoadedAssemblies().Public.Constructors();
				string description = constructors.GetDescription();

				await That(constructors).ArePublic();
				await That(description).IsEqualTo("public constructors in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPublicEvents()
			{
				Reflection.Collections.Filtered.Events events = In.AllLoadedAssemblies().Public.Events();
				string description = events.GetDescription();

				await That(events).ArePublic();
				await That(description).IsEqualTo("public events in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPublicFields()
			{
				Reflection.Collections.Filtered.Fields fields = In.AllLoadedAssemblies().Public.Fields();
				string description = fields.GetDescription();

				await That(fields).ArePublic();
				await That(description).IsEqualTo("public fields in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPublicMethods()
			{
				Reflection.Collections.Filtered.Methods methods = In.AllLoadedAssemblies().Public.Methods();
				string description = methods.GetDescription();

				await That(methods).ArePublic();
				await That(description).IsEqualTo("public methods in all loaded assemblies");
			}

			[Fact]
			public async Task CanFilterForPublicProperties()
			{
				Reflection.Collections.Filtered.Properties properties = In.AllLoadedAssemblies().Public.Properties();
				string description = properties.GetDescription();

				await That(properties).ArePublic();
				await That(description).IsEqualTo("public properties in all loaded assemblies");
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
