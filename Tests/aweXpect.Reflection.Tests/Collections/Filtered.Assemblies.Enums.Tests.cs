using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Enums
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForEnums()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Enums();

					await That(types).All().Satisfy(t => t.IsEnum);
				}

				[Theory]
				[MemberData(nameof(CheckAccessModifiers), MemberType = typeof(Assemblies))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Enums(accessModifier);

					await That(types).All().Satisfy(type => type.IsEnum && check(type));
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Enums())
							.AreAbstract();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that enums in all loaded assemblies
						             are all abstract,
						             but it contained non-abstract types [
						               *
						             ]
						             """).AsWildcard();
				}
			}
		}
	}
}
