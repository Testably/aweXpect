using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Results;

namespace aweXpect.Tests;

public sealed class CustomResultTests
{
	[Fact]
	public async Task ConstraintResult_WithSubjectOfResult_ShouldMergeTheSubjectLikeTheBuiltInExpectations()
	{
		int subject = 5;

		async Task Act() => await That(subject).IsEvenNumber().And.IsNegativeNumber();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is even and is negative,
			             but it was odd and was positive
			             """);
	}

	[Fact]
	public async Task CustomResult_ShouldAccessTheExpectationBuilder()
	{
		int subject = 5;

		async Task Act() => await That(subject).IsEvenNumber().OrNegative();

		await That(Act).Throws<XunitException>()
			.WithMessage("""
			             Expected that subject
			             is even or is negative,
			             but it was odd and was positive
			             """);
	}
}

file static class CustomResultExtensions
{
	public static EitherResult IsEvenNumber(this IThat<int> subject)
		=> new(((IExpectThat<int>)subject).ExpectationBuilder.AddConstraint((it, grammars)
			=> new NumberConstraint(it, grammars, value => value % 2 == 0, "is even", "odd")), subject);

	public static AndOrResult<int, IThat<int>> IsNegativeNumber(this IThat<int> subject)
		=> new(((IExpectThat<int>)subject).ExpectationBuilder.AddConstraint((it, grammars)
			=> new NumberConstraint(it, grammars, value => value < 0, "is negative", "positive")), subject);

	public sealed class EitherResult(ExpectationBuilder expectationBuilder, IThat<int> subject)
		: AndOrResult<int, IThat<int>>(expectationBuilder, subject)
	{
		public AndOrResult<int, IThat<int>> OrNegative()
		{
			ExpectationBuilder.Or();
			return subject.IsNegativeNumber();
		}
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
