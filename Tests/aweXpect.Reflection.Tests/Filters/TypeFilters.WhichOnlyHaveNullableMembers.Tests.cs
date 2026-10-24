using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Tests.TestHelpers.Types;

namespace aweXpect.Reflection.Tests.Filters;

public sealed partial class TypeFilters
{
	public sealed class WhichOnlyHaveNullableMembers
	{
		public sealed class Tests
		{
			[Test]
			public async Task ShouldAllowFilteringForTypesWhichOnlyHaveNullableMembers()
			{
				Filtered.Types types = In.AssemblyContaining<ClassWithNullableMembers>()
					.Types().WhichOnlyHaveNullableMembers();

				await That(types).OnlyHaveNullableMembers().And.IsNotEmpty();
				await That(types.GetDescription())
					.IsEqualTo("types which only have nullable members in assembly").AsPrefix();
			}

			[Test]
			public async Task ShouldOnlyKeepTypesWhichOnlyHaveNullableMembers()
			{
				Filtered.Types types = In
					.Types(typeof(ClassWithNullableMembers), typeof(ClassWithMixedNullableMembers),
						typeof(ClassWithNonNullableMembers))
					.WhichOnlyHaveNullableMembers();

				await That(types).IsEqualTo([typeof(ClassWithNullableMembers),]);
			}

			[Test]
			public async Task WhenDirectly_ShouldDescribeTheScope()
			{
				Filtered.Types types = In.AssemblyContaining<ClassWithNullableMembers>()
					.Types().WhichOnlyHaveNullableMembers().Directly();

				await That(types.GetDescription())
					.IsEqualTo("types which only declare nullable members in assembly").AsPrefix();
			}

			[Test]
			public async Task WhenDirectly_ShouldOnlyConsiderDeclaredMembers()
			{
				Filtered.Types inherited = In.Types(typeof(DerivedClassWithNullableMembers))
					.WhichOnlyHaveNullableMembers();
				Filtered.Types directly = In.Types(typeof(DerivedClassWithNullableMembers))
					.WhichOnlyHaveNullableMembers().Directly();

				await That(inherited).IsEmpty()
					.Because("the non-nullable members of the base type are inherited");
				await That(directly).IsEqualTo([typeof(DerivedClassWithNullableMembers),]);
			}

			[Test]
			public async Task WhenIncludingObjectMembers_ShouldDescribeTheScope()
			{
				Filtered.Types types = In.AssemblyContaining<ClassWithNullableMembers>()
					.Types().WhichOnlyHaveNullableMembers().IncludingObjectMembers();

				await That(types.GetDescription())
					.IsEqualTo("types which only have nullable members including object members in assembly").AsPrefix();
			}
		}
	}
}
