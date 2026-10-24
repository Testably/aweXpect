using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Records
		{
			public sealed class Tests
			{
				[Test]
				public async Task ShouldApplyFilterForRecords()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Records();

					await That(types).All().Satisfy(t => t.IsClass).And.IsNotEmpty();
				}

				[Test]
				[MethodDataSource(typeof(Assemblies), nameof(CheckAccessModifiers))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Records().WhichAre(accessModifier);

					await That(types).All().Satisfy(type => type.IsClass && check(type)).And.IsNotEmpty();
				}

				[Test]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Records())
							.AreAbstract();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that records in all loaded assemblies
						             all are abstract,
						             but only * of * were

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
