using System.Runtime.CompilerServices;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Core.Tests.Core.Extending;

public sealed class ExpectationBuilderExtensionsTests
{
	[Test]
	public async Task AddEqualityConstraint_ShouldCompareWithTheOptionsOfTheResult()
	{
		string subject = "foo";

		async Task Act()
			=> await IsEqualTo(That<string?>(subject), "FOO", false).Using(StringComparer.OrdinalIgnoreCase);

		await That(Act).DoesNotThrow()
			.Because("the comparer of the result ignores the casing");
	}

	[Test]
	public async Task AddEqualityConstraint_WhenExpectedIsNullableAndSubjectIsNot_ShouldCompareTheValues()
	{
		int subject = 3;

		async Task Act()
			=> await IsEqualTo<int, int?>(That(subject), null, false);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to <null>,
			             but it was 3
			             """);
	}

	[Test]
	public async Task AddEqualityConstraint_WhenNegated_ShouldFailWhenTheValuesAreEqual()
	{
		int subject = 3;

		async Task Act()
			=> await IsEqualTo(That(subject), 3, true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not equal to 3,
			             but it was 3
			             """);
	}

	[Test]
	public async Task AddEqualityConstraint_WhenNegatedAndSubjectIsNull_ShouldSucceed()
	{
		string? subject = null;

		async Task Act()
			=> await IsEqualTo(That(subject), "foo", true);

		await That(Act).DoesNotThrow()
			.Because("a null subject is not equal to a value that is not null");
	}

	[Test]
	public async Task AddEqualityConstraint_WhenNegatedAndTheMatchTypeRejectsTheUnexpectedValue_ShouldNameTheParameter()
	{
		int? subject = 3;

		async Task Act()
			=> await IsEqualTo(That(subject), 3, true, new RejectingMatchType<int?>(3));

		await That(Act).Throws<ArgumentException>()
			.WithParamName("unexpected").And
			.WithMessage("The 'unexpected' value 3 is rejected.").AsPrefix()
			.Because("the negated constraint receives the unexpected value");
	}

	[Test]
	public async Task AddEqualityConstraint_WhenSubjectIsNull_ShouldFail()
	{
		string? subject = null;

		async Task Act()
			=> await IsEqualTo(That(subject), "foo", false);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to "foo",
			             but it was <null>
			             """);
	}

	[Test]
	public async Task AddEqualityConstraint_WhenTheMatchTypeRejectsANullExpectedValue_ShouldThrowItsException()
	{
		string subject = "foo";

		async Task Act()
			=> await IsEqualTo<string?, string?>(That<string?>(subject), null, false,
				new RejectingMatchType<string?>(null));

		await That(Act).Throws<ArgumentException>()
			.WithParamName("expected").And
			.WithMessage("The 'expected' value <null> is rejected.").AsPrefix()
			.Because("a null expected value is validated as well when the subject can be null");
	}

	[Test]
	public async Task AddEqualityConstraint_WhenTheMatchTypeRejectsTheExpectedValue_ShouldThrowItsException()
	{
		int? subject = null;

		async Task Act()
			=> await IsEqualTo(That(subject), 3, false, new RejectingMatchType<int?>(3));

		await That(Act).Throws<ArgumentException>()
			.WithParamName("expected").And
			.WithMessage("The 'expected' value 3 is rejected.").AsPrefix()
			.Because("the match type rejects the expected value before the subject is compared");
	}

	[Test]
	public async Task AddEqualityConstraint_WhenValuesAreEqual_ShouldSucceed()
	{
		int subject = 3;

		async Task Act()
			=> await IsEqualTo(That(subject), 3, false);

		await That(Act).DoesNotThrow();
	}

	[Test]
	public async Task AddEqualityConstraint_WhenValuesDiffer_ShouldFail()
	{
		int subject = 3;

		async Task Act()
			=> await IsEqualTo(That(subject), 4, false);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to 4,
			             but it was 3
			             """);
	}

	[Test]
	public async Task AddEqualityConstraint_WithExpectedExpression_ShouldShowTheExpression()
	{
		int subject = 4;
		int expected = 3;

		async Task Act()
			=> await IsEqualToWithExpression(That(subject), expected, false);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to expected,
			             but it was 4
			             """);
	}

	[Test]
	public async Task AddEqualityConstraint_WithExpectedExpression_ShouldTrimTheCommonWhiteSpace()
	{
		int subject = 4;

		async Task Act()
			=> await IsEqualToWithExpression(That(subject), Sum(
				1,
				2), false);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to Sum(
			             1,
			             2),
			             but it was 4
			             """);
	}

	[Test]
	public async Task AddEqualityConstraint_WithExpectedExpression_WhenExpectedIsNull_ShouldShowNull()
	{
		string subject = "foo";
		string? expected = null;

		async Task Act()
			=> await IsEqualToWithExpression(That<string?>(subject), expected, false);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is equal to <null>,
			             but it was "foo"
			             """);
	}

	[Test]
	public async Task AddEqualityConstraint_WithExpectedExpression_WhenNegated_ShouldShowTheExpression()
	{
		int subject = 3;
		int unexpected = 3;

		async Task Act()
			=> await IsEqualToWithExpression(That(subject), unexpected, true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is not equal to unexpected,
			             but it was 3
			             """);
	}

	[Test]
	public async Task AddStringOccurrenceConstraint_ShouldCountTheOccurrences()
	{
		string subject = "a b a";

		async Task Act()
			=> await ContainsOccurrences(That(subject), "a", false).Exactly(3);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             contains "a" exactly 3 times,
			             but it contained "a" twice in "a b a"
			             """);
	}

	[Test]
	public async Task AddStringOccurrenceConstraint_WhenNegated_ShouldFailWhenTheValueOccurs()
	{
		string subject = "a b a";

		async Task Act()
			=> await ContainsOccurrences(That(subject), "a", true);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             does not contain "a",
			             but it contained "a" at least once in "a b a"
			             """);
	}

	private static ObjectEqualityResult<TSubject, IThat<TSubject>, TSubject> IsEqualTo<TSubject, TExpected>(
		IThat<TSubject> subject, TExpected expected, bool isNegated, ObjectMatchType<TSubject>? matchType = null)
	{
		OptionsHolder<ObjectMatchType<TSubject>> options = new(matchType ?? ObjectMatchType<TSubject>.Default);
		return new ObjectEqualityResult<TSubject, IThat<TSubject>, TSubject>(
			subject.Get().ExpectationBuilder.AddEqualityConstraint(expected, options, isNegated),
			subject, options);
	}

	private static ObjectEqualityResult<TSubject, IThat<TSubject>, TSubject> IsEqualToWithExpression<TSubject,
		TExpected>(IThat<TSubject> subject, TExpected expected, bool isNegated,
		[CallerArgumentExpression("expected")] string expectedExpression = "")
	{
		OptionsHolder<ObjectMatchType<TSubject>> options = new(ObjectMatchType<TSubject>.Default);
		return new ObjectEqualityResult<TSubject, IThat<TSubject>, TSubject>(
			subject.Get().ExpectationBuilder.AddEqualityConstraint(expected, options, isNegated, expectedExpression),
			subject, options);
	}

	private static int Sum(int left, int right) => left + right;

	private sealed class RejectingMatchType<TSubject>(TSubject rejected) : ObjectMatchType<TSubject>("Rejecting")
	{
		public override MatchPhrase Phrase => Default.Phrase;

		public override ObjectMatcher<TSubject> ForEvaluation(MatchEnvironment environment)
			=> Default.ForEvaluation(environment);

		public override void ValidateExpected(TSubject expected, string parameterName)
		{
			if (Equals(expected, rejected))
			{
				throw new ArgumentException(
					$"The '{parameterName}' value {Formatter.Format(expected)} is rejected.", parameterName);
			}
		}
	}

	private static StringOccurrenceCountResult<string, IThat<string>> ContainsOccurrences(IThat<string> subject,
		string expected, bool isNegated)
	{
		OptionsHolder<Quantifier> quantifier = new(Quantifier.Default);
		OptionsHolder<StringEqualityOptions> options = new(StringEqualityOptions.Default);
		return new StringOccurrenceCountResult<string, IThat<string>>(
			subject.Get().ExpectationBuilder.AddStringOccurrenceConstraint(expected, quantifier, options, isNegated),
			subject, quantifier, options);
	}
}
