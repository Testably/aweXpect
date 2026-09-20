using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Core;
using aweXpect.Core.Constraints;
using aweXpect.Core.EvaluationContext;
using aweXpect.Core.Helpers;

namespace aweXpect.Equivalency;

public static partial class EquivalencyComparison
{
	private static bool CompareByValue<TActual, TExpected>(
		[DisallowNull] TActual actual,
		[DisallowNull] TExpected expected,
		StringBuilder failureBuilder,
		string memberPath,
		MemberType memberType,
		EquivalencyContext context)
	{
		if (DateTimeKindComparison.AreKindsIncompatible(actual, expected))
		{
			AppendDifference(failureBuilder, memberType, memberPath, actual, expected, context);
			return false;
		}

		bool isEqual;
		try
		{
			isEqual = actual.Equals(expected);
		}
		catch (Exception exception)
		{
			throw Tracing.WriteException(
				new InvalidOperationException(
					$"The equals method of {Formatter.Format(actual.GetType())} threw an {Formatter.Format(exception.GetType())}: {exception.Message}",
					exception));
		}

		if (!isEqual)
		{
			AppendDifference(failureBuilder, memberType, memberPath, actual, expected, context);
			return false;
		}

		return true;
	}

	private static bool CompareNulls<TActual, TExpected>(TActual actual, TExpected expected,
		StringBuilder failureBuilder, string memberPath, MemberType memberType, EquivalencyContext context)
	{
		if (actual is null && expected is null)
		{
			return true;
		}

		AppendDifference(failureBuilder, memberType, memberPath, actual, expected, context);
		return false;
	}

	/// <remarks>
	///     Starts the entry that the caller then fills, and counts it, so that a comparison can be told apart from
	///     one that found fewer differences even when neither of them is equivalent.
	/// </remarks>
	private static void AppendEntry(StringBuilder failureBuilder, MemberType memberType, string memberPath,
		EquivalencyContext context)
	{
		context.DifferenceCount++;
		failureBuilder.AppendLine();
		if (failureBuilder.Length > 2)
		{
			failureBuilder.AppendLine("and");
		}

		failureBuilder.Append("  ");
		failureBuilder.Append(GetMemberPath(memberType, memberPath));
	}

	private static void AppendDifference<TActual, TExpected>(StringBuilder failureBuilder,
		MemberType memberType, string memberPath, TActual actual, TExpected expected, EquivalencyContext context)
	{
		AppendDifferenceHeader(failureBuilder, memberType, memberPath, context);
		(string actualText, string expectedText) =
			ValuePairFormatter.Format(actual, expected, FormattingOptions.SingleLine);
		failureBuilder.Append(actualText).AppendLine().Append("    Expected: ").Append(expectedText);
	}

	private static void AppendDifferenceHeader(StringBuilder failureBuilder, MemberType memberType,
		string memberPath, EquivalencyContext context)
	{
		AppendEntry(failureBuilder, memberType, memberPath, context);
		failureBuilder.AppendLine(" differed:");
		failureBuilder.Append("       Found: ");
	}

	private static void AppendMaxRecursionDepthExceeded(StringBuilder failureBuilder, MemberType memberType,
		string memberPath, int maxRecursionDepth, EquivalencyContext context)
	{
		AppendEntry(failureBuilder, memberType, memberPath, context);
		failureBuilder.Append(" exceeded the maximum recursion depth of ");
		failureBuilder.Append(maxRecursionDepth);
	}

	private static void AppendMissingElement(StringBuilder failureBuilder, string memberPath, object? expected,
		EquivalencyContext context)
	{
		AppendEntry(failureBuilder, MemberType.Element, memberPath, context);
		failureBuilder.Append(" was missing ");
		Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
	}

	private static void AppendMissingMember(StringBuilder failureBuilder, MemberType memberType, string memberPath,
		EquivalencyContext context)
	{
		AppendEntry(failureBuilder, memberType, memberPath, context);
		failureBuilder.Append(" is missing on the actual object");
	}

	private static void AppendSuperfluousElement(StringBuilder failureBuilder, string memberPath, object? actual,
		EquivalencyContext context)
	{
		AppendEntry(failureBuilder, MemberType.Element, memberPath, context);
		failureBuilder.Append(" had superfluous ");
		Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
	}

