using System;
using System.Collections;
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

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	public partial class Elements
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		public StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, TMember> AreUnique<TMember>(
			Func<string?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		public StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, TMember> AreNotUnique<TMember>(
			Func<string?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreNotUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUniqueCore(
			bool expectUnique)
		{
			StringEqualityOptions options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
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

		private ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, TMember>
			AreUniqueCore<TMember>(
				Func<string?, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ObjectEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>, TMember>(
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

		private StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>> AreUniqueCore(
			Func<string?, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
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
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember> AreUnique<TMember>(
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember> AreNotUnique<TMember>(
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreNotUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem> AreUniqueCore(
			bool expectUnique)
		{
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
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

		private ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember> AreUniqueCore<TMember>(
			Func<TItem, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ObjectEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TMember>(
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

		private StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>> AreUniqueCore(
			Func<TItem, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>>(
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

	public partial class ElementsForEnumerable<TEnumerable>
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, TMember> AreUnique<TMember>(
			Func<object?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, TMember> AreNotUnique<TMember>(
			Func<object?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?> AreUniqueCore(bool expectUnique)
		{
			ObjectEqualityOptions<object?> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, object?>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, object?>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), options),
						a => a,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, TMember> AreUniqueCore<TMember>(
			Func<object?, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ObjectEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable?>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, TMember>(
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

	public partial class ElementsForStructEnumerable<TEnumerable, TItem>
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreUnique<TMember>(
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreNotUnique<TMember>(
			Func<TItem, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreNotUnique(
			Func<TItem, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem> AreUniqueCore(bool expectUnique)
		{
			ObjectEqualityOptions<TItem> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TItem>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, TItem>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), options),
						a => (TItem)a!,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreUniqueCore<TMember>(
			Func<TItem, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ObjectEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, TMember>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						a => memberAccessor((TItem)a!),
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}

		private StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUniqueCore(
			Func<TItem, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, string>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						a => memberAccessor((TItem)a!),
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}
	}

	public partial class ElementsForStructEnumerable<TEnumerable>
	{
		/// <summary>
		///     …are unique, i.e. they occur exactly once in the collection.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUnique()
			=> AreUniqueCore(true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreUnique<TMember>(
			Func<string?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …have unique members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, true);

		/// <summary>
		///     …are not unique, i.e. they occur more than once in the collection.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreNotUnique()
			=> AreUniqueCore(false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreNotUnique<TMember>(
			Func<string?, TMember> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		/// <summary>
		///     …have duplicate members specified by the <paramref name="memberAccessor" />.
		/// </summary>
		public StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreNotUnique(
			Func<string?, string> memberAccessor,
			[CallerArgumentExpression("memberAccessor")]
			string doNotPopulateThisValue = "")
			=> AreUniqueCore(memberAccessor, doNotPopulateThisValue, false);

		private StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUniqueCore(bool expectUnique)
		{
			StringEqualityOptions options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, string?>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUnique(expectUnique ? g : g.Negate(), options),
						a => (string?)a,
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}

		private ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember> AreUniqueCore<TMember>(
			Func<string?, TMember> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			ObjectEqualityOptions<TMember> options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new ObjectEqualityResult<TEnumerable, IThat<TEnumerable>, TMember>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, TMember>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						a => memberAccessor((string?)a),
						(a, b) => options.AreConsideredEqual(a, b),
						expectUnique)),
				_subject,
				options);
		}

		private StringEqualityResult<TEnumerable, IThat<TEnumerable>> AreUniqueCore(
			Func<string?, string> memberAccessor, string memberAccessorExpression, bool expectUnique)
		{
			memberAccessor.ThrowIfNull();
			StringEqualityOptions options = new();
			ExpectationBuilder expectationBuilder = _subject.Get().ExpectationBuilder;
			return new StringEqualityResult<TEnumerable, IThat<TEnumerable>>(
				expectationBuilder.AddConstraint((it, grammars)
					=> new AreUniqueForEnumerableConstraint<TEnumerable, string>(
						expectationBuilder, it, grammars,
						_quantifier,
						g => ElementExpectations.IsUniqueFor(expectUnique ? g : g.Negate(),
							memberAccessorExpression.TrimCommonWhiteSpace(), options),
						a => memberAccessor((string?)a),
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
#if NET8_0_OR_GREATER
		Func<TMember, TMember, ValueTask<bool>> areConsideredEqual,
#else
		Func<TMember, TMember, Task<bool>> areConsideredEqual,
#endif
		bool expectUnique)
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>(it, grammars),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
		private int _matchingCount;
		private LimitedCollection<TItem>? _matchingItems;
		private int _notMatchingCount;
		private LimitedCollection<TItem>? _notMatchingItems;
		private int? _totalCount;

		public async Task<ConstraintResult> IsMetBy(
			IEnumerable<TItem>? actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable<TItem> materialized = context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			List<TMember> distinctMembers = [];
			List<int> occurrences = [];
			List<(TItem Item, int MemberIndex)> items = [];
			foreach (TItem item in materialized)
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
				items.Add((item, memberIndex));

				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					expectationBuilder.AddCollectionContext(materialized, true);
					return this;
				}
			}

			int maxItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
			_matchingCount = 0;
			_notMatchingCount = 0;
			_matchingItems = new LimitedCollection<TItem>(maxItems);
			_notMatchingItems = new LimitedCollection<TItem>(maxItems);
			foreach ((TItem item, int memberIndex) in items)
			{
				bool isUnique = occurrences[memberIndex] == 1;
				if (isUnique == expectUnique)
				{
					_matchingCount++;
					_matchingItems.Add(item);
				}
				else
				{
					_notMatchingCount++;
					_notMatchingItems.Add(item);
				}
			}

			_totalCount = items.Count;
			Outcome = quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			AppendContexts();
			expectationBuilder.AddCollectionContext(materialized);
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
				stringBuilder.Append(For);
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
				stringBuilder.Append(For);
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

	private sealed class AreUniqueForEnumerableConstraint<TEnumerable, TMember>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		EnumerableQuantifier quantifier,
		Func<ExpectationGrammars, string> expectationText,
		Func<object?, TMember> memberAccessor,
#if NET8_0_OR_GREATER
		Func<TMember, TMember, ValueTask<bool>> areConsideredEqual,
#else
		Func<TMember, TMember, Task<bool>> areConsideredEqual,
#endif
		bool expectUnique)
		: ConstraintResult.WithNotNullValue<TEnumerable>(it, grammars),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private Type? _itemType;
		private int _matchingCount;
		private LimitedCollection<object?>? _matchingItems;
		private int _notMatchingCount;
		private LimitedCollection<object?>? _notMatchingItems;
		private int? _totalCount;

		public async Task<ConstraintResult> IsMetBy(
			TEnumerable actual,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IEnumerable materialized = context.UseMaterializedEnumerable(actual);
			List<TMember> distinctMembers = [];
			List<int> occurrences = [];
			List<(object? Item, int MemberIndex)> items = [];
			foreach (object? item in materialized)
			{
				if (_itemType is null && item is not null)
				{
					_itemType = item.GetType();
				}

				TMember member = memberAccessor(item);
				int memberIndex = await distinctMembers.IndexOfAsync(member, areConsideredEqual);
				if (memberIndex < 0)
				{
					memberIndex = distinctMembers.Count;
					distinctMembers.Add(member);
					occurrences.Add(0);
				}

				occurrences[memberIndex]++;
				items.Add((item, memberIndex));

				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					expectationBuilder.AddCollectionContext(materialized, true);
					return this;
				}
			}

			int maxItems = Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get() + 1;
			_matchingCount = 0;
			_notMatchingCount = 0;
			_matchingItems = new LimitedCollection<object?>(maxItems);
			_notMatchingItems = new LimitedCollection<object?>(maxItems);
			foreach ((object? item, int memberIndex) in items)
			{
				bool isUnique = occurrences[memberIndex] == 1;
				if (isUnique == expectUnique)
				{
					_matchingCount++;
					_matchingItems.Add(item);
				}
				else
				{
					_notMatchingCount++;
					_notMatchingItems.Add(item);
				}
			}

			_totalCount = items.Count;
			Outcome = quantifier.GetOutcome(_matchingCount, _notMatchingCount, _totalCount);
			AppendContexts();
			expectationBuilder.AddCollectionContext(materialized);
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
				stringBuilder.Append(For);
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
				stringBuilder.Append(For);
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
							(_itemType ?? typeof(object)).GetFormattingOption(_matchingItems?.Count)),
						int.MaxValue));
			}

			if (quantifierContexts.HasFlag(EnumerableQuantifier.QuantifierContexts.NotMatchingItems) &&
			    _notMatchingItems?.Count > 0)
			{
				expectationBuilder.AddContext(new ResultContext.SyncCallback("Not matching items",
						() => Formatter.Format(_notMatchingItems,
							(_itemType ?? typeof(object)).GetFormattingOption(_notMatchingItems?.Count)),
						int.MaxValue));
			}
		}
	}
}
