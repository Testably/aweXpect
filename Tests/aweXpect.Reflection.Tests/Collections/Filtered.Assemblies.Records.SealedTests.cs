namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Records
		{
			public sealed class SealedTests
			{
				[Fact]
				public async Task ShouldApplyFilterForRecords()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Sealed.Records();

					await That(types).All().Satisfy(t => t is
						{ IsClass: true, IsAbstract: false, IsSealed: true, IsInterface: false, }).And.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Sealed.Records())
							.AreInternal();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that sealed records in all loaded assemblies
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
