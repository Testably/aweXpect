using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichOnlyHaveNonNullableMembers
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForTypesWhichOnlyHaveNonNullableMembers()
			{
				Filtered.Types types = In.AssemblyContaining<ClassWithNonNullableMembers>()
					.Types().WhichOnlyHaveNonNullableMembers();

				await That(types).OnlyHaveNonNullableMembers().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("types which only have non-nullable members in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldOnlyKeepTypesWhichOnlyHaveNonNullableMembers()
			{
				Filtered.Types types = In
					.Types(typeof(ClassWithNullableMembers), typeof(ClassWithMixedNullableMembers),
						typeof(ClassWithNonNullableMembers))
					.WhichOnlyHaveNonNullableMembers();

				await That(types).IsEqualTo([typeof(ClassWithNonNullableMembers),]);
			}
		}
	}
}
