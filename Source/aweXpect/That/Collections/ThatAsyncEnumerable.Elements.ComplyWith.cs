#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	public partial class Elements<TItem>
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
			ComplyWith(Action<IThatSubject<TItem>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
					=> new AsyncComplyWithConstraint<TItem>(it, grammars, _quantifier, expectations)),
				_subject);
		}
	}

	public partial class Elements
	{
		/// <summary>
		///     …comply with the <paramref name="expectations" />.
		/// </summary>
		public AndOrResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
			ComplyWith(Action<IThatSubject<string?>> expectations)
		{
			expectations.ThrowIfNull();
			return new(
				_subject.Get().ExpectationBuilder.AddConstraint((it, grammars)
					=> new AsyncComplyWithConstraint<string?>(it, grammars, _quantifier, expectations)),
				_subject);
		}
	}
}
#endif
