using aweXpect.Options;

namespace aweXpect.Core.Tests.Options;

public sealed partial class StringEqualityOptionsTests
{
	public sealed class SuffixMatchTypeTests
	{
		[Test]
		[Arguments("foobar", "bar", true)]
		[Arguments("bar", "bar", true)]
		[Arguments("foobar", "foo", false)]
		[Arguments("bar", "foobar", false)]
		[Arguments("fooBar", "bar", false)]
		public async Task AreConsideredEqual_ShouldCompareTheSuffix(string actual, string expected,
			bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
		[Arguments("barfoo", true)]
		[Arguments("oo", false)]
		public async Task AreConsideredEqual_WhenAComparerIsUsed_ShouldCompareTheSuffixWithIt(string actual,
			bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().Using(StringComparer.OrdinalIgnoreCase);

			bool result = await sut.AreConsideredEqual(actual, "FOO");

			await That(result).IsEqualTo(expectMatch)
				.Because("a subject shorter than the suffix cannot end with it");
		}

		[Test]
		[Arguments(false, false)]
		[Arguments(true, true)]
		public async Task AreConsideredEqual_WhenCaseIsIgnored_ShouldIgnoreCase(bool ignoreCase, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringCase(ignoreCase);

			bool result = await sut.AreConsideredEqual("fooBAR", "bar");

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
		public async Task AreConsideredEqual_WhenExpectedIsEmpty_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			async Task Act() => await sut.AreConsideredEqual("foo", "");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an empty suffix matches every value and therefore says nothing about the subject");
		}

		[Test]
		[Arguments("foo")]
		[Arguments(null)]
		public async Task AreConsideredEqual_WhenExpectedIsNull_ShouldThrowArgumentNullException(string? actual)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			async Task Act() => await sut.AreConsideredEqual(actual, (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("expected").And
				.WithMessage("The 'expected' suffix cannot be null.").AsPrefix()
				.Because("a missing suffix is rejected before the subject is looked at");
		}

		[Test]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpace_ShouldThrowBeforeTheTaskIsAwaited()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringTrailingWhiteSpace();

#if NET8_0_OR_GREATER
			void Act() => _ = sut.AreConsideredEqual("foo", " ").AsTask();
#else
			void Act() => _ = sut.AreConsideredEqual("foo", " ");
#endif

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("an unusable suffix must throw at the call instead of inside the returned task");
		}

		[Test]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpaceThatIsIgnored_ShouldThrowArgumentException()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringTrailingWhiteSpace();

			async Task Act() => await sut.AreConsideredEqual("foo ", " ");

			await That(Act).Throws<ArgumentException>()
				.WithMessage("The 'expected' suffix cannot be empty.").AsPrefix().And
				.WithParamName("expected")
				.Because("the suffix is checked after the normalization, where it is just as meaningless as ''");
		}

		[Test]
		public async Task AreConsideredEqual_WhenExpectedIsOnlyWhiteSpaceThatIsNotIgnored_ShouldCompareIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			bool result = await sut.AreConsideredEqual("foo ", " ");

			await That(result).IsTrue()
				.Because("whitespace that is not ignored is a regular suffix");
		}

		[Test]
		public async Task AreConsideredEqual_WhenIndentationIsIgnored_ShouldIgnoreTheIndentationOfEachLine()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringIndentation();

			bool result = await sut.AreConsideredEqual("foo\r\n  bar\n    baz", "bar\n  baz");

