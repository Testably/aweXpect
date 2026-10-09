using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Json.Tests;

public sealed class StringEqualityOptionsTests
{
	[Fact]
	public async Task EqualJson_ShouldReturnEmptyFailure()
	{
		string actual = "{}";
		string expected = "{  }";
#pragma warning disable aweXpect0001
		IOptionsProvider<StringEqualityOptions> optionsProvider = That(actual).IsEqualTo(expected).AsJson();
#pragma warning restore aweXpect0001

		bool result = await optionsProvider.Options.AreConsideredEqual(actual, expected);
		string failure = optionsProvider.Options.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

		await That(result).IsTrue();
		await That(failure).IsEmpty();
	}

	[Fact]
	public async Task WhenComparedAgain_ShouldDescribeOnlyTheLastComparison()
	{
		string expected = "{}";
#pragma warning disable aweXpect0001
		IOptionsProvider<StringEqualityOptions> optionsProvider = That("").IsEqualTo(expected).AsJson();
#pragma warning restore aweXpect0001

		await optionsProvider.Options.AreConsideredEqual("{\"foo\":1}", expected);
		await optionsProvider.Options.AreConsideredEqual("{\"bar\":1}", expected);
		string failure =
			optionsProvider.Options.GetExtendedFailure("it", ExpectationGrammars.None, "{\"bar\":1}", expected);

		await That(failure).IsEqualTo("it differed as $.bar had unexpected 1")
			.Because("the differences of the previous subject must not be reported for the next one");
	}

	[Fact]
	public async Task WhenCallingGetExtendedFailureWithoutAreConsideredEqual_ShouldReturnEmptystring()
	{
		string actual = "foo";
		string expected = "bar";
#pragma warning disable aweXpect0001
		IOptionsProvider<StringEqualityOptions> optionsProvider = That(actual).IsEqualTo(expected).AsJson();
#pragma warning restore aweXpect0001

		string failure = optionsProvider.Options.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected);

		await That(failure).IsEmpty();
	}

	[Fact]
	public async Task WhenIgnoringCaseAfterAsJson_ShouldThrowAtIgnoringCase()
	{
		StringEqualityResult result = new StringEqualityResult().AsJson();

		void Act() => result.IgnoringCase();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("IgnoringCase cannot be combined with AsJson.")
			.Because("the conflict is rejected at the call that specifies it, not only when the expectation is evaluated");
	}

	[Fact]
	public async Task WhenIgnoringCaseBeforeAsJson_ShouldThrowAtAsJson()
	{
		StringEqualityResult result = new StringEqualityResult().IgnoringCase();

		void Act() => result.AsJson();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("IgnoringCase cannot be combined with AsJson.")
			.Because("the conflict is rejected at the call that specifies it, not only when the expectation is evaluated");
	}

	[Fact]
	public async Task WhenIgnoringCaseIsDisabled_ShouldAllowAsJson()
	{
		StringEqualityResult result = new StringEqualityResult().AsJson();

		void Act() => result.IgnoringCase(false);

		await That(Act).DoesNotThrow()
			.Because("a casing that is not ignored is what a JSON comparison does anyway");
	}

	[Fact]
	public async Task WhenUsingComparerAfterAsJson_ShouldThrowAtUsing()
	{
		StringEqualityResult result = new StringEqualityResult().AsJson();

		void Act() => result.Using(StringComparer.Ordinal);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with AsJson.")
			.Because("the conflict is rejected at the call that specifies it, not only when the expectation is evaluated");
	}

	[Fact]
	public async Task WhenUsingComparerBeforeAsJson_ShouldThrowAtAsJson()
	{
		StringEqualityResult result = new StringEqualityResult().Using(StringComparer.Ordinal);

		void Act() => result.AsJson();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with AsJson.")
			.Because("the conflict is rejected at the call that specifies it, not only when the expectation is evaluated");
	}

	private sealed class StringEqualityResult : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
	{
		public StringEqualityOptions Options { get; } = new("expected");
	}
}
