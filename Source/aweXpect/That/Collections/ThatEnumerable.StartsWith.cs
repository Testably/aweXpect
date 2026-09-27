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
	private const string StartsWithSummary =
		"Verifies that the collection starts with the provided <paramref name=\"expected\" /> collection.";

	private const string DoesNotStartWithSummary =
		"Verifies that the collection does not start with the provided <paramref name=\"unexpected\" /> collection.";

	private const string SingleValueRemarks =
		"Without this overload a <see cref=\"string\" /> argument would bind to the collection overload and be\n" +
		"expected as a sequence of characters.";

	private const string LowerPriorityRemarks =
		"The priority is below the one of the collection overload, so that a collection argument binds as the\n" +
		"expected sequence instead of as a single expected item.";

	private const string UntypedCollectionRemarks =
		"Without this overload a collection argument without an item type would bind to the <c>params</c> overload\n" +
		"and be expected as a single item. The priority only takes effect with C# 13 or later.";

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary,
		Remarks = SetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>
		StartsWithCore<TItem>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		ItemEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				SubjectEqualityOptions<TItem, TItem> itemOptions = new(options, () => options.HasDefaultMatchType);
				StartsWithConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, ExpectedType = "string",
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	internal static StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>
		StartsWithForStringsCore(
			IThat<IEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IEnumerable<string?>, IThat<IEnumerable<string?>?>>(
			expectationBuilder.AddConstraint<IEnumerable<string?>?>((it, grammars) =>
			{
				SubjectEqualityOptions<string?, string?> itemOptions =
					new(options, () => options.ComparesByOrdinalEquality);
				StartsWithConstraint<string?, string?> constraint = new(expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary, Remarks = SetComparerRemarks)]
	internal static ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>
		StartsWithWithToleranceCore<TItem, TTolerance>(
			IThat<IEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string? expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IEnumerable<TItem>, IThat<IEnumerable<TItem>?>, TItem, TTolerance>(
			expectationBuilder.AddConstraint<IEnumerable<TItem>?>((it, grammars) =>
			{
				SubjectEqualityOptions<TItem, TItem> itemOptions = new(options,
					() => ObjectEqualityWithToleranceOptionsFactory.HasDefaultMatchType(options));
				StartsWithConstraint<TItem, TItem> constraint = new(expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Priority = -1, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary,
		Remarks = UntypedSetComparerRemarks)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, Priority = -2, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary,
		Remarks = LowerPriorityRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, TItem>
		StartsWithForEnumerableCore<TItem>(
			IThat<IEnumerable?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		ItemEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, TItem>(
			expectationBuilder.AddConstraint<IEnumerable?>((it, grammars) =>
			{
				SubjectEqualityOptions<TItem, TItem> itemOptions = new(options, () => options.HasDefaultMatchType);
				StartsWithForEnumerableConstraint<IEnumerable, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Priority = -1, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary,
		Remarks = UntypedCollectionRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, object?>
		StartsWithForObjectsCore(
			IThat<IEnumerable?> subject,
			IEnumerable expected,
			string? expectedExpression,
			bool negated)
		=> StartsWithForEnumerableCore<object?>(subject, expected?.Cast<object?>()!, expectedExpression, negated);

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Priority = -1, ExpectedType = "string?",
		Summary = "Verifies that the collection starts with the provided <paramref name=\"expected\" /> value.",
		NegatedSummary =
			"Verifies that the collection does not start with the provided <paramref name=\"unexpected\" /> value.",
		Remarks = SingleValueRemarks + "\n" + UntypedSetComparerRemarks)]
	internal static ObjectEqualityResult<IEnumerable, IThat<IEnumerable?>, string?>
		StartsWithSingleStringCore(
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
				StartsWithForEnumerableConstraint<IEnumerable, string?> constraint = new(
					expectationBuilder, it, grammars,
					Formatter.Format(expectedItems), expectedItems, itemOptions, itemOptions.UseComparerOf);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true, Params = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>
		StartsWithForCollectionCore<TCollection, TItem>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		expected.ThrowIfNullOrEmpty(negated);
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<TCollection, IThat<TCollection>, TItem>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				StartsWithForEnumerableConstraint<TCollection, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true, Params = true,
		ExpectedType = "string",
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static StringEqualityTypeResult<TCollection, IThat<TCollection>>
		StartsWithForCollectionStringsCore<TCollection>(
			IThat<TCollection> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		expected.ThrowIfNullOrEmpty(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<TCollection, IThat<TCollection>>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				StartsWithForEnumerableConstraint<TCollection, string?> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}


	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", PerSubject = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>
		StartsWithWithToleranceForCollectionCore<TCollection, TItem, TTolerance>(
			IThat<TCollection> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string? expectedExpression,
			bool negated)
		where TCollection : IEnumerable
	{
		expected.ThrowIfNullOrEmpty(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<TCollection, IThat<TCollection>, TItem, TTolerance>(
			expectationBuilder.AddConstraint<TCollection?>((it, grammars) =>
			{
				StartsWithForEnumerableConstraint<TCollection, TItem> constraint = new(
					expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	private sealed class StartsWithConstraint<TItem, TMatch>
		: ConstraintResult.WithNotNullValue<IEnumerable<TItem>?>,
			IAsyncContextConstraint<IEnumerable<TItem>?>
		where TItem : TMatch
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly TItem[] _expected;
		private readonly string _expectedExpression;
		private readonly string _it;
		private readonly IOptionsEquality<TMatch> _options;
		private readonly Func<object?, bool>? _useComparerOf;
		private TItem? _firstMismatchItem;
		private bool _foundMismatch;
		private int _index;

		public StartsWithConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			string expectedExpression,
			TItem[] expected,
			IOptionsEquality<TMatch> options,
			Func<object?, bool>? useComparerOf = null) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_expectedExpression = expectedExpression;
			_expected = expected;
			_options = options;
			_useComparerOf = useComparerOf;
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

			_useComparerOf?.Invoke(actual);
			if (_expected.Length == 0)
			{
				Outcome = Outcome.Success;
				return this;
			}

			IEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedEnumerable<TItem, IEnumerable<TItem>>(actual);
			_index = 0;
			foreach (TItem item in materializedEnumerable)
			{
				TItem expectedItem = _expected[_index++];
				if (!await _options.AreConsideredEqual(item, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_expectationBuilder.AddCollectionContext(materializedEnumerable,
						materializedEnumerable.ExceedsFormatterLimit());
					Outcome = Outcome.Failure;
					return this;
				}

				if (_expected.Length == _index)
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			_expectationBuilder.AddCollectionContext(materializedEnumerable);
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("starts with ", "start with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_foundMismatch)
			{
				stringBuilder.Append(_it).Append(" contained item ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index - 1).Append(" instead of ");
				stringBuilder.AppendExpectedItem(_expected[_index - 1], _options);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").AppendItemCount(_index).Append(" and lacked ")
					.AppendItemCount(_expected.Length - _index).Append(": ");
				Formatter.Format(stringBuilder, _expected.Skip(_index), FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not start with ", "do not start with "))
				.Append(_expectedExpression);
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
				stringBuilder.Append(_it).Append(" did start with ");
				Formatter.Format(stringBuilder, Actual?.Take(_index), FormattingOptions.MultipleLines);
			}
		}
	}

	private sealed class StartsWithForEnumerableConstraint<TEnumerable, TMatch>
		: ConstraintResult.WithNotNullValue<TEnumerable?>,
			IAsyncContextConstraint<TEnumerable?>
		where TEnumerable : IEnumerable
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly TMatch[] _expected;
		private readonly string _expectedExpression;
		private readonly string _it;
		private readonly IOptionsEquality<TMatch> _options;
		private readonly Func<object?, bool>? _useComparerOf;
		private object? _firstMismatchItem;
		private bool _foundMismatch;
		private int _index;

		public StartsWithForEnumerableConstraint(
			ExpectationBuilder expectationBuilder,
			string it,
			ExpectationGrammars grammars,
			string expectedExpression,
			TMatch[] expected,
			IOptionsEquality<TMatch> options,
			Func<object?, bool>? useComparerOf = null) : base(it, grammars)
		{
			_expectationBuilder = expectationBuilder;
			_it = it;
			_expectedExpression = expectedExpression;
			_expected = expected;
			_options = options;
			_useComparerOf = useComparerOf;
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

			_useComparerOf?.Invoke(actual);
			if (_expected.Length == 0)
			{
				Outcome = Outcome.Success;
				return this;
			}

			IEnumerable materializedEnumerable = context.UseMaterializedEnumerable(actual);
			_index = 0;
			foreach (object? item in materializedEnumerable)
			{
				object? expectedItem = _expected[_index++];
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

				if (_expected.Length == _index)
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			_expectationBuilder.AddCollectionContext(materializedEnumerable);
			Outcome = Outcome.Failure;
			return this;
		}

		protected override void AppendNormalExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("starts with ", "start with ")).Append(_expectedExpression);
			stringBuilder.Append(_options);
		}

		protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_foundMismatch)
			{
				stringBuilder.Append(_it).Append(" contained item ");
				Formatter.Format(stringBuilder, _firstMismatchItem);
				stringBuilder.Append(" at index ").Append(_index - 1).Append(" instead of ");
				stringBuilder.AppendExpectedItem(_expected[_index - 1], _options);
			}
			else
			{
				stringBuilder.Append(_it).Append(" contained only ").AppendItemCount(_index).Append(" and lacked ")
					.AppendItemCount(_expected.Length - _index).Append(": ");
				Formatter.Format(stringBuilder, _expected.Skip(_index), FormattingOptions.MultipleLines);
			}
		}

		protected override void AppendNegatedExpectation(StringBuilder stringBuilder, string? indentation = null)
		{
			stringBuilder.Append(Grammars.Verb("does not start with ", "do not start with "))
				.Append(_expectedExpression);
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
				stringBuilder.Append(_it).Append(" did start with ");
				Formatter.Format(stringBuilder, Actual?.Cast<object?>().Take(_index), FormattingOptions.MultipleLines);
			}
		}
	}
}
