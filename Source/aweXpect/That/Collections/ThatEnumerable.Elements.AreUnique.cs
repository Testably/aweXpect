using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
		: QuantifiedCollectionConstraint<IEnumerable<TItem>?, TItem>(expectationBuilder, it, grammars, quantifier,
				expectationText, "were"),
			IAsyncContextConstraint<IEnumerable<TItem>?>
	{
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
			OccurrenceCounter<TMember> occurrences = new(areConsideredEqual);
			List<(TItem Item, int MemberIndex)> items = [];
			foreach (TItem item in materialized)
			{
				items.Add((item, await occurrences.Add(memberAccessor(item))));
				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					ExpectationBuilder.AddCollectionContext(materialized, true);
					return this;
				}
			}

			foreach ((TItem item, int memberIndex) in items)
			{
				Record(item, occurrences.IsUnique(memberIndex) == expectUnique);
			}

			Complete();
			ExpectationBuilder.AddCollectionContext(materialized);
			return this;
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
		: QuantifiedCollectionConstraint<TEnumerable, object?>(expectationBuilder, it, grammars, quantifier,
				expectationText, "were"),
			IAsyncContextConstraint<TEnumerable>
		where TEnumerable : IEnumerable?
	{
		private Type? _itemType;

		protected override Type ItemType => _itemType ?? typeof(object);

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
			OccurrenceCounter<TMember> occurrences = new(areConsideredEqual);
			List<(object? Item, int MemberIndex)> items = [];
			foreach (object? item in materialized)
			{
				_itemType ??= item?.GetType();
				items.Add((item, await occurrences.Add(memberAccessor(item))));
				if (cancellationToken.IsCancellationRequested)
				{
					Outcome = Outcome.Undecided;
					ExpectationBuilder.AddCollectionContext(materialized, true);
					return this;
				}
			}

			foreach ((object? item, int memberIndex) in items)
			{
				Record(item, occurrences.IsUnique(memberIndex) == expectUnique);
			}

			Complete();
			ExpectationBuilder.AddCollectionContext(materialized);
			return this;
		}
	}
}
