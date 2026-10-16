namespace aweXpect.Formatting;

/// <summary>
///     Formatting options used in the <see cref="ValueFormatter" />.
/// </summary>
public record FormattingOptions
{
	/// <summary>
	///     Format the objects completely on a single line.
	/// </summary>
	/// <remarks>
	///     Use <see langword="with" /> to combine it with other options, e.g.
	///     <c>FormattingOptions.Indented() with { IsComplete = true }</c>.
	/// </remarks>
	public static FormattingOptions Complete { get; } = new()
	{
		UseLineBreaks = false,
		IsComplete = true,
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
	///     Flag indicating if the objects should be formatted completely, without shortening them for a failure message.
	/// </summary>
	/// <remarks>
	///     Neither the <c>MaximumNumberOfCollectionItems</c> nor the <c>MaximumStringLength</c> of the formatting
	///     customization applies, nor the limits on how deeply and how many nested objects, collections and tuples are
	///     written. An object or collection that contains itself is still written as <c>*recursive*</c> where it repeats,
	///     and a value that is nested too deeply for the remaining stack throws an
	///     <see cref="System.InsufficientExecutionStackException" /> instead of being shortened. The items of a
	///     collection are written in the order it enumerates them, so an endless sequence never completes.
	/// </remarks>
	public bool IsComplete { get; init; }

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
