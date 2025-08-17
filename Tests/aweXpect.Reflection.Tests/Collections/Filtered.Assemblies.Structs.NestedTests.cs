using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Structs
		{
			public sealed class NestedTests
			{
				[Fact]
				public async Task ShouldApplyFilterForStructs()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Structs();

					await That(types).All()
						.Satisfy(t => t is { IsClass: false, IsEnum: false, IsValueType: true, IsNested: true, }).And
						.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Nested.Structs(AccessModifiers.Protected);

					await That(types).All().Satisfy(type
						=> type is
						{
							IsClass: false, IsEnum: false, IsValueType: true, IsNested: true, IsNestedFamily: true,
						}).And.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Nested.Structs())
							.AreInternal();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that nested structs in all loaded assemblies
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
