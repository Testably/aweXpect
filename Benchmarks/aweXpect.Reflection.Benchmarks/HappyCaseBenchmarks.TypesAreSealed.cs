using System.Reflection;
using aweXpect.Reflection.Benchmarks.Subjects;
using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Types;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     In this benchmark we verify the convention that all types in a namespace of an assembly are sealed, as an
///     architecture test does.
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Assembly _assembly = typeof(HappyCaseBenchmarks).Assembly;
	private readonly string _namespace = typeof(Album).Namespace!;

	[Benchmark]
	public async Task TypesAreSealed_aweXpect()
		=> await Expect.That(In.AssemblyContaining<HappyCaseBenchmarks>().Types().WhichAreWithinNamespace(_namespace))
			.AreSealed();

	[Benchmark]
	public AndConstraint<TypeSelectorAssertions> TypesAreSealed_FluentAssertions()
		=> AllTypes.From(_assembly).ThatAreUnderNamespace(_namespace).Should().BeSealed();
}
