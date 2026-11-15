using aweXpect.Reflection.Benchmarks.Subjects;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Types;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     In this benchmark we verify that the <see cref="AlbumRepository" /> type is not static.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Type _repositoryType = typeof(AlbumRepository);

	[Benchmark]
	public async Task TypeIsNotStatic_aweXpect()
		=> await Expect.That(_repositoryType).IsNotStatic();

	[Benchmark]
	public AndConstraint<TypeAssertions> TypeIsNotStatic_FluentAssertions()
		=> _repositoryType.Should().NotBeStatic();
}
