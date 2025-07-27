using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Types
		{
			public sealed class SealedTests
			{
				[Fact]
				public async Task ShouldApplyFilterForSealedTypes()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Sealed.Types();

					await That(types).All().Satisfy(type =>
						type is { IsAbstract: false, IsSealed: true, IsInterface: false, });
				}

				[Fact]
				public async Task ShouldIncludeSealedInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Sealed.Types())
							.AreNotSealed();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that sealed types in all loaded assemblies
						             are all not sealed,
						             but it contained sealed types [
						               *
						             ]
						             """).AsWildcard();
				}

				[Theory]
				[MemberData(nameof(Filters.AssemblyFilters.GetAccessModifiers),
					MemberType = typeof(Assemblies))]
				public async Task WithAccessModifier_ShouldIncludeSealedInformationInErrorMessage(
					AccessModifiers accessModifier, string expectedString)
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Sealed.Types(accessModifier))
							.AreNotSealed();

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that {expectedString}sealed types in all loaded assemblies
						              are all not sealed,
						              but it contained sealed types [
						                *
						              ]
						              """).AsWildcard();
				}
			}
		}
	}
}
