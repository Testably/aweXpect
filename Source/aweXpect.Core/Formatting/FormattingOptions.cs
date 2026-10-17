using System;

namespace aweXpect.Formatting;

/// <summary>
///     Formatting options used in the <see cref="ValueFormatter" />.
/// </summary>
public record FormattingOptions
{
	/// <summary>
	///     Format the objects for a snapshot on a single line.
	/// </summary>
	/// <remarks>
	///     Use <see langword="with" /> to combine it with other options, e.g.
	///     <c>FormattingOptions.Indented() with { IsSnapshot = true }</c>.
	/// </remarks>
	public static FormattingOptions Snapshot { get; } = new()
	{
		UseLineBreaks = false,
		IsSnapshot = true,
	};

	/// <summary>
	///     Format the objects on multiple lines.
	/// </summary>
	public static FormattingOptions MultipleLines { get; } = new()
	{
		UseLineBreaks = true,
	};

	/// <summary>
	///     Format the objects on a single line.
	/// </summary>
	public static FormattingOptions SingleLine { get; } = new()
	{
		UseLineBreaks = false,
	};

	/// <summary>
	///     Format the objects on a single line with prefixed type information.
	/// </summary>
	public static FormattingOptions WithType { get; } = new()
	{
		UseLineBreaks = false,
		IncludeType = true,
	};

	/// <summary>
	///     Flag indicating if line-breaks should be used during formatting.
	/// </summary>
	public bool UseLineBreaks { get; init; }

	/// <summary>
	///     Flag indicating if the type should be included during formatting.
	/// </summary>
	public bool IncludeType { get; init; }

	/// <summary>
	///     The indentation prefix for subsequent lines when <see cref="UseLineBreaks" /> is <see langword="true" />.
	/// </summary>
	public string Indentation { get; init; } = "";

	/// <summary>
	///     The total number of items in the formatted collection, when the caller knows it but the collection does not
	///     report it.
	/// </summary>
	/// <remarks>
	///     A truncated collection can only name the number of remaining items when the total is known; otherwise it says
	///     that more items may follow.
	/// </remarks>
	public int? TotalItemCount { get; init; }

	/// <summary>
	///     Flag indicating if the objects should be formatted for a snapshot: completely, without shortening them for a
	///     failure message, and the same in every run.
	/// </summary>
	/// <remarks>
	///     Neither the <c>MaximumNumberOfCollectionItems</c> nor the <c>MaximumStringLength</c> of the formatting
	///     customization applies, nor the limits on how deeply and how many nested objects, collections and tuples are
	///     written. An object or collection that contains itself is still written as <c>*recursive*</c> where it repeats,
	///     and a value that is nested too deeply for the remaining stack throws an
	///     <see cref="System.InsufficientExecutionStackException" /> instead of being shortened. A member, enumeration,
	///     <see cref="object.ToString()" /> or formatter that throws is still written as a placeholder that names the
	///     exception. Backslashes in unquoted text, like an exception message, are escaped as well, so that an escaped
	///     line break cannot be mistaken for a backslash followed by an <c>n</c>. The items of a collection are written
	///     in the order it enumerates them, so an endless sequence never completes. Only collections whose order depends on
	///     hash codes or threads, so that it can differ in every run, are written sorted instead: the
	///     items of a <see cref="System.Collections.Generic.HashSet{T}" />, an <c>ImmutableHashSet&lt;T&gt;</c>, a
	///     <c>FrozenSet&lt;T&gt;</c> and a <see cref="System.Collections.Concurrent.ConcurrentBag{T}" /> by their
	///     formatted text, and the entries of a
	///     <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}" />, an
	///     <c>ImmutableDictionary&lt;TKey, TValue&gt;</c>, a <c>FrozenDictionary&lt;TKey, TValue&gt;</c> and a
	///     <see cref="System.Collections.Hashtable" /> by their formatted key, each ordinal. A
	///     <see cref="System.Collections.Generic.Dictionary{TKey,TValue}" /> keeps the order in which its entries were
	///     added.
	/// </remarks>
	public bool IsSnapshot { get; init; }

	/// <summary>
	///     Decides for each nested value whether it is written, left out or replaced.
	/// </summary>
	/// <remarks>
	///     Every value nested in the formatted value is offered: the members of an object that is written member-wise,
	///     the items of a tuple and the items of a collection, with the entries of a dictionary as its items. The
	///     formatted value itself is not offered, nor the keys of a dictionary, nor the values nested in a value that a
	///     registered <see cref="IValueFormatter" /> or its own <see cref="object.ToString()" /> renders.
	///     <para />
	///     A left out item of a tuple changes the position of the following items. An exception that the hook throws is
	///     not written as a placeholder, but thrown out of the formatting, except when it is the exception of
	///     <see cref="FormattedValue.GetValue()" />.
	/// </remarks>
	public Func<FormattedValue, ValueFormatting>? FormatValue { get; init; }

	/// <summary>
	///     Format the objects on multiple lines with the given <paramref name="indentation" />.
	/// </summary>
	/// <remarks>
	///     Use an indentation of 2 blanks if the <paramref name="indentation" /> is <see langword="null" />.
	/// </remarks>
	public static FormattingOptions Indented(string? indentation = null, bool includeType = false)
		=> new()
		{
			UseLineBreaks = true,
			Indentation = indentation ?? "  ",
			IncludeType = includeType,
		};
}
