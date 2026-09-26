#if NET8_0_OR_GREATER
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Customization;
using aweXpect.Helpers;
using aweXpect.Options;
using aweXpect.Results;
using aweXpect.SourceGenerators;

// ReSharper disable PossibleMultipleEnumeration

namespace aweXpect;

public static partial class ThatAsyncEnumerable
{
	private const string EndsWithSummary =
		"Verifies that the collection ends with the provided <paramref name=\"expected\" /> collection.";

	private const string DoesNotEndWithSummary =
		"Verifies that the collection does not end with the provided <paramref name=\"unexpected\" /> collection.";

	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	[CreateCollectionExpectation("EndsWith", NegatedName = "DoesNotEndWith", GuaranteesNotNull = true,
		Params = true, Summary = EndsWithSummary, NegatedSummary = DoesNotEndWithSummary)]
	internal static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		EndsWithCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<TItem>?>((it, grammars) =>
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
	internal static StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		EndsWithForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
	{
		expected.ThrowIfNullOrEmpty(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<string?>?>((it, grammars) =>
			{
				EndsWithConstraint<string?, string?> constraint = new(expectationBuilder, it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expected),
					expected.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	private sealed class EndsWithConstraint<TItem, TMatch>
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>,
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TItem : TMatch
	{
		private readonly ExpectationBuilder _expectationBuilder;
		private readonly TItem[] _expected;
		private readonly string _expectedExpression;
		private readonly List<TItem> _foundValues = [];
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

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			Actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual, cancellationToken);
			if (_expected.Length == 0)
			{
				int maximumNumberOfCollectionItems =
					Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
				await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
				{
					_foundValues.Add(item);
					if (_foundValues.Count == maximumNumberOfCollectionItems)
					{
						break;
					}
				}

				Outcome = Outcome.Success;
				return this;
			}

			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				_foundValues.Add(item);
			}

			_itemsCount = _foundValues.Count;
			_offset = _itemsCount - _expected.Length;
			for (_index = _expected.Length - 1; _index >= 0; _index--)
			{
				if (_index + _offset < 0)
				{
					Outcome = Outcome.Failure;
					await _expectationBuilder.AddCollectionContext(
						materializedEnumerable as IMaterializedEnumerable<TItem>);
					return this;
				}

				TItem item = _foundValues[_index + _offset];
				TItem expectedItem = _expected[_index];
				if (!await _options.AreConsideredEqual(item, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					await _expectationBuilder.AddCollectionContext(
						materializedEnumerable as IMaterializedEnumerable<TItem>);
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
				Formatter.Format(stringBuilder, _expected[_index]);
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
				Formatter.Format(stringBuilder, _foundValues, FormattingOptions.MultipleLines);
			}
			else
			{
				stringBuilder.Append(_it).Append(" did end with ");
				Formatter.Format(stringBuilder, _foundValues, FormattingOptions.MultipleLines);
			}
		}
	}
}
#endif
