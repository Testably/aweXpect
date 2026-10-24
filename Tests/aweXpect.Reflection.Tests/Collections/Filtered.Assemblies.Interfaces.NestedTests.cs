using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Interfaces
		{
			public sealed class NestedTests
			{
				[Test]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Interfaces();

					await That(types).All().Satisfy(t => t is { IsInterface: true, IsNested: true, });
				}

				[Test]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Public.Nested.Interfaces();

					await That(types).All().Satisfy(type
						=> type is { IsInterface: true, IsNested: true, IsNestedPublic: true, });
				}

				[Test]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Nested.Interfaces())
							.AreInternal();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that nested interfaces in all loaded assemblies
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
