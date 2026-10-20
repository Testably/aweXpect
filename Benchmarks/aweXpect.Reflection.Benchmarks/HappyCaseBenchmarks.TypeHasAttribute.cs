using aweXpect.Reflection.Benchmarks.Subjects;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Types;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     In this benchmark we verify that a type is decorated with an attribute.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Type _entityType = typeof(Album);

	[Benchmark]
	public async Task TypeHasAttribute_aweXpect()
		=> await Expect.That(_entityType).Has<EntityAttribute>();

	[Benchmark]
	public AndWhichConstraint<TypeAssertions, EntityAttribute> TypeHasAttribute_FluentAssertions()
		=> _entityType.Should().BeDecoratedWith<EntityAttribute>();
}
