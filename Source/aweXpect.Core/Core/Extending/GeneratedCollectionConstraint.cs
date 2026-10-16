using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;
using aweXpect.Options;

namespace aweXpect.Core.Extending;

/// <summary>
///     The base class of the constraints that <see cref="GenerateCollectionExpectationAttribute" /> generates for a
///     collection that is enumerated synchronously, which classify every item with <see cref="IsMatch" />.
/// </summary>
/// <remarks>
///     A collection whose number of items is known is read to its end, so that the result counts all items, e.g. "but
///     only 2 of 5 were". Any other collection is only read until the outcome is determined.
/// </remarks>
/// <typeparam name="TCollection">The type of the collection.</typeparam>
/// <typeparam name="TItem">The type of the items in the collection.</typeparam>
/// <param name="it">The name of the subject.</param>
/// <param name="grammars">The grammars of the expectation.</param>
/// <param name="quantifier">The quantifier for the items.</param>
public abstract class GeneratedCollectionConstraint<TCollection, TItem>(
	string it,
	ExpectationGrammars grammars,
	EnumerableQuantifier quantifier)
	: QuantifiedCollectionConstraintBase<TCollection, TItem>(it, grammars, quantifier),
		IAsyncContextConstraint<TCollection>
	where TCollection : IEnumerable<TItem>?
{
	private static readonly bool IsImmutableArray = typeof(TCollection).IsValueType &&
	                                                typeof(TCollection).IsGenericType &&
	                                                typeof(TCollection).GetGenericTypeDefinition().FullName ==
	                                                "System.Collections.Immutable.ImmutableArray`1";

	private IEnumerable<TItem>? _collection;
	private bool _isDefaultImmutableArray;

	/// <inheritdoc cref="IAsyncContextConstraint{TValue}.IsMetBy(TValue, IEvaluationContext, CancellationToken)" />
	/// <remarks>
	///     When <see cref="IsMatch" /> throws for an item after the ones that determine the outcome, the reading ends
	///     there, so that the outcome is the same as for a collection that is read only until the outcome is determined.
	/// </remarks>
	public async ValueTask<ConstraintResult> IsMetBy(TCollection actual, IEvaluationContext context,
		CancellationToken cancellationToken)
	{
		StartEvaluation();
		Actual = actual;
		_collection = null;
		_isDefaultImmutableArray = false;
		if (actual is null)
		{
			return this;
		}

		if (IsImmutableArray && IsDefault((ICollection)actual))
		{
			_isDefaultImmutableArray = true;
			Outcome = Outcome.FailureBothWays;
			return this;
		}

		IEnumerable<TItem> items = context.UseMaterializedEnumerable<TItem>(actual);
		_collection = items;
		bool readsAllItems = actual is ICollection<TItem> or IReadOnlyCollection<TItem>;
		bool hasItems = false;
		foreach (TItem item in items)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				Outcome = Outcome.Undecided;
				return this;
			}

			bool isMatch;
			try
			{
				isMatch = await IsMatch(item, cancellationToken);
			}
			catch (Exception) when (hasItems && IsDetermined)
			{
				CompleteEarly();
				return this;
			}

			hasItems = true;
			Record(item, isMatch);
			if (!readsAllItems && IsDetermined)
			{
				CompleteEarly();
				return this;
			}
		}

		Complete();
		return this;
	}

	/// <summary>
	///     Whether the <paramref name="item" /> matches the expectation.
	/// </summary>
	protected abstract ValueTask<bool> IsMatch(TItem item, CancellationToken cancellationToken);

	/// <inheritdoc />
	public override void AppendContexts(ResultContextCollector contexts)
	{
		contexts.AddCollectionContext(_collection);
		base.AppendContexts(contexts);
	}

	/// <inheritdoc />
	protected override void AppendNormalResult(StringBuilder stringBuilder, string? indentation = null)
	{
		if (_isDefaultImmutableArray)
		{
			stringBuilder.ItWasNull(It, Grammars);
		}
		else
		{
			base.AppendNormalResult(stringBuilder, indentation);
		}
	}

	/// <inheritdoc />
	protected override void AppendNegatedResult(StringBuilder stringBuilder, string? indentation = null)
		=> AppendNormalResult(stringBuilder, indentation);

	/// <summary>
	///     A default <c>ImmutableArray&lt;T&gt;</c> is the struct's version of a <see langword="null" /> collection, and
	///     its count is the cheapest member that tells it apart, as it throws only for a default one.
	/// </summary>
	private static bool IsDefault(ICollection immutableArray)
	{
		try
		{
			_ = immutableArray.Count;
			return false;
		}
		catch (InvalidOperationException)
		{
			return true;
		}
	}
}
