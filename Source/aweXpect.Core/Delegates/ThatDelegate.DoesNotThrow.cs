using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	private sealed class DoesNotThrowConstraint(
		string it,
		ExpectationGrammars grammars,
		Type exceptionType,
		bool exactly,
		Type? valueType)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue>
	{
		private DelegateValue? _actual;
		private bool _isNegated;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause
			=> Outcome is Outcome.Failure or Outcome.FailureBothWays ? _actual?.Exception : null;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(DelegateValue value)
		{
			_actual = value;
			UpdateOutcome(value);
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (!exactly && exceptionType == typeof(Exception))
			{
				stringBuilder.Append(_isNegated ? "throws an exception" : "does not throw any exception");
			}
			else
			{
				stringBuilder.Append(_isNegated ? "throws " : "does not throw ")
					.Append(exactly ? "exactly " : "")
					.Append(Formatter.Format(exceptionType).PrependAOrAn());
			}
		}

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual?.IsNull != false)
			{
				AppendNullResult(stringBuilder, it, _actual);
			}
			else if (_actual.ExceededTimeout is { } exceededTimeout)
			{
				stringBuilder.ItDidNotFinishWithin(it, exceededTimeout);
			}
			else if (_actual.Exception is null)
			{
				stringBuilder.Append(it).Append(" did not throw any exception");
			}
			else
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(FormatForMessage(_actual.Exception, indentation));
			}
		}

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			=> TryGetDelegateValue(_actual, valueType, out value);

		/// <remarks>
		///     A negating expectation, such as <c>DoesNotComplyWith</c>, negates the result after the evaluation, so
		///     the outcome is updated as well.
		/// </remarks>
		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			if (_actual is not null)
			{
				UpdateOutcome(_actual);
			}

			return this;
		}

		private void UpdateOutcome(DelegateValue value)
		{
			if (value.IsNull || value.ExceededTimeout is not null)
			{
				Outcome = Outcome.FailureBothWays;
				return;
			}

			Outcome = _isNegated != IsExpectedException(value.Exception, exceptionType, exactly)
				? Outcome.Failure
				: Outcome.Success;
		}
	}
}
