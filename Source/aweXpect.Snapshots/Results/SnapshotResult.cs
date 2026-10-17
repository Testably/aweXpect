using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
	///     Adds a rule for values that change from run to run or are irrelevant for the test, e.g.
	///     <c>.For.Members().WithName("Id").Mask()</c> or <c>.For.Guids().Mask()</c>.
	/// </summary>
	/// <remarks>
	///     A rule selects what it applies to, filters it, then ends with one action: <c>Ignore()</c>, <c>Mask()</c>,
	///     <c>Mask("…")</c>, <c>Convert(…)</c> or <c>Keep()</c>. It is written like a rule for all snapshots,
	///     <c>SnapshotRule.For.…</c>, and applied before those. A member or value rule throws on a <see cref="string" />
	///     subject, which is compared as raw text, and a line rule on any other subject.
	/// </remarks>
	public SnapshotFor<SnapshotResult<TType, TThat>> For => new(AddRule);

	/// <summary>
	///     Keeps a snapshot of its own for each environment, e.g. <c>.UniqueFor.TargetFramework()</c>, when the subject
	///     legitimately differs between them.
	/// </summary>
	/// <remarks>
	///     Several environments are combined by repeating <c>.UniqueFor</c>, e.g.
	///     <c>.UniqueFor.TargetFramework().UniqueFor.OperatingSystem()</c>, which is stored in
	///     <c>&lt;Class&gt;.&lt;Method&gt;@net8.0@windows.snapshot.txt</c>.
	/// </remarks>
	public SnapshotUniqueFor<TType, TThat> UniqueFor => new(this, _options);

	internal SnapshotOptions Options => _options;

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

	/// <summary>
	///     Names the snapshot after the arguments of a parameterized test, e.g. <c>.NamedAfter(title, year)</c> as
	///     <c>title=Abbey Road_year=1969</c>, so that its cases are kept apart.
	/// </summary>
	/// <remarks>
	///     Each argument is written as <c>&lt;name&gt;=&lt;value&gt;</c>, separated by <c>_</c>. The compiler passes the
	///     expression of each argument: a name or member access is written like <see langword="nameof" />, e.g.
	///     <c>Title</c> for <c>album.Title</c>, and any other expression as written on one line; without an expression,
	///     e.g. before C# 10, the position is used, e.g. <c>argument1</c>. A value is written in the invariant culture, and
	///     <see langword="null" /> as <c>null</c>.<br />
	///     The name replaces one set before, also by <c>.Named(…)</c>.
	/// </remarks>
	public SnapshotResult<TType, TThat> NamedAfter(object? argument1,
		EndOfArguments endOfArguments = default,
		[CallerArgumentExpression(nameof(argument1))] string doNotPopulateThisValue1 = "")
		=> SetNameAfter((argument1, doNotPopulateThisValue1));

	/// <inheritdoc cref="NamedAfter(object, EndOfArguments, string)" />
	public SnapshotResult<TType, TThat> NamedAfter(object? argument1, object? argument2,
		EndOfArguments endOfArguments = default,
		[CallerArgumentExpression(nameof(argument1))] string doNotPopulateThisValue1 = "",
		[CallerArgumentExpression(nameof(argument2))] string doNotPopulateThisValue2 = "")
		=> SetNameAfter((argument1, doNotPopulateThisValue1), (argument2, doNotPopulateThisValue2));

	/// <inheritdoc cref="NamedAfter(object, EndOfArguments, string)" />
	public SnapshotResult<TType, TThat> NamedAfter(object? argument1, object? argument2, object? argument3,
		EndOfArguments endOfArguments = default,
		[CallerArgumentExpression(nameof(argument1))] string doNotPopulateThisValue1 = "",
		[CallerArgumentExpression(nameof(argument2))] string doNotPopulateThisValue2 = "",
		[CallerArgumentExpression(nameof(argument3))] string doNotPopulateThisValue3 = "")
		=> SetNameAfter((argument1, doNotPopulateThisValue1), (argument2, doNotPopulateThisValue2),
			(argument3, doNotPopulateThisValue3));

	/// <inheritdoc cref="NamedAfter(object, EndOfArguments, string)" />
	[SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters",
		Justification = "Each argument needs its own caller argument expression parameter.")]
	public SnapshotResult<TType, TThat> NamedAfter(object? argument1, object? argument2, object? argument3,
		object? argument4,
		EndOfArguments endOfArguments = default,
		[CallerArgumentExpression(nameof(argument1))] string doNotPopulateThisValue1 = "",
		[CallerArgumentExpression(nameof(argument2))] string doNotPopulateThisValue2 = "",
		[CallerArgumentExpression(nameof(argument3))] string doNotPopulateThisValue3 = "",
		[CallerArgumentExpression(nameof(argument4))] string doNotPopulateThisValue4 = "")
		=> SetNameAfter((argument1, doNotPopulateThisValue1), (argument2, doNotPopulateThisValue2),
			(argument3, doNotPopulateThisValue3), (argument4, doNotPopulateThisValue4));

	private SnapshotResult<TType, TThat> SetNameAfter(params (object? Value, string Expression)[] arguments)
	{
		_options.Name = string.Join("_", arguments.Select((argument, index)
			=> $"{GetArgumentName(argument.Expression, index)}={FormatArgumentValue(argument.Value)}"));
		return this;
	}

	/// <remarks>
	///     Whitespace is collapsed, so that an expression over several lines has the same name with every line ending.
	/// </remarks>
	private static string GetArgumentName(string expression, int index)
	{
		string singleLine = string.Join(" ", expression.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
		if (singleLine.Length == 0)
		{
			return "argument" + (index + 1).ToString(CultureInfo.InvariantCulture);
		}

		string[] segments = singleLine.Split('.');
		return segments.All(IsIdentifier) ? segments[segments.Length - 1] : singleLine;
	}

	private static bool IsIdentifier(string segment)
		=> segment.Length > 0 && !char.IsDigit(segment[0]) &&
		   segment.All(character => character == '_' || char.IsLetterOrDigit(character));

	private static string FormatArgumentValue(object? value)
		=> (value is IFormattable formattable
			? formattable.ToString(null, CultureInfo.InvariantCulture)
			: value?.ToString()) ?? "null";

	private SnapshotResult<TType, TThat> AddRule(SnapshotRule rule)
	{
		StructuralRules.ThrowIfNotApplicable<TType>(rule);
		_options.Rules.Add(rule);
		return this;
	}
}
