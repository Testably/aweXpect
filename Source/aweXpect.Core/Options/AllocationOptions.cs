#if NET8_0_OR_GREATER
using System;
using System.Text;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Options for the allocations of a synchronous delegate.
/// </summary>
internal sealed class AllocationOptions
{
	private string? _limitOption;
	private long _maximum = long.MaxValue;
	private long _minimum;

	/// <summary>
	///     Flag indicating if a thrown exception leaves the outcome to the measured allocations.
	/// </summary>
	public bool AreExceptionsAllowed { get; private set; }

	/// <summary>
	///     Flag indicating if the delegate is invoked once, without measuring it, before the measured invocation.
	/// </summary>
	public bool IsWarmingUp { get; private set; }

	public void AllowingExceptions(bool allowExceptions)
		=> AreExceptionsAllowed = allowExceptions;

	public void AfterWarmUp()
		=> IsWarmingUp = true;

	/// <summary>
	///     Checks if the <paramref name="exception" /> leaves the outcome to the measured allocations.
	/// </summary>
	public bool AllowsException(Exception? exception)
		=> exception is null || AreExceptionsAllowed;

	public bool IsWithinLimit(long allocatedBytes)
		=> allocatedBytes >= _minimum && allocatedBytes <= _maximum;

	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public void Nothing()
		=> SetLimit(0, 0, nameof(Nothing));

	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="maximum" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public void AtMost(long maximum)
	{
		ThrowHelper.ThrowIfBytesAreNegative(maximum);
		SetLimit(0, maximum, nameof(AtMost));
	}

	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="minimum" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public void AtLeast(long minimum)
	{
		ThrowHelper.ThrowIfBytesAreNegative(minimum);
		SetLimit(minimum, long.MaxValue, nameof(AtLeast));
	}

	/// <exception cref="ArgumentOutOfRangeException">The range is negative or inverted.</exception>
	/// <exception cref="InvalidOperationException">A limit of another option is already specified.</exception>
	public void Between(long minimum, long maximum)
	{
		ThrowHelper.ThrowIfBytesAreNegative(minimum);
		ThrowHelper.ThrowIfBytesAreNegative(maximum);
		ThrowHelper.ThrowIfMaximumIsBelowMinimum<long>(minimum, maximum);
		SetLimit(minimum, maximum, nameof(Between));
	}

	public void AppendTo(StringBuilder stringBuilder)
	{
		switch (_limitOption)
		{
			case nameof(Nothing):
				stringBuilder.Append("nothing");
				break;
			case nameof(AtMost):
				AppendBytes(stringBuilder.Append("at most "), _maximum);
				break;
			case nameof(AtLeast):
				AppendBytes(stringBuilder.Append("at least "), _minimum);
				break;
			default:
				stringBuilder.Append("between ").Append(_minimum).Append(" and ");
				AppendBytes(stringBuilder, _maximum);
				break;
		}

		if (IsWarmingUp)
		{
			stringBuilder.Append(" after a warm-up");
		}

		if (AreExceptionsAllowed)
		{
			stringBuilder.Append(" allowing exceptions");
		}
	}

	public void AppendFailureResult(StringBuilder stringBuilder, long allocatedBytes)
	{
		if (allocatedBytes < _minimum)
		{
			stringBuilder.Append("only ");
		}

		AppendBytes(stringBuilder, allocatedBytes);
	}

	private static void AppendBytes(StringBuilder stringBuilder, long bytes)
		=> stringBuilder.Append(bytes).Append(bytes == 1 ? " byte" : " bytes");

	/// <summary>
	///     Replaces the limit of the same <paramref name="option" />, but rejects a limit of another option, because it
	///     would silently replace the first one.
	/// </summary>
	private void SetLimit(long minimum, long maximum, string option)
	{
		ThrowHelper.ThrowIfCompetingOption(_limitOption, option);
		_limitOption = option;
		_minimum = minimum;
		_maximum = maximum;
	}
}
#endif
