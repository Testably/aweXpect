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

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection has an item that complies with the <paramref name="expectations" />…
	/// </summary>
	public static HasItemResult<IAsyncEnumerable<TItem>?> HasItemThat<TItem>(
		this IThat<IAsyncEnumerable<TItem>?> source, Action<IThatSubject<TItem>> expectations)
	{
		CollectionIndexOptions indexOptions = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new HasItemResult<IAsyncEnumerable<TItem>?>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new HasItemThatConstraint<TItem>(expectationBuilder, it, grammars, expectations, indexOptions)),
			source,
			indexOptions);
	}

	private sealed class HasItemThatConstraint<TItem> : ConstraintResult.WithValue<IAsyncEnumerable<TItem>?>,
		IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly string _it;
		private readonly ManualExpectationBuilder<TItem> _itemExpectationBuilder;
		private readonly CollectionIndexOptions _options;
		private TItem? _actual;
		private bool _hasIndex;

		public HasItemThatConstraint(ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			Action<IThatSubject<TItem>> expectations,
			CollectionIndexOptions options) : base(grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_options = options;

			_itemExpectationBuilder = new ManualExpectationBuilder<TItem>(null, Grammars & ~ExpectationGrammars.Plural);
			expectations.Invoke(new ThatSubject<TItem>(_itemExpectationBuilder));
		}

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materialized =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			await _expectationBuilder.AddCollectionContext(materialized as IMaterializedEnumerable<TItem>);
			_hasIndex = false;
			Outcome = Outcome.Failure;

			int? count = null;
			if (_options.Match is CollectionIndexOptions.IMatchFromEnd)
			{
				count = (await (materialized as IMaterializedEnumerable<TItem>)!.MaterializeItems(null)).Count;
			}

			int index = -1;
			await foreach (TItem item in materialized.WithCancellation(cancellationToken))
			{
				index++;
				bool? isIndexInRange = _options.Match switch
				{
					CollectionIndexOptions.IMatchFromBeginning fromBeginning => fromBeginning.MatchesIndex(index),
					CollectionIndexOptions.IMatchFromEnd fromEnd => fromEnd.MatchesIndex(index, count),
					_ => false,
				};
				if (isIndexInRange != true)
				{
					if (isIndexInRange == false)
					{
						break;
					}

					continue;
				}

				_hasIndex = true;
				_actual = item;
				ConstraintResult isMatch = await _itemExpectationBuilder.IsMetBy(item, context, cancellationToken);
				Outcome = isMatch.Outcome;
				if (Outcome == Outcome.Success)
				{
					break;
				}
			}

			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("has item that ", "have item that "));
			_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
			stringBuilder.Append(_options.Match.GetDescription());
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Actual is null)
			{
				stringBuilder.ItWasNull(_it, Grammars);
			}
			else if (_hasIndex)
			{
				if (_options.Match.OnlySingleIndex())
				{
					stringBuilder.Append(_it).Append(" had item ");
					Formatter.Format(stringBuilder, _actual);
					stringBuilder.Append(_options.Match.GetDescription());
				}
				else
				{
					string optionDescription = _options.Match.GetDescription();
					if (string.IsNullOrEmpty(optionDescription))
					{
						optionDescription = " at any index";
					}

					stringBuilder.Append(_it).Append(" did not match").Append(optionDescription);
				}
			}
			else
			{
				stringBuilder.Append(_it).Append(" did not contain any item").Append(_options.Match.GetDescription());
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not have item that ", "do not have item that "));
			_itemExpectationBuilder.AppendExpectation(stringBuilder, indentation);
			stringBuilder.Append(_options.Match.GetDescription());
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual is null)
			{
				stringBuilder.ItWasNull(_it, Grammars);
			}
			else
			{
				stringBuilder.Append(_it).Append(" did");
			}
		}
	}
}
#endif
