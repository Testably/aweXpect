using System;
using aweXpect.Core;
using aweXpect.Snapshots;

namespace aweXpect.Results;

/// <summary>
///     The result of a snapshot expectation, which allows setting its options.
/// </summary>
public class SnapshotResult<TType, TThat> : AndOrResult<TType, TThat, SnapshotResult<TType, TThat>>
{
	private readonly SnapshotOptions _options;

	internal SnapshotResult(ExpectationBuilder expectationBuilder, TThat returnValue, SnapshotOptions options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
	}

	/// <summary>
	///     Names the snapshot, so that several snapshots of the same test method, e.g. of the cases of a parameterized
	///     test, are kept apart.
	/// </summary>
	/// <remarks>
	///     The name is appended to the snapshot, e.g. <c>&lt;Class&gt;.&lt;Method&gt;.&lt;Name&gt;.snapshot.txt</c>.
	/// </remarks>
	public SnapshotResult<TType, TThat> Named(string name)
	{
		if (name is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(name), "The 'name' cannot be null."));
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			throw Tracing.WriteException(new ArgumentException("The 'name' cannot be empty.", nameof(name)));
		}

		_options.Name = name;
		return this;
	}
}
