using System.Threading;
#if NET8_0_OR_GREATER
using System.Threading.Tasks;
#else
using System.Collections;
using aweXpect.Core.Extending;
#endif
using System.Collections.Generic;
using System.Linq;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     Container for filtered collections.
/// </summary>
public static partial class Filtered;

/// <summary>
///     Base class for filtered collections of <typeparamref name="T" />.
/// </summary>
/// <remarks>
///     Filtered collections are immutable value objects: the underlying <c>source</c> and the <see cref="Filters" />
///     captured at construction are never mutated afterwards. Every filtering operation (<see cref="Which" /> and the
///     <c>WhichAre*</c> / <c>With*</c> / <c>Without*</c> extensions that funnel through it) returns a NEW instance via
///     <c>CloneWith</c>, so deriving multiple filtered views from the same base collection cannot corrupt each other.
///     The options on a filtered result (e.g. <c>OrWith</c>) also return a NEW instance
///     and leave the one they are called on unchanged. So do the options of aweXpect that they use, e.g.
///     <c>IgnoringCase</c>, <c>AtIndex</c> or <c>Once</c>, as these results implement
///     <see cref="aweXpect.Core.ICopyOnOption{TOptions, TSelf}" />.
/// </remarks>
#if NET8_0_OR_GREATER
public abstract class Filtered<T, TFiltered>
	: IAsyncEnumerable<T>
#else
public abstract class Filtered<T, TFiltered>
	: IEnumerable<T>
#endif
	where TFiltered : Filtered<T, TFiltered>
{
#if NET8_0_OR_GREATER
	private readonly IAsyncEnumerable<T> _source;
#else
	private readonly IEnumerable<T> _source;
#endif

	/// <summary>
	///     Base class for filtered collections of <typeparamref name="T" />.
	/// </summary>
#if NET8_0_OR_GREATER
	private protected Filtered(IAsyncEnumerable<T> source, IReadOnlyList<IFilter<T>>? filters = null)
#else
	private protected Filtered(IEnumerable<T> source, IReadOnlyList<IFilter<T>>? filters = null)
#endif
	{
		_source = source;
		Filters = filters ?? [];
	}

	/// <summary>
	///     Creates a clone that reuses the ORIGINAL <paramref name="original" />'s underlying source (never wrapping the
	///     instance itself) together with a fresh <paramref name="filters" /> list, so that filters are never re-applied
	///     on top of an already-filtered sequence.
	/// </summary>
	private protected Filtered(Filtered<T, TFiltered> original, IReadOnlyList<IFilter<T>> filters)
		: this(original._source, filters)
	{
	}

	/// <summary>
	///     The filters on the source.
	/// </summary>
	private protected IReadOnlyList<IFilter<T>> Filters { get; }

#if NET8_0_OR_GREATER
	/// <inheritdoc />
	public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new())
	{
		await foreach (T item in _source.WithCancellation(cancellationToken))
		{
			bool applies = true;
			for (int index = 0; index < Filters.Count; index++)
			{
				if (!await Filters[index].Applies(item, cancellationToken))
				{
					applies = false;
					break;
				}
			}

			if (applies)
			{
				yield return item;
			}
		}
	}
#else
	/// <inheritdoc />
	public IEnumerator<T> GetEnumerator()
		=> _source.Where(a => Filters.All(f => SynchronousBoundary.Wait(() => f.Applies(a, CancellationToken.None)))).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif

	/// <summary>
	///     Filters the applicable <typeparamref name="T" /> on which the expectations should be applied.
	/// </summary>
	/// <param name="filter">The filter to apply on <typeparamref name="T" />.</param>
	public TFiltered Which(IFilter<T> filter)
		=> CloneWith([.. Filters, filter,]);

	/// <summary>
	///     Creates a new instance of <typeparamref name="TFiltered" /> that shares this collection's original source and
	///     metadata, but uses the given <paramref name="filters" />.
	/// </summary>
	private protected abstract TFiltered CloneWith(IReadOnlyList<IFilter<T>> filters);
}
