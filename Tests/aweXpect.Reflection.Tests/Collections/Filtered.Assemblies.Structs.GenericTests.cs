using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Structs
		{
			public sealed class GenericTests
			{
				[Test]
				public async Task ShouldApplyFilterForGenericTypes()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Generic.Structs();

					await That(types).All().Satisfy(t => t is
						{ IsClass: false, IsEnum: false, IsValueType: true, IsGenericType: true, }).And.IsNotEmpty();
				}

				[Test]
				public async Task ShouldIncludeNestedInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Generic.Structs())
							.AreNotGeneric();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that generic structs in all loaded assemblies
						             none are generic,
						             but * of * were

						             Matching items:
						             [
						               *
						             ]

						             Collection:
						             [
						               *
						             ]
						             """).AsWildcard();
				}

				[Test]
				[MethodDataSource(typeof(Assemblies), nameof(Filters.AssemblyFilters.GetAccessModifiers))]
				public async Task WithAccessModifier_ShouldIncludeNestedInformationInErrorMessage(
					AccessModifiers accessModifier, string expectedString)
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Generic.Structs().WhichAre(accessModifier))
							.AreNotGeneric();
					}

					await That(Act).Throws()
						.WithMessage($"""
						              Expected that {expectedString}generic structs in all loaded assemblies
						              none are generic,
						              but * of * were

						              Matching items:
						              [
						                *
						              ]

						              Collection:
						              [
						                *
						              ]
						              """).AsWildcard();
				}
			}
		}
	}
}
