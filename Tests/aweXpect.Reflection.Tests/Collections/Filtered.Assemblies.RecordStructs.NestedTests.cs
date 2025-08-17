using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class RecordStructs
		{
			public sealed class NestedTests
			{
				[Fact]
				public async Task ShouldApplyFilterForRecordStructs()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Nested.RecordStructs();

					await That(types).All()
						.Satisfy(t => t is { IsClass: false, IsEnum: false, IsValueType: true, IsNested: true, }).And
						.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Nested.RecordStructs(AccessModifiers.Protected);

					await That(types).All().Satisfy(type
						=> type is
						{
							IsClass: false, IsEnum: false, IsValueType: true, IsNested: true, IsNestedFamily: true,
						}).And.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Nested.RecordStructs())
							.AreInternal();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that nested record structs in all loaded assemblies
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
