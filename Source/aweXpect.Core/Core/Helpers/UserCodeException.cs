using System;

namespace aweXpect.Core.Helpers;

/// <summary>
///     Carries the <see cref="Exception" /> that code of the caller threw, so that the evaluation can tell it apart from
///     an exception of aweXpect itself.
/// </summary>
#pragma warning disable S3871 // Only the evaluation catches it, and it unwraps the exception of the caller before anyone else sees it
internal sealed class UserCodeException(Exception exception, string? thrower = null)
	: Exception("The code of the caller threw an exception while the expectation was evaluated.", exception)
{
	/// <summary>
	///     The exception that the code of the caller threw.
	/// </summary>
	public Exception Exception { get; } = exception;

	/// <summary>
	///     Who threw the <see cref="Exception" /> in the failure message, or <see langword="null" /> for the subject.
	/// </summary>
	public string? Thrower { get; } = thrower;
}
#pragma warning restore S3871
