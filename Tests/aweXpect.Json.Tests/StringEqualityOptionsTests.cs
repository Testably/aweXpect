using aweXpect.Core;
using aweXpect.Options;

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

		await optionsProvider.Options.AreConsideredEqual("foo", expected);
		await optionsProvider.Options.AreConsideredEqual("{\"bar\":1}", expected);
		string failure =
			optionsProvider.Options.GetExtendedFailure("it", ExpectationGrammars.None, "{\"bar\":1}", expected);

		await That(failure).IsEqualTo("it differed as $.bar had unexpected 1")
			.Because("the parse error of the previous subject must not be reported for the next one");
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
}
