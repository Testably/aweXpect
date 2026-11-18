using System.Reflection;
using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Types
	{
		public sealed class NavigationTests
		{
			[Test]
			public async Task BaseTypes_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<NavigationTests>()
					.Types().BaseTypes();

				string description = types.GetDescription();

				await That(description)
					.IsEqualTo("base types of types in assembly containing type Filtered.Types.NavigationTests");
			}

			[Test]
			public async Task BaseTypes_ShouldIncludeTheWholeChainOnce()
			{
				Reflection.Collections.Filtered.Types types = In.Types<DerivedClass, OtherDerivedClass>().BaseTypes();

				await That(types).IsEqualTo([typeof(MiddleClass), typeof(BaseClass),]).InAnyOrder();
			}

			[Test]
			public async Task BaseTypes_WhenTypesOnlyHaveImplicitBaseTypes_ShouldBeEmpty()
			{
				Reflection.Collections.Filtered.Types types = In.Types(
					typeof(BaseClass), typeof(NavigationStruct), typeof(NavigationEnum), typeof(IChildInterface)).BaseTypes();

				await That(types).IsEmpty()
					.Because("object, ValueType and Enum are implicit base types and an interface has no base type");
			}

			[Test]
			public async Task ImplementedInterfaces_Directly_ShouldExcludeInheritedInterfaces()
			{
				Reflection.Collections.Filtered.Types types = In.Type<DerivedClass>().ImplementedInterfaces().Directly();

				await That(types).IsEqualTo([typeof(IChildInterface),]);
			}

			[Test]
			public async Task ImplementedInterfaces_Directly_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<NavigationTests>()
					.Types().ImplementedInterfaces().Directly();

				string description = types.GetDescription();

				await That(description)
					.IsEqualTo(
						"interfaces directly implemented by types in assembly containing type Filtered.Types.NavigationTests");
			}

			[Test]
			public async Task ImplementedInterfaces_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<NavigationTests>()
					.Types().ImplementedInterfaces();

				string description = types.GetDescription();

				await That(description)
					.IsEqualTo(
						"interfaces implemented by types in assembly containing type Filtered.Types.NavigationTests");
			}

			[Test]
			public async Task ImplementedInterfaces_ShouldIncludeInheritedInterfacesOnce()
			{
				Reflection.Collections.Filtered.Types types = In.Types<DerivedClass, OtherDerivedClass>()
					.ImplementedInterfaces();

				await That(types)
					.IsEqualTo([typeof(IChildInterface), typeof(IParentInterface), typeof(IBaseInterface),])
					.InAnyOrder();
			}

			[Test]
			public async Task ImplementedInterfaces_WithFilter_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<NavigationTests>()
					.Types().ImplementedInterfaces().WhichAreNotWithinNamespace("System");

				string description = types.GetDescription();

				await That(description)
					.IsEqualTo(
						"interfaces implemented by types in assembly containing type Filtered.Types.NavigationTests which are not within namespace \"System\"");
			}

			[Test]
			public async Task NestedTypes_ShouldExcludeCompilerGeneratedTypes()
			{
				Reflection.Collections.Filtered.Types types = In.Type<OuterClass>().NestedTypes();

				await That(types)
					.IsEqualTo([typeof(OuterClass.PublicNestedClass), OuterClass.PrivateNestedType,])
					.InAnyOrder();
				await That(typeof(OuterClass).GetNestedTypes(BindingFlags.NonPublic).Length).IsGreaterThan(1)
					.Because("the lambda in OuterClass makes the compiler generate a nested type");
			}

			[Test]
			public async Task NestedTypes_ShouldHaveExpectedDescription()
			{
				Reflection.Collections.Filtered.Types types = In.AssemblyContaining<NavigationTests>()
					.Types().NestedTypes();

				string description = types.GetDescription();

				await That(description)
					.IsEqualTo("nested types of types in assembly containing type Filtered.Types.NavigationTests");
			}

			[Test]
			public async Task NestedTypes_ShouldOnlyIncludeDirectlyNestedTypes()
			{
				Reflection.Collections.Filtered.Types types = In.Type<NavigationTests>().NestedTypes();

				await That(types).Contains(typeof(OuterClass))
					.And.DoesNotContain(typeof(OuterClass.PublicNestedClass));
			}

			private interface IBaseInterface;

			private interface IParentInterface;

			private interface IChildInterface : IParentInterface;

			private class BaseClass : IBaseInterface;

			private class MiddleClass : BaseClass;

			private sealed class DerivedClass : MiddleClass, IChildInterface;

			private sealed class OtherDerivedClass : MiddleClass;

			private struct NavigationStruct;

			private enum NavigationEnum
			{
				None,
			}

			private sealed class OuterClass
			{
				public static Type PrivateNestedType => typeof(PrivateNestedClass);

				public static Func<int> CreateCounter(int start) => () => start + 1;

				public sealed class PublicNestedClass;

				private sealed class PrivateNestedClass;
			}
		}
	}
}
