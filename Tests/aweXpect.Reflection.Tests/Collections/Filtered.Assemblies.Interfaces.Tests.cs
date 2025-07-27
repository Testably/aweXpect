using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Interfaces
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForInterfaces()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Interfaces();

					await That(types).All().Satisfy(t => t.IsInterface);
				}

				[Theory]
				[MemberData(nameof(CheckAccessModifiers), MemberType = typeof(Assemblies))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Interfaces(accessModifier);

					await That(types).All().Satisfy(type => type.IsInterface && check(type));
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Interfaces())
							.AreAbstract();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that interfaces in all loaded assemblies
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
