using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatEnumerable
{
	private const string EndsWithSummary =
		"Verifies that the collection ends with the provided <paramref name=\"expected\" /> collection.";

	private const string DoesNotEndWithSummary =
		"Verifies that the collection does not end with the provided <paramref name=\"unexpected\" /> collection.";

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary,
		Remarks = SetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		EndsWithCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ItemEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				SubjectEqualityOptions<TItem, TItem> itemOptions = new(options, () => options.HasDefaultMatchType);
				EndsWithConstraint<TItem, TItem> constraint = new(it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, ExpectedType = "string?",
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	internal static StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		EndsWithForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint<IEnumerable<string?>?>((it, grammars) =>
			{
				SubjectEqualityOptions<string?, string?> itemOptions =
					new(options, () => options.ComparesByOrdinalEquality);
				EndsWithConstraint<string?, string?> constraint = new(it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary, Remarks = SetComparerRemarks)]
	internal static ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>
		EndsWithWithToleranceCore<TItem, TTolerance>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				SubjectEqualityOptions<TItem, TItem> itemOptions = new(options,
					() => ObjectEqualityWithToleranceOptionsFactory.HasDefaultMatchType(options));
				EndsWithConstraint<TItem, TItem> constraint = new(it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Priority = -1, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary,
		Remarks = UntypedSetComparerRemarks)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, Priority = -2, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary,
		Remarks = LowerPriorityRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, TItem>
		EndsWithForEnumerableCore<TItem>(
			IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ItemEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable?>((it, grammars) =>
			{
				SubjectEqualityOptions<TItem, TItem> itemOptions = new(options, () => options.HasDefaultMatchType);
				EndsWithForEnumerableConstraint<IEnumerable, TItem> constraint = new(
					it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Priority = -1, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary,
		Remarks = UntypedCollectionRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>
		EndsWithForObjectsCore(
			IThat<IEnumerable?> subject,
			IEnumerable expected,
			string? expectedExpression,
			bool negated)
		=> EndsWithForEnumerableCore<object?>(subject, expected?.Cast<object?>()!, expectedExpression, negated);

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Priority = -1, ExpectedType = "string?",
		Summary = "Verifies that the collection ends with the provided <paramref name=\"expected\" /> value.",
		NegatedSummary =
			"Verifies that the collection does not end with the provided <paramref name=\"unexpected\" /> value.",
		Remarks = SingleValueRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, string?>
		EndsWithSingleStringCore(
			IThat<IEnumerable?> subject,
			string? expected,
			bool negated)
	{
		string?[] expectedItems = [expected,];
		ItemEqualityOptions<string?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, string?>(
			expectationBuilder.AddConstraint<IEnumerable?>((it, grammars) =>
			{
				SubjectEqualityOptions<string?, string?> itemOptions =
					new(options, () => options.HasDefaultMatchType);
				EndsWithForEnumerableConstraint<IEnumerable, string?> constraint = new(
					it, grammars,
					Formatter.Format(expectedItems), expectedItems, itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true, Params = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>
		EndsWithForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				EndsWithForEnumerableConstraint<TCollection, TItem> constraint = new(
					it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true, Params = true,
		ExpectedType = "string?",
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static StringEqualityTypeResult<TCollection, IThat<TCollection>>
		EndsWithForCollectionStringsCore<TCollection>(
			IThat<TCollection> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				EndsWithForEnumerableConstraint<TCollection, string?> constraint = new(
					it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateExpectationFamily("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		EndsWithWithToleranceForCollectionCore<TCollection, TItem, TTolerance>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				EndsWithForEnumerableConstraint<TCollection, TItem> constraint = new(
					it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	private sealed class EndsWithConstraint<TItem, TMatch>
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>,
			IAsyncContextConstraint<IEnumerable<TItem>?>
		where TItem : TMatch
	{
		private CollectionContext _collectionContext;
		private readonly TItem[] _expected;
		private readonly string _expectedExpression;
		private readonly string _it;
		private readonly IOptionsEquality<TMatch> _options;
		private readonly Func<object?, bool>? _useComparerOf;
		private TItem? _firstMismatchItem;
		private bool _foundMismatch;
		private int _index;
		private List<TItem>? _items;
		private int _itemsCount;
		private int _offset;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _collectionContext.AppendTo(contexts);

		public EndsWithConstraint(
			string it,
			ExpectationGrammars grammars,
			string expectedExpression,
			TItem[] expected,
			IOptionsEquality<TMatch> options,
			Func<object?, bool>? useComparerOf = null) : base(it, grammars)
		{
			_it = it;
			_expectedExpression = expectedExpression;
			_expected = expected;
			_options = options;
			_useComparerOf = useComparerOf;
		}

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_useComparerOf?.Invoke(actual);
			if (_expected.Length == 0)
			{
				Outcome = Outcome.Success;
				return this;
			}

			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem>(actual);
			_items = [];
			_foundMismatch = false;
			foreach (TItem item in materializedEnumerable)
			{
				if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materializedEnumerable, true);
					return this;
				}

				_items.Add(item);
			}

			_itemsCount = _items.Count;
			_offset = _itemsCount - _expected.Length;
			for (_index = _expected.Length - 1; _index >= 0; _index--)
			{
				if (_index + _offset < 0)
				{
					Outcome = Outcome.Failure;
					_collectionContext.Set(materializedEnumerable);
					return this;
				}

				TItem item = _items[_index + _offset];
				TItem expectedItem = _expected[_index];
				if (!await _options.AreConsideredEqual(item, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_collectionContext.Set(materializedEnumerable);
					Outcome = Outcome.Failure;
					return this;
				}
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("ends with ", "end with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_foundMismatch)
			{
				stringBuilder.Append(_it).Append(" contained item ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index + _offset).Append(" instead of ");
				stringBuilder.AppendExpectedItem(_expected[_index], _options);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").AppendItemCount(_itemsCount).Append(" and lacked ")
					.AppendItemCount(_expected.Length - _itemsCount).Append(": ");
				Formatter.Format(stringBuilder, _expected.Take(-_offset), FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not end with ", "do not end with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_expected.Length == 0)
			{
				stringBuilder.Append(_it).Append(Grammars.SubjectVerb(_it, " was ", " were "));
				Formatter.Format(stringBuilder, Actual, FormattingOptions.MultipleLines);
			}
			else
			{
				stringBuilder.Append(_it).Append(" did end with ");
				Formatter.Format(stringBuilder, _items?.Skip(_offset),
					typeof(TItem).GetFormattingOption(_expected.Length));
			}
		}
	}

	private sealed class EndsWithForEnumerableConstraint<TEnumerable, TMatch>
		: ConstraintResult.WithNotNullValue<TEnumerable?>,
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable
	{
		private CollectionContext _collectionContext;
		private readonly TMatch[] _expected;
		private readonly string _expectedExpression;
		private readonly string _it;
		private readonly IOptionsEquality<TMatch> _options;
		private readonly Func<object?, bool>? _useComparerOf;
		private object? _firstMismatchItem;
		private bool _foundMismatch;
		private int _index;
		private List<object?>? _items;
		private int _itemsCount;
		private int _offset;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _collectionContext.AppendTo(contexts);

		public EndsWithForEnumerableConstraint(
			string it,
			ExpectationGrammars grammars,
			string expectedExpression,
			TMatch[] expected,
			IOptionsEquality<TMatch> options,
			Func<object?, bool>? useComparerOf = null) : base(it, grammars)
		{
			_it = it;
			_expectedExpression = expectedExpression;
			_expected = expected;
			_options = options;
			_useComparerOf = useComparerOf;
		}

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			if (actual.IsDefaultImmutableArray())
			{
				return this.AsNullSubject(It);
			}

			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			_useComparerOf?.Invoke(actual);
			if (_expected.Length == 0)
			{
				Outcome = Outcome.Success;
				return this;
			}

			IEnumerable materializedEnumerable = context.UseMaterializedEnumerable(actual);
			_items = [];
			_foundMismatch = false;
			foreach (object? item in materializedEnumerable)
			{
				if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
				{
					Outcome = Outcome.Undecided;
					_collectionContext.Set(materializedEnumerable, true);
					return this;
				}

				_items.Add(item);
			}

			_itemsCount = _items.Count;
			_offset = _itemsCount - _expected.Length;
			for (_index = _expected.Length - 1; _index >= 0; _index--)
			{
				if (_index + _offset < 0)
				{
					Outcome = Outcome.Failure;
					_collectionContext.Set(materializedEnumerable);
					return this;
				}

				object? item = _items[_index + _offset];
				TMatch expectedItem = _expected[_index];
				if (!TryCastItem(item, out TMatch matchedItem) ||
				    !await _options.AreConsideredEqual(matchedItem, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_collectionContext.Set(materializedEnumerable);
					Outcome = Outcome.Failure;
					return this;
				}
			}

			Outcome = Outcome.Success;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("ends with ", "end with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_foundMismatch)
			{
				stringBuilder.Append(_it).Append(" contained item ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index + _offset).Append(" instead of ");
				stringBuilder.AppendExpectedItem(_expected[_index], _options);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").AppendItemCount(_itemsCount).Append(" and lacked ")
					.AppendItemCount(_expected.Length - _itemsCount).Append(": ");
				Formatter.Format(stringBuilder, _expected.Take(-_offset), FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not end with ", "do not end with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_expected.Length == 0)
			{
				stringBuilder.Append(_it).Append(Grammars.SubjectVerb(_it, " was ", " were "));
				Formatter.Format(stringBuilder, Actual, FormattingOptions.MultipleLines);
			}
			else
			{
				IEnumerable<object?> suffix = _items?.Skip(_offset) ?? [];
				stringBuilder.Append(_it).Append(" did end with ");
				Formatter.Format(stringBuilder, suffix, suffix.GetItemType().GetFormattingOption(_expected.Length));
			}
		}
	}
}