	private static string ConcatMemberPath(string memberPath, string memberName)
	{
		if (string.IsNullOrEmpty(memberPath))
		{
			return memberName;
		}

		return $"{memberPath}.{memberName}";
	}

	private static string GetMemberPath(MemberType type, string memberPath)
	{
		if (string.IsNullOrEmpty(memberPath))
		{
			return "It";
		}

		return $"{type} {memberPath}";
	}

	private enum MemberType
	{
		Property,
		Field,
		Value,
		Element,
	}

	/// <remarks>
	///     A rule scoped to one kind of member carries that scope in its type, so only the rules that match the
	///     <paramref name="memberType" /> are asked. A collection element is neither a field nor a property, so a
	///     scoped rule never applies to one.
	/// </remarks>
	private static bool AppliesTo(MemberToIgnore memberToIgnore, MemberType memberType)
		=> memberType switch
		{
			MemberType.Field => memberToIgnore is not MemberToIgnore.ByPropertyPredicate,
			MemberType.Property => memberToIgnore is not MemberToIgnore.ByFieldPredicate,
			_ => memberToIgnore is not MemberToIgnore.ByFieldPredicate and
			     not MemberToIgnore.ByPropertyPredicate,
		};
#pragma warning disable S3776 // https://rules.sonarsource.com/csharp/RSPEC-3776
#pragma warning disable S107 // https://rules.sonarsource.com/csharp/RSPEC-107
#if NET8_0_OR_GREATER
	private static async ValueTask<bool>
#else
	private static async Task<bool>
#endif
		Compare<TActual, TExpected>(
			TActual actual,
			TExpected expected,
			EquivalencyOptions equivalencyOptions,
			EquivalencyTypeOptions typeOptions,
			StringBuilder failureBuilder,
			string memberPath,
			MemberType memberType,
			EquivalencyContext context)
	{
		if (expected is Expectation &&
		    expected is IOptionsProvider<ExpectationBuilder>
		    {
			    Options: EquivalencyExpectationBuilder equivalencyExpectationBuilder,
		    })
		{
			ConstraintResult? result =
				await equivalencyExpectationBuilder.IsMetBy(actual, new EvaluationContext(), CancellationToken.None);
			if (result.Outcome == Outcome.Success)
			{
				return true;
			}

			if (result.Outcome == Outcome.Failure)
			{
				AppendDifferenceHeader(failureBuilder, memberType, memberPath, context);
				Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
				if (actual is not null && !equivalencyExpectationBuilder.IsOfExpectedType(actual))
				{
					failureBuilder.Append(" (");
					Formatter.Format(failureBuilder, actual.GetType());
					failureBuilder.Append(')');
				}

				failureBuilder.AppendLine().Append("    Expected: ");
				failureBuilder.Append(equivalencyExpectationBuilder.ToString().Indent("    ", false));
				return false;
			}
		}

		if (actual is null || expected is null)
		{
			return CompareNulls(actual, expected, failureBuilder, memberPath, memberType, context);
		}

		EquivalencyComparisonType comparisonType = typeOptions.ComparisonType
		                                           ?? equivalencyOptions.DefaultComparisonTypeSelector.Invoke(
			                                           actual.GetType());
		if (comparisonType == EquivalencyComparisonType.ByValue)
		{
			return CompareByValue(actual, expected, failureBuilder, memberPath, memberType, context);
		}

		ComparedPair comparedPair = new(actual, expected);
		if (!context.ComparedPairs.Add(comparedPair))
		{
			return true;
		}

		context.Depth++;
		try
		{
			if (context.Depth > equivalencyOptions.MaxRecursionDepth)
			{
				AppendMaxRecursionDepthExceeded(failureBuilder, memberType, memberPath,
					equivalencyOptions.MaxRecursionDepth, context);
				return false;
			}

			if (actual is IDictionary actualDictionary && expected is IDictionary expectedDictionary)
			{
				return await CompareDictionaries(actualDictionary, expectedDictionary, failureBuilder, memberPath,
					equivalencyOptions, typeOptions, context);
			}

			if (actual is IEnumerable actualEnumerable && expected is IEnumerable expectedEnumerable)
			{
				return await CompareEnumerables(actualEnumerable, expectedEnumerable, failureBuilder, memberPath,
					equivalencyOptions, typeOptions, context);
			}

			return await CompareObjects(actual, expected, failureBuilder, memberType, memberPath,
				equivalencyOptions, typeOptions, context);
		}
		finally
		{
			context.Depth--;
			context.ComparedPairs.Remove(comparedPair);
		}
	}

