using System.Text;
using aweXpect.Core.Constraints;

namespace aweXpect.Core.Tests.TestHelpers;

internal class DummyConstraint(string expectationText, Func<ConstraintResult>? constraintResult = null)
	: IValueConstraint<int>
{
	public ConstraintResult IsMetBy(int actual)
		=> constraintResult == null
			? new DummyConstraintResult<int>(Outcome.Success, actual, expectationText)
			: constraintResult();

	public void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		=> stringBuilder.Append(expectationText);
}

internal class DummyConstraint<T>(
	Func<T, bool> predicate,
	string? expectationText = null)
	: ConstraintResult(ExpectationGrammars.None), IValueConstraint<T>
{
	private T? _actual;

	public ConstraintResult IsMetBy(T actual)
	{
		_actual = actual;
		Outcome = predicate(actual) ? Outcome.Success : Outcome.Failure;
		return this;
	}

	public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		if (expectationText != null)
		{
			stringBuilder.Append(expectationText);
		}
	}

	public override void AppendResult(StringBuilder stringBuilder, string? indentation = null) { }

	public override ConstraintResult Negate()
		=> this;

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
}
