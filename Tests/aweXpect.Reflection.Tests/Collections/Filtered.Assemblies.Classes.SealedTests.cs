namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Classes
		{
			public sealed class SealedTests
			{
				[Fact]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Sealed.Classes();

					await That(types).All().Satisfy(t => t is
						{ IsClass: true, IsAbstract: false, IsSealed: true, IsInterface: false, });
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Sealed.Classes())
							.AreInternal();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that sealed classes in all loaded assemblies
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