	/// <remarks>
	///     The members to compare come from the expected object, and each one is looked up on the actual type by its
	///     own kind first. Whether the actual type stores a member as a field or as a property is an implementation
	///     detail of that type - a DTO with public fields is routinely compared against an anonymous object, which can
	///     only have properties - so a member the actual type does not have as that kind falls back to the other kind
	///     of the same name. The fallback uses the visibility requested for the kind it reaches, so a kind the caller
	///     excluded is never reached through the other one, and the failure keeps the kind of the expected member,
	///     which is also the kind a scoped ignore rule applies to.
	/// </remarks>
#if NET8_0_OR_GREATER
	private static async ValueTask<bool>
#else
	private static async Task<bool>
#endif
		CompareObjects<TActual, TExpected>([DisallowNull] TActual actual,
			[DisallowNull] TExpected expected,
			StringBuilder failureBuilder, MemberType memberType, string memberPath,
			EquivalencyOptions options, EquivalencyTypeOptions typeOptions, EquivalencyContext context)
	{
		bool result = true;
		int memberCount = 0;
		if (typeOptions.Fields != IncludeMembers.None)
		{
			foreach (EquivalencyMember field in EquivalencyMembers.GetFields(expected.GetType(), typeOptions.Fields))
			{
				memberCount++;
				string fieldMemberPath = ConcatMemberPath(memberPath, field.Name);
				if (typeOptions.MembersToIgnore.Any(memberToIgnore
					    => AppliesTo(memberToIgnore, MemberType.Field) &&
					       memberToIgnore.IgnoreMember(fieldMemberPath, field.DeclaredType)))
				{
					continue;
				}

				Func<object, object?>? actualFieldAccessor =
					EquivalencyMembers.FindField(actual.GetType(), field.Name, typeOptions.Fields) ??
					EquivalencyMembers.FindProperty(actual.GetType(), field.Name, typeOptions.Properties);
				if (actualFieldAccessor is null)
				{
					AppendMissingMember(failureBuilder, MemberType.Field, fieldMemberPath, context);
					result = false;
					continue;
				}

				object? actualFieldValue = actualFieldAccessor.Invoke(actual);
				object? expectedFieldValue = field.GetValue(expected);

				if (!await Compare(actualFieldValue, expectedFieldValue,
					    options, options.GetTypeOptions(actualFieldValue?.GetType(), typeOptions),
					    failureBuilder, fieldMemberPath, MemberType.Field, context))
				{
					result = false;
				}
			}
		}

		if (typeOptions.Properties != IncludeMembers.None)
		{
			foreach (EquivalencyMember property in EquivalencyMembers.GetProperties(expected.GetType(),
				         typeOptions.Properties))
			{
				memberCount++;
				string propertyMemberPath = ConcatMemberPath(memberPath, property.Name);
				if (typeOptions.MembersToIgnore.Any(memberToIgnore
					    => AppliesTo(memberToIgnore, MemberType.Property) &&
					       memberToIgnore.IgnoreMember(propertyMemberPath, property.DeclaredType)))
				{
					continue;
				}

				Func<object, object?>? actualPropertyAccessor =
					EquivalencyMembers.FindProperty(actual.GetType(), property.Name, typeOptions.Properties) ??
					EquivalencyMembers.FindField(actual.GetType(), property.Name, typeOptions.Fields);
				if (actualPropertyAccessor is null)
				{
					AppendMissingMember(failureBuilder, MemberType.Property, propertyMemberPath, context);
					result = false;
					continue;
				}

				object? actualPropertyValue = actualPropertyAccessor.Invoke(actual);
				object? expectedPropertyValue = property.GetValue(expected);

				if (!await Compare(actualPropertyValue, expectedPropertyValue,
					    options, options.GetTypeOptions(actualPropertyValue?.GetType(), typeOptions),
					    failureBuilder, propertyMemberPath, MemberType.Property, context))
				{
					result = false;
				}
			}
		}

		if (memberCount == 0)
		{
			if (actual.GetType() != expected.GetType())
			{
				AppendDifference(failureBuilder, memberType, memberPath, actual, expected, context);
				result = false;
			}
			else if (typeOptions.Fields != IncludeMembers.None || typeOptions.Properties != IncludeMembers.None)
			{
				throw Tracing.WriteException(
					new InvalidOperationException(
						$"{GetMemberPath(memberType, memberPath)} has no members that could be compared on {Formatter.Format(expected.GetType())}, which would make the equivalency comparison succeed without verifying anything. Adjust the equivalency options to include the relevant members or to compare this type by value, or - when publishing with trimming or Native AOT enabled - ensure that the type is rooted, so that its members are preserved."));
			}
		}

		return result;
	}
#if NET8_0_OR_GREATER
	private static async ValueTask<bool>
#else
	private static async Task<bool>
#endif
		CompareDictionaries(
			IDictionary actual,
			IDictionary expected,
			StringBuilder failureBuilder,
			string memberPath,
			EquivalencyOptions options,
			EquivalencyTypeOptions typeOptions,
			EquivalencyContext context)
	{
		bool result = true;

		foreach (object? key in actual.Keys)
		{
			string elementMemberPath = $"{memberPath}[{key}]";

			object? actualObject = actual[key];
			if (typeOptions.MembersToIgnore.Any(memberToIgnore
				    => AppliesTo(memberToIgnore, MemberType.Element) &&
				       memberToIgnore.IgnoreMember(elementMemberPath, actualObject?.GetType() ?? typeof(object))))
			{
				continue;
			}
			if (expected.Contains(key))
			{
				object? expectedObject = expected[key];

				if (!await Compare(actualObject, expectedObject,
					    options, options.GetTypeOptions(actualObject?.GetType(), typeOptions),
					    failureBuilder, elementMemberPath, MemberType.Element, context))
				{
					result = false;
				}
			}
			else
			{
				AppendSuperfluousElement(failureBuilder, elementMemberPath, actualObject, context);
				result = false;
			}
		}

		foreach (object? key in expected.Keys)
		{
			if (actual.Contains(key))
			{
				continue;
			}

			string elementMemberPath = $"{memberPath}[{key}]";
			object? expectedObject = expected[key];
			if (typeOptions.MembersToIgnore.Any(memberToIgnore
				    => AppliesTo(memberToIgnore, MemberType.Element) &&
				       memberToIgnore.IgnoreMember(elementMemberPath, expectedObject?.GetType() ?? typeof(object))))
			{
				continue;
			}

			AppendMissingElement(failureBuilder, elementMemberPath, expectedObject, context);
			result = false;
		}

		return result;
	}
#if NET8_0_OR_GREATER
	private static async ValueTask<bool>
#else
	private static async Task<bool>
#endif
		CompareEnumerables(
			IEnumerable actual,
			IEnumerable expected,
			StringBuilder failureBuilder,
			string memberPath,
			EquivalencyOptions options,
			EquivalencyTypeOptions typeOptions,
			EquivalencyContext context)
	{
		bool result = true;
		object?[] actualObjects = actual.Cast<object?>().ToArray();
		object?[] expectedObjects = expected.Cast<object?>().ToArray();

		if (typeOptions.IgnoreCollectionOrder)
		{
			return await CompareInAnyOrder(actualObjects, expectedObjects, failureBuilder, memberPath, options,
				typeOptions, context);
		}

		for (int i = 0; i < Math.Min(actualObjects.Length, expectedObjects.Length); i++)
		{
			string elementMemberPath = $"{memberPath}[{i}]";
			object? actualObject = actualObjects.ElementAtOrDefault(i);
			if (typeOptions.MembersToIgnore.Any(memberToIgnore
				    => AppliesTo(memberToIgnore, MemberType.Element) &&
				       memberToIgnore.IgnoreMember(elementMemberPath, actualObject?.GetType() ?? typeof(object))))
			{
				continue;
			}
			
			object? expectedObject = expectedObjects.ElementAtOrDefault(i);

			if (!await Compare(actualObject, expectedObject,
				    options, options.GetTypeOptions(actualObject?.GetType(), typeOptions),
				    failureBuilder, elementMemberPath, MemberType.Element, context))
			{
				result = false;
			}
		}

		if (expectedObjects.Length > actualObjects.Length)
		{
			for (int i = actualObjects.Length; i < expectedObjects.Length; i++)
			{
				string elementMemberPath = $"{memberPath}[{i}]";
				object? expectedObject = expectedObjects.ElementAtOrDefault(i);
				if (typeOptions.MembersToIgnore.Any(memberToIgnore
					    => AppliesTo(memberToIgnore, MemberType.Element) &&
					       memberToIgnore.IgnoreMember(elementMemberPath, expectedObject?.GetType() ?? typeof(object))))
				{
					continue;
				}

				AppendMissingElement(failureBuilder, elementMemberPath, expectedObject, context);
				result = false;
			}
		}

		if (expectedObjects.Length < actualObjects.Length)
		{
			for (int i = expectedObjects.Length; i < actualObjects.Length; i++)
			{
				string elementMemberPath = $"{memberPath}[{i}]";
				object? actualObject = actualObjects.ElementAtOrDefault(i);
				if (typeOptions.MembersToIgnore.Any(memberToIgnore
					    => AppliesTo(memberToIgnore, MemberType.Element) &&
					       memberToIgnore.IgnoreMember(elementMemberPath, actualObject?.GetType() ?? typeof(object))))
				{
					continue;
				}

				AppendSuperfluousElement(failureBuilder, elementMemberPath, actualObject, context);
				result = false;
			}
		}

		return result;
	}

