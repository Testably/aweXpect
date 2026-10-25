using System;
using System.Collections.Generic;

namespace aweXpect.Core.Adapters;

/// <summary>
///     Identifies a test by its class, its method and the arguments of the test case, which are the same in every run.
/// </summary>
public sealed class TestIdentity
{
	/// <summary>
	///     Identifies a test.
	/// </summary>
	/// <param name="className">The full name of the test class, as the test framework reports it.</param>
	/// <param name="methodName">The name of the test method.</param>
	/// <param name="arguments">The arguments of the test case, which are empty for a test without parameters.</param>
	public TestIdentity(string className, string methodName, IReadOnlyList<object?> arguments)
	{
		ClassName = className ?? throw Tracing.WriteException(new ArgumentNullException(nameof(className)));
		MethodName = methodName ?? throw Tracing.WriteException(new ArgumentNullException(nameof(methodName)));
		Arguments = arguments ?? throw Tracing.WriteException(new ArgumentNullException(nameof(arguments)));
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
	/// <remarks>
	///     They are the instances that the test framework passes to the test, usually in its own array, so do not modify
	///     the list or its values.
	/// </remarks>
	public IReadOnlyList<object?> Arguments { get; }
}
