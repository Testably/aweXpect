using System.Collections;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Core.EvaluationContext;

namespace aweXpect.Helpers;

/// <summary>
///     Casts the items of a sequence lazily like <see cref="Enumerable.Cast{TResult}" />, but keeps the count the
///     source knows, as it decides the layout in which the items are formatted.
/// </summary>
internal sealed class CastingEnumerable<TSource, TResult> : IEnumerable<TResult>, ICountable
{
	private readonly IEnumerable<TResult> _items;
	private readonly IEnumerable<TSource> _source;

	public CastingEnumerable(IEnumerable<TSource> source)
	{
		_items = source.Cast<TResult>();
		_source = source;
	}

	/// <remarks>
	///     The count is read from the source each time, as a source like a materializing sequence learns it only later.
	/// </remarks>
	public int? Count => _source switch
	{
		ICollection<TSource> collection => collection.Count,
		IReadOnlyCollection<TSource> collection => collection.Count,
		ICountable countable => countable.Count,
		_ => null,
	};

	#region IEnumerable<TResult> Members

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();

	/// <inheritdoc />
	public IEnumerator<TResult> GetEnumerator()
		=> _items.GetEnumerator();

	#endregion
}
