using System;
using System.Collections;
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
		MemberType memberType)
	{
		if (!actual.Equals(expected))
		{
			AppendDifference(failureBuilder, memberType, memberPath, actual, expected);
			return false;
		}

		return true;
	}

	private static bool CompareNulls<TActual, TExpected>(TActual actual, TExpected expected,
		StringBuilder failureBuilder, string memberPath, MemberType memberType)
	{
		if (actual is null && expected is null)
		{
			return true;
		}

		AppendDifference(failureBuilder, memberType, memberPath, actual, expected);
		return false;
	}

	private static void AppendDifference<TActual, TExpected>(StringBuilder failureBuilder,
		MemberType memberType, string memberPath, TActual actual, TExpected expected)
	{
		AppendDifferenceHeader(failureBuilder, memberType, memberPath);
		Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
		failureBuilder.AppendLine().Append("    Expected: ");
		Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
	}

	private static void AppendDifferenceHeader(StringBuilder failureBuilder, MemberType memberType,
		string memberPath)
	{
		failureBuilder.AppendLine();
		if (failureBuilder.Length > 2)
		{
			failureBuilder.AppendLine("and");
		}

		failureBuilder.Append("  ");
		failureBuilder.Append(GetMemberPath(memberType, memberPath));
		failureBuilder.AppendLine(" differed:");
		failureBuilder.Append("       Found: ");
	}

	private static void AppendMaxRecursionDepthExceeded(StringBuilder failureBuilder, MemberType memberType,
		string memberPath, int maxRecursionDepth)
	{
		failureBuilder.AppendLine();
		if (failureBuilder.Length > 2)
		{
			failureBuilder.AppendLine("and");
		}

		failureBuilder.Append("  ");
		failureBuilder.Append(GetMemberPath(memberType, memberPath));
		failureBuilder.Append(" exceeded the maximum recursion depth of ");
		failureBuilder.Append(maxRecursionDepth);
	}

	private static void AppendMissingMember(StringBuilder failureBuilder, MemberType memberType, string memberPath)
	{
		failureBuilder.AppendLine();
		if (failureBuilder.Length > 2)
		{
			failureBuilder.AppendLine("and");
		}

		failureBuilder.Append("  ");
		failureBuilder.Append(GetMemberPath(memberType, memberPath));
		failureBuilder.Append(" is missing on the actual object");
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
				AppendDifferenceHeader(failureBuilder, memberType, memberPath);
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
			return CompareNulls(actual, expected, failureBuilder, memberPath, memberType);
		}

		EquivalencyComparisonType comparisonType = typeOptions.ComparisonType
		                                           ?? equivalencyOptions.DefaultComparisonTypeSelector.Invoke(
			                                           actual.GetType());
		if (comparisonType == EquivalencyComparisonType.ByValue)
		{
			return CompareByValue(actual, expected, failureBuilder, memberPath, memberType);
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
					equivalencyOptions.MaxRecursionDepth);
				return false;
			}

			try
			{
				if (actual.Equals(expected))
				{
					return true;
				}
			}
			catch (Exception exception)
			{
				throw Tracing.WriteException(
					new InvalidOperationException(
						$"The equals method of {Formatter.Format(actual.GetType())} threw an {Formatter.Format(exception.GetType())}: {exception.Message}",
						exception));
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
					AppendMissingMember(failureBuilder, MemberType.Field, fieldMemberPath);
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
					AppendMissingMember(failureBuilder, MemberType.Property, propertyMemberPath);
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
				AppendDifference(failureBuilder, memberType, memberPath, actual, expected);
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
				failureBuilder.AppendLine();
				if (failureBuilder.Length > 2)
				{
					failureBuilder.AppendLine("and");
				}

				failureBuilder.Append("  ");
				failureBuilder.Append(GetMemberPath(MemberType.Element, elementMemberPath));
				failureBuilder.Append(" had superfluous ");
				Formatter.Format(failureBuilder, actualObject, FormattingOptions.SingleLine);
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

			failureBuilder.AppendLine();
			if (failureBuilder.Length > 2)
			{
				failureBuilder.AppendLine("and");
			}

			failureBuilder.Append("  ");
			failureBuilder.Append(GetMemberPath(MemberType.Element, elementMemberPath));
			failureBuilder.Append(" was missing ");
			Formatter.Format(failureBuilder, expectedObject, FormattingOptions.SingleLine);
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

		int[]? keys = null;
		if (typeOptions.IgnoreCollectionOrder)
		{
			keys = new int[actualObjects.Length];
			for (int i = 0; i < actualObjects.Length; i++)
			{
				keys[i] = i;
			}

			Array.Sort(actualObjects, keys);
			Array.Sort(expectedObjects);
		}

		for (int i = 0; i < Math.Min(actualObjects.Length, expectedObjects.Length); i++)
		{
			string elementMemberPath = $"{memberPath}[{(keys is null ? i : keys[i])}]";
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

				failureBuilder.AppendLine();
				if (failureBuilder.Length > 2)
				{
					failureBuilder.AppendLine("and");
				}

				failureBuilder.Append("  ");
				failureBuilder.Append(GetMemberPath(MemberType.Element, elementMemberPath));
				failureBuilder.Append(" was missing ");
				Formatter.Format(failureBuilder, expectedObject, FormattingOptions.SingleLine);
				result = false;
			}
		}

		if (expectedObjects.Length < actualObjects.Length)
		{
			for (int i = expectedObjects.Length; i < actualObjects.Length; i++)
			{
				string elementMemberPath = $"{memberPath}[{(keys is null ? i : keys[i])}]";
				object? actualObject = actualObjects.ElementAtOrDefault(i);
				if (typeOptions.MembersToIgnore.Any(memberToIgnore
					    => AppliesTo(memberToIgnore, MemberType.Element) &&
					       memberToIgnore.IgnoreMember(elementMemberPath, actualObject?.GetType() ?? typeof(object))))
				{
					continue;
				}

				failureBuilder.AppendLine();
				if (failureBuilder.Length > 2)
				{
					failureBuilder.AppendLine("and");
				}

				failureBuilder.Append("  ");
				failureBuilder.Append(GetMemberPath(MemberType.Element, elementMemberPath));
				failureBuilder.Append(" had superfluous ");
				Formatter.Format(failureBuilder, actualObject, FormattingOptions.SingleLine);
				result = false;
			}
		}

		return result;
	}
#pragma warning restore S107
#pragma warning restore S3776
}
