using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that two objects are equivalent to one another.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly Nested _nestedExpectation = Nested.Create(10);
	private readonly Nested _nestedSubject = Nested.Create(10);

	[Benchmark]
	public async Task<Nested> Equivalency_aweXpect()
		=> await Expect.That(_nestedSubject).IsEquivalentTo(_nestedExpectation);

	[Benchmark]
	public AndConstraint<ObjectAssertions> Equivalency_FluentAssertions()
		=> _nestedSubject.Should().BeEquivalentTo(_nestedExpectation);

	[Benchmark]
	public async Task<Nested?> Equivalency_TUnit()
		=> await Assert.That(_nestedSubject).IsEquivalentTo(_nestedExpectation);

	public sealed class Nested
	{
		public int A { get; set; }

		public Nested? B { get; set; }

		public Nested? C { get; set; }

		public static Nested Create(int i, int objectCount = 1)
		{
			if (i <= 0)
			{
				return new Nested();
			}

			return new Nested
			{
				A = ++objectCount,
				B = Create(i - 1, objectCount),
				C = Create(i - 2, objectCount),
			};
		}
	}
}
