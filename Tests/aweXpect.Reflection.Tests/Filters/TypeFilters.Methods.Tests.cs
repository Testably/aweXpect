using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Methods
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldIncludeOverridesOfObjectMembers()
			{
				Filtered.Methods methods =
					In.Type<ClassOverridingObjectMembers>().Methods();

				await That(methods).HasSingle().Which.Satisfies(m => m?.Name == nameof(ToString))
					.Because("an override is declared on the type itself, not on System.Object");
			}

			[Test]
			public async Task ShouldNotIncludeInheritedMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods();

				await That(methods).Contains(m => m.Name == nameof(DerivedClassWithMembers.DerivedMethod));
				await That(methods).None().Satisfy(m => m.Name == nameof(BaseClassWithMembers.BaseMethod))
					.Because("listing the methods only includes the declared methods by default");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldDescribeTheScope()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods().IncludingInherited();

				await That(methods.GetDescription()).IsEqualTo(
					"declared and inherited methods in type DerivedClassWithMembers");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInheritedInterfaceMembers()
			{
				Filtered.Methods methods =
					In.Type<IDerivedInterfaceWithMembers>().Methods().IncludingInherited();

				await That(methods).Contains(m => m.Name == nameof(IDerivedInterfaceWithMembers.DerivedInterfaceMethod));
				await That(methods).Contains(m => m.Name == nameof(IBaseInterfaceWithMembers.BaseInterfaceMethod))
					.Because("an interface inherits the members of its base interfaces");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInheritedMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods().IncludingInherited();

				await That(methods).Contains(m => m.Name == nameof(DerivedClassWithMembers.DerivedMethod));
				await That(methods).Contains(m => m.Name == nameof(BaseClassWithMembers.BaseMethod));
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInternalMembersOfBaseTypesInTheSameAssembly()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithInternalMembers>().Methods().IncludingInherited();

				await That(methods).Contains(m => m.Name == "BaseInternalMethod")
					.Because("a derived type can access the internal members of a base type in the same assembly");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldNotIncludeEnumMembers()
			{
				Filtered.Methods methods =
					In.Type<EnumWithMembers>().Methods().IncludingInherited();

				await That(methods).IsEmpty()
					.Because("the members declared on System.Enum, System.ValueType and System.Object are excluded");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldNotIncludeInaccessibleMembersOfBaseTypesInOtherAssemblies()
			{
				Filtered.Methods methods =
					In.Type<ExceptionWithoutMembers>().Methods().IncludingInherited();

				await That(methods).Contains(m => m.Name == nameof(Exception.GetBaseException));
				await That(methods).None().Satisfy(m => m.IsAssembly || m.IsFamilyAndAssembly)
					.Because("a derived type cannot access the internal members of a base type in another assembly");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods().IncludingInherited()
						.WhichArePrivate();

				await That(methods).None().Satisfy(m => m.Name == "BasePrivateMethod")
					.Because("private members of base types are not inherited");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldNotIncludeObjectMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods().IncludingInherited();

				await That(methods).None().Satisfy(m => m.Name == nameof(ToString) ||
				                                        m.Name == nameof(Equals) ||
				                                        m.Name == nameof(GetHashCode) ||
				                                        m.Name == nameof(GetType))
					.Because("the members declared on System.Object are excluded");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldNotIncludeValueTypeMembers()
			{
				Filtered.Methods methods =
					In.Type<StructWithMembers>().Methods().IncludingInherited();

				await That(methods).IsEmpty()
					.Because("the members declared on System.ValueType and System.Object are excluded");
			}

			[Test]
			public async Task WhenIncludingInheritedAfterIncludingObjectMembers_ShouldKeepObjectMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods().IncludingObjectMembers().IncludingInherited();

				await That(methods).Contains(m => m.Name == nameof(ToString))
					.Because("the options chain in any order");
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldDescribeTheScope()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods().IncludingObjectMembers();

				await That(methods.GetDescription()).IsEqualTo(
					"declared and inherited methods including object members in type DerivedClassWithMembers");
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldIncludeEnumMembers()
			{
				Filtered.Methods methods =
					In.Type<EnumWithMembers>().Methods().IncludingObjectMembers();

				await That(methods).Contains(m => m.Name == nameof(Enum.HasFlag));
				await That(methods).Contains(m => m.Name == nameof(GetType));
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldIncludeObjectMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods().IncludingObjectMembers();

				await That(methods).Contains(m => m.Name == nameof(ToString));
				await That(methods).Contains(m => m.Name == nameof(Equals));
				await That(methods).Contains(m => m.Name == nameof(BaseClassWithMembers.BaseMethod))
					.Because("the members of System.Object are inherited, so the other inherited members are included too");
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldIncludeValueTypeMembers()
			{
				Filtered.Methods methods =
					In.Type<StructWithMembers>().Methods().IncludingObjectMembers();

				await That(methods).Contains(m => m.Name == nameof(Equals) && m.DeclaringType == typeof(ValueType));
				await That(methods).Contains(m => m.Name == nameof(GetType) && m.DeclaringType == typeof(object));
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods().IncludingObjectMembers()
						.WhichArePrivate();

				await That(methods).None().Satisfy(m => m.Name == "BasePrivateMethod");
			}
		}
	}
}
