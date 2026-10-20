using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Types;

namespace aweXpect.Reflection.Benchmarks;

/// <summary>
///     In this benchmark we verify the convention that all methods returning a <see cref="Task" /> in a namespace of an
///     assembly are async, as an architecture test does.
/// </summary>
public partial class HappyCaseBenchmarks
{
	[Benchmark]
	public async Task MethodsAreAsync_aweXpect()
		=> await Expect.That(In.AssemblyContaining<HappyCaseBenchmarks>().Types().WhichAreWithinNamespace(_namespace)
				.Methods().WhichReturn<Task>())
			.AreAsync();

	[Benchmark]
	public AndConstraint<MethodInfoSelectorAssertions> MethodsAreAsync_FluentAssertions()
		=> AllTypes.From(_assembly).ThatAreUnderNamespace(_namespace).Methods().ThatReturn<Task>().Should().BeAsync();
}
