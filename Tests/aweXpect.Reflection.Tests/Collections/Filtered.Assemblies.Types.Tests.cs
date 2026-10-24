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
				[Test]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Types();

					await That(types)
						.AtLeast(1).Satisfy(t => t.IsClass).And
						.AtLeast(1).Satisfy(t => t.IsInterface).And
						.AtLeast(1).Satisfy(t => t.IsEnum);
				}

				[Test]
				[MethodDataSource(typeof(Assemblies), nameof(CheckAccessModifiers))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Types().WhichAre(accessModifier);

					await That(types).All().Satisfy(check);
				}

				[Test]
				public async Task ShouldConsiderPrivateAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Private.Types();

					await That(types).All().Satisfy(type
						=> type.IsNested ? type.IsNestedPrivate : type.IsNotPublic);
				}

				[Test]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Types())
							.AreAbstract();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that types in all loaded assemblies
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
