using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Options;

/// <summary>
///     Options for matching a collection.
/// </summary>
public partial class CollectionMatchOptions(
	CollectionMatchOptions.EquivalenceRelations equivalenceRelations
		= CollectionMatchOptions.EquivalenceRelations.Equivalent)
{
#pragma warning disable S4070 // Non-flags enums should not be marked with "FlagsAttribute"
	/// <summary>
	///     Specifies the equivalence relation between subject and expected.
	/// </summary>
	[Flags]
	public enum EquivalenceRelations
	{
		/// <summary>
		///     The subject and expected collection must be equivalent (have the same items)
		/// </summary>
		Equivalent = 1,

		/// <summary>
		///     The subject collection is contained in the expected collection which has at least one additional item.
		/// </summary>
		IsContainedInProperly = 2 | IsContainedIn,

		/// <summary>
		///     The subject collection contains the expected collection and at least one additional item.
		/// </summary>
		ContainsProperly = 2 | Contains,

		/// <summary>
		///     The subject collection is contained in the expected collection.
		/// </summary>
		IsContainedIn = 4,

		/// <summary>
		///     The subject collection contains the expected collection.
		/// </summary>
		Contains = 8,
	}
#pragma warning restore S4070

	private EquivalenceRelations _equivalenceRelations = equivalenceRelations;
	private bool _ignoringDuplicates;
	private bool _ignoringInterspersedItems;
	private bool _inAnyOrder;

	/// <summary>
	///     Specifies the equivalence relation between subject and expected.
	/// </summary>
	public void SetEquivalenceRelation(EquivalenceRelations equivalenceRelation)
		=> _equivalenceRelations = equivalenceRelation;

	/// <summary>
	///     Ignores the order in the subject and expected values.
	/// </summary>
	public void InAnyOrder() => _inAnyOrder = true;

	/// <summary>
	///     Ignores duplicates in both collections.
	/// </summary>
	public void IgnoringDuplicates() => _ignoringDuplicates = true;

	/// <summary>
	///     Ignores interspersed items in the actual collection.
	/// </summary>
	public void IgnoringInterspersedItems() => _ignoringInterspersedItems = true;

	/// <summary>
	///     Get the collection matcher for the <paramref name="expected" /> enumerable.
	/// </summary>
	public ICollectionMatcher<T, T2> GetCollectionMatcher<T, T2>(IEnumerable<T> expected)
		where T : T2
		=> (_inAnyOrder, _ignoringDuplicates) switch
		{
			(true, true) => new AnyOrderIgnoreDuplicatesCollectionMatcher<T, T2>(_equivalenceRelations, expected),
			(true, false) => new AnyOrderCollectionMatcher<T, T2>(_equivalenceRelations, expected),
			(false, true) => new SameOrderIgnoreDuplicatesCollectionMatcher<T, T2>(_equivalenceRelations, expected,
				_ignoringInterspersedItems),
			(false, false) => new SameOrderCollectionMatcher<T, T2>(_equivalenceRelations, expected,
				_ignoringInterspersedItems),
		};

	/// <summary>
	///     Get the collection matcher for the <paramref name="expected" /> enumerable of predicates.
	/// </summary>
	public ICollectionMatcher<T, T2> GetCollectionMatcher<T, T2>(IEnumerable<Expression<Func<T, bool>>> expected)
		where T : T2
		=> (_inAnyOrder, _ignoringDuplicates) switch
		{
			(true, true) => new AnyOrderIgnoreDuplicatesFromPredicateCollectionMatcher<T, T2>(_equivalenceRelations,
				expected),
			(true, false) => new AnyOrderFromPredicateCollectionMatcher<T, T2>(_equivalenceRelations, expected),
			(false, true) => new SameOrderIgnoreDuplicatesFromPredicateCollectionMatcher<T, T2>(_equivalenceRelations,
				expected,
				_ignoringInterspersedItems),
			(false, false) => new SameOrderFromPredicateCollectionMatcher<T, T2>(_equivalenceRelations, expected,
				_ignoringInterspersedItems),
		};

	/// <summary>
	///     Get the collection matcher for the <paramref name="expected" /> enumerable of predicates.
	/// </summary>
	public ICollectionMatcher<T, T2> GetCollectionMatcher<T, T2>(IEnumerable<ExpectationItem<T>> expected)
		where T : T2
		=> (_inAnyOrder, _ignoringDuplicates) switch
		{
			(true, true) => new AnyOrderIgnoreDuplicatesFromExpectationCollectionMatcher<T, T2>(_equivalenceRelations,
				expected),
			(true, false) => new AnyOrderFromExpectationCollectionMatcher<T, T2>(_equivalenceRelations, expected),
			(false, true) => new SameOrderIgnoreDuplicatesFromExpectationCollectionMatcher<T, T2>(_equivalenceRelations,
				expected,
				_ignoringInterspersedItems),
			(false, false) => new SameOrderFromExpectationCollectionMatcher<T, T2>(_equivalenceRelations, expected,
				_ignoringInterspersedItems),
		};

	/// <summary>
	///     Specifies the expectation for the <paramref name="expectedExpression" /> using the provided
	///     <paramref name="grammars" />.
	/// </summary>
	public string GetExpectation(string expectedExpression, ExpectationGrammars grammars)
		=> (_inAnyOrder, _ignoringDuplicates, _ignoringInterspersedItems) switch
		{
			(true, true, _) => GetString(_equivalenceRelations, expectedExpression, grammars) +
			                   " in any order ignoring duplicates",
			(true, false, _) => GetString(_equivalenceRelations, expectedExpression, grammars) + " in any order",
			(false, true, false) => GetString(_equivalenceRelations, expectedExpression, grammars) +
			                        " in order ignoring duplicates",
			(false, false, false) => GetString(_equivalenceRelations, expectedExpression, grammars) + " in order",
			(false, true, true) => GetString(_equivalenceRelations, expectedExpression, grammars) +
			                       " in order ignoring duplicates and interspersed items",
			(false, false, true) => GetString(_equivalenceRelations, expectedExpression, grammars) +
			                        " in order ignoring interspersed items",
		};

	private static string GetString(EquivalenceRelations equivalenceRelation, string expectedExpression,
		ExpectationGrammars grammars)
		=> (equivalenceRelation, grammars.IsNegated()) switch
		{
			(EquivalenceRelations.Contains, false)
				=> $"{grammars.Verb("contains", "contain")} collection {expectedExpression}",
			(EquivalenceRelations.Contains, true)
				=> $"{grammars.Verb("does not contain", "do not contain")} collection {expectedExpression}",
			(EquivalenceRelations.ContainsProperly, false)
				=> $"{grammars.Verb("contains", "contain")} collection {expectedExpression} " +
				   "and at least one additional item",
			(EquivalenceRelations.ContainsProperly, true)
				=> $"{grammars.Verb("does not contain", "do not contain")} collection {expectedExpression} " +
				   "and at least one additional item",
			(EquivalenceRelations.IsContainedIn, false)
				=> $"{grammars.Verb("is", "are")} contained in collection {expectedExpression}",
			(EquivalenceRelations.IsContainedIn, true)
				=> $"{grammars.Verb("is", "are")} not contained in collection {expectedExpression}",
			(EquivalenceRelations.IsContainedInProperly, false)
				=> $"{grammars.Verb("is", "are")} contained in collection {expectedExpression} " +
				   "which has at least one additional item",
			(EquivalenceRelations.IsContainedInProperly, true)
				=> $"{grammars.Verb("is", "are")} not contained in collection {expectedExpression} " +
				   "which has at least one additional item",
			(_, false) => $"{grammars.Verb("matches", "match")} collection {expectedExpression}",
			(_, true) => $"{grammars.Verb("does not match", "do not match")} collection {expectedExpression}",
		};

	private static string? ReturnErrorString(string it, List<string> errors)
	{
		if (errors.Count > 0)
		{
			if (errors.Count > 1)
			{
				StringBuilder sb = new();
				sb.Append(it);
				foreach (string error in errors)
				{
					sb.AppendLine().Append(error.Indent()).Append(" and");
				}

				sb.Length -= 4;
				return sb.ToString();
			}

			return $"{it} {errors[0]}";
		}

		return null;
	}

	private static IEnumerable<string> AdditionalItemsError<T>(Dictionary<int, T> additionalItems)
	{
		bool hasAdditionalItems = additionalItems.Any();
		if (hasAdditionalItems)
		{
			foreach (KeyValuePair<int, T> additionalItem in additionalItems)
			{
				yield return
					$"contained item {Formatter.Format(additionalItem.Value)} at index {additionalItem.Key} that was not expected";
			}
		}
	}

	private static IEnumerable<string> IncorrectItemsError<T, TExpected>(
		Dictionary<int, (T Item, TExpected Expected)> incorrectItems)
	{
		bool hasIncorrectItems = incorrectItems.Any();
		if (hasIncorrectItems)
		{
			foreach (KeyValuePair<int, (T Item, TExpected Expected)> incorrectItem in incorrectItems)
			{
				yield return
					$"contained item {Formatter.Format(incorrectItem.Value.Item)} at index {incorrectItem.Key} instead of {Formatter.Format(incorrectItem.Value.Expected)}";
			}
		}
	}

	private static IEnumerable<string> MissingItemsError<T>(int total, List<T> missingItems,
		EquivalenceRelations equivalenceRelation, bool ignoringDuplicates)
	{
		if (total == 0)
		{
			yield break;
		}

		bool hasMissingItems = missingItems.Any();
		if (total == missingItems.Count)
		{
			yield return ignoringDuplicates switch
			{
				true => $"lacked all {total} unique expected items",
				false => $"lacked all {total} expected items",
			};
			yield break;
		}

		if (hasMissingItems && !equivalenceRelation.HasFlag(EquivalenceRelations.IsContainedIn))
		{
			if (missingItems.Count == 1)
			{
				yield return
					$"lacked {missingItems.Count} of {total} expected items: {Formatter.Format(missingItems[0])}";
				yield break;
			}

			StringBuilder sb = new();
			sb.Append("lacked ").Append(missingItems.Count).Append(" of ")
				.Append(total).Append(" expected items:");
			foreach (T missingItem in missingItems)
			{
				sb.AppendLine().Append("  ");
				Formatter.Format(sb, missingItem);
				sb.Append(',');
			}

			sb.Length--;
			yield return sb.ToString();
		}
	}

#if NET8_0_OR_GREATER
	private static async ValueTask<bool> All<T>(IEnumerable<T> items, Func<T, ValueTask<bool>> predicate,
		bool invert = false)
#else
	private static async Task<bool> All<T>(IEnumerable<T> items, Func<T, Task<bool>> predicate,
		bool invert = false)
#endif
	{
		foreach (T item in items)
		{
			if (await predicate(item) == invert)
			{
				return false;
			}
		}

		return true;
	}

#if NET8_0_OR_GREATER
	private static async ValueTask<bool> Any<T>(IEnumerable<T> items, Func<T, ValueTask<bool>> predicate,
		bool invert = false)
#else
	private static async Task<bool> Any<T>(IEnumerable<T> items, Func<T, Task<bool>> predicate,
		bool invert = false)
#endif
	{
		foreach (T item in items)
		{
			if (await predicate(item) != invert)
			{
				return true;
			}
		}

		return false;
	}

#if NET8_0_OR_GREATER
	private static async ValueTask<List<TMember>> Filter<T, TMember>(IEnumerable<T> items,
		Func<T, ValueTask<bool>> predicate, Func<T, TMember> memberSelector)
#else
	private static async Task<List<TMember>> Filter<T, TMember>(IEnumerable<T> items,
		Func<T, Task<bool>> predicate, Func<T, TMember> memberSelector)
#endif
	{
		List<TMember> list = new();
		foreach (T item in items)
		{
			if (await predicate(item))
			{
				list.Add(memberSelector(item));
			}
		}

		return list;
	}

#if NET8_0_OR_GREATER
	private static async ValueTask<T?> FirstOrDefault<T>(IEnumerable<T> items, Func<T, ValueTask<bool>> predicate)
#else
	private static async Task<T?> FirstOrDefault<T>(IEnumerable<T> items, Func<T, Task<bool>> predicate)
#endif
	{
		foreach (T item in items)
		{
			if (await predicate(item))
			{
				return item;
			}
		}

		return default;
	}

#if NET8_0_OR_GREATER
	private static async ValueTask RemoveFirst<T>(List<T> items, Func<T, ValueTask<bool>> predicate)
#else
	private static async Task RemoveFirst<T>(List<T> items, Func<T, Task<bool>> predicate)
#endif
	{
		int index = -1;
		foreach (T item in items)
		{
			index++;
			if (await predicate(item))
			{
				items.RemoveAt(index);
				break;
			}
		}
	}

	/// <summary>
	///     Element of a collection of expectations.
	/// </summary>
	public sealed class ExpectationItem<TItem>
	{
		private readonly CancellationToken _cancellationToken;
		private readonly IEvaluationContext _context;
		internal readonly ManualExpectationBuilder<TItem> ItemExpectationBuilder;

		/// <inheritdoc cref="ExpectationItem{TItem}" />
		public ExpectationItem(Action<IThatSubject<TItem?>> expectation,
			ExpectationGrammars grammars,
			IEvaluationContext context,
			CancellationToken cancellationToken)
		{
			_context = context;
			_cancellationToken = cancellationToken;
			ItemExpectationBuilder = new ManualExpectationBuilder<TItem>(null, grammars & ~ExpectationGrammars.Plural);
			expectation.Invoke(new ThatSubject<TItem?>(ItemExpectationBuilder));
		}

		/// <summary>
		///     Verifies if the <paramref name="value" /> is met by the expectation.
		/// </summary>
#if NET8_0_OR_GREATER
		public async ValueTask<bool> IsMetBy(TItem value)
#else
		public async Task<bool> IsMetBy(TItem value)
#endif
		{
			ConstraintResult result = await ItemExpectationBuilder.IsMetBy(value, _context, _cancellationToken);
			return result.Outcome == Outcome.Success;
		}

		/// <inheritdoc cref="object.Equals(object?)" />
		public override bool Equals(object? obj) => obj is ExpectationItem<TItem> other && Equals(other);

		private bool Equals(ExpectationItem<TItem> other)
			=> ItemExpectationBuilder.Equals(other.ItemExpectationBuilder);

		/// <inheritdoc cref="object.GetHashCode()" />
		public override int GetHashCode() => ItemExpectationBuilder.GetHashCode();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString() => ItemExpectationBuilder.ToString();
	}

	internal sealed class ExpectationItemEqualityComparer<TItem> : IEqualityComparer<ExpectationItem<TItem>>
	{
		public bool Equals(ExpectationItem<TItem>? x, ExpectationItem<TItem>? y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}

			if (x is null || y is null)
			{
				return false;
			}

			if (x.GetType() != y.GetType())
			{
				return false;
			}

			return x.ItemExpectationBuilder.Equals(y.ItemExpectationBuilder);
		}

		public int GetHashCode(ExpectationItem<TItem> obj) => obj.ItemExpectationBuilder.GetHashCode();
	}
}
