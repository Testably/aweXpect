using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class FilteredExtensions
{
	public sealed partial class Assemblies
	{
		public sealed class StaticTypes
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForBaseType()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().StaticTypes();

					await That(types).All().Satisfy(t => t.IsStatic());
				}

				[Fact]
				public async Task ShouldIncludeStaticInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().StaticTypes())
							.AreNotStatic();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that static types in all loaded assemblies
						             are all not static,
						             but it contained static types [
						               *
						             ]
						             """).AsWildcard();
				}

				[Theory]
				[MemberData(nameof(GetAccessModifiers), MemberType = typeof(FilteredExtensions))]
				public async Task WithAccessModifier_ShouldIncludeStaticInformationInErrorMessage(AccessModifiers accessModifier, string expectedString)
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().StaticTypes(accessModifier))
							.AreNotStatic();

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that {expectedString}static types in all loaded assemblies
						              are all not static,
						              but it contained static types [
						                *
						              ]
						              """).AsWildcard();
				}
			}
		}
	}
}
