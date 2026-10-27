using System;
using System.Text;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Results;

namespace aweXpect.Options;

/// <summary>
///     Options for the execution time of a delegate.
/// </summary>
/// <remarks>
///     An immutable value: the option methods return new options, and <see langword="default" /> has no limit, like
///     <see cref="Default" />.
/// </remarks>
[ImmutableOptions]
public readonly struct ExecutionTimeOptions
{
	/// <summary>
	///     The limit, which also names its option and knows its upper bound, so that the value needs no further state.
	/// </summary>
	private readonly Limit? _limit;

	private ExecutionTimeOptions(Limit? limit, bool areExceptionsAllowed)
	{
		_limit = limit;
		AreExceptionsAllowed = areExceptionsAllowed;
	}

	/// <summary>
	///     The options without a limit.
	/// </summary>
	public static ExecutionTimeOptions Default => default;

	/// <summary>
	///     Flag indicating if a thrown exception leaves the outcome to the measured duration.
	/// </summary>
	internal bool AreExceptionsAllowed { get; }

	/// <summary>
	///     The upper bound of the limit, which also limits the evaluation, or <see langword="null" /> if it has none.
	/// </summary>
	internal TimeSpan? UpperBound => _limit?.UpperBound;

	/// <summary>
	///     Allows the delegate to throw an exception without failing the expectation,
	///     according to the <paramref name="allowExceptions" /> parameter.
	/// </summary>
	internal ExecutionTimeOptions AllowingExceptions(bool allowExceptions)
		=> new(_limit, allowExceptions);

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
		=> timeout >= UpperBound || !IsWithinLimit(duration);

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
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public ExecutionTimeOptions Within(TimeSpan duration)
	{
		ThrowHelper.ThrowIfDurationIsNegative(duration);
		return WithLimit(new MaximumLimit(duration, true));
	}

	/// <summary>
	///     Requires the value to be at most <paramref name="maximum" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public ExecutionTimeOptions AtMost(TimeSpan maximum)
	{
		ThrowHelper.ThrowIfDurationIsNegative(maximum);
		return WithLimit(new MaximumLimit(maximum));
	}

	/// <summary>
	///     Requires the value to be at least <paramref name="minimum" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public ExecutionTimeOptions AtLeast(TimeSpan minimum)
	{
		ThrowHelper.ThrowIfDurationIsNegative(minimum);
		return WithLimit(new MinimumLimit(minimum));
	}

	/// <summary>
	///     Requires the value to be approximately <paramref name="expected" />,
	///     using the provided <paramref name="tolerance" />.
	/// </summary>
	/// <remarks>
	///     The limit is named after <see cref="ExecutesInToleranceResult{TResult}.Within(TimeSpan)" />, which specifies
	///     the <paramref name="tolerance" />.
	/// </remarks>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	internal ExecutionTimeOptions Approximately(TimeSpan expected, TimeSpan tolerance)
	{
		ToleranceHelpers.ThrowIfInvalid(tolerance);
		return WithLimit(new ApproximatelyLimit(expected, tolerance));
	}

	/// <summary>
	///     Requires the value to be between <paramref name="minimum" /> and <paramref name="maximum" />.
	/// </summary>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public ExecutionTimeOptions Between(TimeSpan minimum, TimeSpan maximum)
	{
		ThrowHelper.ThrowIfDurationIsNegative(minimum);
		ThrowHelper.ThrowIfDurationIsNegative(maximum);
		ThrowHelper.ThrowIfMaximumIsBelowMinimum<TimeSpan>(minimum, maximum);
		return WithLimit(new BetweenLimit(minimum, maximum));
	}

	/// <summary>
	///     Replaces the limit of the same option, but rejects a limit of another option, because it would silently
	///     replace the first one.
	/// </summary>
	private ExecutionTimeOptions WithLimit(Limit limit)
	{
		ThrowHelper.ThrowIfCompetingOption(_limit?.Option, limit.Option);
		return new ExecutionTimeOptions(limit, AreExceptionsAllowed);
	}

	private abstract record Limit
	{
		public abstract string Option { get; }

		public virtual TimeSpan? UpperBound => null;

		public abstract bool IsWithinLimit(TimeSpan actual);

		public virtual void AppendFailureResult(StringBuilder stringBuilder, TimeSpan actual)
			=> Formatter.Format(stringBuilder, actual);

		public abstract void AppendTo(StringBuilder stringBuilder, string prefix);
	}

	private sealed record ApproximatelyLimit(TimeSpan Expected, TimeSpan Tolerance) : Limit
	{
		public override string Option => nameof(ExecutesInToleranceResult<>.Within);

		public override TimeSpan? UpperBound
			=> Tolerance > TimeSpan.MaxValue - Expected ? TimeSpan.MaxValue : Expected + Tolerance;

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
		public override string Option => nameof(Between);

		public override TimeSpan? UpperBound => Maximum;

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
		public override string Option => nameof(AtLeast);

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
		public override string Option => IsWithin ? nameof(Within) : nameof(AtMost);

		public override TimeSpan? UpperBound => Maximum;

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
