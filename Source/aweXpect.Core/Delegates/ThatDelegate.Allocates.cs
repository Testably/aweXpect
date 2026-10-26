#if NET8_0_OR_GREATER
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
	/// <summary>
	///     Lets the subject measure its allocations, which only a synchronous delegate can.
	/// </summary>
	/// <exception cref="NotSupportedException">The subject is an asynchronous delegate or a task.</exception>
	private AllocationOptions MeasureAllocations()
	{
		AllocationOptions options = new();
		if (!_expectationBuilder.TryMeasureAllocations(options))
		{
			throw Tracing.WriteException(new NotSupportedException(
				"Allocates() is only supported for synchronous delegates, because an asynchronous delegate can continue on another thread, whose allocations are not measured."));
		}

		return options;
	}

	private sealed class AllocatesConstraint(
		string it,
		ExpectationGrammars grammars,
		AllocationOptions options,
		Type? valueType)
		: ConstraintResult(grammars),
			IValueConstraint<DelegateValue>
	{
		private DelegateValue? _actual;

		/// <inheritdoc cref="ConstraintResult.FailureCause" />
		public override Exception? FailureCause
			=> Outcome == Outcome.Failure && IsFailedByException(_actual)
				? _actual?.Exception
				: null;

		/// <inheritdoc />
		public ConstraintResult IsMetBy(DelegateValue value)
		{
			_actual = value;
			Outcome = !value.IsNull && !IsFailedByException(value) && options.IsWithinLimit(value.AllocatedBytes)
				? Outcome.Success
				: Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append("allocates ");
			options.AppendTo(stringBuilder);
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
			else if (_actual.Exception is { } exception && (_actual.IsWarmUpException || !options.AreExceptionsAllowed))
			{
				stringBuilder.Append(it).Append(" did throw ");
				stringBuilder.Append(FormatForMessage(exception, indentation,
					suffix: _actual.IsWarmUpException ? " in the warm-up" : ""));
			}
			else
			{
				stringBuilder.Append(it).Append(" allocated ");
				options.AppendFailureResult(stringBuilder, _actual.AllocatedBytes);
				if (_actual.Exception is { } allowedException)
				{
					stringBuilder.Append(" and did throw ");
					stringBuilder.Append(FormatForMessage(allowedException, indentation));
				}
			}
		}

		public override bool TryGetStoredValue<TValue>(out TValue? value) where TValue : default
			=> TryGetDelegateValue(_actual, valueType, out value);

		protected internal override void NegateInPlace()
			=> throw Tracing.WriteException(
				new NotSupportedException($"Negation of {nameof(WithoutValue.Allocates)} is not supported."));

		private bool IsFailedByException(DelegateValue? value)
			=> value?.ExceededTimeout is not null || value?.IsWarmUpException == true ||
			   !options.AllowsException(value?.Exception);
	}
}
#endif
