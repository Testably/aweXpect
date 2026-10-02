using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Core.Constraints;

internal sealed class IsOfTypeConstraint<TActual, TType>(
	ExpectationBuilder expectationBuilder,
	string it,
	ExpectationGrammars grammars)
	: ConstraintResult.WithNotNullValue<TActual>(it, grammars),
		IValueConstraint<TActual>
{
	private bool _addsContextWhenNegated;

	public ConstraintResult IsMetBy(TActual actual)
	{
		Actual = actual;
		Outcome = actual is TType ? Outcome.Success : Outcome.Failure;
		_addsContextWhenNegated = Outcome == Outcome.Success;
		if (Outcome == Outcome.Failure)
		{
			AddActualContext(false);
		}

		return this;
	}

	public override ConstraintResult Negate()
	{
		base.Negate();
		// A negation after the evaluation (e.g. by `DoesNotComplyWith`) turns the success into a failure.
		// Only the first one adds the context, as the next one reverts it before a repeated evaluation.
		if (_addsContextWhenNegated)
		{
			_addsContextWhenNegated = false;
			AddActualContext(true);
		}

		return this;
	}

	/// <remarks>
	///     A negation after the evaluation (e.g. by <c>DoesNotComplyWith</c>) adds the context of a success, which a
	///     further negation can turn back into a success, so it is then only shown when the outcome is a failure.
	/// </remarks>
	private void AddActualContext(bool onlyOnFailure)
	{
		if (Actual is not null)
		{
			string actual = Formatter.Format(Actual, FormattingOptions.MultipleLines);
			expectationBuilder.AddContext(new ResultContext.SyncCallback("Actual",
				() => onlyOnFailure && Outcome != Outcome.Failure ? null : actual));
		}
	}

	protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("is of type ", "are of type "));
		Formatter.Format(stringBuilder, typeof(TType));
	}

	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(It).Append(Grammars.SubjectVerb(It, " was ", " were "));
		Formatter.Format(stringBuilder, Actual!.GetType());
	}

	protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
	{
		stringBuilder.Append(Grammars.Verb("is not of type ", "are not of type "));
		Formatter.Format(stringBuilder, typeof(TType));
	}

	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalResult(stringBuilder, indentation);
}
