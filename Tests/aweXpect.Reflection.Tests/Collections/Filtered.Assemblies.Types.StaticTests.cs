using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Types
		{
			public sealed class StaticTests
			{
				[Fact]
				public async Task ShouldApplyFilterForStaticTypes()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Static.Types();

					await That(types).All().Satisfy(t => t.IsStatic());
				}

				[Fact]
				public async Task ShouldIncludeStaticInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Static.Types())
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
				[MemberData(nameof(Filters.AssemblyFilters.GetAccessModifiers),
					MemberType = typeof(Assemblies))]
				public async Task WithAccessModifier_ShouldIncludeStaticInformationInErrorMessage(
					AccessModifiers accessModifier, string expectedString)
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Static.Types(accessModifier))
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
