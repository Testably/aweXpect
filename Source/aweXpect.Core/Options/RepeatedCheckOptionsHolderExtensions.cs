using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="RepeatedCheckOptions" /> on the <see cref="OptionsHolder{TOptions}" /> of an
///     expectation.
/// </summary>
public static class RepeatedCheckOptionsHolderExtensions
{
	/// <inheritdoc cref="RepeatedCheckOptions.CheckEvery(TimeSpan)" />
	public static OptionsHolder<RepeatedCheckOptions> CheckEvery(this OptionsHolder<RepeatedCheckOptions> holder,
		TimeSpan interval)
	{
		holder.Value = holder.Value.CheckEvery(interval);
		return holder;
	}

	/// <inheritdoc cref="RepeatedCheckOptions.Within(TimeSpan)" />
	public static OptionsHolder<RepeatedCheckOptions> Within(this OptionsHolder<RepeatedCheckOptions> holder,
		TimeSpan timeout)
	{
		holder.Value = holder.Value.Within(timeout);
		return holder;
	}
}
