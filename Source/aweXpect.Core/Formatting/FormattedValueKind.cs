namespace aweXpect.Formatting;

/// <summary>
///     The kind of a <see cref="FormattedValue" />.
/// </summary>
public enum FormattedValueKind
{
	/// <summary>
	///     A field or property of an object.
	/// </summary>
	Member,

	/// <summary>
	///     An item of a tuple.
	/// </summary>
	TupleItem,

	/// <summary>
	///     An item of a collection, or the value of a dictionary entry.
	/// </summary>
	CollectionItem,
}
