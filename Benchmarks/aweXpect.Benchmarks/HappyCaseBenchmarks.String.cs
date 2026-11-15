using BenchmarkDotNet.Attributes;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace aweXpect.Benchmarks;

/// <summary>
///     In this benchmark we verify that a <see cref="string" /> is equal to another one.<br />
/// </summary>
public partial class HappyCaseBenchmarks
{
	private readonly string _title = "Help!";
	private readonly string _upperCaseTitle = "HELP!";

	[Benchmark]
	public async Task<string?> String_aweXpect()
		=> await Expect.That(_upperCaseTitle).IsEqualTo(_title).IgnoringCase();

	[Benchmark]
	public AndConstraint<StringAssertions> String_FluentAssertions()
		=> _upperCaseTitle.Should().BeEquivalentTo(_title);

	[Benchmark]
	public async Task<string?> String_TUnit()
		=> await Assert.That(_upperCaseTitle).IsEqualTo(_title).IgnoringCase();
}
