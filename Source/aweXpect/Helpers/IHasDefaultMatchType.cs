namespace aweXpect.Helpers;

/// <summary>
///     Options for the items of a collection that tell whether their match type is still the default one.
/// </summary>
internal interface IHasDefaultMatchType
{
	/// <summary>
	///     Whether the match type is still the default one.
	/// </summary>
	bool HasDefaultMatchType { get; }
}
