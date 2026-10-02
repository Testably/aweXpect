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
	private const string StartsWithSummary =
		"Verifies that the collection starts with the provided <paramref name=\"expected\" /> collection.";

	private const string DoesNotStartWithSummary =
		"Verifies that the collection does not start with the provided <paramref name=\"unexpected\" /> collection.";

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		StartsWithCore<TItem>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<TItem>?>((it, grammars) =>
			{
				StartsWithConstraint<TItem, TItem> constraint = new(it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory),
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Factory = typeof(ObjectEqualityWithToleranceOptionsFactory), Params = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>
		StartsWithWithToleranceCore<TItem, TTolerance>(
			IThat<IAsyncEnumerable<TItem>?> subject,
			IEnumerable<TItem> expected,
			ObjectEqualityWithToleranceOptions<TItem, TTolerance> options,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<TItem> expectedValues = expected.ToNonEmptyValues(negated);
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new ObjectEqualityWithToleranceResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem,
			TTolerance>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<TItem>?>((it, grammars) =>
			{
				StartsWithConstraint<TItem, TItem> constraint = new(it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	[CreateExpectationFamily("StartsWith", NegatedName = "DoesNotStartWith", GuaranteesNotNull = true,
		Params = true, ExpectedType = "string?",
		Summary = StartsWithSummary, NegatedSummary = DoesNotStartWithSummary)]
	internal static StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		StartsWithForStringsCore(
			IThat<IAsyncEnumerable<string?>?> subject,
			IEnumerable<string?> expected,
			string? expectedExpression,
			bool negated)
	{
		IEnumerable<string?> expectedValues = expected.ToNonEmptyValues(negated);
		StringEqualityOptions options = new(negated ? "unexpected" : "expected");
		ExpectationBuilder expectationBuilder = subject.Get().ExpectationBuilder;
		return new StringEqualityTypeResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint<IAsyncEnumerable<string?>?>((it, grammars) =>
			{
				StartsWithConstraint<string?, string?> constraint = new(it, grammars,
					expectedExpression?.TrimCommonWhiteSpace() ?? Formatter.Format(expectedValues),
					expectedValues.ToArray(), options);
				return negated ? constraint.Invert() : constraint;
			}),
			subject,
			options);
	}

	private sealed class StartsWithConstraint<TItem, TMatch>
		: ConstraintResult.WithNotNullValue<IAsyncEnumerable<TItem>?>,
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
		where TItem : TMatch
	{
		private CollectionContext _collectionContext;
		private readonly TItem[] _expected;
		private readonly string _expectedExpression;
		private readonly List<TItem> _foundValues = [];
		private readonly string _it;
		private readonly IOptionsEquality<TMatch> _options;
		private TItem? _firstMismatchItem;
		private bool _foundMismatch;
		private int _index;

		/// <inheritdoc />
		public override void AppendContexts(ResultContextCollector contexts)
			=> _collectionContext.AppendTo(contexts);

		public StartsWithConstraint(
			string it,
			ExpectationGrammars grammars,
			string expectedExpression,
			TItem[] expected,
			IOptionsEquality<TMatch> options) : base(it, grammars)
		{
			_it = it;
			_expectedExpression = expectedExpression;
			_expected = expected;
			_options = options;
		}

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_collectionContext = default;
			Actual = actual;
			_foundValues.Clear();
			_foundMismatch = false;
			_index = 0;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem>(actual, cancellationToken);
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

			await foreach (TItem item in materializedEnumerable.UntilCancelled(cancellationToken))
			{
				TItem expectedItem = _expected[_index++];
				if (!await _options.AreConsideredEqual(item, expectedItem))
				{
					_firstMismatchItem = item;
					_foundMismatch = true;
					_collectionContext.Set(
						materializedEnumerable as IMaterializedAsyncEnumerable<TItem>);
					Outcome = Outcome.Failure;
					return this;
				}

				_foundValues.Add(item);
				if (_expected.Length == _index)
				{
					Outcome = Outcome.Success;
					return this;
				}
			}

			if (cancellationToken.IsCanceledBeforeTheEndOf(materializedEnumerable))
			{
				Outcome = Outcome.Undecided;
				_collectionContext.Set(
					materializedEnumerable as IMaterializedAsyncEnumerable<TItem>, true);
				return this;
			}

			_collectionContext.Set(materializedEnumerable as IMaterializedAsyncEnumerable<TItem>);
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
				Formatter.Format(stringBuilder, _foundValues, FormattingOptions.MultipleLines);
			}
			else
			{
				stringBuilder.Append(_it).Append(" did start with ");
				Formatter.Format(stringBuilder, _foundValues, typeof(TItem).GetFormattingOption(_foundValues.Count));
			}
		}
	}
}
#endif
