using System.Reflection;
using aweXpect.Reflection.Benchmarks.Subjects;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Collections;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     In this benchmark we verify that all members of a type are non-nullable.<br />
///     FluentAssertions has no expectations on nullability, so it reads the nullability of the declared properties with a
///     <see cref="NullabilityInfoContext" /> and verifies that none of them is nullable. As the type only has
///     auto-properties, whose backing fields are compiler-generated, this covers the same members.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Type _trackType = typeof(Track);

	[Benchmark]
	public async Task TypeNullability_aweXpect()
		=> await Expect.That(_trackType).OnlyHasNonNullableMembers();

	[Benchmark]
	public AndConstraint<GenericCollectionAssertions<NullabilityState>> TypeNullability_FluentAssertions()
	{
		NullabilityInfoContext context = new();
		return _trackType.GetProperties(DeclaredMembers).Select(property => context.Create(property).ReadState)
			.Should().NotContain(NullabilityState.Nullable);
	}
}
