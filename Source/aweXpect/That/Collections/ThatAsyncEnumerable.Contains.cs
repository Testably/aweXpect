#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
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

public static partial class ThatAsyncEnumerable
{
	/// <summary>
	///     Verifies that the collection contains the <paramref name="expected" /> value.
	/// </summary>
	public static ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			TItem expected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(expected)}{options}"
						: $"{g.Verb("contains", "contain")} {Formatter.Format(expected)}{options} {q}",
					a => options.AreConsideredEqual(a, expected),
					quantifier)),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains the <paramref name="expected" /> value.
	/// </summary>
	public static StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		Contains(
			this IThat<IAsyncEnumerable<string?>?> source,
			string? expected)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<string?>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(expected)}{options}"
						: $"{g.Verb("contains", "contain")} {Formatter.Format(expected)}{options} {q}",
					a => options.AreConsideredEqual(a, expected),
					quantifier)),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains an item that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainConstraint<TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"
						: $"{g.Verb("contains", "contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} {q}",
					predicate,
					quantifier)),
			source,
			quantifier);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	public static ObjectCollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			IEnumerable<TItem> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(), expected,
					options, matchOptions)),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection.
	/// </summary>
	public static StringCollectionContainResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		Contains(
			this IThat<IAsyncEnumerable<string?>?> source,
			IEnumerable<string?> expected,
			[CallerArgumentExpression("expected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringCollectionContainResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(
					expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected, options, matchOptions)),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection of predicates.
	/// </summary>
	public static CollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			IEnumerable<Expression<Func<TItem, bool>>> expected,
			[CallerArgumentExpression("expected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions)),
			source,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection contains the provided <paramref name="expected" /> collection of expectations.
	/// </summary>
	public static CollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		Contains<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			IEnumerable<Action<IThatSubject<TItem?>>> expected,
			[CallerArgumentExpression("expected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					expected,
					matchOptions)),
			source,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	public static ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			TItem unexpected)
	{
		Quantifier quantifier = new();
		ObjectEqualityOptions<TItem> options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options}"
						: $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options} {q.ToNegatedString()}",
					a => options.AreConsideredEqual(a, unexpected),
					quantifier).Invert()),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection does not contain the <paramref name="unexpected" /> value.
	/// </summary>
	public static StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		DoesNotContain(
			this IThat<IAsyncEnumerable<string?>?> source,
			string? unexpected)
	{
		Quantifier quantifier = new();
		StringEqualityOptions options = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringEqualityTypeCountResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new AsyncContainConstraint<string?>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options}"
						: $"{g.Verb("does not contain", "do not contain")} {Formatter.Format(unexpected)}{options} {q.ToNegatedString()}",
					a => options.AreConsideredEqual(a, unexpected),
					quantifier).Invert()),
			source,
			quantifier,
			options);
	}

	/// <summary>
	///     Verifies that the collection contains no item that satisfies the <paramref name="predicate" />.
	/// </summary>
	public static CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			Func<TItem, bool> predicate,
			[CallerArgumentExpression("predicate")]
			string doNotPopulateThisValue = "")
	{
		predicate.ThrowIfNull();
		Quantifier quantifier = new();
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CountResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new ContainConstraint<TItem>(
					expectationBuilder, it, grammars,
					(q, g) => q.IsNever
						? $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()}"
						: $"{g.Verb("does not contain", "do not contain")} item matching {doNotPopulateThisValue.TrimCommonWhiteSpace()} {q.ToNegatedString()}",
					predicate,
					quantifier).Invert()),
			source,
			quantifier);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static ObjectCollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			IEnumerable<TItem> unexpected,
			[CallerArgumentExpression("unexpected")] string doNotPopulateThisValue = "")
	{
		ObjectEqualityOptions<TItem> options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new ObjectCollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<TItem, TItem>(
					expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(), unexpected,
					options, matchOptions).Invert()),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection.
	/// </summary>
	public static StringCollectionContainResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>
		DoesNotContain(
			this IThat<IAsyncEnumerable<string?>?> source,
			IEnumerable<string?> unexpected,
			[CallerArgumentExpression("unexpected")] string doNotPopulateThisValue = "")
	{
		StringEqualityOptions options = new();
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new StringCollectionContainResult<IAsyncEnumerable<string?>, IThat<IAsyncEnumerable<string?>?>>(
			expectationBuilder.AddConstraint((it, grammars) =>
				new IsEqualToConstraint<string?, string?>(
					expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected, options, matchOptions).Invert()),
			source,
			options,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection of predicates.
	/// </summary>
	public static CollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			IEnumerable<Expression<Func<TItem, bool>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromPredicateConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions).Invert()),
			source,
			matchOptions);
	}

	/// <summary>
	///     Verifies that the collection does not contain the provided <paramref name="unexpected" /> collection of expectations.
	/// </summary>
	public static CollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>
		DoesNotContain<TItem>(
			this IThat<IAsyncEnumerable<TItem>?> source,
			IEnumerable<Action<IThatSubject<TItem?>>> unexpected,
			[CallerArgumentExpression("unexpected")]
			string doNotPopulateThisValue = "")
	{
		CollectionMatchOptions matchOptions = new(CollectionMatchOptions.EquivalenceRelations.Contains);
		ExpectationBuilder expectationBuilder = source.Get().ExpectationBuilder;
		return new CollectionContainResult<IAsyncEnumerable<TItem>, IThat<IAsyncEnumerable<TItem>?>, TItem>(
			expectationBuilder.AddConstraint((it, grammars)
				=> new IsEqualToFromExpectationsConstraint<TItem, TItem>(expectationBuilder, it, grammars,
					doNotPopulateThisValue.TrimCommonWhiteSpace(),
					unexpected,
					matchOptions).Invert()),
			source,
			matchOptions);
	}

	private sealed class ContainConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
		Func<TItem, bool> predicate,
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private readonly List<TItem> _items = [];
		private IAsyncEnumerable<TItem>? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			_count = 0;
			bool isFailed = false;
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (_items.Count <= maximumNumberOfCollectionItems)
				{
					_items.Add(item);
				}

				if (predicate(item))
				{
					_count++;
					bool? check = quantifier.Check(_count, false);
					if (check == false)
					{
						isFailed = true;
					}

					if (check == true)
					{
						Outcome = Outcome.Success;
						return this;
					}
				}

				if (_items.Count > maximumNumberOfCollectionItems && isFailed)
				{
					Outcome = Outcome.Failure;
					expectationBuilder.AddCollectionContext(_items, true);
					return this;
				}
			}

			expectationBuilder.AddCollectionContext(_items);
			if (quantifier.Check(_count, true) ?? _isNegated)
			{
				Outcome = Outcome.Success;
				return this;
			}

			_isFinished = true;
			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectationText.Invoke(quantifier, Grammars));

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_isFinished)
			{
				if (_count == 0)
				{
					stringBuilder.Append(it).Append(" did not contain it");
				}
				else if (_count == 1)
				{
					stringBuilder.Append(it).Append(" contained it once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append(it).Append(" contained it twice");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained it ").Append(_count).Append(" times");
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" contained it at least ");
				if (_count == 1)
				{
					stringBuilder.Append("once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append("twice");
				}
				else
				{
					stringBuilder.Append(_count).Append(" times");
				}
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IAsyncEnumerable<TItem>));
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			quantifier.Negate();
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}

	private sealed class AsyncContainConstraint<TItem>(
		ExpectationBuilder expectationBuilder,
		string it,
		ExpectationGrammars grammars,
		Func<Quantifier, ExpectationGrammars, string> expectationText,
#if NET8_0_OR_GREATER
		Func<TItem, ValueTask<bool>> predicate,
#else
		Func<TItem, Task<bool>> predicate,
#endif
		Quantifier quantifier)
		: ConstraintResult(grammars),
			IAsyncContextConstraint<IAsyncEnumerable<TItem>?>
	{
		private readonly List<TItem> _items = [];
		private IAsyncEnumerable<TItem>? _actual;
		private int _count;
		private bool _isFinished;
		private bool _isNegated;

		public async Task<ConstraintResult> IsMetBy(IAsyncEnumerable<TItem>? actual, IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_actual = actual;
			if (actual is null)
			{
				Outcome = Outcome.Failure;
				return this;
			}

			IAsyncEnumerable<TItem> materializedEnumerable =
				context.UseMaterializedAsyncEnumerable<TItem, IAsyncEnumerable<TItem>>(actual);
			int maximumNumberOfCollectionItems =
				Customize.aweXpect.Formatting().MaximumNumberOfCollectionItems.Get();
			_count = 0;
			bool isFailed = false;
			await foreach (TItem item in materializedEnumerable.WithCancellation(cancellationToken))
			{
				if (_items.Count <= maximumNumberOfCollectionItems)
				{
					_items.Add(item);
				}

				if (await predicate(item))
				{
					_count++;
					bool? check = quantifier.Check(_count, false);
					if (check == false)
					{
						isFailed = true;
					}

					if (check == true)
					{
						Outcome = Outcome.Success;
						return this;
					}
				}

				if (_items.Count > maximumNumberOfCollectionItems && isFailed)
				{
					Outcome = Outcome.Failure;
					expectationBuilder.AddCollectionContext(_items, true);
					return this;
				}
			}

			expectationBuilder.AddCollectionContext(_items);
			if (quantifier.Check(_count, true) ?? _isNegated)
			{
				Outcome = Outcome.Success;
				return this;
			}

			_isFinished = true;
			Outcome = Outcome.Failure;
			return this;
		}

		public override void AppendExpectation(StringBuilder stringBuilder, string? indentation = null)
			=> stringBuilder.Append(expectationText.Invoke(quantifier, Grammars));

		public override void AppendResult(StringBuilder stringBuilder, string? indentation = null)
		{
			if (_actual == null)
			{
				stringBuilder.ItWasNull(it, Grammars);
			}
			else if (_isFinished)
			{
				if (_count == 0)
				{
					stringBuilder.Append(it).Append(" did not contain it");
				}
				else if (_count == 1)
				{
					stringBuilder.Append(it).Append(" contained it once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append(it).Append(" contained it twice");
				}
				else
				{
					stringBuilder.Append(it).Append(" contained it ").Append(_count).Append(" times");
				}
			}
			else
			{
				stringBuilder.Append(it).Append(" contained it at least ");
				if (_count == 1)
				{
					stringBuilder.Append("once");
				}
				else if (_count == 2)
				{
					stringBuilder.Append("twice");
				}
				else
				{
					stringBuilder.Append(_count).Append(" times");
				}
			}
		}

		/// <inheritdoc cref="ConstraintResult.TryGetValue{TValue}(out TValue)" />
		public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue? value) where TValue : default
		{
			if (_actual is TValue typedValue)
			{
				value = typedValue;
				return true;
			}

			value = default;
			return typeof(TValue).IsAssignableFrom(typeof(IAsyncEnumerable<TItem>));
		}

		public override ConstraintResult Negate()
		{
			_isNegated = !_isNegated;
			quantifier.Negate();
			Outcome = Outcome switch
			{
				Outcome.Failure => Outcome.Success,
				Outcome.Success => Outcome.Failure,
				_ => Outcome,
			};
			return this;
		}
	}
}
#endif
