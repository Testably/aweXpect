using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Records
		{
			public sealed class NestedTests
			{
				[Fact]
				public async Task ShouldApplyFilterForRecords()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Records();

					await That(types).All().Satisfy(t => t is { IsClass: true, IsNested: true, }).And.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Nested.Records(AccessModifiers.Public);

					await That(types).All().Satisfy(type
						=> type is { IsClass: true, IsNested: true, IsNestedPublic: true, }).And.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Nested.Records())
							.AreInternal();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that nested records in all loaded assemblies
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
