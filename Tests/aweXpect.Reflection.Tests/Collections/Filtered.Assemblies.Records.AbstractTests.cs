using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers;

namespace aweXpect.Reflection.Tests.Collections;

public sealed partial class Filtered
{
	public sealed partial class Assemblies
	{
		public sealed partial class Records
		{
			public sealed class AbstractTests
			{
				[Fact]
				public async Task ShouldApplyFilterForRecords()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Abstract.Records();

					await That(types).All().Satisfy(t => t.IsRecordClass()).And.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Abstract.Records(AccessModifiers.Public);

					await That(types).All().Satisfy(type
						=> type.IsAbstract && type.IsRecordClass() &&
						   (type.IsNested ? type.IsNestedPublic : type.IsPublic)).And.IsNotEmpty();
				}

				[Fact]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
						=> await That(In.AllLoadedAssemblies().Abstract.Records())
							.AreInternal();

					await That(Act).ThrowsException()
						.WithMessage("""
						             Expected that abstract records in all loaded assemblies
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
