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
	private readonly TThat _returnValue;

	internal SnapshotResult(ExpectationBuilder expectationBuilder, TThat returnValue, SnapshotOptions options)
		: base(expectationBuilder, returnValue)
	{
		_options = options;
		_returnValue = returnValue;
	}

	/// <summary>
	///     Accepts a missing or different snapshot when <paramref name="accept" /> is <see langword="true" />: the
	///     actual value replaces the snapshot, and the expectation still fails, saying that the snapshot was accepted, so
	///     that the next run compares with it.
	/// </summary>
	/// <remarks>
	///     It overrides <c>Customize.aweXpect.Snapshots().AcceptChanges</c> and the environment variable
	///     <c>AWEXPECT_ACCEPT_SNAPSHOTS</c>, also on a build server.<br />
	///     It is the last option of the expectation, so that it is easy to find and remove again.
	/// </remarks>
	public AndOrResult<TType, TThat> AcceptingChanges(bool accept = true)
	{
		_options.AcceptChanges = accept;
		return new AndOrResult<TType, TThat>(ExpectationBuilder, _returnValue);
	}

	/// <summary>
	///     Masks values in the subject that change from run to run, e.g. an id or a timestamp, before it is compared with
	///     the snapshot.
	/// </summary>
	/// <remarks>
	///     The maskings have the same names and semantics as the factories of <see cref="SnapshotMasking" /> for all
	///     snapshots, e.g. <c>.Masking.Text(order.Id.ToString(), "&lt;order-id&gt;")</c> or <c>.Masking.Guids()</c>. They are applied
	///     in the order in which they are added, before the maskings for all snapshots.
	/// </remarks>
	public SnapshotMaskings<TType, TThat> Masking => new(this, _options);

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
