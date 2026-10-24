using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Structs
		{
			public sealed class Tests
			{
				[Test]
				public async Task ShouldApplyFilterForStructs()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Structs();

					await That(types).All().Satisfy(t => t is { IsClass: false, IsEnum: false, IsValueType: true, }).And
						.IsNotEmpty();
				}

				[Test]
				[MethodDataSource(typeof(Assemblies), nameof(CheckAccessModifiers))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Structs().WhichAre(accessModifier);

					await That(types).All().Satisfy(type
							=> type is { IsClass: false, IsEnum: false, IsValueType: true, } && check(type)).And
						.IsNotEmpty();
				}

				[Test]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Structs())
							.AreAbstract();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that structs in all loaded assemblies
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
