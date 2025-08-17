using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Records
		{
			public sealed class Tests
			{
				[Fact]
				public async Task ShouldApplyFilterForRecords()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Records();

					await That(types).All().Satisfy(t => t.IsClass).And.IsNotEmpty();
				}

				[Theory]
				[MemberData(nameof(CheckAccessModifiers), MemberType = typeof(Assemblies))]
				public async Task ShouldConsiderAccessModifier(AccessModifiers accessModifier, Func<Type, bool> check)
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Records(accessModifier);

					await That(types).All().Satisfy(type => type.IsClass && check(type)).And.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Records())
							.AreAbstract();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that records in all loaded assemblies
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
