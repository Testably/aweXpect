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
				[Test]
				public async Task ShouldApplyFilterForAbstractTypes()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Abstract.Types();

					await That(types).All().Satisfy(t => t.IsAbstract);
				}

				[Test]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Abstract.Types(AccessModifiers.Public);

					await That(types).All().Satisfy(type
						=> type is { IsAbstract: true, IsSealed: false, IsInterface: false, } &&
						   (type.IsNested ? type.IsNestedPublic : type.IsPublic));
				}

				[Test]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Abstract.Types())
							.AreNotAbstract();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that abstract types in all loaded assemblies
						             none are abstract,
						             but * of * were

						             Matching items:
						             [
						               *
						             ]

						             Collection:
						             [
						               *
						             ]
						             """).AsWildcard();
				}

				[Test]
				[MethodDataSource(typeof(Assemblies), nameof(GetAccessModifiers))]
				public async Task WithAccessModifier_ShouldIncludeAbstractInformationInErrorMessage(
					AccessModifiers accessModifier, string expectedString)
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Abstract.Types(accessModifier))
							.AreNotAbstract();
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that {expectedString}abstract types in all loaded assemblies
						              none are abstract,
						              but * of * were

						              Matching items:
						              [
						                *
						              ]

						              Collection:
						              [
						                *
						              ]
						              """).AsWildcard();
				}
			}
		}
	}
}
