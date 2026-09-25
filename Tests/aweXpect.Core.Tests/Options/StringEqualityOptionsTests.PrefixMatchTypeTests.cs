using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class PrefixMatchTypeTests
	{
		[Theory]
		[InlineData("foobar", "foo", true)]
		[InlineData("foo", "foo", true)]
		[InlineData("foobar", "bar", false)]
		[InlineData("foo", "foobar", false)]
		[InlineData("Foobar", "foo", false)]
		public async Task AreConsideredEqual_ShouldCompareThePrefix(string actual, string expected,
			bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectMatch);
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(true, true)]
		public async Task AreConsideredEqual_WhenCaseIsIgnored_ShouldIgnoreCase(bool ignoreCase, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringCase(ignoreCase);

			bool result = await sut.AreConsideredEqual("FOObar", "foo");

			await That(result).IsEqualTo(expectMatch);
		}

		[Fact]
		public async Task AreConsideredEqual_WhenExpectedIsEmpty_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			async Task Act() => await sut.AreConsideredEqual("foo", "");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an empty prefix matches every value and therefore says nothing about the subject");
		}

		[Theory]
		[InlineData("foo")]
		[InlineData(null)]
		public async Task AreConsideredEqual_WhenExpectedIsNull_ShouldThrowArgumentNullException(string? actual)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			async Task Act() => await sut.AreConsideredEqual(actual, (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' prefix cannot be null.").AsPrefix()
				.Because("a missing prefix is rejected before the subject is looked at");
		}

		[Theory]
		[InlineData(true, false)]
		[InlineData(false, true)]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpaceThatIsIgnored_ShouldThrowArgumentException(
			bool ignoreLeadingWhiteSpace, bool ignoreTrailingWhiteSpace)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix()
				.IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace)
				.IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

			async Task Act() => await sut.AreConsideredEqual(" foo", " ");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("the prefix is checked after the normalization, where it is just as meaningless as ''");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpaceThatIsNotIgnored_ShouldCompareIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			bool result = await sut.AreConsideredEqual(" foo", " ");

			await That(result).IsTrue()
				.Because("white-space that is not ignored is a regular prefix");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpace_ShouldThrowBeforeTheTaskIsAwaited()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringLeadingWhiteSpace();

#if NET8_0_OR_GREATER
			void Act() => _ = sut.AreConsideredEqual("foo", " ").AsTask();
#else
			void Act() => _ = sut.AreConsideredEqual("foo", " ");
#endif

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an unusable prefix must throw at the call instead of inside the returned task");
		}

		[Fact]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldIgnoreTheIndentationOfEachLine()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("  foo\r\n    bar\nbaz", "foo\n  bar");

			await That(result).IsTrue();
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(true, true)]
		public async Task AreConsideredEqual_WhenLeadingWhiteSpaceIsIgnored_ShouldIgnoreIt(
			bool ignoreLeadingWhiteSpace, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);

			bool result = await sut.AreConsideredEqual("  foobar", " foo");

			await That(result).IsEqualTo(expectMatch);
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(true, true)]
		public async Task AreConsideredEqual_WhenNewlineStyleIsIgnored_ShouldIgnoreIt(
			bool ignoreNewlineStyle, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringNewlineStyle(ignoreNewlineStyle);

			bool result = await sut.AreConsideredEqual("foo\r\nbar\r\nbaz", "foo\nbar");

			await That(result).IsEqualTo(expectMatch);
		}

		[Fact]
		public async Task AreConsideredEqual_WhenSubjectIsNull_ShouldReturnFalse()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			bool result = await sut.AreConsideredEqual(null, "foo");

			await That(result).IsFalse();
		}

		[Theory]
		[InlineData(false, false)]
		[InlineData(true, true)]
		public async Task AreConsideredEqual_WhenTrailingWhiteSpaceIsIgnored_ShouldIgnoreItOnTheExpectedValue(
			bool ignoreTrailingWhiteSpace, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

			bool result = await sut.AreConsideredEqual("foo", "foo ");

			await That(result).IsEqualTo(expectMatch);
		}

		[Fact]
		public async Task AreConsideredEqual_WithParameterName_WhenExpectedIsEmpty_ShouldNameIt()
		{
			StringEqualityOptions sut = new("unexpected");
			sut.AsPrefix().IgnoringLeadingWhiteSpace();

			async Task Act() => await sut.AreConsideredEqual("foo", " ");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'unexpected' prefix cannot be empty.").AsPrefix().And
				.WithParamName("unexpected")
				.Because("a negated expectation receives the prefix as 'unexpected'");
		}

		[Fact]
		public async Task AreConsideredEqual_WithParameterName_WhenExpectedIsNull_ShouldNameIt()
		{
			StringEqualityOptions sut = new("unexpected");
			sut.AsPrefix();

			async Task Act() => await sut.AreConsideredEqual("foo", (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' prefix cannot be null.").AsPrefix()
				.Because("a negated expectation receives the prefix as 'unexpected'");
		}

		[Fact]
		public async Task AsPrefix_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new("expected");

			StringEqualityOptions result = sut.AsPrefix();

			await That(result).IsSameAs(sut);
		}

		[Theory]
		[InlineData(ExpectationGrammars.Active, "starts with \"foo\"")]
		[InlineData(ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not start with \"foo\"")]
		[InlineData(ExpectationGrammars.None, "starting with \"foo\"")]
		[InlineData(ExpectationGrammars.Negated, "not starting with \"foo\"")]
		public async Task GetExpectation_ShouldDescribeThePrefix(ExpectationGrammars grammars, string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			string result = sut.GetExpectation("foo", grammars);

			await That(result).IsEqualTo(expected);
		}

		[Fact]
		public async Task GetExpectation_ShouldRenderTheOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringCase().IgnoringLeadingWhiteSpace();

			string result = sut.GetExpectation("foo", ExpectationGrammars.Active);

			await That(result).IsEqualTo("starts with \"foo\" ignoring case and leading whitespace")
				.Because("an option that decides the outcome must not be invisible in the expectation");
		}

		[Theory]
		[InlineData(false, false, " as prefix")]
		[InlineData(true, false, " as prefix ignoring case")]
		[InlineData(false, true, " as prefix ignoring leading whitespace")]
		[InlineData(true, true, " as prefix ignoring case and leading whitespace")]
		public async Task ToString_ShouldIncludeTheMatchTypeAndTheOptions(bool ignoreCase,
			bool ignoreLeadingWhiteSpace, string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringCase(ignoreCase).IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);

			string result = sut.ToString();

			await That(result).IsEqualTo(expected);
		}
	}
}
