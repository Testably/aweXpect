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
		///     The subject and expected collection must be equivalent (have the same items).
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

	private const string ItemsMatchInADifferentOrderHint = "(but the items match in a different order)";
	private EquivalenceRelations _equivalenceRelations = equivalenceRelations;
	private bool _ignoringDuplicates;
	private bool _ignoringInterspersedItems;
	private bool _inAnyOrder;
	private bool _isIgnoringDuplicatesSpecified;
	private bool _isIgnoringInterspersedItemsSpecified;
	private bool _isProperlySpecified;

	/// <summary>
	///     The option that already specified how the order is matched.
	/// </summary>
	private string? OrderOption
	{
		get
		{
			if (_inAnyOrder)
			{
				return nameof(InAnyOrder);
			}

			return _ignoringInterspersedItems ? nameof(IgnoringInterspersedItems) : null;
		}
	}

	/// <summary>
	///     Only equality guarantees that a successful any-order match means the same items in a different order; the
	///     containment relations require the items to be contiguous, so their any-order match can succeed for other
	///     reasons.
	/// </summary>
	/// <remarks>
	///     Ignoring duplicates, the in-order matcher knows whether the items match in any order, so it adds the hint
	///     itself.
	/// </remarks>
	private bool AddsInAnyOrderHint => _equivalenceRelations == EquivalenceRelations.Equivalent;

	/// <summary>
	///     Specifies the equivalence relation between subject and expected.
	/// </summary>
	public void SetEquivalenceRelation(EquivalenceRelations equivalenceRelation)
		=> _equivalenceRelations = equivalenceRelation;

	/// <summary>
	///     Ignores the order in the subject and expected values.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     The order is already specified, e.g. interspersed items are already ignored via
	///     <see cref="IgnoringInterspersedItems(bool)" />.
	/// </exception>
	public void InAnyOrder()
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(OrderOption, nameof(InAnyOrder));
		_inAnyOrder = true;
	}

	/// <summary>
	///     Ignores duplicates in both collections,
	///     according to the <paramref name="ignoreDuplicates" /> parameter.
	/// </summary>
	/// <remarks>
	///     Only which items occur matters, not how often: every expected item has to be matched by an item, and every
	///     item has to match an expected item, as far as the relation requires it, so <c>[1, 1, 2]</c> matches
	///     <c>[1, 2]</c>.
	/// </remarks>
	/// <exception cref="InvalidOperationException">The duplicates are already specified.</exception>
	public void IgnoringDuplicates(bool ignoreDuplicates = true)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isIgnoringDuplicatesSpecified, nameof(IgnoringDuplicates));
		_isIgnoringDuplicatesSpecified = true;
		_ignoringDuplicates = ignoreDuplicates;
	}

	/// <summary>
	///     Verifies that the two collections differ by at least one additional item.
	/// </summary>
	/// <remarks>
	///     This turns <see cref="EquivalenceRelations.Contains" /> into <see cref="EquivalenceRelations.ContainsProperly" />
	///     and <see cref="EquivalenceRelations.IsContainedIn" /> into
	///     <see cref="EquivalenceRelations.IsContainedInProperly" />.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	///     The option is already specified, or the relation is neither <see cref="EquivalenceRelations.Contains" /> nor
	///     <see cref="EquivalenceRelations.IsContainedIn" />.
	/// </exception>
	public void Properly()
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isProperlySpecified, nameof(Properly));
		_equivalenceRelations = _equivalenceRelations switch
		{
			EquivalenceRelations.Contains => EquivalenceRelations.ContainsProperly,
			EquivalenceRelations.IsContainedIn => EquivalenceRelations.IsContainedInProperly,
			// ReSharper disable once LocalizableElement
			_ => throw Tracing.WriteException(new InvalidOperationException(
				$"{nameof(Properly)} requires a containment relation, but the relation is {_equivalenceRelations}.")),
		};
		_isProperlySpecified = true;
	}

	/// <summary>
	///     Ignores items that appear in between the matched items,
	///     according to the <paramref name="ignoreInterspersedItems" /> parameter.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	///     The interspersed items are already specified, or the order is already ignored via
	///     <see cref="InAnyOrder()" />.
	/// </exception>
	public void IgnoringInterspersedItems(bool ignoreInterspersedItems = true)
	{
		ThrowHelper.ThrowIfOptionIsAlreadySpecified(_isIgnoringInterspersedItemsSpecified,
			nameof(IgnoringInterspersedItems));
		if (ignoreInterspersedItems)
		{
			ThrowHelper.ThrowIfOptionIsAlreadySpecified(OrderOption, nameof(IgnoringInterspersedItems));
		}

		_isIgnoringInterspersedItemsSpecified = true;
		_ignoringInterspersedItems = ignoreInterspersedItems;
	}

	/// <summary>
	///     Get the collection matcher for the <paramref name="expected" /> enumerable.
	/// </summary>
	public ICollectionMatcher<T, T2> GetCollectionMatcher<T, T2>(IEnumerable<T> expected)
		where T : T2
		=> GetCollectionMatcher<T, T2>(expected, null);

	/// <summary>
	///     Get the collection matcher for the <paramref name="expected" /> enumerable and a subject that is an array
	///     with the <paramref name="dimensions" />.
	/// </summary>
	/// <remarks>
	///     An array of rank greater than one enumerates its items as one flat sequence with the last dimension changing
	///     first, so with the length of each of its <paramref name="dimensions" /> a failure names an item by its index
	///     in every dimension, e.g. <c>[1,1]</c>. Without more than one dimension it names an item by its position in
	///     the sequence.
	/// </remarks>
	public ICollectionMatcher<T, T2> GetCollectionMatcher<T, T2>(IEnumerable<T> expected, int[]? dimensions)
		where T : T2
	{
		if (dimensions?.Length <= 1)
		{
			dimensions = null;
		}

		return (_inAnyOrder, _ignoringDuplicates) switch
		{
			(true, true) => new AnyOrderIgnoreDuplicatesCollectionMatcher<T, T2>(_equivalenceRelations, expected,
				dimensions),
			(true, false) => new AnyOrderCollectionMatcher<T, T2>(_equivalenceRelations, expected, dimensions),
			(false, true) => new SameOrderIgnoreDuplicatesCollectionMatcher<T, T2>(_equivalenceRelations, expected,
				_ignoringInterspersedItems, dimensions),
			(false, false) => new SameOrderCollectionMatcher<T, T2>(_equivalenceRelations, expected,
				_ignoringInterspersedItems, AddsInAnyOrderHint, dimensions),
		};
	}

	/// <summary>
	///     Get the collection matcher for the <paramref name="expected" /> enumerable of predicates.
	/// </summary>
	/// <exception cref="ArgumentException">
	///     A predicate of the <paramref name="expected" /> enumerable is <see langword="null" />.
	/// </exception>
	public ICollectionMatcher<T, T2> GetCollectionMatcher<T, T2>(IEnumerable<Expression<Func<T, bool>>> expected)
		where T : T2
	{
		Expression<Func<T, bool>>[] predicates = WithoutNullElements(expected, nameof(expected));
		return (_inAnyOrder, _ignoringDuplicates) switch
		{
			(true, true) => new AnyOrderIgnoreDuplicatesFromPredicateCollectionMatcher<T, T2>(_equivalenceRelations,
				predicates),
			(true, false) => new AnyOrderFromPredicateCollectionMatcher<T, T2>(_equivalenceRelations, predicates),
			(false, true) => new SameOrderIgnoreDuplicatesFromPredicateCollectionMatcher<T, T2>(_equivalenceRelations,
				predicates, _ignoringInterspersedItems),
			(false, false) => new SameOrderFromPredicateCollectionMatcher<T, T2>(_equivalenceRelations, predicates,
				_ignoringInterspersedItems, AddsInAnyOrderHint),
		};
	}

	/// <summary>
	///     Get the collection matcher for the <paramref name="expected" /> enumerable of predicates.
	/// </summary>
	/// <exception cref="ArgumentException">
	///     An expectation of the <paramref name="expected" /> enumerable is <see langword="null" />.
	/// </exception>
	public ICollectionMatcher<T, T2> GetCollectionMatcher<T, T2>(IEnumerable<ExpectationItem<T>> expected)
		where T : T2
	{
		ExpectationItem<T>[] expectations = WithoutNullElements(expected, nameof(expected));
		return (_inAnyOrder, _ignoringDuplicates) switch
		{
			(true, true) => new AnyOrderIgnoreDuplicatesFromExpectationCollectionMatcher<T, T2>(_equivalenceRelations,
				expectations),
			(true, false) => new AnyOrderFromExpectationCollectionMatcher<T, T2>(_equivalenceRelations, expectations),
			(false, true) => new SameOrderIgnoreDuplicatesFromExpectationCollectionMatcher<T, T2>(_equivalenceRelations,
				expectations, _ignoringInterspersedItems),
			(false, false) => new SameOrderFromExpectationCollectionMatcher<T, T2>(_equivalenceRelations, expectations,
				_ignoringInterspersedItems, AddsInAnyOrderHint),
		};
	}

	/// <summary>
	///     Materializes the <paramref name="expected" /> predicates or expectations, which the matcher needs anyway,
	///     and rejects a <see langword="null" /> one, because it could not be evaluated for any item.
	/// </summary>
	private static T[] WithoutNullElements<T>(IEnumerable<T> expected, string paramName)
		where T : class
	{
		T[] elements = expected as T[] ?? expected.ToArray();
		if (elements.Any(element => element is null))
		{
			// ReSharper disable once LocalizableElement
			throw Tracing.WriteException(new ArgumentException(
				$"The '{paramName}' collection cannot contain <null>.", paramName));
		}

		return elements;
	}

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
			                        " in order" + ContiguousSuffix() + " ignoring duplicates",
			(false, false, false) => GetString(_equivalenceRelations, expectedExpression, grammars) + " in order" +
			                         ContiguousSuffix(),
			(false, true, true) => GetString(_equivalenceRelations, expectedExpression, grammars) +
			                       " in order ignoring duplicates and interspersed items",
			(false, false, true) => GetString(_equivalenceRelations, expectedExpression, grammars) +
			                        " in order ignoring interspersed items",
		};

	/// <summary>
	///     Specifies the verb of the negated result, so that it agrees with the negated expectation built by
	///     <see cref="GetExpectation" />.
	/// </summary>
	/// <remarks>
	///     Only the containment relation reads "does not contain", which is answered with "did"; the other relations
	///     read "is not" and are answered with "was".
	/// </remarks>
	public string GetNegatedResultVerb(string it, ExpectationGrammars grammars)
		=> _equivalenceRelations.Includes(EquivalenceRelations.Contains)
			? " did"
			: grammars.SubjectVerb(it, " was", " were");

	/// <summary>
	///     Only the containment relations require the items to appear without other items in between; equality implies
	///     contiguity anyway.
	/// </summary>
	private string ContiguousSuffix()
		=> _equivalenceRelations.Includes(EquivalenceRelations.Contains) ||
		   _equivalenceRelations.Includes(EquivalenceRelations.IsContainedIn)
			? " and contiguous"
			: "";

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
				   "that has at least one additional item",
			(EquivalenceRelations.IsContainedInProperly, true)
				=> $"{grammars.Verb("is", "are")} not contained in collection {expectedExpression} " +
				   "that has at least one additional item",
			(_, false) => $"{grammars.Verb("is", "are")} equal to collection {expectedExpression}",
			(_, true) => $"{grammars.Verb("is", "are")} not equal to collection {expectedExpression}",
		};

	private static string? ReturnErrorString(string it, List<string> errors)
	{
		if (errors.Count > 0)
		{
			if (errors.Count > 1)
			{
				string separator = errors.Any(error => error.Contains('\n', StringComparison.Ordinal))
					? $"{Environment.NewLine}and{Environment.NewLine}"
					: $" and{Environment.NewLine}";
				return $"{it}{Environment.NewLine}{string.Join(separator, errors.Select(error => error.Indent()))}";
			}

			return $"{it} {errors[0]}";
		}

		return null;
	}

	/// <summary>
	///     An unexpected item and a missing item that format identically differ only in their runtime type, which the
	///     reader cannot see unless it is named, so both sides are formatted through the returned formatter.
	/// </summary>
	private static Func<object?, string> GetItemFormatter(IEnumerable<object?> unexpectedItems,
		IEnumerable<object?> missingItems)
	{
		List<(string Text, Type? Type)> unexpected = unexpectedItems
			.Select(item => (Formatter.Format(item), item?.GetType()))
			.ToList();
		HashSet<string> ambiguousTexts = new(StringComparer.Ordinal);
		foreach (object? missingItem in missingItems)
		{
			string text = Formatter.Format(missingItem);
			Type? type = missingItem?.GetType();
			if (unexpected.Any(item => item.Type != type &&
			                           string.Equals(item.Text, text, StringComparison.Ordinal)))
			{
				ambiguousTexts.Add(text);
			}
		}

		return value =>
		{
			string text = Formatter.Format(value);
			return ambiguousTexts.Contains(text) ? ValuePairFormatter.AppendRuntimeType(text, value) : text;
		};
	}

	/// <summary>
	///     Aborting the run leaves the total number of deviations unknown, so the listed ones end with the truncation
	///     marker for an unknown total.
	/// </summary>
	/// <remarks>
	///     An aborted run only lists the deviations of the subject items that were inspected, because the expected
	///     items are not accounted for completely. A completed run knows the <paramref name="missingItems" />, which
	///     are listed after the deviations of the subject items, and whether these were truncated.
	/// </remarks>
	/// <remarks>
	///     The <paramref name="exceededDeviations" /> are twice the <paramref name="maximumNumber" />, unless the run
	///     was aborted at a lower limit.
	/// </remarks>
	private static string TooManyDeviationsError(string it, int maximumNumber, IEnumerable<string> deviations,
		IEnumerable<string>? missingItems = null, long? exceededDeviations = null)
	{
		StringBuilder sb = new();
		sb.Append(it).Append(" had more than ").Append(exceededDeviations ?? 2L * maximumNumber)
			.Append(" deviations");
		List<string> allDeviations = deviations.ToList();
		bool isTruncated = missingItems is null || allDeviations.Count > maximumNumber;
		List<string> entries = allDeviations.Take(maximumNumber).ToList();
		if (entries.Count > 0 && isTruncated)
		{
			entries.Add("(… and maybe more)");
		}

		entries.AddRange(missingItems ?? []);
		if (entries.Count == 0)
		{
			return sb.ToString();
		}

		return sb.Append(':').AppendLine()
			.Append(string.Join($",{Environment.NewLine}", entries.Select(entry => entry.Indent())))
			.ToString();
	}

	/// <summary>
	///     The index of an item as a failure names it, which is the index in each dimension for a subject with the
	///     <paramref name="dimensions" /> of an array of rank greater than one, as it enumerates its items with the
	///     last dimension changing first.
	/// </summary>
	private static string FormatIndex(int index, int[]? dimensions)
	{
		if (dimensions is null)
		{
			return index.ToString();
		}

		int[] indices = new int[dimensions.Length];
		for (int dimension = dimensions.Length - 1; dimension >= 0; dimension--)
		{
			indices[dimension] = index % dimensions[dimension];
			index /= dimensions[dimension];
		}

		return $"[{string.Join(",", indices)}]";
	}

	private static IEnumerable<string> AdditionalItemsError<T>(Dictionary<int, T> additionalItems,
		Func<object?, string> formatItem, int[]? dimensions)
	{
		bool hasAdditionalItems = additionalItems.Any();
		if (hasAdditionalItems)
		{
			foreach (KeyValuePair<int, T> additionalItem in additionalItems)
			{
				yield return
					$"contained item {formatItem(additionalItem.Value)} at index {FormatIndex(additionalItem.Key, dimensions)} that was not expected";
			}
		}
	}

	private static IEnumerable<string> IncorrectItemsError<T, TExpected>(
		Dictionary<int, (T Item, TExpected Expected)> incorrectItems, object options, int[]? dimensions)
	{
		bool hasIncorrectItems = incorrectItems.Any();
		if (hasIncorrectItems)
		{
			foreach (KeyValuePair<int, (T Item, TExpected Expected)> incorrectItem in incorrectItems)
			{
				(string item, string expected) =
					ValuePairFormatter.Format(incorrectItem.Value.Item, incorrectItem.Value.Expected);
				yield return
					$"contained item {item} at index {FormatIndex(incorrectItem.Key, dimensions)} instead of {DescribeExpected(expected, options)}";
			}
		}
	}

	/// <summary>
	///     A string pattern names its kind, so that it is not mistaken for the value an item had to be equal to.
	/// </summary>
	/// <remarks>
	///     Options that wrap other options, e.g. to let the comparer of the subject decide, provide the wrapped ones.
	/// </remarks>
	private static string DescribeExpected(string formattedExpected, object options)
	{
		object itemOptions = options is IOptionsProvider<object> provider ? provider.Options : options;
		return itemOptions is StringEqualityOptions stringEqualityOptions
			? stringEqualityOptions.WithPatternKind(formattedExpected)
			: formattedExpected;
	}

	private static IEnumerable<string> OutOfOrderItemsError<T>(Dictionary<int, T> outOfOrderItems,
		int[]? dimensions)
	{
		foreach (KeyValuePair<int, T> outOfOrderItem in outOfOrderItems)
		{
			yield return
				$"contained item {Formatter.Format(outOfOrderItem.Value)} at index {FormatIndex(outOfOrderItem.Key, dimensions)} in wrong order";
		}
	}

	private static IEnumerable<string> MissingItemsError<T>(int total, List<T> missingItems,
		EquivalenceRelations equivalenceRelation, bool ignoringDuplicates, Func<object?, string> formatItem,
		object options, int maximumNumber)
	{
		if (total == 0)
		{
			yield break;
		}

		bool hasMissingItems = missingItems.Any();
		if (total == missingItems.Count)
		{
			yield return (total, ignoringDuplicates) switch
			{
				(1, true) => "lacked the one unique expected item",
				(1, false) => "lacked the one expected item",
				(_, true) => $"lacked all {total} unique expected items",
				(_, false) => $"lacked all {total} expected items",
			};
			yield break;
		}

		if (hasMissingItems && !equivalenceRelation.Includes(EquivalenceRelations.IsContainedIn))
		{
			if (missingItems.Count == 1)
			{
				yield return
					$"lacked {missingItems.Count} of {total} expected items: {DescribeExpected(formatItem(missingItems[0]), options)}";
				yield break;
			}

			StringBuilder sb = new();
			sb.Append("lacked ").Append(missingItems.Count).Append(" of ")
				.Append(total).Append(" expected items:");
			AppendMissingItems(sb, missingItems, formatItem, options, maximumNumber);
			yield return sb.ToString();
		}
	}

	/// <remarks>
	///     More than twice <paramref name="maximumNumber" /> missing items are truncated to the first
	///     <paramref name="maximumNumber" /> ones, like the deviations when there are too many of them.
	/// </remarks>
	private static void AppendMissingItems<T>(StringBuilder sb, List<T> missingItems,
		Func<object?, string> formatItem, object options, int maximumNumber)
	{
		bool isTruncated = missingItems.Count > 2L * maximumNumber;
		foreach (T missingItem in isTruncated ? missingItems.Take(maximumNumber) : missingItems)
		{
			sb.AppendLine().Append("  ");
			sb.Append(DescribeExpected(formatItem(missingItem), options));
			sb.Append(',');
		}

		if (isTruncated)
		{
			sb.AppendLine().Append("  (… and ").Append(missingItems.Count - maximumNumber).Append(" more)");
		}
		else
		{
			sb.Length--;
		}
	}

	/// <summary>
	///     Whether items of <typeparamref name="T" /> that are equal by <see cref="object.Equals(object)" /> are also
	///     equal for the comparison of the <paramref name="options" />, so that ignoring duplicates may merge them without
	///     comparing them.
	/// </summary>
	/// <remarks>
	///     This holds when the comparison is <see cref="object.Equals(object)" /> itself, except for a
	///     <see cref="DateTime" />, as the comparison also checks its kind. Otherwise only equal values that are
	///     identical can be merged, see <see cref="IsIdenticalToEqualValues{TValue}(TValue)" />.
	/// </remarks>
	private static bool CanMergeEqualItems<T, T2>(IOptionsEquality<T2> options)
		=> options is ObjectEqualityOptions<T2> { UsesEqualsMatch: true, } &&
		   !typeof(T).IsAssignableFrom(typeof(DateTime));

	/// <summary>
	///     Whether the <paramref name="value" /> is identical to every value that it is equal to by
	///     <see cref="object.Equals(object)" />, so that no comparison can tell them apart.
	/// </summary>
	private static bool IsIdenticalToEqualValues<TValue>(TValue value)
		=> value is null or string or Enum or bool or char or byte or sbyte or short or ushort or int or uint or long
			or ulong;

	private static async ValueTask<bool> Any<T>(IEnumerable<T> items, Func<T, ValueTask<bool>> predicate)
	{
		foreach (T item in items)
		{
			if (await predicate(item))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Compiling a predicate costs far more than evaluating it, and matching can evaluate it for many items.
	/// </summary>
	private sealed class CompiledPredicates<T>
	{
		private readonly Dictionary<Expression<Func<T, bool>>, Func<T, bool>> _predicates = new();

		/// <summary>
		///     Evaluates the <paramref name="predicate" /> for the <paramref name="value" /> at the
		///     <paramref name="index" />, which an exception of the predicate names.
		/// </summary>
		public ValueTask<bool> Invoke(Expression<Func<T, bool>> predicate, T value, int index)
		{
			if (!_predicates.TryGetValue(predicate, out Func<T, bool>? compiled))
			{
				compiled = predicate.Compile();
				_predicates.Add(predicate, compiled);
			}

			try
			{
				return new ValueTask<bool>(compiled(value));
			}
			catch (Exception exception)
			{
				throw new UserCodeException(exception, "the predicate", index);
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
			expectation.ThrowIfNull();
			_context = context;
			_cancellationToken = cancellationToken;
			ItemExpectationBuilder = new ManualExpectationBuilder<TItem>((grammars & ~ExpectationGrammars.Plural) | ExpectationGrammars.Introduced);
			expectation.Invoke(new ThatSubject<TItem?>(ItemExpectationBuilder));
		}

		/// <summary>
		///     Whether an evaluation in <see cref="IsMetBy(TItem)" /> could not be decided, e.g. because it was canceled,
		///     so that its value was not reliably found not to meet the expectation.
		/// </summary>
		public bool IsUndecided { get; private set; }

		/// <summary>
		///     Verifies if the <paramref name="value" /> is met by the expectation.
		/// </summary>
		/// <remarks>
		///     A <paramref name="value" /> for which the expectation fails both ways (e.g. because code of the caller threw)
		///     throws, so that the collection fails with the result of the item, unless another expected item matches it.
		/// </remarks>
		public ValueTask<bool> IsMetBy(TItem value)
			=> IsMetBy(value, null);

		/// <inheritdoc cref="IsMetBy(TItem)" />
		/// <remarks>
		///     The exception for a <paramref name="value" /> for which the expectation fails both ways names it by its
		///     <paramref name="index" /> in the collection.
		/// </remarks>
		internal async ValueTask<bool> IsMetBy(TItem value, int? index)
		{
			ConstraintResult result = await ItemExpectationBuilder.IsMetBy(value, _context, _cancellationToken);
			IsUndecided |= result.Outcome == Outcome.Undecided;
			if (result.Outcome == Outcome.FailureBothWays)
			{
				throw new UnansweredItemException(result, value, index);
			}

			return result.Outcome == Outcome.Success;
		}

		/// <summary>
		///     Prepares the expectation text without evaluating the expectation, so that it is also complete when no value
		///     is evaluated.
		/// </summary>
		/// <remarks>
		///     See <see cref="ManualExpectationBuilder{TValue}.PrepareExpectation" />.
		/// </remarks>
		public Task PrepareExpectation()
			=> ItemExpectationBuilder.PrepareExpectation(_context, _cancellationToken);

		/// <inheritdoc cref="object.Equals(object?)" />
		public override bool Equals(object? obj) => obj is ExpectationItem<TItem> other && Equals(other);

		private bool Equals(ExpectationItem<TItem> other)
			=> ItemExpectationBuilder.Equals(other.ItemExpectationBuilder);

		/// <inheritdoc cref="object.GetHashCode()" />
		public override int GetHashCode() => ItemExpectationBuilder.GetHashCode();

		/// <inheritdoc cref="object.ToString()" />
		public override string ToString()
		{
			StringBuilder sb = new();
			sb.Append("an item that ");
			ItemExpectationBuilder.AppendExpectation(sb);
			ItemExpectationBuilder.AppendReasons(sb);
			return sb.ToString();
		}
	}
}
