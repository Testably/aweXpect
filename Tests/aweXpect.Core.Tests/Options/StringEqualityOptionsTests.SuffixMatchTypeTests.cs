using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class SuffixMatchTypeTests
	{
		[Theory]
		[InlineData("foobar", "bar", true)]
		[InlineData("bar", "bar", true)]
		[InlineData("foobar", "foo", false)]
		[InlineData("bar", "foobar", false)]
		[InlineData("fooBar", "bar", false)]
		public async Task AreConsideredEqual_ShouldCompareTheSuffix(string actual, string expected,
			bool expectMatch)
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectMatch);
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(true, true)]
		public async Task AreConsideredEqual_WhenCaseIsIgnored_ShouldIgnoreCase(bool ignoreCase, bool expectMatch)
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix().IgnoringCase(ignoreCase);

			bool result = await sut.AreConsideredEqual("fooBAR", "bar");

			await That(result).IsEqualTo(expectMatch);
		}

		[Fact]
		public async Task AreConsideredEqual_WhenExpectedIsEmpty_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix();

			async Task Act() => await sut.AreConsideredEqual("foo", "");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an empty suffix matches every value and therefore says nothing about the subject");
		}

		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpaceThatIsIgnored_ShouldThrowArgumentException(
			bool ignoreLeadingWhiteSpace, bool ignoreTrailingWhiteSpace)
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix()
				.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace)
				.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

			async Task Act() => await sut.AreConsideredEqual("foo ", " ");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("the suffix is checked after the normalization, where it is just as meaningless as ''");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpaceThatIsNotIgnored_ShouldCompareIt()
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix();

			bool result = await sut.AreConsideredEqual("foo ", " ");

			await That(result).IsTrue()
				.Because("white-space that is not ignored is a regular suffix");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpace_ShouldThrowBeforeTheTaskIsAwaited()
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix().IgnoringTrailingWhiteSpace();

			void Act() => _ = sut.AreConsideredEqual("foo", " ");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an unusable suffix must throw at the call instead of inside the returned task");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldIgnoreTheIndentationOfEachLine()
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix().IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("foo\r\n  bar\n    baz", "bar\n  baz");

			await That(result).IsTrue();
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(true, true)]
		public async Task AreConsideredEqual_WhenLeadingWhiteSpaceIsIgnored_ShouldIgnoreItOnTheExpectedValue(
			bool ignoreLeadingWhiteSpace, bool expectMatch)
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix().IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);

			bool result = await sut.AreConsideredEqual("bar", " bar");

			await That(result).IsEqualTo(expectMatch);
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(true, true)]
		public async Task AreConsideredEqual_WhenNewlineStyleIsIgnored_ShouldIgnoreIt(
			bool ignoreNewlineStyle, bool expectMatch)
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix().IgnoringNewlineStyle(ignoreNewlineStyle);

			bool result = await sut.AreConsideredEqual("foo\r\nbar\r\nbaz", "bar\nbaz");

			await That(result).IsEqualTo(expectMatch);
		}

		[Theory]
		[InlineData("foo", null, false)]
		[InlineData(null, "foo", false)]
		[InlineData(null, null, true)]
		public async Task AreConsideredEqual_WhenSubjectOrExpectedIsNull_ShouldOnlyMatchWhenBothAreNull(
			string? actual, string? expected, bool expectMatch)
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectMatch)
				.Because("a missing suffix is not rejected, but it can only match a missing subject");
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(true, true)]
		public async Task AreConsideredEqual_WhenTrailingWhiteSpaceIsIgnored_ShouldIgnoreIt(
			bool ignoreTrailingWhiteSpace, bool expectMatch)
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix().IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

			bool result = await sut.AreConsideredEqual("foobar  ", "bar ");

			await That(result).IsEqualTo(expectMatch);
		}

		[Fact]
		public async Task AreConsideredEqual_WithParameterName_WhenExpectedIsEmpty_ShouldNameIt()
		{
			StringEqualityOptions sut = new("unexpected");
			sut.AsSuffix().IgnoringTrailingWhiteSpace();

			async Task Act() => await sut.AreConsideredEqual("foo", " ");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'unexpected' suffix cannot be empty.").AsPrefix().And
				.WithParamName("unexpected")
				.Because("a negated expectation receives the suffix as 'unexpected'");
		}

		[Fact]
		public async Task AsSuffix_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new();

			StringEqualityOptions result = sut.AsSuffix();

			await That(result).IsSameAs(sut);
		}

		[Theory]
		[InlineData(ExpectationGrammars.Active, "ends with \"foo\"")]
		[InlineData(ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not end with \"foo\"")]
		[InlineData(ExpectationGrammars.None, "ending with \"foo\"")]
		[InlineData(ExpectationGrammars.Negated, "not ending with \"foo\"")]
		public async Task GetExpectation_ShouldDescribeTheSuffix(ExpectationGrammars grammars, string expected)
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix();

			string result = sut.GetExpectation("foo", grammars);

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task GetExpectation_ShouldRenderTheOptions()
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix().IgnoringCase().IgnoringTrailingWhiteSpace();

			string result = sut.GetExpectation("foo", ExpectationGrammars.Active);

			await That(result).IsEqualTo("ends with \"foo\" ignoring case and trailing whitespace")
				.Because("an option that decides the outcome must not be invisible in the expectation");
		}

		[Theory]
		[InlineData(false, false, " as suffix")]
		[InlineData(true, false, " as suffix ignoring case")]
		[InlineData(false, true, " as suffix ignoring trailing whitespace")]
		[InlineData(true, true, " as suffix ignoring case and trailing whitespace")]
		public async Task ToString_ShouldIncludeTheMatchTypeAndTheOptions(bool ignoreCase,
			bool ignoreTrailingWhiteSpace, string expected)
		{
			StringEqualityOptions sut = new();
			sut.AsSuffix().IgnoringCase(ignoreCase).IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

			string result = sut.ToString();

			await That(result).IsEqualTo(expected);
		}
	}
}
