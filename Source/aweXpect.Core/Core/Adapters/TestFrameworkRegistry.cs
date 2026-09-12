using System;

namespace aweXpect.Core.Adapters;

/// <summary>
///     Registry for the <see cref="ITestFrameworkAdapter" /> that is used to report test results.
/// </summary>
/// <remarks>
///     Registering an adapter explicitly avoids scanning all loaded assemblies for one, which cannot work when the
///     application is published with trimming or Native AOT enabled, because the adapter is only referenced via
///     reflection and is therefore removed.
/// </remarks>
public static class TestFrameworkRegistry
{
	/// <remarks>
	///     An instance instead of static fields, so that it can be verified without affecting the adapter that
	///     reports the results of the running test.
	/// </remarks>
	internal static Registration Instance { get; } = new();

	/// <summary>
	///     Registers the <paramref name="testFrameworkAdapter" /> as the adapter that reports test results.
	/// </summary>
	/// <remarks>
	///     Adapters whose <see cref="ITestFrameworkAdapter.IsAvailable" /> is <see langword="false" /> are ignored.<br />
	///     When <paramref name="overwrite" /> is <see langword="true" />, a previously registered adapter is replaced;
	///     otherwise the adapter is only used when none was registered yet. The generated registration for a detected
	///     test framework passes <see langword="false" />, so that an explicit registration always wins, independent of
	///     the order in which both run.<br />
	///     The adapter has to be registered before the first expectation is evaluated, because it is resolved once and
	///     then cached.<br />
	///     When no adapter is registered, the loaded assemblies are scanned for one instead.
	/// </remarks>
	public static void Register(ITestFrameworkAdapter testFrameworkAdapter, bool overwrite = true)
		=> Instance.Add(testFrameworkAdapter, overwrite);

	internal sealed class Registration
	{
		private readonly object _lock = new();

		/// <remarks>
		///     A reference is read atomically, so the lock is only required for the read-modify-write in
		///     <see cref="Add" />.
		/// </remarks>
		private volatile ITestFrameworkAdapter? _testFrameworkAdapter;

		public ITestFrameworkAdapter? TestFrameworkAdapter => _testFrameworkAdapter;

		public void Add(ITestFrameworkAdapter testFrameworkAdapter, bool overwrite)
		{
			if (testFrameworkAdapter is null)
			{
				throw new ArgumentNullException(nameof(testFrameworkAdapter));
			}

			if (!testFrameworkAdapter.IsAvailable)
			{
				return;
			}

			lock (_lock)
			{
				if (overwrite || _testFrameworkAdapter is null)
				{
					_testFrameworkAdapter = testFrameworkAdapter;
				}
			}
		}
	}
}
