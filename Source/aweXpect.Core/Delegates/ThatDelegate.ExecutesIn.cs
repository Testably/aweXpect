using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.Helpers;
using aweXpect.Core.Sources;
using aweXpect.Options;

namespace aweXpect.Delegates;

public abstract partial class ThatDelegate
{
	private sealed class ExecutesInConstraint(
		string it,
		ExpectationGrammars grammars,
		ExecutionTimeOptions options,
		Type? valueType)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue>
	{
		private DelegateValue? _actual;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause
			=> Outcome == Outcome.Failure &&
			   (_actual?.ExceededTimeout is not null || !options.AllowsException(_actual?.Exception))
				? _actual?.Exception
				: null;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(DelegateValue value)
		{
			if (value is { LateResult: { } lateResult, ExceededTimeout: { } timeout, } &&
			    options.JudgesLateResult(timeout, lateResult.Duration))
			{
				value = lateResult;
			}

			_actual = value;
			if (value.IsNull || value.ExceededTimeout is not null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			Outcome = options.AllowsException(value.Exception) && options.IsWithinLimit(value.Duration)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("executes ");
			options.AppendTo(stringBuilder, "in ");
			if (options.AreExceptionsAllowed)
			{
				stringBuilder.Append(" allowing exceptions");
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
			else if (_actual.Exception is { } exception && !options.AreExceptionsAllowed)
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(FormatForMessage(exception, indentation));
			}
			else
			{
				stringBuilder.Append(it).Append(" took ");
				options.AppendFailureResult(stringBuilder, _actual.Duration);
				if (_actual.Exception is { } allowedException)
				{
					stringBuilder.Append(" and did throw ");
					stringBuilder.Append(FormatForMessage(allowedException, indentation));
				}
			}
		}

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			=> TryGetDelegateValue(_actual, valueType, out value);

		public override ConstraintResult Negate()
			=> throw Tracing.WriteException(
				new NotSupportedException($"Negation of {nameof(WithoutValue.ExecutesIn)} is not supported."));
	}
}
