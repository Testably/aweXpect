using System;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Snapshots.Results;

/// <summary>
///     The result of a snapshot expectation of a <see cref="string" />, which allows setting its options.
/// </summary>
public class StringSnapshotResult<TType, TThat> : SnapshotResult<TType, TThat, StringSnapshotResult<TType, TThat>>
{
	internal StringSnapshotResult(ExpectationBuilder expectationBuilder, TThat returnValue, SnapshotOptions options)
		: base(expectationBuilder, returnValue, options)
	{
	}

	/// <summary>
	///     Adds a rule for values that change from run to run or are irrelevant for the test, e.g.
	///     <c>.For.Lines().Containing("elapsed").Ignore()</c> or <c>.For.Guids().Mask()</c>.
	/// </summary>
	/// <remarks>
	///     A rule selects what it applies to, filters it, then ends with one action: <c>Ignore()</c>, <c>Mask()</c>,
	///     <c>Mask("…")</c>, <c>Convert(…)</c> or <c>Keep()</c>. It is written like a rule for all snapshots,
	///     <c>SnapshotRule.For.…</c>, and applied before those. A <see cref="string" /> is compared as raw text, so it
	///     offers no member or value rules.
	/// </remarks>
	public StringSnapshotFor<StringSnapshotResult<TType, TThat>> For => new(AddRule);

	/// <summary>
	///     Stores the snapshot with the file <paramref name="extension" /> instead of <c>txt</c>, e.g. <c>json</c> in
	///     <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.json</c>, so that editors and diff tools highlight it.
	/// </summary>
	/// <param name="extension">The file extension without a leading dot, e.g. <c>json</c>, <c>html</c> or <c>xml</c>.</param>
	/// <remarks>
	///     The pending file keeps the extension as well, e.g.
	///     <c>&lt;Class&gt;.&lt;Method&gt;.awexpect-snapshot.pending.json</c>.
	///     The extension is kept as given, and a store receives it as <see cref="SnapshotId.Extension" />.<br />
	///     It cannot be empty, contain a <c>.</c>, whitespace, control characters or any of
	///     <c>&lt; &gt; : " / \ | ? * % @</c>, or be <c>pending</c>.<br />
	///     A snapshot that was already used with another extension in the same test run fails.
	/// </remarks>
	public StringSnapshotResult<TType, TThat> WithExtension(string extension)
	{
		SnapshotExtension.ThrowIfInvalid(extension);
		Options.Extension = extension;
		return this;
	}

	private StringSnapshotResult<TType, TThat> AddRule(SnapshotRule rule)
	{
		Options.Rules.Add(rule);
		return this;
	}
}

/// <summary>
///     The result of a snapshot expectation of an object, which allows setting its options.
/// </summary>
public class ObjectSnapshotResult<TType, TThat> : SnapshotResult<TType, TThat, ObjectSnapshotResult<TType, TThat>>
{
	internal ObjectSnapshotResult(ExpectationBuilder expectationBuilder, TThat returnValue, SnapshotOptions options)
		: base(expectationBuilder, returnValue, options)
	{
	}

	/// <summary>
	///     Adds a rule for values that change from run to run or are irrelevant for the test, e.g.
	///     <c>.For.Members().WithName("Id").Mask()</c> or <c>.For.Guids().Mask()</c>.
	/// </summary>
	/// <remarks>
	///     A rule selects what it applies to, filters it, then ends with one action: <c>Ignore()</c>, <c>Mask()</c>,
	///     <c>Mask("…")</c>, <c>Convert(…)</c> or <c>Keep()</c>. It is written like a rule for all snapshots,
	///     <c>SnapshotRule.For.…</c>, and applied before those. An object is written by the formatter, so it offers no
	///     line rules.
	/// </remarks>
	public ObjectSnapshotFor<ObjectSnapshotResult<TType, TThat>> For => new(AddRule);

	private ObjectSnapshotResult<TType, TThat> AddRule(SnapshotRule rule)
	{
		StructuralRules.ThrowIfNotApplicable<TType>(rule);
		Options.Rules.Add(rule);
		return this;
	}
}

/// <summary>
///     The result of a snapshot expectation of binary data, which allows setting its options.
/// </summary>
/// <remarks>
///     Binary data is compared byte by byte, so it offers no rules, and the rules for all snapshots are ignored.
/// </remarks>
public class BinarySnapshotResult<TType, TThat> : SnapshotResult<TType, TThat, BinarySnapshotResult<TType, TThat>>
{
	internal BinarySnapshotResult(ExpectationBuilder expectationBuilder, TThat returnValue, SnapshotOptions options)
		: base(expectationBuilder, returnValue, options)
	{
	}
}

