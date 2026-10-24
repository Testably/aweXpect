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
			public async Task ShouldNotIncludeInheritedMembers()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithMembers>().Fields();

				await That(fields).Contains(f => f.Name == nameof(DerivedClassWithMembers.DerivedField));
				await That(fields).None().Satisfy(f => f.Name == nameof(BaseClassWithMembers.BaseField))
					.Because("listing the fields only includes the declared fields by default");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInheritedMembers()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithMembers>().Fields().IncludingInherited();

				await That(fields).Contains(f => f.Name == nameof(DerivedClassWithMembers.DerivedField));
				await That(fields).Contains(f => f.Name == nameof(BaseClassWithMembers.BaseField));
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldIncludeInternalMembersOfBaseTypesInTheSameAssembly()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithInternalMembers>().Fields().IncludingInherited();

				await That(fields).Contains(f => f.Name == "BaseInternalField")
					.Because("a derived type can access the internal members of a base type in the same assembly");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldNotIncludeInaccessibleMembersOfBaseTypesInOtherAssemblies()
			{
				Filtered.Fields fields =
					In.Type<ExceptionWithoutMembers>().Fields().IncludingInherited();

				await That(fields).None().Satisfy(f => f.IsAssembly || f.IsFamilyAndAssembly || f.IsPrivate)
					.Because("a derived type cannot access the internal members of a base type in another assembly");
			}

			[Test]
			public async Task WhenIncludingInherited_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithMembers>().Fields().IncludingInherited()
						.WhichArePrivate();

				await That(fields).None().Satisfy(f => f.Name == "_basePrivateField")
					.Because("private members of base types are not inherited");
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldNotIncludeInheritedPrivateMembers()
			{
				Filtered.Fields fields =
					In.Type<DerivedClassWithMembers>().Fields().IncludingObjectMembers()
						.WhichArePrivate();

				await That(fields).None().Satisfy(f => f.Name == "_basePrivateField");
			}
		}
	}
}
