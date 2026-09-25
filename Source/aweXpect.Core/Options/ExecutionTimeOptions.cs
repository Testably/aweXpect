using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Options for the execution time of a delegate.
/// </summary>
public class ExecutionTimeOptions
{
	private Limit? _limit;
	private Action<TimeSpan>? _onUpperBound;

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
	public void Within(TimeSpan duration)
	{
		ThrowHelper.ThrowIfDurationIsNegative(duration);
		_limit = new MaximumLimit(duration, true);
		_onUpperBound?.Invoke(duration);
	}

	/// <summary>
	///     Requires the value to be at most <paramref name="maximum" />.
	/// </summary>
	public void AtMost(TimeSpan maximum)
	{
		ThrowHelper.ThrowIfDurationIsNegative(maximum);
		_limit = new MaximumLimit(maximum);
		_onUpperBound?.Invoke(maximum);
	}

	/// <summary>
	///     Requires the value to be at least <paramref name="minimum" />.
	/// </summary>
	public void AtLeast(TimeSpan minimum)
	{
		ThrowHelper.ThrowIfDurationIsNegative(minimum);
		_limit = new MinimumLimit(minimum);
	}

	/// <summary>
	///     Requires the value to be approximately <paramref name="expected" />,
	///     using the provided <paramref name="tolerance" />.
	/// </summary>
	internal void Approximately(TimeSpan expected, TimeSpan tolerance)
	{
		if (tolerance < TimeSpan.Zero)
		{
			throw Tracing.WriteException(
				new ArgumentOutOfRangeException(nameof(tolerance), tolerance, "The tolerance must not be negative."));
		}

		_limit = new ApproximatelyLimit(expected, tolerance);
		_onUpperBound?.Invoke(expected + tolerance);
	}

	/// <summary>
	///     Requires the value to be between <paramref name="minimum" /> and <paramref name="maximum" />.
	/// </summary>
	public void Between(TimeSpan minimum, TimeSpan maximum)
	{
		ThrowHelper.ThrowIfDurationIsNegative(minimum);
		ThrowHelper.ThrowIfDurationIsNegative(maximum);
		ThrowHelper.ThrowIfMaximumIsBelowMinimum<TimeSpan>(minimum, maximum);
		_limit = new BetweenLimit(minimum, maximum);
		_onUpperBound?.Invoke(maximum);
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
			=> actual >= Expected - Tolerance && actual <= Expected + Tolerance;

		public override void AppendTo(StringBuilder stringBuilder, string prefix)
		{
			stringBuilder.Append(prefix);
			stringBuilder.Append("approximately ");
			Formatter.Format(stringBuilder, Expected);
			stringBuilder.Append(" ± ");
			Formatter.Format(stringBuilder, Tolerance);
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
