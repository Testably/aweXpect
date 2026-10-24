using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Properties
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldNotIncludeInheritedMembers()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Properties();

				await That(properties).Contains(p => p.Name == nameof(DerivedClassWithMembers.DerivedProperty));
				await That(properties).None().Satisfy(p => p.Name == nameof(BaseClassWithMembers.BaseProperty))
					.Because("listing the properties only includes the declared properties by default");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldComposeWithAccessModifierFilter()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Public.Properties().IncludingInherited();

				await That(properties).Contains(p => p.Name == nameof(DerivedClassWithMembers.DerivedProperty));
				await That(properties).Contains(p => p.Name == nameof(BaseClassWithMembers.BaseProperty));
				await That(properties).All().Satisfy(p => p.GetMethod?.IsPublic == true).And.IsNotEmpty();
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInheritedInterfaceMembers()
			{
				Filtered.Properties properties =
					In.Type<IDerivedInterfaceWithMembers>().Properties().IncludingInherited();

				await That(properties).Contains(p => p.Name == nameof(IBaseInterfaceWithMembers.BaseInterfaceProperty))
					.Because("an interface inherits the members of its base interfaces");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInheritedMembers()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Properties().IncludingInherited();

				await That(properties).Contains(p => p.Name == nameof(DerivedClassWithMembers.DerivedProperty));
				await That(properties).Contains(p => p.Name == nameof(BaseClassWithMembers.BaseProperty));
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInternalMembersOfBaseTypesInTheSameAssembly()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithInternalMembers>().Properties().IncludingInherited();

				await That(properties).Contains(p => p.Name == "BaseInternalProperty")
					.Because("a derived type can access the internal members of a base type in the same assembly");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Properties().IncludingInherited()
						.WhichArePrivate();

				await That(properties).None().Satisfy(p => p.Name == "BasePrivateProperty")
					.Because("private members of base types are not inherited");
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Properties().IncludingObjectMembers()
						.WhichArePrivate();

				await That(properties).None().Satisfy(p => p.Name == "BasePrivateProperty");
			}
		}
	}
}
