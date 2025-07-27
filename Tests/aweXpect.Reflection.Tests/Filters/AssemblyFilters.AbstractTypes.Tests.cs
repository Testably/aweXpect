using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class AssemblyFilters
{
	public sealed class AbstractTypes
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldApplyFilterForBaseType()
			{
				Filtered.Types types = In.AllLoadedAssemblies().AbstractTypes();

				await That(types).All().Satisfy(t => t.IsAbstract);
			}

			[Fact]
			public async Task ShouldIncludeAbstractInformationInErrorMessage()
			{
				async Task Act()
					=> await That(In.AllLoadedAssemblies().AbstractTypes())
						.AreNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage("""
					             Expected that abstract types in all loaded assemblies
					             are all not abstract,
					             but it contained abstract types [
					               *
					             ]
					             """).AsWildcard();
			}

			[Theory]
			[MemberData(nameof(GetAccessModifiers), MemberType = typeof(AssemblyFilters))]
			public async Task WithAccessModifier_ShouldIncludeAbstractInformationInErrorMessage(
				AccessModifiers accessModifier, string expectedString)
			{
				async Task Act()
					=> await That(In.AllLoadedAssemblies().AbstractTypes(accessModifier))
						.AreNotAbstract();

				await That(Act).ThrowsException()
					.WithMessage($"""
					              Expected that {expectedString}abstract types in all loaded assemblies
					              are all not abstract,
					              but it contained abstract types [
					                *
					              ]
					              """).AsWildcard();
			}
		}
	}
}
