using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Types
		{
			public sealed class NestedTests
			{
				[Fact]
				public async Task ShouldApplyFilterForNestedTypes()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.Types();

					await That(types).All().Satisfy(t => t.IsNested);
				}

				[Fact]
				public async Task ShouldIncludeNestedInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Nested.Types())
							.AreNotNested();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that nested types in all loaded assemblies
						             are all not nested,
						             but it contained nested types [
						               *
						             ]
						             """).AsWildcard();
				}

				[Theory]
				[MemberData(nameof(Filters.AssemblyFilters.GetAccessModifiers),
					MemberType = typeof(Assemblies))]
				public async Task WithAccessModifier_ShouldIncludeNestedInformationInErrorMessage(
					AccessModifiers accessModifier, string expectedString)
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Nested.Types(accessModifier))
							.AreNotNested();

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that {expectedString}nested types in all loaded assemblies
						              are all not nested,
						              but it contained nested types [
						                *
						              ]
						              """).AsWildcard();
				}
			}
		}
	}
}
