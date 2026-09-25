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
	/// <param name="callback">The code of the caller.</param>
	/// <param name="thrower">
	///     Who threw in the failure message (e.g. <c>the predicate</c>), or <see langword="null" /> for the subject.
	/// </param>
	public static TResult Invoke<TResult>(Func<TResult> callback, string? thrower = null)
	{
		try
		{
			return callback();
		}
		catch (Exception exception) when (exception is not UserCodeException)
		{
			throw new UserCodeException(exception, thrower);
		}
	}

	/// <summary>
	///     Calls the <paramref name="callback" /> of the caller.
	/// </summary>
	/// <remarks>
	///     The <paramref name="thrower" /> is only created when the <paramref name="callback" /> throws, so that a name
	///     that has to be formatted costs nothing while the code of the caller succeeds.
	/// </remarks>
	internal static TResult Invoke<TResult>(Func<TResult> callback, Func<string> thrower)
	{
		try
		{
			return callback();
		}
		catch (Exception exception) when (exception is not UserCodeException)
		{
			throw new UserCodeException(exception, thrower());
		}
	}

	/// <summary>
	///     Returns the name of the <see cref="object.Equals(object)" /> method of the <paramref name="value" /> for the
	///     failure message.
	/// </summary>
	internal static string EqualsOf(object value)
		=> $"Equals of {Formatter.Format(value.GetType())}";

	/// <summary>
	///     Calls the <paramref name="callback" /> of the caller with the <paramref name="argument" />.
	/// </summary>
	/// <param name="callback">The code of the caller.</param>
	/// <param name="argument">The argument of the <paramref name="callback" />.</param>
	/// <param name="thrower">
	///     Who threw in the failure message (e.g. <c>the predicate</c>), or <see langword="null" /> for the subject.
	/// </param>
	public static TResult Invoke<TArgument, TResult>(Func<TArgument, TResult> callback, TArgument argument,
		string? thrower = null)
	{
		try
		{
			return callback(argument);
		}
		catch (Exception exception) when (exception is not UserCodeException)
		{
			throw new UserCodeException(exception, thrower);
		}
	}

	/// <summary>
	///     Calls the asynchronous <paramref name="callback" /> of the caller.
	/// </summary>
	/// <param name="callback">The code of the caller.</param>
	/// <param name="cancellationToken">The cancellation of the evaluation.</param>
	/// <param name="thrower">
	///     Who threw in the failure message (e.g. <c>the predicate</c>), or <see langword="null" /> for the subject.
	/// </param>
	/// <remarks>
	///     An <see cref="OperationCanceledException" /> while the <paramref name="cancellationToken" /> is canceled is
	///     thrown as it is, so that the caller can react to the cancellation before it aborts the evaluation.
	/// </remarks>
	public static async ValueTask<TResult> InvokeAsync<TResult>(Func<ValueTask<TResult>> callback,
		CancellationToken cancellationToken = default, string? thrower = null)
	{
		try
		{
			return await callback();
		}
		catch (Exception exception) when (exception is not UserCodeException &&
		                                  !(exception is OperationCanceledException &&
		                                    cancellationToken.IsCancellationRequested))
		{
			throw new UserCodeException(exception, thrower);
		}
	}
}
