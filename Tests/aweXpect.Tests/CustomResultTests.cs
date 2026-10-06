using System.Linq;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed class CustomResultTests
{
	[Test]
	public async Task ConstraintResult_WithSubjectOfResult_ShouldMergeTheSubjectLikeTheBuiltInExpectations()
	{
		int subject = 5;

		async Task Act() => await That(subject).IsEvenNumber().And.IsNegativeNumber();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is even and is negative,
			             but it was odd and was positive
			             """);
	}

	[Test]
	public async Task CustomResult_ShouldAccessTheExpectationBuilder()
	{
		int subject = 5;

		async Task Act() => await That(subject).IsEvenNumber().OrNegative();

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             is even or is negative,
			             but it was odd and was positive
			             """);
	}

	[Test]
	public async Task CustomResult_WithQuantifier_ShouldOfferTheCountOptions()
	{
		string subject = "1a2b3";

		async Task Act() => await That(subject).HasDigits().Because("of the count").Between(4).And(5);

		await That(Act).Throws<FailException>()
			.WithMessage("""
			             Expected that subject
			             has digits between 4 and 5 times, because of the count,
			             but it had 3 digits
			             """)
			.Because("a result that provides a quantifier gets the count options, also after Because");
	}
}

static file class CustomResultExtensions
{
	public static EitherResult IsEvenNumber(this IThat<int> subject)
		=> new(((IExpectThat<int>)subject).ExpectationBuilder.AddConstraint((it, grammars)
			=> new NumberConstraint(it, grammars, value => value % 2 == 0, "is even", "odd")), subject);

	public static DigitCountResult HasDigits(this IThat<string> subject)
	{
		Quantifier quantifier = new();
		return new DigitCountResult(((IExpectThat<string>)subject).ExpectationBuilder.AddConstraint((it, grammars)
			=> new DigitCountConstraint(it, grammars, quantifier)), subject, quantifier);
	}

	public static AndOrResult<int, IThat<int>> IsNegativeNumber(this IThat<int> subject)
		=> new(((IExpectThat<int>)subject).ExpectationBuilder.AddConstraint((it, grammars)
			=> new NumberConstraint(it, grammars, value => value < 0, "is negative", "positive")), subject);

	public sealed class DigitCountResult(
		ExpectationBuilder expectationBuilder,
		IThat<string> subject,
		Quantifier quantifier)
		: AndOrResult<string, IThat<string>, DigitCountResult>(expectationBuilder, subject),
			IOptionsProvider<Quantifier>
	{
		Quantifier IOptionsProvider<Quantifier>.Options => quantifier;
	}

	public sealed class EitherResult(ExpectationBuilder expectationBuilder, IThat<int> subject)
		: AndOrResult<int, IThat<int>>(expectationBuilder, subject)
	{
		public AndOrResult<int, IThat<int>> OrNegative()
			=> Or.IsNegativeNumber();
	}

	private sealed class DigitCountConstraint(string it, ExpectationGrammars grammars, Quantifier quantifier)
		: ConstraintResult.WithValue<string>(it, grammars),
			IValueConstraint<string>
	{
		private int _count;

		public ConstraintResult IsMetBy(string actual)
		{
			Actual = actual;
			_count = actual.Count(c => char.IsDigit(c));
			Outcome = quantifier.Check(_count, true) == true ? Outcome.Success : Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append("has digits ").Append(quantifier);

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(It).Append(" had ").Append(_count).Append(" digits");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> throw new NotSupportedException();

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> throw new NotSupportedException();
	}

	private sealed class NumberConstraint(
		string it,
		ExpectationGrammars grammars,
		Func<int, bool> predicate,
		string expectation,
		string otherwise)
		: ConstraintResult(grammars),
			IValueConstraint<int>
	{
		private int _actual;

		public override string? LeadingSubject => GetSubjectOfResult(it);

		public override string? TrailingSubject => GetSubjectOfResult(it);

		public ConstraintResult IsMetBy(int actual)
		{
			_actual = actual;
			Outcome = predicate(actual) ? Outcome.Success : Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectation);

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(it).Append(" was ").Append(otherwise);

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return false;
		}

		public override ConstraintResult Negate()
			=> throw new NotSupportedException();
	}
}
