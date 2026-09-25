using System;

namespace aweXpect.Core.Helpers;

/// <summary>
///     Carries the <see cref="Exception" /> that code of the caller threw, so that the evaluation can tell it apart from
///     an exception of aweXpect itself.
/// </summary>
internal sealed class UserCodeException(Exception exception)
	: Exception("The code of the caller threw an exception while the expectation was evaluated.", exception)
{
	/// <summary>
	///     The exception that the code of the caller threw.
	/// </summary>
	public Exception Exception { get; } = exception;
}
