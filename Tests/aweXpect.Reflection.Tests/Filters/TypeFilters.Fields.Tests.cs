using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class Fields
	{
		public sealed class Tests
		{
			[Test]
			public async Task WithDeclaredOnly_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithMembers>().Fields()
						.WhichArePrivate();

				await That(fields).None().Satisfy(f => f.Name == "_basePrivateField");
			}

			[Test]
			public async Task WithDeclaredOnly_ShouldOnlyIncludeDeclaredMembers()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithMembers>().Fields();

				await That(fields).Contains(f => f.Name == nameof(DerivedClassWithMembers.DerivedField));
				await That(fields).None().Satisfy(f => f.Name == nameof(BaseClassWithMembers.BaseField));
			}

			[Test]
			public async Task WithDefaultScope_ShouldOnlyIncludeDeclaredMembers()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithMembers>().Fields();

				await That(fields).Contains(f => f.Name == nameof(DerivedClassWithMembers.DerivedField));
				await That(fields).None().Satisfy(f => f.Name == nameof(BaseClassWithMembers.BaseField));
			}

			[Test]
			public async Task WithIncludingInherited_ShouldIncludeInheritedMembers()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithMembers>().Fields(MemberScope.IncludingInherited);

				await That(fields).Contains(f => f.Name == nameof(DerivedClassWithMembers.DerivedField));
				await That(fields).Contains(f => f.Name == nameof(BaseClassWithMembers.BaseField));
			}

			[Test]
			public async Task WithIncludingInherited_ShouldIncludeInheritedPrivateMembers()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithMembers>().Fields(MemberScope.IncludingInherited)
						.WhichArePrivate();

				await That(fields).Contains(f => f.Name == "_basePrivateField");
			}
		}
	}
}
