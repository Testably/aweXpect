using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="ExecutionTimeOptions" /> on the <see cref="OptionsHolder{TOptions}" /> of an
///     expectation.
/// </summary>
public static class ExecutionTimeOptionsHolderExtensions
{
	/// <inheritdoc cref="ExecutionTimeOptions.AtLeast(TimeSpan)" />
	public static OptionsHolder<ExecutionTimeOptions> AtLeast(this OptionsHolder<ExecutionTimeOptions> holder,
		TimeSpan minimum)
	{
		holder.Value = holder.Value.AtLeast(minimum);
		return holder;
	}

	/// <inheritdoc cref="ExecutionTimeOptions.AtMost(TimeSpan)" />
	public static OptionsHolder<ExecutionTimeOptions> AtMost(this OptionsHolder<ExecutionTimeOptions> holder,
		TimeSpan maximum)
	{
		holder.Value = holder.Value.AtMost(maximum);
		return holder;
	}

	/// <inheritdoc cref="ExecutionTimeOptions.Between(TimeSpan, TimeSpan)" />
	public static OptionsHolder<ExecutionTimeOptions> Between(this OptionsHolder<ExecutionTimeOptions> holder,
		TimeSpan minimum, TimeSpan maximum)
	{
		holder.Value = holder.Value.Between(minimum, maximum);
		return holder;
	}

	/// <inheritdoc cref="ExecutionTimeOptions.Within(TimeSpan)" />
	public static OptionsHolder<ExecutionTimeOptions> Within(this OptionsHolder<ExecutionTimeOptions> holder,
		TimeSpan duration)
	{
		holder.Value = holder.Value.Within(duration);
		return holder;
	}
}
