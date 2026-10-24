using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichAre
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldAllowFilteringForAnyAccessModifier()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Any);

				await That(types)
					.AtLeast(1).Satisfy(type => type.IsPublic).And
					.AtLeast(1).Satisfy(type => type.IsNestedPrivate).And
					.AtLeast(1).Satisfy(type => type.IsNestedFamily);
				await That(In.AssemblyContaining<AssemblyFilters>().Types().WhichAreNot(AccessModifiers.Any)).IsEmpty()
					.Because("every type has one of the access modifiers combined in Any, so none is filtered out");
				await That(types.GetDescription())
					.IsEqualTo("types in assembly").AsPrefix();
			}

			[Test]
			public async Task ShouldAllowFilteringForCombinedAccessModifiers()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Public | AccessModifiers.Internal);

				await That(types)
					.All().Satisfy(type => !type.IsNested || type.IsNestedPublic || type.IsNestedAssembly).And
					.AtLeast(1).Satisfy(type => type.IsNested ? type.IsNestedPublic : type.IsPublic).And
					.AtLeast(1).Satisfy(type => type.IsNested ? type.IsNestedAssembly : type.IsNotPublic)
					.Because("the combined flags select types with any of the given access modifiers");
				await That(types.GetDescription())
					.IsEqualTo("public or internal types in assembly").AsPrefix();
			}

			[Test]
			public async Task ShouldAllowFilteringForInternalTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Internal);

				await That(types).AreInternal().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("internal types in assembly").AsPrefix();
			}

			[Test]
			public async Task ShouldAllowFilteringForPrivateTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Private);

				await That(types).ArePrivate().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("private types in assembly").AsPrefix();
			}

			[Test]
			public async Task ShouldAllowFilteringForProtectedTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Protected);

				await That(types).AreProtected().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("protected types in assembly").AsPrefix();
			}

			[Test]
			public async Task ShouldAllowFilteringForPublicTypes()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichAre(AccessModifiers.Public);

				await That(types).ArePublic().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("public types in assembly").AsPrefix();
			}

			[Test]
			[MethodDataSource(typeof(AssemblyFilters), nameof(AssemblyFilters.GetAccessModifiers))]
			public async Task WithAccessModifier_ShouldIncludeAccessModifierInErrorMessage(
				AccessModifiers accessModifier, string expectedString)
			{
				async Task Act()
				{
					await That(In.AssemblyContaining<AssemblyFilters>().Types().WhichAre(accessModifier))
						.AreAbstract();
				}

				await That(Act).Throws()
					.WithMessage($"""
					              Expected that {expectedString}types in assembly*
					              all are abstract,
					              but *
					              """).AsWildcard();
			}
		}
	}
}
