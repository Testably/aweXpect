using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Types
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Types();

					await That(types)
						.AtLeast(1).Satisfy(t => t.IsClass).And
						.AtLeast(1).Satisfy(t => t.IsInterface).And
						.AtLeast(1).Satisfy(t => t.IsEnum);
				}

				[Theory]
				[MemberData(nameof(CheckAccessModifiers), MemberType = typeof(Assemblies))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Types(accessModifier);

					await That(types).All().Satisfy(check);
				}

				[Fact]
				public async Task ShouldConsiderPrivateAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Types(AccessModifiers.Private);

					await That(types).All().Satisfy(type
						=> type.IsNested ? type.IsNestedPrivate : type.IsNotPublic);
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Types())
							.AreAbstract();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that types in all loaded assemblies
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
