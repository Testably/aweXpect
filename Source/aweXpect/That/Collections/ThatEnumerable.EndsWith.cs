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

	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		EndsWithCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated ? "unexpected" : "expected");
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				EndsWithConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, ExpectedType = "string",
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		EndsWithForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated ? "unexpected" : "expected");
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint<IEnumerable<string?>?>((it, grammars) =>
			{
				EndsWithConstraint<string?, string?> constraint = new(expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Priority = -1, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, Priority = -2, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary,
		Remarks = LowerPriorityRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, TItem>
		EndsWithForEnumerableCore<TItem>(
			IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated ? "unexpected" : "expected");
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable?>((it, grammars) =>
			{
				EndsWithForEnumerableConstraint<IEnumerable, TItem> constraint = new(
					expectationBuilder, it, grammars,
					Formatter.Format(expected), expected.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Priority = -1, ExpectedType = "string?",
		Summary = "Verifies that the collection ends with the provided <paramref name=\"expected\" /> value.",
		NegatedSummary =
			"Verifies that the collection does not end with the provided <paramref name=\"unexpected\" /> value.",
		Remarks = SingleValueRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, string?>
		EndsWithSingleStringCore(
			IThat<IEnumerable?> subject,
			string? expected,
			bool negated)
	{
		string?[] expectedItems = [expected,];
		ObjectEqualityOptions<string?> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, string?>(
			expectationBuilder.AddConstraint<IEnumerable?>((it, grammars) =>
			{
				EndsWithForEnumerableConstraint<IEnumerable, string?> constraint = new(
					expectationBuilder, it, grammars,
					Formatter.Format(expectedItems), expectedItems, options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true, Params = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>
		EndsWithForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		expected.ThrowIfNullOrEmpty(negated ? "unexpected" : "expected");
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				EndsWithForEnumerableConstraint<TCollection, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", PerSubject = true, Params = true,
		ExpectedType = "string",
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static StringEqualityResult<TCollection, IThat<TCollection>>
		EndsWithForCollectionStringsCore<TCollection>(
			IThat<TCollection> subject,
			IEnumerable<string?> expected,
			bool negated)
		where TCollection : IEnumerable
	{
		expected.ThrowIfNullOrEmpty(negated ? "unexpected" : "expected");
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				EndsWithForEnumerableConstraint<TCollection, string?> constraint = new(
					expectationBuilder, it, grammars,
					Formatter.Format(expected), expected.ToArray(), options);
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
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly TItem[] _expected;
		private readonly string _expectedExpression;
		private readonly string _it;
		private readonly IOptionsEquality<TMatch> _options;
		private TItem? _firstMismatchItem;
		private bool _foundMismatch;
		private int _index;
		private int _itemsCount;
		private int _offset;

		public EndsWithConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			string expectedExpression,
			TItem[] expected,
			IOptionsEquality<TMatch> options) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_expectedExpression = expectedExpression;
			_expected = expected;
			_options = options;
		}

		public async Task<ConstraintResult> IsMetBy(IEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (_expected.Length == 0)
			{
				Outcome = Outcome.Success;
				return this;
			}

			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			List<TItem> items = materializedEnumerable.ToList();

			_itemsCount = items.Count;
			_offset = _itemsCount - _expected.Length;
			for (_index = _expected.Length - 1; _index >= 0; _index--)
			{
				if (_index + _offset < 0)
				{
					Outcome = Outcome.Failure;
					_expectationBuilder.AddCollectionContext(materializedEnumerable);
					return this;
				}

				TItem item = items[_index + _offset];
				TItem expectedItem = _expected[_index];
				if (!await _options.AreConsideredEqual(item, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
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
				stringBuilder.Append(_it).Append(" contained ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index + _offset).Append(" instead of ");
				Formatter.Format(stringBuilder, _expected[_index]);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").AppendItemCount(_itemsCount).Append(" and misses ")
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
				Formatter.Format(stringBuilder, Actual?.Skip(_offset), FormattingOptions.MultipleLines);
			}
		}
	}

	private sealed class EndsWithForEnumerableConstraint<TEnumerable, TMatch>
		: ConstraintResult.WithNotNullValue<TEnumerable?>,
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly TMatch[] _expected;
		private readonly string _expectedExpression;
		private readonly string _it;
		private readonly IOptionsEquality<TMatch> _options;
		private object? _firstMismatchItem;
		private bool _foundMismatch;
		private int _index;
		private int _itemsCount;
		private int _offset;

		public EndsWithForEnumerableConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			string expectedExpression,
			TMatch[] expected,
			IOptionsEquality<TMatch> options) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_expectedExpression = expectedExpression;
			_expected = expected;
			_options = options;
		}

		public async Task<ConstraintResult> IsMetBy(TEnumerable? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			if (_expected.Length == 0)
			{
				Outcome = Outcome.Success;
				return this;
			}

			IEnumerable materializedEnumerable = context.UseMaterializedEnumerable(actual);
			List<object?> items = materializedEnumerable.Cast<object?>().ToList();

			_itemsCount = items.Count;
			_offset = _itemsCount - _expected.Length;
			for (_index = _expected.Length - 1; _index >= 0; _index--)
			{
				if (_index + _offset < 0)
				{
					Outcome = Outcome.Failure;
					_expectationBuilder.AddCollectionContext(materializedEnumerable);
					return this;
				}

				object? item = items[_index + _offset];
				TMatch expectedItem = _expected[_index];
				if (!TryCastItem(item, out TMatch matchedItem) ||
				    !await _options.AreConsideredEqual(matchedItem, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
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
				stringBuilder.Append(_it).Append(" contained ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index + _offset).Append(" instead of ");
				Formatter.Format(stringBuilder, _expected[_index]);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").AppendItemCount(_itemsCount).Append(" and misses ")
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
				Formatter.Format(stringBuilder, Actual?.Cast<object?>().Skip(_offset), FormattingOptions.MultipleLines);
			}
		}
	}
}
