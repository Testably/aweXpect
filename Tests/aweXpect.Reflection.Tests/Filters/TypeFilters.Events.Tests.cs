using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Events
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldNotIncludeInheritedMembers()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithMembers>().Events();

				await That(events).Contains(e => e.Name == nameof(DerivedClassWithMembers.DerivedEvent));
				await That(events).None().Satisfy(e => e.Name == nameof(BaseClassWithMembers.BaseEvent))
					.Because("listing the events only includes the declared events by default");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInheritedInterfaceMembers()
			{
				Filtered.Events events =
					In.Type<IDerivedInterfaceWithMembers>().Events().IncludingInherited();

				await That(events).Contains(e => e.Name == nameof(IBaseInterfaceWithMembers.BaseInterfaceEvent))
					.Because("an interface inherits the members of its base interfaces");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInheritedMembers()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithMembers>().Events().IncludingInherited();

				await That(events).Contains(e => e.Name == nameof(DerivedClassWithMembers.DerivedEvent));
				await That(events).Contains(e => e.Name == nameof(BaseClassWithMembers.BaseEvent));
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInternalMembersOfBaseTypesInTheSameAssembly()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithInternalMembers>().Events().IncludingInherited();

				await That(events).Contains(e => e.Name == "BaseInternalEvent")
					.Because("a derived type can access the internal members of a base type in the same assembly");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithMembers>().Events().IncludingInherited()
						.WhichArePrivate();

				await That(events).None().Satisfy(e => e.Name == "BasePrivateEvent")
					.Because("private members of base types are not inherited");
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithMembers>().Events().IncludingObjectMembers()
						.WhichArePrivate();

				await That(events).None().Satisfy(e => e.Name == "BasePrivateEvent");
			}
		}
	}
}
