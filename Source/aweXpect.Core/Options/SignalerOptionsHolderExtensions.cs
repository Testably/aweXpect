using System;
using aweXpect.Core;

namespace aweXpect.Options;

/// <summary>
///     The option methods of <see cref="SignalerOptions" /> on the <see cref="OptionsHolder{TOptions}" /> of an
///     expectation.
/// </summary>
public static class SignalerOptionsHolderExtensions
{
	/// <inheritdoc cref="SignalerOptions.Within(TimeSpan)" />
	public static OptionsHolder<SignalerOptions> Within(this OptionsHolder<SignalerOptions> holder, TimeSpan timeout)
	{
		holder.Value = holder.Value.Within(timeout);
		return holder;
	}
}
