using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Classes
		{
			public sealed class StaticTests
			{
				[Test]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Static.Classes();

					await That(types).All().Satisfy(t => t.IsClass && t.IsStatic()).And.IsNotEmpty();
				}

				[Test]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Static.Classes())
							.AreInternal();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that static classes in all loaded assemblies
						             all are internal,
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
