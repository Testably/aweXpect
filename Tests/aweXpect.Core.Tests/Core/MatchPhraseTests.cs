namespace aweXpect.Core.Tests.Core;

public sealed class MatchPhraseTests
{
	[Test]
	[Arguments(ExpectationGrammars.Active, "is titled like \"x\"")]
	[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Plural, "are titled like \"x\"")]
	[Arguments(ExpectationGrammars.None, "titled like \"x\"")]
	[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Negated, "is not titled like \"x\"")]
	[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Negated | ExpectationGrammars.Plural,
		"are not titled like \"x\"")]
	[Arguments(ExpectationGrammars.Negated, "not titled like \"x\"")]
	public async Task Being_ShouldWriteEveryGrammar(ExpectationGrammars grammars, string expected)
	{
		MatchPhrase sut = MatchPhrase.Being("titled like");

		string result = sut.GetExpectation("\"x\"", grammars, false);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task Being_WhenComparisonIsNull_ShouldThrowArgumentNullException()
	{
		void Act() => MatchPhrase.Being(null!);

		await That(Act).Throws<ArgumentNullException>()
			.WithMessage("The 'comparison' cannot be null.").AsPrefix().And
			.WithParamName("comparison");
	}

	[Test]
	[Arguments(null, null, "\"x\"")]
	[Arguments("an item", "equal to", "an item equal to \"x\"")]
	[Arguments(null, "equal to", "equal to \"x\"")]
	public async Task EqualTo_GetItemExpectation_ShouldUseTheComparisonOfTheCaller(string? itemNoun,
		string? comparison, string expected)
	{
		MatchPhrase sut = MatchPhrase.EqualTo;

		string result = sut.GetItemExpectation("\"x\"", itemNoun, comparison);

		await That(result).IsEqualTo(expected)
			.Because("the equality may be left out where the verb already names the item");
	}

	[Test]
	public async Task GetExpectation_WhenAlwaysActive_ShouldIgnoreTheMissingActiveGrammar()
	{
		MatchPhrase sut = MatchPhrase.EqualTo;

		string result = sut.GetExpectation("3", ExpectationGrammars.Negated, true);

		await That(result).IsEqualTo("is not equal to 3");
	}

	[Test]
	[Arguments(null, "titled like \"x\"")]
	[Arguments("an item", "an item titled like \"x\"")]
	public async Task GetItemExpectation_ShouldAlwaysNameAComparisonOtherThanEquality(string? itemNoun,
		string expected)
	{
		MatchPhrase sut = MatchPhrase.Being("titled like");

		string result = sut.GetItemExpectation("\"x\"", itemNoun, "equal to");

		await That(result).IsEqualTo(expected);
	}

	[Test]
	[Arguments(ExpectationGrammars.Active, "starts with \"x\"")]
	[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Plural, "start with \"x\"")]
	[Arguments(ExpectationGrammars.None, "starting with \"x\"")]
	[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not start with \"x\"")]
	[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Negated | ExpectationGrammars.Plural,
		"do not start with \"x\"")]
	[Arguments(ExpectationGrammars.Negated, "not starting with \"x\"")]
	public async Task Verb_ShouldWriteEveryGrammar(ExpectationGrammars grammars, string expected)
	{
		MatchPhrase sut = MatchPhrase.Verb("starts with", "start with", "starting with");

		string result = sut.GetExpectation("\"x\"", grammars, false);

		await That(result).IsEqualTo(expected);
	}

	[Test]
	public async Task WithName_ShouldKeepTheWords()
	{
		MatchPhrase sut = MatchPhrase.Being("titled like").WithName(" as title");

		string result = sut.GetExpectation("\"x\"", ExpectationGrammars.Active, false);

		await That(result).IsEqualTo("is titled like \"x\"");
		await That(sut.Name).IsEqualTo(" as title");
	}

	[Test]
	public async Task WithPattern_ShouldPrecedeAnItemWithTheKind()
	{
		MatchPhrase sut = MatchPhrase.Verb("matches", "match", "matching").WithPattern("glob", "glob pattern");

		await That(sut.PatternItemPrefix).IsEqualTo("glob ");
		await That(sut.PatternNoun).IsEqualTo("glob pattern");
	}

	[Test]
	public async Task WithSuffix_ShouldFollowTheExpectedValue()
	{
		MatchPhrase sut = MatchPhrase.Verb("matches", "match", "matching").WithSuffix(" as block");

		string result = sut.GetExpectation("\"x\"", ExpectationGrammars.Active, false);

		await That(result).IsEqualTo("matches \"x\" as block");
	}

	[Test]
	public async Task WithUnformattedExpected_ShouldNotFormatTheExpectedValue()
	{
		MatchPhrase sut = MatchPhrase.Being("JSON equivalent to");

		MatchPhrase result = sut.WithUnformattedExpected();

		await That(sut.FormatsExpected).IsTrue();
		await That(result.FormatsExpected).IsFalse();
	}
}
