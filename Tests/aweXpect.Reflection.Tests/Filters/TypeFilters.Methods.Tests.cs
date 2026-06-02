using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Methods
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WithDefaultScope_ShouldOnlyIncludeDeclaredMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods();

				await That(methods).Contains(m => m.Name == nameof(DerivedClassWithMembers.DerivedMethod));
				await That(methods).None().Satisfy(m => m.Name == nameof(BaseClassWithMembers.BaseMethod));
			}

			[Fact]
			public async Task WithDeclaredOnly_ShouldOnlyIncludeDeclaredMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods(MemberScope.DeclaredOnly);

				await That(methods).Contains(m => m.Name == nameof(DerivedClassWithMembers.DerivedMethod));
				await That(methods).None().Satisfy(m => m.Name == nameof(BaseClassWithMembers.BaseMethod));
			}

			[Fact]
			public async Task WithIncludingInherited_ShouldIncludeInheritedMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods(MemberScope.IncludingInherited);

				await That(methods).Contains(m => m.Name == nameof(DerivedClassWithMembers.DerivedMethod));
				await That(methods).Contains(m => m.Name == nameof(BaseClassWithMembers.BaseMethod));
			}

			[Fact]
			public async Task WithIncludingInherited_ShouldIncludeInheritedPrivateMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods(MemberScope.IncludingInherited)
						.WhichArePrivate();

				await That(methods).Contains(m => m.Name == "BasePrivateMethod");
			}

			[Fact]
			public async Task WithDeclaredOnly_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Methods methods =
					In.Type<DerivedClassWithMembers>().Methods(MemberScope.DeclaredOnly)
						.WhichArePrivate();

				await That(methods).None().Satisfy(m => m.Name == "BasePrivateMethod");
			}
		}
	}
}
