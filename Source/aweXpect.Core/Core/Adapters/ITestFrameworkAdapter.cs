using System;
using System.Diagnostics.CodeAnalysis;

namespace aweXpect.Core.Adapters;

/// <summary>
///     Represents an adapter to a particular test framework such as xUnit, nUnit, etc.
/// </summary>
public interface ITestFrameworkAdapter
{
	/// <summary>
	///     Gets a value indicating whether the corresponding test framework is currently available.
	/// </summary>
	bool IsAvailable { get; }

	/// <summary>
	///     Throws a framework-specific exception to indicate a skipped unit test.
	/// </summary>
	[DoesNotReturn]
	void Skip(string message);

	/// <summary>
	///     Throws a framework-specific exception to indicate a failing unit test.
	/// </summary>
	[DoesNotReturn]
	void Fail(string message);
	
	/// <summary>
	///     Throws a framework-specific exception to indicate a failing unit test, using the
	///     <paramref name="innerException" /> as cause of the failure.
	/// </summary>
	[DoesNotReturn]
	void Fail(string message, Exception innerException);

	/// <summary>
	///     Throws a framework-specific exception to indicate an inconclusive unit test.
	/// </summary>
	/// <remarks>
	///     It should be used in situations where another run with different data or a different environment might run to
	///     completion, with either a success or failure outcome.
	/// </remarks>
	[DoesNotReturn]
	void Inconclusive(string message);
}
