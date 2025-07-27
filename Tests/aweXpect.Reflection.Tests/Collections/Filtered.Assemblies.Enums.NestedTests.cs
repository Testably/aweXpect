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
				[Fact]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Enums();

					await That(types).All().Satisfy(t => t is { IsEnum: true, IsNested: true, });
				}

				[Fact]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Nested.Enums(AccessModifiers.Public);

					await That(types).All().Satisfy(type
						=> type is { IsEnum: true, IsNested: true, IsNestedPublic: true, });
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Nested.Enums())
							.AreInternal();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that nested enums in all loaded assemblies
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
