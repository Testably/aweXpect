using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a <see cref="string" /> is equal to another one.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _stringExpectation = "foo";
	private readonly string _stringSubject = "FOO";

	[Benchmark]
	public async Task<string?> String_aweXpect()
		=> await Expect.That(_stringSubject).IsEqualTo(_stringExpectation).IgnoringCase();

	[Benchmark]
	public AndConstraint<StringAssertions> String_FluentAssertions()
		=> _stringSubject.Should().BeEquivalentTo(_stringExpectation, o => o.IgnoringCase());

	[Benchmark]
	public async Task<string?> String_TUnit()
		=> await Assert.That(_stringSubject).IsEqualTo(_stringExpectation).IgnoringCase();
}
