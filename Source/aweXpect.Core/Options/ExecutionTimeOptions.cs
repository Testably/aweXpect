using System;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Options for the execution time of a delegate.
/// </summary>
public class ExecutionTimeOptions
{
	private Limit? _limit;
	private string? _limitOption;
	private Action<TimeSpan>? _onUpperBound;
	private TimeSpan? _upperBound;

	/// <summary>
	///     Flag indicating if a thrown exception leaves the outcome to the measured duration.
	/// </summary>
	internal bool AreExceptionsAllowed { get; private set; }

	/// <summary>
	///     Registers a <paramref name="callback" /> that receives the upper bound of the limit.
	/// </summary>
	/// <remarks>
	///     The limit is only known after the constraint was created, so an upper bound that should become the
	///     timeout of the expectation has to be reported back once it is set.
	/// </remarks>
	internal void OnUpperBound(Action<TimeSpan> callback) => _onUpperBound = callback;

	/// <summary>
	///     Allows the delegate to throw an exception without failing the expectation.
	/// </summary>
	internal void AllowExceptions() => AreExceptionsAllowed = true;

	/// <summary>
	///     Checks if the <paramref name="exception" /> leaves the outcome to the measured duration.
	/// </summary>
	internal bool AllowsException(Exception? exception)
		=> exception is null || AreExceptionsAllowed;

	/// <summary>
	///     Checks if the <paramref name="actual" /> value is within the required limit.
	/// </summary>
	/// <param name="actual">The measured execution time.</param>
	/// <returns>
	///     <see langword="true" /> if a limit was set and the <paramref name="actual" /> value is within it; otherwise
	///     <see langword="false" />.
	/// </returns>
	public bool IsWithinLimit(TimeSpan? actual)
	{
		if (actual is null || _limit is null)
		{
			return false;
		}

		return _limit.IsWithinLimit(actual.Value);
	}

	/// <summary>
	///     Checks if a synchronous delegate that returned only after the <paramref name="timeout" /> elapsed is judged by
	///     its measured <paramref name="duration" /> instead of failing with the timeout.
	/// </summary>
	/// <remarks>
	///     A timeout tighter than the upper bound of the limit wins, unless the <paramref name="duration" /> violates the
	///     limit on its own.
	/// </remarks>
	internal bool JudgesLateResult(TimeSpan timeout, TimeSpan duration)
		=> timeout >= _upperBound || !IsWithinLimit(duration);

	/// <summary>
	///     Appends the failure result text of the <paramref name="actual" /> value to the <paramref name="stringBuilder" />.
	/// </summary>
	public void AppendFailureResult(StringBuilder stringBuilder, TimeSpan actual)
		=> _limit?.AppendFailureResult(stringBuilder, actual);

	/// <summary>
	///     Appends the <paramref name="prefix" /> and the option description to the <paramref name="stringBuilder" />.
	/// </summary>
	public void AppendTo(StringBuilder stringBuilder, string prefix)
		=> _limit?.AppendTo(stringBuilder, prefix);

	/// <summary>
	///     Requires the value to be within the given <paramref name="duration" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit is already specified.</exception>
	public void Within(TimeSpan duration)
	{
		ThrowHelper.ThrowIfDurationIsNegative(duration);
		SetLimit(new MaximumLimit(duration, true), nameof(Within));
		SetUpperBound(duration);
	}

	/// <summary>
	///     Requires the value to be at most <paramref name="maximum" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit is already specified.</exception>
	public void AtMost(TimeSpan maximum)
	{
		ThrowHelper.ThrowIfDurationIsNegative(maximum);
		SetLimit(new MaximumLimit(maximum), nameof(AtMost));
		SetUpperBound(maximum);
	}

	/// <summary>
	///     Requires the value to be at least <paramref name="minimum" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit is already specified.</exception>
	public void AtLeast(TimeSpan minimum)
	{
		ThrowHelper.ThrowIfDurationIsNegative(minimum);
		SetLimit(new MinimumLimit(minimum), nameof(AtLeast));
	}

	/// <summary>
	///     Requires the value to be approximately <paramref name="expected" />,
	///     using the provided <paramref name="tolerance" />.
	/// </summary>
	/// <remarks>
	///     The limit is named after <c>Within</c>, which specifies the <paramref name="tolerance" />.
	/// </remarks>
	/// <exception cref="InvalidOperationException">A limit is already specified.</exception>
	internal void Approximately(TimeSpan expected, TimeSpan tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		SetLimit(new ApproximatelyLimit(expected, tolerance), nameof(Within));
		SetUpperBound(tolerance > TimeSpan.MaxValue - expected ? TimeSpan.MaxValue : expected + tolerance);
	}

