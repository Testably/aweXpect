using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Events
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WithDeclaredOnly_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithMembers>().Events()
						.WhichArePrivate();

				await That(events).None().Satisfy(e => e.Name == "BasePrivateEvent");
			}

			[Fact]
			public async Task WithDeclaredOnly_ShouldOnlyIncludeDeclaredMembers()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithMembers>().Events();

				await That(events).Contains(e => e.Name == nameof(DerivedClassWithMembers.DerivedEvent));
				await That(events).None().Satisfy(e => e.Name == nameof(BaseClassWithMembers.BaseEvent));
			}

			[Fact]
			public async Task WithDefaultScope_ShouldOnlyIncludeDeclaredMembers()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithMembers>().Events();

				await That(events).Contains(e => e.Name == nameof(DerivedClassWithMembers.DerivedEvent));
				await That(events).None().Satisfy(e => e.Name == nameof(BaseClassWithMembers.BaseEvent));
			}

			[Fact]
			public async Task WithIncludingInherited_ShouldIncludeInheritedMembers()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithMembers>().Events(MemberScope.IncludingInherited);

				await That(events).Contains(e => e.Name == nameof(DerivedClassWithMembers.DerivedEvent));
				await That(events).Contains(e => e.Name == nameof(BaseClassWithMembers.BaseEvent));
			}

			[Fact]
			public async Task WithIncludingInherited_ShouldIncludeInheritedPrivateMembers()
			{
				Filtered.Events events =
					In.Type<DerivedClassWithMembers>().Events(MemberScope.IncludingInherited)
						.WhichArePrivate();

				await That(events).Contains(e => e.Name == "BasePrivateEvent");
			}
		}
	}
}
