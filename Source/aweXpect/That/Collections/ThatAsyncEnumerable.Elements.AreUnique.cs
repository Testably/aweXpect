#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	public partial class Elements
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		public StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>, TMember>
			AreUnique<TMember>(
				Func<string?, TMember> memberAccessor,
				[CallerArgumentExpression("memberAccessor")]
				string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		public StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>, TMember>
			AreNotUnique<TMember>(
				Func<string?, TMember> memberAccessor,
				[CallerArgumentExpression("memberAccessor")]
				string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreNotUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreUniqueCore(
			bool expectUnique)
		{
			StringEqualityOptions options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<string?, string?>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), options),
						a => a,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}

		private ObjectEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>, TMember>
			AreUniqueCore<TMember>(
				Func<string?, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ObjectEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<string?, TMember>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}

		private StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>> AreUniqueCore(
			Func<string?, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<string?, string>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}
	}

	public partial class Elements<TItem>
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>
			AreUnique<TMember>(
				Func<TItem, TMember> memberAccessor,
				[CallerArgumentExpression("memberAccessor")]
				string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> AreUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>
			AreNotUnique<TMember>(
				Func<TItem, TMember> memberAccessor,
				[CallerArgumentExpression("memberAccessor")]
				string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> AreNotUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem> AreUniqueCore(
			bool expectUnique)
		{
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<TItem, TItem>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), options),
						a => a,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}

		private ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>
			AreUniqueCore<TMember>(
				Func<TItem, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ObjectEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<TItem, TMember>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}

		private StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>> AreUniqueCore(
			Func<TItem, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueConstraint<TItem, string>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						memberAccessor,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}
	}

	private sealed class AreUniqueConstraint<TItem, TMember>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<TItem, TMember> memberAccessor,
		Func<TMember, TMember, ValueTask<bool>> areConsideredEqual,
		bool expectUnique)
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private int _matchingCount;
		private LimitedCollection<TItem>? _matchingItems;
		private int _notMatchingCount;
		private LimitedCollection<TItem>? _notMatchingItems;
		private int? _totalCount;

		public async Task<ConstraintResult> IsMetBy(
			IAsyncEnumerable<TItem>? actual,
			IEvaluationContext context,
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
			List<TMember> distinctMembers = [];
			List<int> occurrences = [];
			List<TItem> items = [];
			List<int> memberIndexes = [];
			await foreach (TItem item in materialized.WithCancellation(cancellationToken))
			{
				TMember member = memberAccessor(item);
				int memberIndex = await distinctMembers.IndexOfAsync(member, areConsideredEqual);
				if (memberIndex < 0)
				{
					memberIndex = distinctMembers.Count;
					distinctMembers.Add(member);
					occurrences.Add(0);
				}

				occurrences[memberIndex]++;
				items.Add(item);
				memberIndexes.Add(memberIndex);
			}

			if (cancellationToken.IsCancellationRequested)
			{
				Outcome = Outcome.Undecided;
				expectationBuilder.AddCollectionContext(items, true);
				return this;
			}

			int maxItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
			_matchingCount = 0;
			_notMatchingCount = 0;
			_matchingItems = new LimitedCollection<TItem>(maxItems);
			_notMatchingItems = new LimitedCollection<TItem>(maxItems);
			for (int i = 0; i < items.Count; i++)
			{
				bool isUnique = occurrences[memberIndexes[i]] == 1;
				if (isUnique == expectUnique)
				{
					_matchingCount++;
					_matchingItems.Add(items[i]);
				}
				else
				{
					_notMatchingCount++;
					_notMatchingItems.Add(items[i]);
				}
			}

			_totalCount = items.Count;
			Outcome = quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			AppendContexts();
			expectationBuilder.AddCollectionContext(items);
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append(quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(expectationText(Grammars));
				stringBuilder.Append(" for ");
				stringBuilder.Append(quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(quantifier.GetItemString());
			}
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
			=> quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount,
				"were");

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			if (Grammars.HasFlag(ExpectationGrammars.Nested))
			{
				stringBuilder.Append("not ");
				stringBuilder.Append(quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(expectationText(Grammars));
			}
			else
			{
				stringBuilder.Append(expectationText(Grammars));
				stringBuilder.Append(" for ");
				stringBuilder.Append(quantifier);
				stringBuilder.Append(' ');
				stringBuilder.Append(quantifier.GetItemString());
			}
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
			=> quantifier.AppendResult(stringBuilder, Grammars, _matchingCount, _notMatchingCount, _totalCount,
				"were");

		private void AppendContexts()
		{
			EnumerableQuantifier.QuantifierContexts quantifierContexts = quantifier.GetQuantifierContext();
			if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.MatchingItems) &&
			    _matchingItems?.Count > 0)
			{
				expectationBuilder.AddContext(new ResultContext.SyncCallback("Matching items",
						() => Formatter.Format(_matchingItems,
							typeof(TItem).GetFormattingOption(_matchingItems?.Count)),
						int.MaxValue));
			}

			if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.NotMatchingItems) &&
			    _notMatchingItems?.Count > 0)
			{
				expectationBuilder.AddContext(new ResultContext.SyncCallback("Not matching items",
						() => Formatter.Format(_notMatchingItems,
							typeof(TItem).GetFormattingOption(_notMatchingItems?.Count)),
						int.MaxValue));
			}
		}
	}
}
#endif
