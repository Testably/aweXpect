using System;
using System.Text.RegularExpressions;
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
	///     Replaces each occurrence of the <paramref name="value" /> in the subject with the
	///     <paramref name="replacement" /> before it is compared with the snapshot, e.g. an id that changes from run to
	///     run.
	/// </summary>
	/// <remarks>
	///     The <paramref name="value" /> is literal text; <c>.AsRegex()</c> interprets it as a regex pattern instead,
	///     whose matches are replaced with the substitutions of <see cref="Regex.Replace(string, string)" />, e.g.
	///     <c>$1</c>.<br />
	///     The maskings are applied in the order in which they are added, so a specific one goes before a general one.
	///     The snapshot and the failure contain the masked subject.
	/// </remarks>
	public SnapshotMaskingResult<TType, TThat> Masking(string value, string replacement)
	{
		_options.Maskings.Add(SnapshotMasking.Text(value, replacement));
		return new SnapshotMaskingResult<TType, TThat>(ExpectationBuilder, _returnValue, _options,
			_options.Maskings.Count - 1);
	}

	/// <summary>
	///     Replaces each match of the <paramref name="pattern" /> in the subject with the
	///     <paramref name="replacement" />, which can use the substitutions of
	///     <see cref="Regex.Replace(string, string)" />, e.g. <c>$1</c>, before it is compared with the snapshot.
	/// </summary>
	/// <remarks>
	///     The options and the timeout of the <paramref name="pattern" /> apply.<br />
	///     The maskings are applied in the order in which they are added, so a specific one goes before a general one.
	///     The snapshot and the failure contain the masked subject.
	/// </remarks>
	public SnapshotResult<TType, TThat> Masking(Regex pattern, string replacement)
	{
		_options.Maskings.Add(SnapshotMasking.Regex(pattern, replacement));
		return this;
	}

	/// <summary>
	///     Replaces each date and date with time in ISO 8601 in the subject with a numbered placeholder, <c>Date_1</c>,
	///     <c>Date_2</c> and so on, before it is compared with the snapshot, where the same text keeps the same number.
	/// </summary>
	/// <remarks>
	///     See <see cref="SnapshotMasking.Dates()" /> for the matched formats. The maskings are applied in the order in
	///     which they are added, and the numbers are counted after the maskings that come before.
	/// </remarks>
	public SnapshotResult<TType, TThat> MaskingDates()
	{
		_options.Maskings.Add(SnapshotMasking.Dates());
		return this;
	}

	/// <summary>
	///     Replaces each <see cref="Guid" /> in the subject with a numbered placeholder, <c>Guid_1</c>, <c>Guid_2</c> and
	///     so on, before it is compared with the snapshot, where the same <see cref="Guid" /> keeps the same number.
	/// </summary>
	/// <remarks>
	///     See <see cref="SnapshotMasking.Guids()" /> for the matched formats. The maskings are applied in the order in
	///     which they are added, and the numbers are counted after the maskings that come before.
	/// </remarks>
	public SnapshotResult<TType, TThat> MaskingGuids()
	{
		_options.Maskings.Add(SnapshotMasking.Guids());
		return this;
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
