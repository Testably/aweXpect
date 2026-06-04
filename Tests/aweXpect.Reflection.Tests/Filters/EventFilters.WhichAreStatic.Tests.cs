using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class EventFilters
{
	public sealed class WhichAreStatic
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForStaticEvents()
			{
				Filtered.Events events = In.AssemblyContaining<TestClassWithStaticMembers>()
					.Types().Events().WhichAreStatic();

				await That(events).All().Satisfy(x => x.AddMethod?.IsStatic == true).And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("static events in types in assembly").AsPrefix();
			}
		}

		public sealed class NegatedTests
		{
			[Fact]
			public async Task ShouldAllowFilteringForNonStaticEvents()
			{
				Filtered.Events events = In.AssemblyContaining<TestClassWithStaticMembers>()
					.Types().Events().WhichAreNotStatic();

				await That(events).All().Satisfy(x => x.AddMethod?.IsStatic == false).And.IsNotEmpty();
				await That(events.GetDescription())
					.IsEqualTo("non-static events in types in assembly").AsPrefix();
			}
		}
	}
}
