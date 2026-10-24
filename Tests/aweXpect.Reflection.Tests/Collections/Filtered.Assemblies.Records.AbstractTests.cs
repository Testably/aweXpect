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
				[Test]
				public async Task ShouldApplyFilterForRecords()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies().Abstract.Records();

					await That(types).All().Satisfy(t => t.IsRecordClass()).And.IsNotEmpty();
				}

				[Test]
				public async Task ShouldConsiderAccessModifier()
				{
					Reflection.Collections.Filtered.Types types = In.AllLoadedAssemblies()
						.Public.Abstract.Records();

					await That(types).All().Satisfy(type
						=> type.IsAbstract && type.IsRecordClass() &&
						   (type.IsNested ? type.IsNestedPublic : type.IsPublic)).And.IsNotEmpty();
				}

				[Test]
				public async Task ShouldIncludeAbstractInformationInErrorMessage()
				{
					async Task Act()
					{
						await That(In.AllLoadedAssemblies().Abstract.Records())
							.AreInternal();
					}

					await That(Act).Throws()
						.WithMessage("""
						             Expected that abstract records in all loaded assemblies
						             all are internal,
						             but none of * were

						             Not matching items:
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
