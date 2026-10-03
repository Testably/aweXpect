#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
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
					=> new ComplyWithConstraint<TItem>(it, grammars, _quantifier, expectations)),
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
					=> new ComplyWithConstraint<string?>(it, grammars, _quantifier, expectations)),
				_subject);
		}
	}

	private sealed class ComplyWithConstraint<TItem>(
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Action<IThatSubject<TItem>> expectations)
		: ComplyWithConstraint<IAsyncEnumerable<TItem>?, TItem>(it, grammars, quantifier,
				expectations),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		public async Task<ConstraintResult> IsMetBy(
			IAsyncEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			await PrepareExpectation(context, cancellationToken);
			if (actual is null)
			{
				return this;
			}

			return await IsMetByItems(context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken),
				context, cancellationToken);
		}
	}
}
#endif
