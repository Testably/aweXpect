#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#else
using System.Collections;
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
///     <para />
///     Scope boundary: the immutability guarantee covers the collection/filter level. The nested
///     <c>StringEqualityResult</c> / <c>StringEqualityResultType</c> refinement classes still refine their
///     <c>StringEqualityOptions</c> via in-place mutation; that options object originates from the aweXpect core library
///     (<c>aweXpect.Options</c>), is mutable by design, and is intentionally left as-is.
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
	protected Filtered(IAsyncEnumerable<T> source, List<IFilter<T>>? filters = null)
#else
	protected Filtered(IEnumerable<T> source, List<IFilter<T>>? filters = null)
#endif
	{
		_source = source;
		Filters = filters ?? [];
	}

	/// <summary>
	///     The filters on the source.
	/// </summary>
	/// <remarks>
	///     Treated as immutable: it is never modified after construction. The concrete type stays <see cref="List{T}" />
	///     to preserve the public API surface.
	/// </remarks>
	protected List<IFilter<T>> Filters { get; }

#if NET8_0_OR_GREATER
	/// <inheritdoc />
	public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new())
	{
		await foreach (T item in _source.WithCancellation(cancellationToken))
		{
			if (await Filters.AllAsync(filter => filter.Applies(item)))
			{
				yield return item;
			}
		}
	}
#else
	/// <inheritdoc />
	public IEnumerator<T> GetEnumerator()
		=> _source.Where(a => Filters.All(f => f.Applies(a).GetAwaiter().GetResult())).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif

	/// <summary>
	///     Filters the applicable <typeparamref name="T" /> on which the expectations should be applied.
	/// </summary>
	/// <param name="filter">The filter to apply on <typeparamref name="T" />.</param>
	public TFiltered Which(IFilter<T> filter)
		=> CloneWith([.. Filters, filter,]);

	/// <summary>
	///     Creates a clone that reuses the ORIGINAL <paramref name="original" />'s underlying source (never wrapping the
	///     instance itself) together with a fresh <paramref name="filters" /> list, so that filters are never re-applied
	///     on top of an already-filtered sequence.
	/// </summary>
	/// <remarks>
	///     Declared after the iterator methods on purpose: it must stay out of the way of the compiler-generated async
	///     iterator state-machine ordinal so the public API surface (which captures that generated name) is preserved.
	/// </remarks>
	private protected Filtered(Filtered<T, TFiltered> original, List<IFilter<T>> filters)
		: this(original._source, filters)
	{
	}

	/// <summary>
	///     Creates a new instance of <typeparamref name="TFiltered" /> that shares this collection's original source and
	///     metadata, but uses the given <paramref name="filters" />.
	/// </summary>
	private protected abstract TFiltered CloneWith(List<IFilter<T>> filters);
}
