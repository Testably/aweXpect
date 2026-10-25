using System;
using System.Collections.Generic;

namespace aweXpect.Core.Adapters;

/// <summary>
///     Identifies a running test by its class, its method and the arguments of the test case.
/// </summary>
public sealed class TestIdentity
{
	/// <summary>
	///     Identifies a running test.
	/// </summary>
	/// <param name="className">The full name of the test class, as the test framework reports it.</param>
	/// <param name="methodName">The name of the test method.</param>
	/// <param name="arguments">The arguments of the test case, which are empty for a test without parameters.</param>
	public TestIdentity(string className, string methodName, IReadOnlyList<object?> arguments)
		: this(className, methodName, arguments, null, null, 0)
	{
	}

	/// <summary>
	///     Identifies a running test, with what else its test framework reports about it.
	/// </summary>
	/// <param name="className">The full name of the test class, as the test framework reports it.</param>
	/// <param name="methodName">The name of the test method.</param>
	/// <param name="arguments">The arguments of the test case, which are empty for a test without parameters.</param>
	/// <param name="id">The id of the test case, or <see langword="null" /> when the test framework has none.</param>
	/// <param name="displayName">
	///     The name of the test case, as the test framework shows it, or <see langword="null" /> when it has none.
	/// </param>
	/// <param name="retryAttempt">How often the test was run before in this test run, e.g. by a retry.</param>
	public TestIdentity(string className, string methodName, IReadOnlyList<object?> arguments, string? id,
		string? displayName, int retryAttempt)
	{
		ClassName = className ?? throw Tracing.WriteException(new ArgumentNullException(nameof(className)));
		MethodName = methodName ?? throw Tracing.WriteException(new ArgumentNullException(nameof(methodName)));
		Arguments = arguments ?? throw Tracing.WriteException(new ArgumentNullException(nameof(arguments)));
		if (retryAttempt < 0)
		{
			throw Tracing.WriteException(new ArgumentOutOfRangeException(nameof(retryAttempt), retryAttempt,
				"The retry attempt must not be negative."));
		}

		Id = id;
		DisplayName = displayName;
		RetryAttempt = retryAttempt;
	}

	/// <summary>
	///     The full name of the test class, as the test framework reports it, e.g. <c>MyTests.AlbumTests</c>.
	/// </summary>
	public string ClassName { get; }

	/// <summary>
	///     The name of the test method.
	/// </summary>
	public string MethodName { get; }

	/// <summary>
	///     The arguments of the test case, e.g. of a parameterized test, in the order of the parameters of the test
	///     method; empty for a test without parameters.
	/// </summary>
	public IReadOnlyList<object?> Arguments { get; }

	/// <summary>
	///     The id with which the test framework tells the test cases apart, or <see langword="null" /> when it has none.
	/// </summary>
	/// <remarks>
	///     It is only unique within one test run, so do not persist it, e.g. in the name of a file.
	/// </remarks>
	public string? Id { get; }

	/// <summary>
	///     The name of the test case, as the test framework shows it, e.g. with its arguments, or
	///     <see langword="null" /> when it has none.
	/// </summary>
	public string? DisplayName { get; }

	/// <summary>
	///     How often the test was run before in this test run, e.g. by a retry or a repetition: <c>0</c> for the first run.
	/// </summary>
	/// <remarks>
	///     Each run is the same test, so it is not part of what identifies the test, e.g. in the id of a snapshot.
	/// </remarks>
	public int RetryAttempt { get; }
}
