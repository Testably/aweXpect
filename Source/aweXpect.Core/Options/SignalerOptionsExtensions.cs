using System;
using System.Runtime.CompilerServices;
using aweXpect.Core;
using aweXpect.Core.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     Extension methods for results that wait for signals with <see cref="SignalerOptions" />.
/// </summary>
public static class SignalerOptionsExtensions
{
	/// <summary>
	///     Specifies a timeout for waiting on the callback.
	/// </summary>
	/// <remarks>
	///     <see cref="System.Threading.Timeout.InfiniteTimeSpan" /> waits without a limit.
	/// </remarks>
	/// <exception cref="ArgumentOutOfRangeException">The <paramref name="timeout" /> is negative.</exception>
	/// <exception cref="InvalidOperationException">A timeout is already set.</exception>
	public static TResult Within<TResult>(this TResult result, TimeSpan timeout)
		where TResult : IOptionsProvider<SignalerOptions>
	{
		ThrowHelper.ThrowIfTimeoutIsNegative(timeout);
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(result.Options.Timeout is not null, nameof(Within));
		result.Options.Timeout = timeout;
		return result;
	}

	/// <summary>
	///     Specifies a predicate to filter for signals with a matching parameter.
	/// </summary>
	/// <remarks>
	///     The predicates of several calls are combined, so that a signal has to match all of them.
	/// </remarks>
	public static TSelf With<TSelf, TParameter>(this ISignalerResult<TSelf, TParameter> result,
		Func<TParameter, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		where TSelf : ISignalerResult<TSelf, TParameter>
	{
		predicate.ThrowIfNull();
		result.Options.WithPredicate(predicate, doNotPopulateThisValue.TrimCommonWhiteSpace());
		return (TSelf)result;
	}
}
