using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class FilteredExtensions
{
	public sealed partial class Assemblies
	{
		public sealed class NestedTypes
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForBaseType()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().NestedTypes();

					await That(types).All().Satisfy(t => t.IsNested);
				}

				[Fact]
				public async Task ShouldIncludeNestedInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().NestedTypes())
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
				[MemberData(nameof(GetAccessModifiers), MemberType = typeof(FilteredExtensions))]
				public async Task WithAccessModifier_ShouldIncludeNestedInformationInErrorMessage(AccessModifiers accessModifier, string expectedString)
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().NestedTypes(accessModifier))
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
