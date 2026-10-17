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
	///     Masks values in the subject that change from run to run, e.g. an id or a timestamp, before it is compared with
	///     the snapshot.
	/// </summary>
	/// <remarks>
	///     The maskings have the same names and semantics as the factories of <see cref="SnapshotMasking" /> for all
	///     snapshots, e.g. <c>.Masking.Text(album.Id.ToString(), "&lt;album-id&gt;")</c> or <c>.Masking.Guids()</c>. They are applied
	///     in the order in which they are added, before the maskings for all snapshots.
	/// </remarks>
	public SnapshotMaskings<TType, TThat> Masking => new(this, _options);

	/// <summary>
	///     Leaves values out of a snapshot of an object, e.g. a member that is irrelevant for the test.
	/// </summary>
	/// <remarks>
	///     The ignorings have the same names and semantics as the factories of <see cref="SnapshotIgnoring" /> for all
	///     snapshots, e.g. <c>.Ignoring.Member("ETag")</c> or <c>.Ignoring.Member&lt;DateTime&gt;()</c>, and win over every
	///     masking. On a <see cref="string" /> subject, which is compared as raw text, they throw.
	/// </remarks>
	public SnapshotIgnorings<TType, TThat> Ignoring => new(this, _options);

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
}
