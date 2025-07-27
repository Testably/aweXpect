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
				[Fact]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Static.Classes();

					await That(types).All().Satisfy(t => t.IsClass && t.IsStatic());
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Static.Classes())
							.AreInternal();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that static classes in all loaded assemblies
						             all are internal,
						             but it contained not matching items [
						               *
						             ]
						             """).AsWildcard();
				}
			}
		}
	}
}
