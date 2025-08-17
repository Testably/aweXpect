using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class RecordStructs
		{
			public sealed class GenericTests
			{
				[Fact]
				public async Task ShouldApplyFilterForGenericTypes()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Generic.RecordStructs();

					await That(types).All().Satisfy(t => t is { IsClass: false, IsEnum: false, IsValueType: true, IsGenericType: true, }).And.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldIncludeNestedInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Generic.RecordStructs())
							.AreNotGeneric();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that generic record structs in all loaded assemblies
						             are all not generic,
						             but it contained generic types [
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
						=> await That(In.AllLoadedAssemblies().Generic.RecordStructs(accessModifier))
							.AreNotGeneric();

					await That(Act).ThrowsException()
						.WithMessage($"""
						              Expected that {expectedString}generic record structs in all loaded assemblies
						              are all not generic,
						              but it contained generic types [
						                *
						              ]
						              """).AsWildcard();
				}
			}
		}
	}
}
