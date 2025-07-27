using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Classes
		{
			public sealed class AbstractTests
			{
				[Fact]
				public async Task ShouldApplyFilterForClasses()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Abstract.Classes();

					await That(types).All().Satisfy(t => t is
						{ IsClass: true, IsAbstract: true, IsSealed: false, IsInterface: false, });
				}

				[Fact]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Abstract.Classes(AccessModifiers.Public);

					await That(types).All().Satisfy(type
						=> type is { IsAbstract: true, IsSealed: false, IsInterface: false, IsClass: true, } &&
						   (type.IsNested ? type.IsNestedPublic : type.IsPublic));
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Abstract.Classes())
							.AreInternal();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that abstract classes in all loaded assemblies
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
