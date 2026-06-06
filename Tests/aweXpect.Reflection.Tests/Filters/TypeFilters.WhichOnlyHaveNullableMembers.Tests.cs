using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichOnlyHaveNullableMembers
	{
		public sealed class Tests
		{
			[Fact]
			public async Task ShouldAllowFilteringForTypesWhichOnlyHaveNullableMembers()
			{
				Filtered.Types types = In.AssemblyContaining<ClassWithNullableMembers>()
					.Types().WhichOnlyHaveNullableMembers();

				await That(types).OnlyHaveNullableMembers().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("types which only have nullable members in assembly").AsPrefix();
			}

			[Fact]
			public async Task ShouldOnlyKeepTypesWhichOnlyHaveNullableMembers()
			{
				Filtered.Types types = In
					.Types(typeof(ClassWithNullableMembers), typeof(ClassWithMixedNullableMembers),
						typeof(ClassWithNonNullableMembers))
					.WhichOnlyHaveNullableMembers();

				await That(types).IsEqualTo([typeof(ClassWithNullableMembers),]);
			}
		}
	}
}
