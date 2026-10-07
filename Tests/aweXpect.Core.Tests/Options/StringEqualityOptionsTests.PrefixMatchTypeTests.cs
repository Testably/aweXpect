using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class PrefixMatchTypeTests
	{
		[Test]
		[Arguments("foobar", "foo", true)]
		[Arguments("foo", "foo", true)]
		[Arguments("foobar", "bar", false)]
		[Arguments("foo", "foobar", false)]
		[Arguments("Foobar", "foo", false)]
		public async Task AreConsideredEqual_ShouldCompareThePrefix(string actual, string expected,
			bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
		[Arguments("foobar", true)]
		[Arguments("fo", false)]
		public async Task AreConsideredEqual_WhenAComparerIsUsed_ShouldCompareThePrefixWithIt(string actual,
			bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().Using(StringComparer.OrdinalIgnoreCase);

			bool result = await sut.AreConsideredEqual(actual, "FOO");

			await That(result).IsEqualTo(expectMatch)
				.Because("a subject shorter than the prefix cannot start with it");
		}

		[Test]
		[Arguments(false, false)]
		[Arguments(true, true)]
		public async Task AreConsideredEqual_WhenCaseIsIgnored_ShouldIgnoreCase(bool ignoreCase, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringCase(ignoreCase);

			bool result = await sut.AreConsideredEqual("FOObar", "foo");

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
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

		[Test]
		[Arguments("foo")]
		[Arguments(null)]
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

		[Test]
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

		[Test]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpaceThatIsIgnored_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringLeadingWhiteSpace();

			async Task Act() => await sut.AreConsideredEqual(" foo", " ");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("the prefix is checked after the normalization, where it is just as meaningless as ''");
		}

		[Test]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpaceThatIsNotIgnored_ShouldCompareIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			bool result = await sut.AreConsideredEqual(" foo", " ");

			await That(result).IsTrue()
				.Because("whitespace that is not ignored is a regular prefix");
		}

		[Test]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldIgnoreTheIndentationOfEachLine()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("  foo\r\n    bar\nbaz", "foo\n  bar");

			await That(result).IsTrue();
		}

		[Test]
		[Arguments(false, false)]
		[Arguments(true, true)]
		public async Task AreConsideredEqual_WhenLeadingWhiteSpaceIsIgnored_ShouldIgnoreIt(
			bool ignoreLeadingWhiteSpace, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);

			bool result = await sut.AreConsideredEqual("  foobar", " foo");

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
		[Arguments(false, false)]
		[Arguments(true, true)]
		public async Task AreConsideredEqual_WhenNewlineStyleIsIgnored_ShouldIgnoreIt(
			bool ignoreNewlineStyle, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringNewlineStyle(ignoreNewlineStyle);

			bool result = await sut.AreConsideredEqual("foo\r\nbar\r\nbaz", "foo\nbar");

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
		public async Task AreConsideredEqual_WhenSubjectIsNull_ShouldReturnFalse()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			bool result = await sut.AreConsideredEqual(null, "foo");

			await That(result).IsFalse();
		}

		[Test]
		[Arguments(false, false)]
		[Arguments(true, true)]
		public async Task AreConsideredEqual_WhenTrailingWhiteSpaceIsIgnored_ShouldIgnoreItOnTheExpectedValue(
			bool ignoreTrailingWhiteSpace, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

			bool result = await sut.AreConsideredEqual("foo", "foo ");

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
		[Arguments("AbbeyRoad", "Abbey ", false)]
		[Arguments("Abbey Road", "Abbey ", true)]
		[Arguments("Abbey", "Abbey \t", true)]
		[Arguments("Abbey \t ", "Abbey  ", true)]
		[Arguments("Abbey Road", "Abbey Road  ", true)]
		[Arguments(" foo", " ", true)]
		[Arguments("foo", " ", false)]
		public async Task
			AreConsideredEqual_WhenTrailingWhiteSpaceIsIgnored_ShouldOnlyIgnoreTheWhiteSpaceOfThePrefixAtTheEndOfTheSubject(
				string actual, string expected, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringTrailingWhiteSpace();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectMatch)
				.Because("the whitespace at the end of the prefix is only optional where it reaches the end of the subject");
		}

		[Test]
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

		[Test]
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

		[Test]
		public async Task AsPrefix_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new("expected");

			StringEqualityOptions result = sut.AsPrefix();

			await That(result).IsSameAs(sut);
		}

		[Test]
		public async Task CountOccurrences_WhenExpectedIsEmptyAfterTheIndentationIsIgnored_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringIndentation();

			async Task Act() => await sut.CountOccurrences("some text", " ");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' prefix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("the counted prefix is the normalized one, which every subject starts with");
		}

		[Test]
		[Arguments(ExpectationGrammars.Active, "starts with \"foo\"")]
		[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not start with \"foo\"")]
		[Arguments(ExpectationGrammars.None, "starting with \"foo\"")]
		[Arguments(ExpectationGrammars.Negated, "not starting with \"foo\"")]
		public async Task GetExpectation_ShouldDescribeThePrefix(ExpectationGrammars grammars, string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			string result = sut.GetExpectation("foo", grammars);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task GetExpectation_ShouldRenderTheOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix().IgnoringCase().IgnoringLeadingWhiteSpace();

			string result = sut.GetExpectation("foo", ExpectationGrammars.Active);

			await That(result).IsEqualTo("starts with \"foo\" ignoring case and leading whitespace")
				.Because("an option that decides the outcome must not be invisible in the expectation");
		}

		[Test]
		public async Task GetExtendedFailure_WhenSubjectIsShorter_ShouldEscapeTheMissingText()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsPrefix();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "foo", "foo\n\"bar\"");

			await That(result).IsEqualTo("""
			                             it was "foo" with a length of 3, which is shorter than the expected length of 9 and misses:
			                               "\n\"bar\""
			                             """).IgnoringNewlineStyle()
				.Because("the missing text is escaped like the other values in the message");
		}

		[Test]
		[Arguments(false, false, " as prefix")]
		[Arguments(true, false, " as prefix ignoring case")]
		[Arguments(false, true, " as prefix ignoring leading whitespace")]
		[Arguments(true, true, " as prefix ignoring case and leading whitespace")]
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
