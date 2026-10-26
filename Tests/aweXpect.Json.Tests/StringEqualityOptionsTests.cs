using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Json.Tests;

public sealed class StringEqualityOptionsTests
{
	[Test]
	public async Task EqualJson_ShouldReturnEmptyFailure()
	{
		string actual = "{}";
		string expected = "{  }";
#pragma warning disable aweXpect0001
		IOptionsProvider<StringEqualityOptions> optionsProvider = That(actual).IsEqualTo(expected).AsJson();
#pragma warning restore aweXpect0001

		MatchResult result = await optionsProvider.Options.Value.MatchWithExplanation(actual, expected, "expected");
		string failure =
			optionsProvider.Options.Value.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected, result);

		await That(result.IsMatch).IsTrue();
		await That(failure).IsEmpty();
	}

	[Test]
	public async Task WhenComparedAgain_ShouldDescribeOnlyTheLastComparison()
	{
		string expected = "{}";
#pragma warning disable aweXpect0001
		IOptionsProvider<StringEqualityOptions> optionsProvider = That("").IsEqualTo(expected).AsJson();
#pragma warning restore aweXpect0001

		await optionsProvider.Options.Value.MatchWithExplanation("{\"foo\":1}", expected, "expected");
		MatchResult result = await optionsProvider.Options.Value.MatchWithExplanation("{\"bar\":1}", expected, "expected");
		string failure =
			optionsProvider.Options.Value.GetExtendedFailure("it", ExpectationGrammars.None, "{\"bar\":1}", expected, result);

		await That(failure).IsEqualTo("it differed as $.bar had unexpected 1")
			.Because("the differences of the previous subject must not be reported for the next one");
	}

	[Test]
	public async Task WhenCallingGetExtendedFailureWithoutAreConsideredEqual_ShouldReturnEmptystring()
	{
		string actual = "foo";
		string expected = "bar";
#pragma warning disable aweXpect0001
		IOptionsProvider<StringEqualityOptions> optionsProvider = That(actual).IsEqualTo(expected).AsJson();
#pragma warning restore aweXpect0001

		string failure =
			optionsProvider.Options.Value.GetExtendedFailure("it", ExpectationGrammars.None, actual, expected, default);

		await That(failure).IsEmpty();
	}

	[Test]
	public async Task WhenIgnoringCaseAfterAsJson_ShouldThrowAtIgnoringCase()
	{
		StringEqualityResult result = new StringEqualityResult().AsJson();

		void Act() => result.IgnoringCase();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("IgnoringCase cannot be combined with AsJson.")
			.Because("the conflict is rejected at the call that specifies it, not only when the expectation is evaluated");
	}

	[Test]
	public async Task WhenIgnoringCaseBeforeAsJson_ShouldThrowAtAsJson()
	{
		StringEqualityResult result = new StringEqualityResult().IgnoringCase();

		void Act() => result.AsJson();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("IgnoringCase cannot be combined with AsJson.")
			.Because("the conflict is rejected at the call that specifies it, not only when the expectation is evaluated");
	}

	[Test]
	public async Task WhenIgnoringCaseIsDisabled_ShouldAllowAsJson()
	{
		StringEqualityResult result = new StringEqualityResult().AsJson();

		void Act() => result.IgnoringCase(false);

		await That(Act).DoesNotThrow()
			.Because("a casing that is not ignored is what a JSON comparison does anyway");
	}

	[Test]
	public async Task WhenTheResultCopiesOnOption_ShouldSetAsJsonOnACopy()
	{
		CopyingResult sut = new(StringEqualityOptions.Default);
		string unchangedOptions = sut.Options.Value.ToString();

		CopyingResult result = sut.AsJson();

		await That(result).IsNotSameAs(sut);
		await That(result.Options.Value.ToString()).IsNotEqualTo(unchangedOptions);
		await That(sut.Options.Value.ToString()).IsEqualTo(unchangedOptions)
			.Because("a stored result must not be changed by an option on its copy");
		await That(() => sut.IgnoringCase()).DoesNotThrow()
			.Because("the stored result has no JSON match type that the option could compete with");
	}

	[Test]
	public async Task WhenUsingComparerAfterAsJson_ShouldThrowAtUsing()
	{
		StringEqualityResult result = new StringEqualityResult().AsJson();

		void Act() => result.Using(StringComparer.Ordinal);

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with AsJson.")
			.Because("the conflict is rejected at the call that specifies it, not only when the expectation is evaluated");
	}

	[Test]
	public async Task WhenUsingComparerBeforeAsJson_ShouldThrowAtAsJson()
	{
		StringEqualityResult result = new StringEqualityResult().Using(StringComparer.Ordinal);

		void Act() => result.AsJson();

		await That(Act).Throws<InvalidOperationException>()
			.WithMessage("Using cannot be combined with AsJson.")
			.Because("the conflict is rejected at the call that specifies it, not only when the expectation is evaluated");
	}

	private sealed class CopyingResult(StringEqualityOptions options)
		: IOptionsProvider<StringEqualityOptions>,
			IStringMatchTypeOptions,
			ICopyOnOption<StringEqualityOptions, CopyingResult>
	{
		public OptionsSlot<StringEqualityOptions> Options { get; } = new(options);

		public CopyingResult WithOptions(StringEqualityOptions copiedOptions) => new(copiedOptions);
	}

	private sealed class StringEqualityResult : IOptionsProvider<StringEqualityOptions>, IStringMatchTypeOptions
	{
		public OptionsSlot<StringEqualityOptions> Options { get; } = new(StringEqualityOptions.Default);
	}
}
