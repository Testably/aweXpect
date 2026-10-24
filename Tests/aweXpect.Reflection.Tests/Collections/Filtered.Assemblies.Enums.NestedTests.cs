using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Enums
		{
			public sealed class NestedTests
			{
				[Test]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Enums();

					await That(types).All().Satisfy(t => t is { IsEnum: true, IsNested: true, });
				}

				[Test]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Nested.Enums(AccessModifiers.Public);

					await That(types).All().Satisfy(type
						=> type is { IsEnum: true, IsNested: true, IsNestedPublic: true, });
				}

				[Test]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Nested.Enums())
							.AreInternal();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that nested enums in all loaded assemblies
						             all are internal,
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
