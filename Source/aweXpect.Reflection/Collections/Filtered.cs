using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Collections;

/// <summary>
///     Container for filtered collections.
/// </summary>
public static partial class Filtered;

/// <summary>
///     Base class for filtered collections of <typeparamref name="T" />.
/// </summary>
public abstract class Filtered<T, TFiltered>(IEnumerable<T> source, List<IFilter<T>>? filters = null) : IEnumerable<T>
	where TFiltered : Filtered<T, TFiltered>
{
	/// <summary>
	///     The filters on the source.
	/// </summary>
	protected List<IFilter<T>> Filters { get; } = filters ?? [];

	/// <inheritdoc />
	public IEnumerator<T> GetEnumerator() => source.Where(a => Filters.All(f => f.Applies(a))).GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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