	/// <summary>
	///     Requires the value to be between <paramref name="minimum" /> and <paramref name="maximum" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit is already specified.</exception>
	public void Between(TimeSpan minimum, TimeSpan maximum)
	{
		ThrowHelper.ThrowIfDurationIsNegative(minimum);
		ThrowHelper.ThrowIfDurationIsNegative(maximum);
		ThrowHelper.ThrowIfMaximumIsBelowMinimum<TimeSpan>(minimum, maximum);
		SetLimit(new BetweenLimit(minimum, maximum), nameof(Between));
		SetUpperBound(maximum);
	}

	/// <summary>
	///     Rejects a second limit, because it would silently replace the first one.
	/// </summary>
	private void SetLimit(Limit limit, string option)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_limitOption, option);
		_limitOption = option;
		_limit = limit;
	}

	private void SetUpperBound(TimeSpan upperBound)
	{
		_upperBound = upperBound;
		_onUpperBound?.Invoke(upperBound);
	}

	private abstract record Limit
	{
		public abstract bool IsWithinLimit(TimeSpan actual);

		public virtual void AppendFailureResult(StringBuilder stringBuilder, TimeSpan actual)
			=> Formatter.Format(stringBuilder, actual);

		public abstract void AppendTo(StringBuilder stringBuilder, string prefix);
	}

	private sealed record ApproximatelyLimit(TimeSpan Expected, TimeSpan Tolerance) : Limit
	{
		public override bool IsWithinLimit(TimeSpan actual)
			=> (actual >= Expected ? actual - Expected : Expected - actual) <= Tolerance;

		public override void AppendTo(StringBuilder stringBuilder, string prefix)
		{
			stringBuilder.Append(prefix);
			stringBuilder.Append("approximately ");
			Formatter.Format(stringBuilder, Expected);
			ToleranceHelpers.Append(stringBuilder, Tolerance);
		}

		public override void AppendFailureResult(StringBuilder stringBuilder, TimeSpan actual)
		{
			if (actual < Expected)
			{
				stringBuilder.Append("only ");
			}

			Formatter.Format(stringBuilder, actual);
		}
	}

	private sealed record BetweenLimit(TimeSpan Minimum, TimeSpan Maximum) : Limit
	{
		public override bool IsWithinLimit(TimeSpan actual)
			=> actual >= Minimum && actual <= Maximum;

		public override void AppendTo(StringBuilder stringBuilder, string prefix)
		{
			stringBuilder.Append(prefix);
			stringBuilder.Append("between ");
			Formatter.Format(stringBuilder, Minimum);
			stringBuilder.Append(" and ");
			Formatter.Format(stringBuilder, Maximum);
		}

		public override void AppendFailureResult(StringBuilder stringBuilder, TimeSpan actual)
		{
			if (actual < Minimum)
			{
				stringBuilder.Append("only ");
			}

			Formatter.Format(stringBuilder, actual);
		}
	}

	private sealed record MinimumLimit(TimeSpan Minimum) : Limit
	{
		public override bool IsWithinLimit(TimeSpan actual)
			=> actual >= Minimum;

		public override void AppendTo(StringBuilder stringBuilder, string prefix)
		{
			stringBuilder.Append(prefix);
			stringBuilder.Append("at least ");
			Formatter.Format(stringBuilder, Minimum);
		}

		public override void AppendFailureResult(StringBuilder stringBuilder, TimeSpan actual)
		{
			stringBuilder.Append("only ");
			Formatter.Format(stringBuilder, actual);
		}
	}

	private sealed record MaximumLimit(TimeSpan Maximum, bool IsWithin = false) : Limit
	{
		public override bool IsWithinLimit(TimeSpan actual)
			=> actual <= Maximum;

		public override void AppendTo(StringBuilder stringBuilder, string prefix)
		{
			if (IsWithin)
			{
				stringBuilder.Append("within ");
				Formatter.Format(stringBuilder, Maximum);
			}
			else
			{
				stringBuilder.Append(prefix);
				stringBuilder.Append("at most ");
				Formatter.Format(stringBuilder, Maximum);
			}
		}
	}
}
