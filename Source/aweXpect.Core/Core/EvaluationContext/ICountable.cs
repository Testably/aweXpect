namespace aweXpect.Core.EvaluationContext;

/// <summary>
///     A sequence that knows its number of items once it was enumerated completely.
/// </summary>
/// <remarks>
///     Implemented by the materialized sequences of the <see cref="EvaluationContextExtensions" />, so that the number
///     of items can be reported without enumerating the source again.
/// </remarks>
public interface ICountable
{
	/// <summary>
	///     The number of items, or <see langword="null" /> as long as the sequence was not enumerated completely.
	/// </summary>
	int? Count { get; }
}
