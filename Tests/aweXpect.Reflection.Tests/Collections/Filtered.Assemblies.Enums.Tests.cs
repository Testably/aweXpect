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
				[Test]
				public async Task ShouldApplyFilterForEnums()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Enums();

					await That(types).All().Satisfy(t => t.IsEnum);
				}

				[Test]
				[MethodDataSource(typeof(Assemblies), nameof(CheckAccessModifiers))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Enums().WhichAre(accessModifier);

					await That(types).All().Satisfy(type => type.IsEnum && check(type));
				}

				[Test]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Enums())
							.AreAbstract();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that enums in all loaded assemblies
						             all are abstract,
						             but none of * were

						             Not matching items:
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
