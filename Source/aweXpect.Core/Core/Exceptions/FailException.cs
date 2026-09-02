using System;

// ReSharper disable once CheckNamespace
namespace aweXpect;

/// <summary>
///     Represents the default failure exception in case no test framework is configured.
/// </summary>
public class FailException : Exception
{
	/// <summary>
	///     Represents the default failure exception in case no test framework is configured.
	/// </summary>
	public FailException(string message) : base(message)
	{
	}

	/// <summary>
	///     Represents the default failure exception in case no test framework is configured, caused by the
	///     <paramref name="innerException" />.
	/// </summary>
	public FailException(string message, Exception? innerException) : base(message, innerException)
	{
	}
}
