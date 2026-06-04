using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreNotObsolete
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonObsoleteEvents()
			{
				Filtered.Events events = In.AssemblyContaining<ClassWithObsoleteMembers>()
					.Types().Events().WhichAreNotObsolete();

				await That(events).All().Satisfy(x => !Attribute.IsDefined(x, typeof(ObsoleteAttribute)))
					.And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("non-obsolete events in types in assembly").AsPrefix();
			}
		}
	}
}
