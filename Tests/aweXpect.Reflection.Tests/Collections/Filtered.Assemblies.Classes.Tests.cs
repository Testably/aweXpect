using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Classes
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Classes();

					await That(types).All().Satisfy(t => t.IsClass);
				}

				[Theory]
				[MemberData(nameof(CheckAccessModifiers), MemberType = typeof(Assemblies))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Classes(accessModifier);

					await That(types).All().Satisfy(type => type.IsClass && check(type));
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Classes())
							.AreAbstract();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that classes in all loaded assemblies
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
