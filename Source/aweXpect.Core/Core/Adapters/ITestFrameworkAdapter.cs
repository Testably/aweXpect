using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

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

	/// <summary>
	///     Returns the test that is running in the current execution context, or <see langword="false" /> when the test
	///     framework does not expose it or no test is running, e.g. in a setup of the whole assembly.
	/// </summary>
	/// <remarks>
	///     It returns <see langword="false" /> instead of throwing when the test framework fails.
	/// </remarks>
	bool TryGetCurrentTest([NotNullWhen(true)] out RunningTest? currentTest);

	/// <summary>
	///     Returns the token with which the test framework cancels the running test, e.g. when it times out, or
	///     <see langword="false" /> when the test framework does not expose it or no test is running.
	/// </summary>
	/// <remarks>
	///     It returns <see langword="false" /> instead of throwing when the test framework fails.
	/// </remarks>
	bool TryGetCancellationToken(out CancellationToken cancellationToken);

	/// <summary>
	///     Attaches the file at the <paramref name="filePath" /> to the result of the running test, or returns
	///     <see langword="false" /> when the test framework does not support attachments or no test is running.
	/// </summary>
	/// <remarks>
	///     Some test frameworks only read the file when the test ends, so it must still exist then. It returns
	///     <see langword="false" /> instead of throwing when the test framework fails, e.g. because the file does not
	///     exist.
	/// </remarks>
	bool TryAddAttachment(string filePath, string? description);
}
