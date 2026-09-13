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
using aweXpect.Results;

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
			failureBuilder.AppendLine();
			if (failureBuilder.Length > 2)
			{
				failureBuilder.AppendLine("and");
			}

			failureBuilder.Append("  ");
			failureBuilder.Append(GetMemberPath(memberType, memberPath));
			failureBuilder.AppendLine(" differed:");
			failureBuilder.Append("       Found: ");
			Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
			failureBuilder.AppendLine().Append("    Expected: ");
			Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
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

		failureBuilder.AppendLine();
		if (failureBuilder.Length > 2)
		{
			failureBuilder.AppendLine("and");
		}

		failureBuilder.Append("  ");
		failureBuilder.Append(GetMemberPath(memberType, memberPath));
		failureBuilder.Append(" was ");
		Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
		failureBuilder.Append(" instead of ");
		Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
		return false;
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
				failureBuilder.AppendLine();
				if (failureBuilder.Length > 2)
				{
					failureBuilder.AppendLine("and");
				}

				failureBuilder.Append("  ");
				failureBuilder.Append(GetMemberPath(memberType, memberPath));
				result.AppendResult(failureBuilder, "  ");
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

		try
		{
			if (!context.ComparedObjects.Add(actual) || actual.Equals(expected))
			{
				return true;
			}
		}
		catch (Exception exception)
		{
			throw new InvalidOperationException(
					$"The equals method of {Formatter.Format(actual.GetType())} threw an {Formatter.Format(exception.GetType())}: {exception.Message}",
					exception)
				.LogTrace();
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
					    => memberToIgnore is not MemberToIgnore.ByPropertyPredicate &&
					       memberToIgnore.IgnoreMember(fieldMemberPath, field.DeclaredType)))
				{
					continue;
				}

				object? actualFieldValue =
					EquivalencyMembers.FindField(actual.GetType(), field.Name, typeOptions.Fields)?.Invoke(actual);
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
					    => memberToIgnore is not MemberToIgnore.ByFieldPredicate &&
					       memberToIgnore.IgnoreMember(propertyMemberPath, property.DeclaredType)))
				{
					continue;
				}

				object? actualPropertyValue =
					EquivalencyMembers.FindProperty(actual.GetType(), property.Name, typeOptions.Properties)
						?.Invoke(actual);
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
				failureBuilder.AppendLine();
				if (failureBuilder.Length > 2)
				{
					failureBuilder.AppendLine("and");
				}

				failureBuilder.Append("  ");
				failureBuilder.Append(GetMemberPath(memberType, memberPath));
				failureBuilder.AppendLine(" differed:");
				failureBuilder.Append("       Found: ");
				Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
				failureBuilder.AppendLine().Append("    Expected: ");
				Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
				result = false;
			}
			else if (typeOptions.Fields != IncludeMembers.None || typeOptions.Properties != IncludeMembers.None)
			{
				throw new InvalidOperationException(
						$"{GetMemberPath(memberType, memberPath)} has no members that could be compared on {Formatter.Format(expected.GetType())}, which would make the equivalency comparison succeed without verifying anything. Adjust the equivalency options to include the relevant members or to compare this type by value, or - when publishing with trimming or Native AOT enabled - ensure that the type is rooted, so that its members are preserved.")
					.LogTrace();
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
				    => memberToIgnore is not MemberToIgnore.ByFieldPredicate &&
				       memberToIgnore is not MemberToIgnore.ByPropertyPredicate &&
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
				    => memberToIgnore is not MemberToIgnore.ByFieldPredicate &&
				       memberToIgnore is not MemberToIgnore.ByPropertyPredicate &&
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
				    => memberToIgnore is not MemberToIgnore.ByFieldPredicate &&
				       memberToIgnore is not MemberToIgnore.ByPropertyPredicate &&
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
					    => memberToIgnore is not MemberToIgnore.ByFieldPredicate &&
					       memberToIgnore is not MemberToIgnore.ByPropertyPredicate &&
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
					    => memberToIgnore is not MemberToIgnore.ByFieldPredicate &&
					       memberToIgnore is not MemberToIgnore.ByPropertyPredicate &&
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
