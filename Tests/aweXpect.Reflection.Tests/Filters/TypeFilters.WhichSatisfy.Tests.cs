using aweXpect.Reflection.Collections;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichSatisfy
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldFilterForTypesWhichSatisfyThePredicate()
			{
				Filtered.Types types = In.AssemblyContaining<AssemblyFilters>()
					.Types().WhichSatisfy(it => it.Name.Equals(nameof(SomeClassToVerifyViaAPredicate)));

				await That(types).HasSingle().Which.IsEqualTo(typeof(SomeClassToVerifyViaAPredicate));
				await That(types.GetDescription())
					.IsEqualTo(
						"types matching it => it.Name.Equals(nameof(SomeClassToVerifyViaAPredicate)) in assembly")
					.AsPrefix();
			}

			private class SomeClassToVerifyViaAPredicate;
		}
	}
}