/// <summary>
///     The options that every snapshot expectation offers.
/// </summary>
public class SnapshotResult<TType, TThat, TSelf> : AndOrResult<TType, TThat, TSelf>
	where TSelf : SnapshotResult<TType, TThat, TSelf>
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
	///     Accepts a missing or different snapshot, with <see cref="SnapshotAcceptance.New" /> only a missing one and with
	///     <see cref="SnapshotAcceptance.None" /> none: the actual value replaces the snapshot, and the expectation still
	///     fails, saying that the snapshot was accepted, so that the next run compares with it.
	/// </summary>
	/// <remarks>
	///     It overrides <c>Customize.aweXpect.Snapshots().Acceptance</c> and the environment variable
	///     <c>AWEXPECT_ACCEPT_SNAPSHOTS</c>, also on a build server.<br />
	///     It is the last option of the expectation, so that it is easy to find and remove again.
	/// </remarks>
	public AndOrResult<TType, TThat> Accepting(SnapshotAcceptance mode)
	{
		_options.Acceptance = mode;
		return new AndOrResult<TType, TThat>(ExpectationBuilder, _returnValue);
	}

	/// <summary>
	///     Compares the subject with the snapshot by the <paramref name="comparer" />, e.g. an image comparer that
	///     tolerates small differences of single pixels.
	/// </summary>
	/// <remarks>
	///     It goes before the comparers of <c>Customize.aweXpect.Snapshots().Comparers</c>. The comparer is only called
	///     when the snapshot exists, and gets the subject after the rules of the expectation were applied. When it
	///     considers the subject equal, the snapshot is kept unchanged.
	/// </remarks>
	public TSelf Using(SnapshotComparer comparer)
	{
		if (comparer is null)
		{
			throw Tracing.WriteException(
				new ArgumentNullException(nameof(comparer), "The 'comparer' cannot be null."));
		}

		_options.Comparer = comparer;
		return (TSelf)this;
	}

	/// <summary>
	///     Keeps a snapshot of its own for each environment, e.g. <c>.UniqueFor.TargetFramework()</c>, when the subject
	///     legitimately differs between them.
	/// </summary>
	/// <remarks>
	///     Several environments are combined by repeating <c>.UniqueFor</c>, e.g.
	///     <c>.UniqueFor.TargetFramework().UniqueFor.OperatingSystem()</c>, which is stored in
	///     <c>&lt;Class&gt;.&lt;Method&gt;@net8.0@windows.awexpect-snapshot.txt</c>.
	/// </remarks>
	public SnapshotUniqueFor<TSelf> UniqueFor => new((TSelf)this, _options);

	internal SnapshotOptions Options => _options;

	/// <summary>
	///     Names the snapshot, so that several snapshots of the same test method, e.g. of the cases of a parameterized
	///     test, are kept apart.
	/// </summary>
	/// <remarks>
	///     The name is appended to the snapshot, e.g.
	///     <c>&lt;Class&gt;.&lt;Method&gt;.&lt;Name&gt;.awexpect-snapshot.txt</c>. When the snapshot is named after the
	///     arguments of the running test, see <c>Customize.aweXpect.Snapshots().NamedAfterTestArguments</c>, the name is
	///     appended to them, e.g. <c>Abbey Road_1969_cover</c>, so that each case of a parameterized test can hold several
	///     snapshots.<br />
	///     The name replaces one set before, also by <c>.NamedAfter(…)</c>. A name longer than 64 characters in the file
	///     name is shortened and ends with <c>~</c> and a hash of the whole name.
	/// </remarks>
	public TSelf Named(string name)
	{
		if (name is null)
		{
			throw Tracing.WriteException(new ArgumentNullException(nameof(name), "The 'name' cannot be null."));
		}

		if (string.IsNullOrWhiteSpace(name))
		{
			throw Tracing.WriteException(new ArgumentException("The 'name' cannot be empty.", nameof(name)));
		}

		_options.NameWithinTestCase(name);
		return (TSelf)this;
	}

	/// <summary>
	///     Names the snapshot after the arguments of a parameterized test, e.g. <c>.NamedAfter(title, year)</c> as
	///     <c>Abbey Road_1969</c>, so that its cases are kept apart.
	/// </summary>
	/// <remarks>
	///     The values are written like the arguments of the running test with
	///     <c>Customize.aweXpect.Snapshots().NamedAfterTestArguments</c>: in the invariant culture, separated by
	///     <c>_</c>, without the names of the parameters, so that renaming a parameter keeps the snapshot.<br />
	///     The name replaces one set before, also by <c>.Named(…)</c>, and the name of the arguments of the running test.
	/// </remarks>
	public TSelf NamedAfter(object? argument1)
		=> SetNameAfter(argument1);

	/// <inheritdoc cref="NamedAfter(object)" />
	public TSelf NamedAfter(object? argument1, object? argument2)
		=> SetNameAfter(argument1, argument2);

	/// <inheritdoc cref="NamedAfter(object)" />
	public TSelf NamedAfter(object? argument1, object? argument2, object? argument3)
		=> SetNameAfter(argument1, argument2, argument3);

	/// <inheritdoc cref="NamedAfter(object)" />
	public TSelf NamedAfter(object? argument1, object? argument2, object? argument3, object? argument4)
		=> SetNameAfter(argument1, argument2, argument3, argument4);

	/// <inheritdoc cref="NamedAfter(object)" />
	/// <remarks>
	///     The values are written like those of <see cref="NamedAfter(object)" />. The further arguments start after the
	///     fourth one, so that a single array is passed to <see cref="NamedAfter(object)" /> and written as <c>[…]</c>,
	///     like an array argument of the running test. Only an array of a reference type as the fifth and last argument is
	///     passed as the further arguments themselves, like to any <see langword="params" /> parameter, so its items are
	///     written one by one. A single <see langword="null" /> as the fifth argument is written as <c>null</c>.
	/// </remarks>
	public TSelf NamedAfter(object? argument1, object? argument2, object? argument3, object? argument4,
		params object?[]? moreArguments)
		=> SetNameAfter([argument1, argument2, argument3, argument4, .. moreArguments ?? [null,],]);

	private TSelf SetNameAfter(params object?[] arguments)
	{
		_options.Name = SnapshotTestArguments.Format(arguments);
		return (TSelf)this;
	}
}
