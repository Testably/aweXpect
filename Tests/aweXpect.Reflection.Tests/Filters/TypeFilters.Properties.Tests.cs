using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Properties
	{
		public sealed class Tests
		{
			[Fact]
			public async Task WithDeclaredOnly_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Properties()
						.WhichArePrivate();

				await That(properties).None().Satisfy(p => p.Name == "BasePrivateProperty");
			}

			[Fact]
			public async Task WithDeclaredOnly_ShouldOnlyIncludeDeclaredMembers()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Properties();

				await That(properties).Contains(p => p.Name == nameof(DerivedClassWithMembers.DerivedProperty));
				await That(properties).None().Satisfy(p => p.Name == nameof(BaseClassWithMembers.BaseProperty));
			}

			[Fact]
			public async Task WithDefaultScope_ShouldOnlyIncludeDeclaredMembers()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Properties();

				await That(properties).Contains(p => p.Name == nameof(DerivedClassWithMembers.DerivedProperty));
				await That(properties).None().Satisfy(p => p.Name == nameof(BaseClassWithMembers.BaseProperty));
			}

			[Fact]
			public async Task WithIncludingInherited_ShouldComposeWithAccessModifierFilter()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Public.Properties(MemberScope.IncludingInherited);

				await That(properties).Contains(p => p.Name == nameof(DerivedClassWithMembers.DerivedProperty));
				await That(properties).Contains(p => p.Name == nameof(BaseClassWithMembers.BaseProperty));
				await That(properties).All().Satisfy(p => p.GetMethod?.IsPublic == true).And.IsNotEmpty();
			}

			[Fact]
			public async Task WithIncludingInherited_ShouldIncludeInheritedMembers()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Properties(MemberScope.IncludingInherited);

				await That(properties).Contains(p => p.Name == nameof(DerivedClassWithMembers.DerivedProperty));
				await That(properties).Contains(p => p.Name == nameof(BaseClassWithMembers.BaseProperty));
			}

			[Fact]
			public async Task WithIncludingInherited_ShouldIncludeInheritedPrivateMembers()
			{
				Filtered.Properties properties =
					In.Type<DerivedClassWithMembers>().Properties(MemberScope.IncludingInherited)
						.WhichArePrivate();

				await That(properties).Contains(p => p.Name == "BasePrivateProperty");
			}
		}
	}
}
