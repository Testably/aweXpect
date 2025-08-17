using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class RecordStructs
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForRecordStructs()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().RecordStructs();

					await That(types).All().Satisfy(t => t is { IsClass: false, IsEnum: false, IsValueType: true, }).And
						.IsNotEmpty();
				}

				[Theory]
				[MemberData(nameof(CheckAccessModifiers), MemberType = typeof(Assemblies))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.RecordStructs(accessModifier);

					await That(types).All().Satisfy(type
							=> type is { IsClass: false, IsEnum: false, IsValueType: true, } && check(type)).And
						.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().RecordStructs())
							.AreAbstract();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that record structs in all loaded assemblies
						             are all abstract,
						             but it contained non-abstract types [
						               *
						             ]
						             """).AsWildcard();
				}
			}
		}
	}
}
