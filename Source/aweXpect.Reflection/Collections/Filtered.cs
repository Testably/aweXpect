using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Helpers;
#if NET8_0_OR_GREATER
using System.Threading;
using System.Threading.Tasks;
#else
using System.Collections;
#endif

namespace aweXpect.Reflection.Collections;

/// <summary>
///     Container for filtered collections.
/// </summary>
public static partial class Filtered;

/// <summary>
///     Base class for filtered collections of <typeparamref name="T" />.
/// </summary>
#if NET8_0_OR_GREATER
public abstract class Filtered<T, TFiltered>(IAsyncEnumerable<T> source, List<IFilter<T>>? filters = null)
	: IAsyncEnumerable<T>
#else
public abstract class Filtered<T, TFiltered>(IEnumerable<T> source, List<IFilter<T>>? filters = null)
	: IEnumerable<T>
#endif
	where TFiltered : Filtered<T, TFiltered>
{
	/// <summary>
	///     The filters on the source.
	/// </summary>
	protected List<IFilter<T>> Filters { get; } = filters ?? [];

#if NET8_0_OR_GREATER
	/// <inheritdoc />
	public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new())
	{
		await foreach (T item in source.WithCancellation(cancellationToken))
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
		=> source.Where(a => Filters.All(f => f.Applies(a).GetAwaiter().GetResult())).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
#endif

	/// <summary>
	///     Filters the applicable <typeparamref name="T" /> on which the expectations should be applied.
	/// </summary>
	/// <param name="filter">The filter to apply on <typeparamref name="T" />.</param>
	public TFiltered Which(IFilter<T> filter)
	{
		Filters.Add(filter);
		return (TFiltered)this;
	}

	/// <summary>
	///     Filters the applicable <typeparamref name="T" /> on which the expectations should be applied
	///     according to the <paramref name="predicate" />.
	/// </summary>
	public TFiltered Which(Func<T, bool> predicate,
		[CallerArgumentExpression("predicate")]
		string doNotPopulateThisValue = "")
		=> Which(Filter.Suffix(predicate, $"matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} "));
}
