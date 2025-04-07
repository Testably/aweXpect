using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class FilteredExtensions
{
	public sealed partial class Assemblies
	{
		public sealed class SealedTypes
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForBaseType()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().SealedTypes();

					await That(types).All().Satisfy(type =>
						type is { IsAbstract: false, IsSealed: true, IsInterface: false, });
				}

				[Fact]
				public async Task ShouldIncludeSealedInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().SealedTypes())
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
				[MemberData(nameof(GetAccessModifiers), MemberType = typeof(FilteredExtensions))]
				public async Task WithAccessModifier_ShouldIncludeSealedInformationInErrorMessage(
					AccessModifiers accessModifier, string expectedString)
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().SealedTypes(accessModifier))
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
