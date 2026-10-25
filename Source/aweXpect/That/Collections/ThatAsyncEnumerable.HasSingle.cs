#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Options;
using aweXpect.Results;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection contains exactly one item.
	/// </summary>
	[GuaranteesNotNull]
	public static AsyncSingleItemResult<IAsyncEnumerable<TItem>, TItem> HasSingle<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> subject)
	{
		OptionsSlot<PredicateOptions<TItem>> options = new(PredicateOptions<TItem>.Default);
		SingleItemSource<TItem> source = new();
		return new AsyncSingleItemResult<IAsyncEnumerable<TItem>, TItem>(
			subject.Get().ExpectationBuilder.AddConstraint((Options: options, Source: source),
				static (state, it, grammars) => state.Source.Use(
					new AsyncHasSingleConstraint<TItem>(it, grammars, state.Options))),
			options,
			source);
	}
}
#endif