	/// <remarks>
	///     The elements are paired by the equivalency comparison itself instead of being sorted, because sorting
	///     requires them to be comparable, which the objects that structural equivalency exists for usually are not.
	///     The pairing is a maximum matching and not a greedy first match, because an actual element that is equivalent
	///     to more than one expected element would otherwise be able to consume the only candidate of another expected
	///     element and report a difference that does not exist. Maximality also means that no leftover actual element
	///     is equivalent to an unmatched expected one, so any two of them can be reported against each other: they
	///     really differ. Which two are reported against each other still decides how much the message helps, so the
	///     leftovers are paired by the fewest differences rather than by their position.
	/// </remarks>
#if NET8_0_OR_GREATER
	private static async ValueTask<bool>
#else
	private static async Task<bool>
#endif
		CompareInAnyOrder(
			object?[] actualObjects,
			object?[] expectedObjects,
			StringBuilder failureBuilder,
			string memberPath,
			EquivalencyOptions options,
			EquivalencyTypeOptions typeOptions,
			EquivalencyContext context)
	{
		int[] actualIndices = GetIndicesToCompare(actualObjects, memberPath, typeOptions);
		int[] expectedIndices = GetIndicesToCompare(expectedObjects, memberPath, typeOptions);
		ElementMatcher matcher = new(actualObjects, actualIndices, expectedObjects, expectedIndices, memberPath,
			options, typeOptions, context);
		await matcher.MatchAll();
		(int Actual, int Expected)[] leftovers = await matcher.GetLeftovers();
		if (leftovers.Length == 0)
		{
			return true;
		}

		foreach ((int actualIndex, int expectedIndex) in leftovers)
		{
			if (actualIndex < 0)
			{
				AppendMissingElement(failureBuilder, $"{memberPath}[{expectedIndex}]",
					expectedObjects[expectedIndex], context);
			}
			else if (expectedIndex < 0)
			{
				AppendSuperfluousElement(failureBuilder, $"{memberPath}[{actualIndex}]",
					actualObjects[actualIndex], context);
			}
			else
			{
				object? actualObject = actualObjects[actualIndex];
				await Compare(actualObject, expectedObjects[expectedIndex],
					options, options.GetTypeOptions(actualObject?.GetType(), typeOptions),
					failureBuilder, $"{memberPath}[{actualIndex}]", MemberType.Element, context);
			}
		}

		return false;
	}

