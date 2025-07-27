using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Types
		{
			public sealed class AbstractTests
			{
				[Fact]
				public async Task ShouldApplyFilterForAbstractTypes()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Abstract.Types();

					await That(types).All().Satisfy(t => t.IsAbstract);
				}

				[Fact]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Abstract.Types(AccessModifiers.Public);

					await That(types).All().Satisfy(type
						=> type is { IsAbstract: true, IsSealed: false, IsInterface: false, } &&
						   (type.IsNested ? type.IsNestedPublic : type.IsPublic));
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Abstract.Types())
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
				[MemberData(nameof(GetAccessModifiers), MemberType = typeof(Assemblies))]
				public async Task WithAccessModifier_ShouldIncludeAbstractInformationInErrorMessage(
					AccessModifiers accessModifier, string expectedString)
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Abstract.Types(accessModifier))
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
}
