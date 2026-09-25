using System;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Helpers;

namespace aweXpect.Core;

/// <summary>
///     Calls code of the caller while an expectation is evaluated, e.g. a predicate, a member selector, a comparer or an
///     <see cref="object.Equals(object)" /> override.
/// </summary>
/// <remarks>
///     An exception thrown by the code fails the expectation and its negation alike, with the exception as
///     <see cref="Constraints.ConstraintResult.FailureCause" />, because the code answered nothing. A cancellation of the
///     evaluation still aborts it.
/// </remarks>
public static class UserCode
{
	/// <summary>
	///     Calls the <paramref name="callback" /> of the caller.
	/// </summary>
	public static TResult Invoke<TResult>(Func<TResult> callback)
	{
		try
		{
			return callback();
		}
		catch (Exception exception) when (exception is not UserCodeException)
		{
			throw new UserCodeException(exception);
		}
	}

	/// <summary>
	///     Calls the <paramref name="callback" /> of the caller with the <paramref name="argument" />.
	/// </summary>
	public static TResult Invoke<TArgument, TResult>(Func<TArgument, TResult> callback, TArgument argument)
	{
		try
		{
			return callback(argument);
		}
		catch (Exception exception) when (exception is not UserCodeException)
		{
			throw new UserCodeException(exception);
		}
	}

	/// <summary>
	///     Calls the asynchronous <paramref name="callback" /> of the caller.
	/// </summary>
	/// <remarks>
	///     An <see cref="OperationCanceledException" /> while the <paramref name="cancellationToken" /> is cancelled is
	///     thrown as it is, so that the caller can react to the cancellation before it aborts the evaluation.
	/// </remarks>
	public static async ValueTask<TResult> InvokeAsync<TResult>(Func<ValueTask<TResult>> callback,
		CancellationToken cancellationToken = default)
	{
		try
		{
			return await callback();
		}
		catch (Exception exception) when (exception is not UserCodeException &&
		                                  !(exception is OperationCanceledException &&
		                                    cancellationToken.IsCancellationRequested))
		{
			throw new UserCodeException(exception);
		}
	}
}