	/// <remarks>
	///     Returns the indices of the elements that take part in the comparison. An ignored element has no pair it
	///     could be skipped in once the order is ignored, so it is left out of the matching on both sides: it neither
	///     has to find a counterpart nor can it be reported as superfluous.
	/// </remarks>
	private static int[] GetIndicesToCompare(object?[] objects, string memberPath,
		EquivalencyTypeOptions typeOptions)
	{
		List<int> indices = new(objects.Length);
		for (int i = 0; i < objects.Length; i++)
		{
			object? element = objects[i];
			string elementMemberPath = $"{memberPath}[{i}]";
			if (!typeOptions.MembersToIgnore.Any(memberToIgnore
				    => AppliesTo(memberToIgnore, MemberType.Element) &&
				       memberToIgnore.IgnoreMember(elementMemberPath, element?.GetType() ?? typeof(object))))
			{
				indices.Add(i);
			}
		}

		return indices.ToArray();
	}

	/// <summary>
	///     Matches the elements of two collections whose order is ignored against each other, using Kuhn's algorithm.
	/// </summary>
	private sealed class ElementMatcher
	{
		private readonly int[] _actualIndices;
		private readonly object?[] _actualObjects;
		private readonly EquivalencyContext _context;

		/// <summary>
		///     The number of differences of every pairwise comparison that <see cref="_results" /> holds a result for.
		/// </summary>
		/// <remarks>
		///     Kept apart from the result, because the search for augmenting paths reads the result of every pair it
		///     considers while only the leftovers need the count, and a wider cell would slow that search down.
		/// </remarks>
		private readonly int[,] _differenceCounts;

