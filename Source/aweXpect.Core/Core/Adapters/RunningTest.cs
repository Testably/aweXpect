using System;

namespace aweXpect.Core.Adapters;

/// <summary>
///     The test that is running in the current execution context: its <see cref="Identity" />, and what the test
///     framework reports about this run of it.
/// </summary>
public sealed class RunningTest
{
	/// <summary>
	///     The running test with the <paramref name="identity" />.
	/// </summary>
	/// <param name="identity">The class, the method and the arguments of the test case.</param>
	/// <param name="id">The id of the test case, or <see langword="null" /> when the test framework has none.</param>
	/// <param name="displayName">
	///     The name of the test case, as the test framework shows it, or <see langword="null" /> when it has none.
	/// </param>
	/// <param name="retryAttempt">How often the test was run before in this test run, e.g. by a retry.</param>
	public RunningTest(TestIdentity identity, string? id, string? displayName, int retryAttempt)
	{
		Identity = identity ?? throw Tracing.WriteException(new ArgumentNullException(nameof(identity)));
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
	///     The class, the method and the arguments of the test case, which are the same in every run.
	/// </summary>
	public TestIdentity Identity { get; }

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
	///     Each run is the same test, so it is not part of its <see cref="Identity" />.
	/// </remarks>
	public int RetryAttempt { get; }
}