			await That(result).IsTrue();
		}

		[Test]
		[Arguments(false, false)]
		[Arguments(true, true)]
		public async Task AreConsideredEqual_WhenLeadingWhiteSpaceIsIgnored_ShouldIgnoreItOnTheExpectedValue(
			bool ignoreLeadingWhiteSpace, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringLeadingWhiteSpace(ignoreLeadingWhiteSpace);

			bool result = await sut.AreConsideredEqual("bar", " bar");

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
		[Arguments("RoadAbbey", " Abbey", false)]
		[Arguments("Road Abbey", " Abbey", true)]
		[Arguments("Abbey", "\t Abbey", true)]
		[Arguments(" \t Abbey", "  Abbey", true)]
		[Arguments("Road Abbey", "  Road Abbey", true)]
		[Arguments("foo ", " ", true)]
		[Arguments("foo", " ", false)]
		public async Task
			AreConsideredEqual_WhenLeadingWhiteSpaceIsIgnored_ShouldOnlyIgnoreTheWhiteSpaceOfTheSuffixAtTheStartOfTheSubject(
				string actual, string expected, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringLeadingWhiteSpace();

			bool result = await sut.AreConsideredEqual(actual, expected);

			await That(result).IsEqualTo(expectMatch)
				.Because("the whitespace at the start of the suffix is only optional where it reaches the start of the subject");
		}

		[Test]
		[Arguments(false, false)]
		[Arguments(true, true)]
		public async Task AreConsideredEqual_WhenNewlineStyleIsIgnored_ShouldIgnoreIt(
			bool ignoreNewlineStyle, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringNewlineStyle(ignoreNewlineStyle);

			bool result = await sut.AreConsideredEqual("foo\r\nbar\r\nbaz", "bar\nbaz");

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
		public async Task AreConsideredEqual_WhenSubjectIsNull_ShouldReturnFalse()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			bool result = await sut.AreConsideredEqual(null, "foo");

			await That(result).IsFalse();
		}

		[Test]
		[Arguments(false, false)]
		[Arguments(true, true)]
		public async Task AreConsideredEqual_WhenTrailingWhiteSpaceIsIgnored_ShouldIgnoreIt(
			bool ignoreTrailingWhiteSpace, bool expectMatch)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

			bool result = await sut.AreConsideredEqual("foobar  ", "bar ");

			await That(result).IsEqualTo(expectMatch);
		}

		[Test]
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

		[Test]
		public async Task AreConsideredEqual_WithParameterName_WhenExpectedIsNull_ShouldNameIt()
		{
			StringEqualityOptions sut = new("unexpected");
			sut.AsSuffix();

			async Task Act() => await sut.AreConsideredEqual("foo", (string?)null);

			await That(Act).Throws<ArgumentNullException>()
				.WithParamName("unexpected").And
				.WithMessage("The 'unexpected' suffix cannot be null.").AsPrefix()
				.Because("a negated expectation receives the suffix as 'unexpected'");
		}

		[Test]
		public async Task AsSuffix_ShouldReturnSameInstance()
		{
			StringEqualityOptions sut = new("expected");

			StringEqualityOptions result = sut.AsSuffix();

			await That(result).IsSameAs(sut);
		}

		[Test]
		[Arguments(ExpectationGrammars.Active, "ends with \"foo\"")]
		[Arguments(ExpectationGrammars.Active | ExpectationGrammars.Negated, "does not end with \"foo\"")]
		[Arguments(ExpectationGrammars.None, "ending with \"foo\"")]
		[Arguments(ExpectationGrammars.Negated, "not ending with \"foo\"")]
		public async Task GetExpectation_ShouldDescribeTheSuffix(ExpectationGrammars grammars, string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			string result = sut.GetExpectation("foo", grammars);

			await That(result).IsEqualTo(expected);
		}

		[Test]
		public async Task GetExpectation_ShouldRenderTheOptions()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringCase().IgnoringTrailingWhiteSpace();

			string result = sut.GetExpectation("foo", ExpectationGrammars.Active);

			await That(result).IsEqualTo("ends with \"foo\" ignoring case and trailing whitespace")
				.Because("an option that decides the outcome must not be invisible in the expectation");
		}

		[Test]
		public async Task GetExtendedFailure_WhenActualHasUnexpectedTrailingWhiteSpace_ShouldNameIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "some text \t ", "some text");

			await That(result)
				.IsEqualTo("it was \"some text \\t \", which has unexpected whitespace (\" \\t \" at the end)");
		}

		[Test]
		public async Task GetExtendedFailure_WhenActualHasUnexpectedTrailingWhiteSpaceAndCaseIsIgnored_ShouldNameIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringCase();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "SOME TEXT \t ", "some text");

			await That(result)
				.IsEqualTo("it was \"SOME TEXT \\t \", which has unexpected whitespace (\" \\t \" at the end)");
		}

		[Test]
		public async Task GetExtendedFailure_WhenActualMissesLeadingWhiteSpace_ShouldNameIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "some text", " \t some text");

			await That(result)
				.IsEqualTo("it was \"some text\", which misses some whitespace (\" \\t \" at the beginning)");
		}

		[Test]
		public async Task GetExtendedFailure_WhenActualMissesTrailingWhiteSpace_ShouldNameIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "some text", "some text \t ");

			await That(result)
				.IsEqualTo("it was \"some text\", which misses some whitespace (\" \\t \" at the end)");
		}

		[Test]
		public async Task GetExtendedFailure_WhenActualMissesTrailingWhiteSpaceWithCustomComparer_ShouldNameIt()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().Using(StringComparer.OrdinalIgnoreCase);

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "SOME TEXT", "some text \t ");

			await That(result)
				.IsEqualTo("it was \"SOME TEXT\", which misses some whitespace (\" \\t \" at the end)");
		}

		[Test]
		public async Task GetExtendedFailure_WhenTrailingWhiteSpaceIsNotTheOnlyDifference_ShouldShowTheDifference()
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix();

			string result = sut.GetExtendedFailure("it", ExpectationGrammars.None, "text \t ", "some text");

			await That(result).StartsWith("it was \"text \\t \", which differs")
				.Because("removing the whitespace would still leave the subject without the expected suffix");
		}

		[Test]
		[Arguments(false, false, " as suffix")]
		[Arguments(true, false, " as suffix ignoring case")]
		[Arguments(false, true, " as suffix ignoring trailing whitespace")]
		[Arguments(true, true, " as suffix ignoring case and trailing whitespace")]
		public async Task ToString_ShouldIncludeTheMatchTypeAndTheOptions(bool ignoreCase,
			bool ignoreTrailingWhiteSpace, string expected)
		{
			StringEqualityOptions sut = new("expected");
			sut.AsSuffix().IgnoringCase(ignoreCase).IgnoringTrailingWhiteSpace(ignoreTrailingWhiteSpace);

			string result = sut.ToString();

			await That(result).IsEqualTo(expected);
		}
	}
}