		private readonly int[] _expectedIndices;
		private readonly object?[] _expectedObjects;

		/// <summary>
		///     The expected element each actual element is matched to, or <c>-1</c> while it is still free.
		/// </summary>
		private readonly int[] _matchedTo;

		private readonly string _memberPath;
		private readonly EquivalencyOptions _options;

		/// <summary>
		///     Caches every pairwise comparison, because the search for augmenting paths revisits pairs and a single
		///     comparison walks a whole object graph.
		/// </summary>
		private readonly bool?[,] _results;

		private readonly EquivalencyTypeOptions _typeOptions;

		/// <summary>
		///     The expected elements that no actual element could be matched to.
		/// </summary>
		private readonly List<int> _unmatchedExpected = [];

		public ElementMatcher(object?[] actualObjects, int[] actualIndices, object?[] expectedObjects,
			int[] expectedIndices, string memberPath, EquivalencyOptions options,
			EquivalencyTypeOptions typeOptions, EquivalencyContext context)
		{
			_actualObjects = actualObjects;
			_actualIndices = actualIndices;
			_expectedObjects = expectedObjects;
			_expectedIndices = expectedIndices;
			_memberPath = memberPath;
			_options = options;
			_typeOptions = typeOptions;
			_context = context;
			_results = new bool?[actualIndices.Length, expectedIndices.Length];
			_differenceCounts = new int[actualIndices.Length, expectedIndices.Length];
			_matchedTo = new int[actualIndices.Length];
			for (int i = 0; i < _matchedTo.Length; i++)
			{
				_matchedTo[i] = -1;
			}
		}

		/// <summary>
		///     Matches as many expected elements as possible.
		/// </summary>
#if NET8_0_OR_GREATER
		public async ValueTask
#else
		public async Task
#endif
			MatchAll()
		{
			for (int i = 0; i < _expectedIndices.Length; i++)
			{
				if (!await TryMatch(i, new bool[_actualIndices.Length]))
				{
					_unmatchedExpected.Add(i);
				}
			}
		}

		/// <summary>
		///     Returns the elements that were left over, as pairs of an actual and an expected index, where
		///     <c>-1</c> stands for the side that has no counterpart left.
		/// </summary>
		/// <remarks>
		///     The leftovers are paired greedily, starting with the pair that differs the least, because which two of
		///     them are reported against each other decides how much the message helps: two elements that only differ
		///     in one member say what went wrong, while the positional pairing this replaces could just as well pick
		///     two elements that have nothing in common and report every member of both. An exact assignment would
		///     have to weigh all combinations against each other, which a failure message does not justify. Sorting
		///     the candidates keeps the pairing to one sort on top of the comparisons the matching already needed.
		/// </remarks>
#if NET8_0_OR_GREATER
		public async ValueTask<(int Actual, int Expected)[]>
#else
		public async Task<(int Actual, int Expected)[]>
#endif
			GetLeftovers()
		{
			List<int> unmatchedActual = [];
			for (int i = 0; i < _matchedTo.Length; i++)
			{
				if (_matchedTo[i] < 0)
				{
					unmatchedActual.Add(i);
				}
			}

			if (unmatchedActual.Count == 0 && _unmatchedExpected.Count == 0)
			{
				return [];
			}

			List<(int DifferenceCount, int Actual, int Expected)> candidates = [];
			foreach (int actualIndex in unmatchedActual)
			{
				foreach (int expectedIndex in _unmatchedExpected)
				{
					candidates.Add(((await GetResult(actualIndex, expectedIndex)).DifferenceCount, actualIndex,
						expectedIndex));
				}
			}

			candidates.Sort();
			bool[] isActualPaired = new bool[_actualIndices.Length];
			bool[] isExpectedPaired = new bool[_expectedIndices.Length];
			List<(int Actual, int Expected)> leftovers = [];
			foreach ((int _, int actualIndex, int expectedIndex) in candidates)
			{
				if (isActualPaired[actualIndex] || isExpectedPaired[expectedIndex])
				{
					continue;
				}

				isActualPaired[actualIndex] = true;
				isExpectedPaired[expectedIndex] = true;
				leftovers.Add((_actualIndices[actualIndex], _expectedIndices[expectedIndex]));
			}

			leftovers.Sort();
			leftovers.AddRange(_unmatchedExpected.Where(i => !isExpectedPaired[i])
				.Select(i => (-1, _expectedIndices[i])));
			leftovers.AddRange(unmatchedActual.Where(i => !isActualPaired[i]).Select(i => (_actualIndices[i], -1)));
			return leftovers.ToArray();
		}

#if NET8_0_OR_GREATER
		private async ValueTask<bool>
#else
		private async Task<bool>
#endif
			TryMatch(int expectedIndex, bool[] visited)
		{
			for (int offset = 0; offset < _actualIndices.Length; offset++)
			{
				// Starting at the same position pairs collections that are already in order without any search.
				int actualIndex = (expectedIndex + offset) % _actualIndices.Length;
				if (visited[actualIndex] || !(await GetResult(actualIndex, expectedIndex)).IsEquivalent)
				{
					continue;
				}

				visited[actualIndex] = true;
				if (_matchedTo[actualIndex] < 0 || await TryMatch(_matchedTo[actualIndex], visited))
				{
					_matchedTo[actualIndex] = expectedIndex;
					return true;
				}
			}

			return false;
		}

		/// <remarks>
		///     The comparison writes into a throwaway builder, because only the differences that survive the matching
		///     belong in the failure message. Its differences are counted nonetheless, so the count is taken from the
		///     context and restored afterwards, which keeps them out of the count of the message that is kept.
		/// </remarks>
#if NET8_0_OR_GREATER
		private async ValueTask<(bool IsEquivalent, int DifferenceCount)>
#else
		private async Task<(bool IsEquivalent, int DifferenceCount)>
#endif
			GetResult(int actualIndex, int expectedIndex)
		{
			if (_results[actualIndex, expectedIndex] is { } cachedResult)
			{
				return (cachedResult, _differenceCounts[actualIndex, expectedIndex]);
			}

			object? actualObject = _actualObjects[_actualIndices[actualIndex]];
			int differenceCount = _context.DifferenceCount;
			bool isEquivalent = await Compare(actualObject, _expectedObjects[_expectedIndices[expectedIndex]],
				_options, _options.GetTypeOptions(actualObject?.GetType(), _typeOptions),
				new StringBuilder(), $"{_memberPath}[{_actualIndices[actualIndex]}]", MemberType.Element, _context);
			_results[actualIndex, expectedIndex] = isEquivalent;
			_differenceCounts[actualIndex, expectedIndex] = _context.DifferenceCount - differenceCount;
			_context.DifferenceCount = differenceCount;
			return (isEquivalent, _differenceCounts[actualIndex, expectedIndex]);
		}
	}
#pragma warning restore S107
#pragma warning restore S3776
}
