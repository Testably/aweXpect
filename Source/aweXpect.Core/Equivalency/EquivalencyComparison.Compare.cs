using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
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
	/// <remarks>
	///     When only <paramref name="expected" /> is compared by value, its <see cref="object.Equals(object)" />
	///     decides, because the type of <paramref name="actual" /> is compared by members, which ignores its
	///     <see cref="object.Equals(object)" />.
	/// </remarks>
	private static bool CompareByValue<TActual, TExpected>(
		[DisallowNull] TActual actual,
		[DisallowNull] TExpected expected,
		bool isDecidedByExpected,
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

		bool isEqual = UserCode.Invoke(() => isDecidedByExpected ? expected.Equals(actual) : actual.Equals(expected),
			() => UserCode.EqualsOf(isDecidedByExpected ? expected : actual));
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

	/// <remarks>
	///     Mirrors the wording of dictionary <c>IsEqualTo</c> for an expected key that the key comparer of the actual
	///     dictionary considers the same as another expected key, so that one entry cannot stand in for both.
	/// </remarks>
	private static void AppendLackedDistinctKey(StringBuilder failureBuilder, string memberPath,
		EquivalencyContext context)
	{
		AppendEntry(failureBuilder, MemberType.Element, memberPath, context);
		failureBuilder.Append(" lacked a distinct key");
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
		bool isAmbiguous, EquivalencyContext context)
	{
		AppendEntry(failureBuilder, memberType, memberPath, context);
		failureBuilder.Append(isAmbiguous
			? " is ambiguous on the actual object, which implements it explicitly for more than one interface"
			: " is missing on the actual object");
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

	/// <remarks>
	///     Asked for both sides, and one of them being compared by value is enough: walking the members of the
	///     expected object would otherwise reduce a value such as a string to the few public members it happens to
	///     have, so that any object with a matching <c>Length</c> would be equivalent to it, and the result would
	///     change when subject and expectation are swapped.
	/// </remarks>
	private static bool IsComparedByValue(Type type, EquivalencyTypeOptions typeOptions,
		EquivalencyOptions equivalencyOptions)
		=> (typeOptions.ComparisonType ?? equivalencyOptions.DefaultComparisonTypeSelector.Invoke(type))
		   == EquivalencyComparisonType.ByValue;

#pragma warning disable S3776 // https://rules.sonarsource.com/csharp/RSPEC-3776
#pragma warning disable S107 // https://rules.sonarsource.com/csharp/RSPEC-107
	/// <remarks>
	///     Receives the options of the enclosing object instead of those of <paramref name="actual" />, because the
	///     options that apply to <paramref name="expected" /> have to be looked up from there as well.
	/// </remarks>
	private static async ValueTask<bool>
		Compare<TActual, TExpected>(
			TActual actual,
			TExpected expected,
			EquivalencyOptions equivalencyOptions,
			EquivalencyTypeOptions parentTypeOptions,
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

		EquivalencyTypeOptions typeOptions = equivalencyOptions.GetTypeOptions(actual.GetType(), parentTypeOptions);
		if (IsComparedByValue(actual.GetType(), typeOptions, equivalencyOptions))
		{
			return CompareByValue(actual, expected, false, failureBuilder, memberPath, memberType, context);
		}

		if (IsComparedByValue(expected.GetType(),
			    equivalencyOptions.GetTypeOptions(expected.GetType(), parentTypeOptions), equivalencyOptions))
		{
			return CompareByValue(actual, expected, true, failureBuilder, memberPath, memberType, context);
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

			if (TryGetDictionary(actual, out IDictionary? actualDictionary, out object? actualKeyComparer) &&
			    TryGetDictionary(expected, out IDictionary? expectedDictionary, out _))
			{
				return await CompareDictionaries(actualDictionary, actualKeyComparer, expectedDictionary,
					failureBuilder, memberType, memberPath, equivalencyOptions, typeOptions, context);
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
	///     which is also the kind a scoped ignore rule applies to. Only when neither kind exists does a property the
	///     actual type implements explicitly for an interface match by its short name.
	/// </remarks>
	private static async ValueTask<bool>
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

				bool isAmbiguous = false;
				Func<object, object?>? actualFieldAccessor =
					EquivalencyMembers.FindField(actual.GetType(), field.Name, typeOptions.Fields) ??
					EquivalencyMembers.FindProperty(actual.GetType(), field.Name, typeOptions.Properties) ??
					EquivalencyMembers.FindExplicitProperty(actual.GetType(), field.Name, typeOptions.Properties,
						out isAmbiguous);
				if (actualFieldAccessor is null)
				{
					AppendMissingMember(failureBuilder, MemberType.Field, fieldMemberPath, isAmbiguous, context);
					result = false;
					continue;
				}

				object? actualFieldValue = UserCode.Invoke(actualFieldAccessor, actual, fieldMemberPath);
				object? expectedFieldValue = UserCode.Invoke(field.GetValue, expected, fieldMemberPath);

				if (!await Compare(actualFieldValue, expectedFieldValue,
					    options, typeOptions,
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

				bool isAmbiguous = false;
				Func<object, object?>? actualPropertyAccessor =
					EquivalencyMembers.FindProperty(actual.GetType(), property.Name, typeOptions.Properties) ??
					EquivalencyMembers.FindField(actual.GetType(), property.Name, typeOptions.Fields) ??
					EquivalencyMembers.FindExplicitProperty(actual.GetType(), property.Name, typeOptions.Properties,
						out isAmbiguous);
				if (actualPropertyAccessor is null)
				{
					AppendMissingMember(failureBuilder, MemberType.Property, propertyMemberPath, isAmbiguous, context);
					result = false;
					continue;
				}

				object? actualPropertyValue = UserCode.Invoke(actualPropertyAccessor, actual, propertyMemberPath);
				object? expectedPropertyValue = UserCode.Invoke(property.GetValue, expected, propertyMemberPath);

				if (!await Compare(actualPropertyValue, expectedPropertyValue,
					    options, typeOptions,
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

	/// <remarks>
	///     A dictionary is a keyed lookup and not a sequence, so it is compared by key, whatever order it enumerates
	///     its entries in. The non-generic <see cref="IDictionary" /> offers that lookup directly, while a type that
	///     only implements <see cref="IReadOnlyDictionary{TKey,TValue}" /> or <see cref="IDictionary{TKey,TValue}" />
	///     cannot be asked for a key without its type arguments, so its entries are copied into one. The copy
	///     compares its keys like the equality comparer of the original dictionary, and with their own
	///     <see cref="object.Equals(object)" /> when that comparer cannot be read or only orders the keys. The
	///     <paramref name="keyComparer" /> is the one the returned <paramref name="dictionary" /> decides with. Anything
	///     the copy cannot represent - an entry that is not a <see cref="KeyValuePair{TKey,TValue}" /> the members of
	///     which are readable, or a <see langword="null" /> key - keeps the comparison as a sequence instead of failing.
	/// </remarks>
	private static bool TryGetDictionary(object value, [NotNullWhen(true)] out IDictionary? dictionary,
		out object? keyComparer)
	{
		keyComparer = null;
		if (value is IDictionary nonGenericDictionary)
		{
			dictionary = nonGenericDictionary;
			keyComparer = GetKeyComparer(value);
			return true;
		}

		dictionary = null;
		if (!ImplementsGenericInterface(value,
			    definition => definition == typeof(IReadOnlyDictionary<,>) || definition == typeof(IDictionary<,>)))
		{
			return false;
		}

		IEqualityComparer<object>? equalityComparer = GetKeyComparer(value) as IEqualityComparer<object>;
		keyComparer = equalityComparer;
		Dictionary<object, object?> entries = new(equalityComparer);
		Func<object, object?>? getKey = null;
		Func<object, object?>? getValue = null;
		foreach (object? entry in (IEnumerable)value)
		{
			if (entry is null)
			{
				return false;
			}

			getKey ??= EquivalencyMembers.FindProperty(entry.GetType(),
				nameof(KeyValuePair<object, object>.Key), IncludeMembers.Public);
			getValue ??= EquivalencyMembers.FindProperty(entry.GetType(),
				nameof(KeyValuePair<object, object>.Value), IncludeMembers.Public);
			if (getKey is null || getValue is null || getKey(entry) is not { } key)
			{
				return false;
			}

			entries[key] = getValue(entry);
		}

		dictionary = entries;
		return true;
	}

	/// <summary>
	///     Returns a comparer for keys of type <see cref="object" /> that decides like the key comparer of the
	///     <paramref name="dictionary" />, or <see langword="null" /> when that comparer cannot be read.
	/// </summary>
	/// <remarks>
	///     The type arguments of the dictionary are out of reach here, so its comparer cannot be read through a type
	///     check. It is read instead from the public <c>Comparer</c> or <c>KeyComparer</c> property that the dictionaries
	///     of the framework expose, and a <see cref="ReadOnlyDictionary{TKey,TValue}" /> is asked for the dictionary it
	///     wraps. This needs reflection, so it is only attempted while the <see cref="ReflectionFallback" /> is
	///     supported, which it is not by default when publishing with Native AOT.
	/// </remarks>
	private static object? GetKeyComparer(object dictionary)
	{
		if (!ReflectionFallback.IsSupported)
		{
			return null;
		}

		Type type = dictionary.GetType();
		if (IsReadOnlyDictionary(type))
		{
			return type.FindProperty("Dictionary", IncludeMembers.Private)?.GetValue(dictionary) is { } inner
				? GetKeyComparer(inner)
				: null;
		}

		PropertyInfo? property = type.FindProperty("Comparer", IncludeMembers.Public) ??
		                         type.FindProperty("KeyComparer", IncludeMembers.Public);
		if (property?.GetValue(dictionary) is not { } comparer || !property.PropertyType.IsGenericType)
		{
			return null;
		}

		Type definition = property.PropertyType.GetGenericTypeDefinition();
		if (definition == typeof(IEqualityComparer<>))
		{
			return new KeyEqualityComparer(comparer, property.PropertyType);
		}

		return definition == typeof(IComparer<>)
			? new KeyOrderComparer(comparer, property.PropertyType)
			: null;
	}

	private static bool IsReadOnlyDictionary(Type type)
	{
		for (Type? current = type; current is not null; current = current.BaseType)
		{
			if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(ReadOnlyDictionary<,>))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	///     Creates an empty set of keys that treats keys as the same exactly when the <paramref name="keyComparer" />
	///     does, or with their own <see cref="object.Equals(object)" /> when there is none.
	/// </summary>
	private static ISet<object> CreateKeySet(object? keyComparer)
		=> keyComparer switch
		{
			IEqualityComparer<object> equalityComparer => new HashSet<object>(equalityComparer),
			IComparer<object> comparer => new SortedSet<object>(comparer),
			_ => new HashSet<object>(),
		};

	/// <summary>
	///     Invokes the <paramref name="method" /> of a comparer, with the exception it throws instead of the wrapping
	///     <see cref="TargetInvocationException" />.
	/// </summary>
	private static object? InvokeComparer(MethodInfo method, object comparer, params object[] arguments)
	{
		try
		{
			return method.Invoke(comparer, arguments);
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
			throw;
		}
	}

	/// <summary>
	///     Compares keys with the <see cref="IEqualityComparer{T}" /> <paramref name="comparer" /> of a dictionary, whose
	///     type argument is only known at runtime.
	/// </summary>
	/// <remarks>
	///     A <see langword="null" /> key, or one that is not of the key type, is compared with its own
	///     <see cref="object.Equals(object)" /> and never handed to the comparer, which would reject it.
	/// </remarks>
#if NET8_0_OR_GREATER
	[UnconditionalSuppressMessage("Trimming", "IL2070",
		Justification = "The methods of IEqualityComparer<T> are kept, because the dictionary calls them.")]
#endif
	private sealed class KeyEqualityComparer(object comparer, Type comparerType) : IEqualityComparer<object>
	{
		private readonly MethodInfo _equals = comparerType.GetMethod(nameof(IEqualityComparer<object>.Equals))!;

		private readonly MethodInfo _getHashCode =
			comparerType.GetMethod(nameof(IEqualityComparer<object>.GetHashCode))!;

		private readonly Type _keyType = comparerType.GetGenericArguments()[0];

		bool IEqualityComparer<object>.Equals(object? x, object? y)
			=> _keyType.IsInstanceOfType(x) && _keyType.IsInstanceOfType(y)
				? (bool)InvokeComparer(_equals, comparer, x!, y!)!
				: Equals(x, y);

		int IEqualityComparer<object>.GetHashCode(object obj)
			=> _keyType.IsInstanceOfType(obj)
				? (int)InvokeComparer(_getHashCode, comparer, obj)!
				: obj.GetHashCode();
	}

	/// <summary>
	///     Orders keys with the <see cref="IComparer{T}" /> <paramref name="comparer" /> of a sorted dictionary, whose
	///     type argument is only known at runtime.
	/// </summary>
	/// <remarks>
	///     Only orders the keys that the dictionary itself holds or found, which are of its key type and not
	///     <see langword="null" />.
	/// </remarks>
#if NET8_0_OR_GREATER
	[UnconditionalSuppressMessage("Trimming", "IL2070",
		Justification = "The methods of IComparer<T> are kept, because the dictionary calls them.")]
#endif
	private sealed class KeyOrderComparer(object comparer, Type comparerType) : IComparer<object>
	{
		private readonly MethodInfo _compare = comparerType.GetMethod(nameof(IComparer<object>.Compare))!;

		int IComparer<object>.Compare(object? x, object? y) => (int)InvokeComparer(_compare, comparer, x!, y!)!;
	}

	/// <remarks>
	///     A set has no order, so comparing two of them by position would report a difference that says nothing about
	///     their content. One side being a set is enough: the other side has nothing left to be compared against in
	///     order.
	/// </remarks>
	private static bool IsSet(object value) => ImplementsGenericInterface(value, IsSetInterface);

	/// <remarks>
	///     netstandard2.0 has no <c>IReadOnlySet&lt;T&gt;</c>, but is served to runtimes that have it, so it is
	///     matched by name there.
	/// </remarks>
	private static bool IsSetInterface(Type definition)
#if NET8_0_OR_GREATER
		=> definition == typeof(ISet<>) || definition == typeof(IReadOnlySet<>);
#else
		=> definition == typeof(ISet<>) ||
		   definition.FullName == "System.Collections.Generic.IReadOnlySet`1";
#endif

	/// <remarks>
	///     The trimmer keeps the implementations of an interface it keeps, and every generic definition this is
	///     matched against is referenced here, so the interfaces that decide the comparison survive trimming.
	/// </remarks>
#if NET8_0_OR_GREATER
	[UnconditionalSuppressMessage("Trimming", "IL2075",
		Justification = "The matched interfaces are referenced, so they are not trimmed away.")]
#endif
	private static bool ImplementsGenericInterface(object value, Func<Type, bool> matchesDefinition)
		=> value.GetType().GetInterfaces()
			.Any(interfaceType => interfaceType.IsGenericType &&
			                      matchesDefinition(interfaceType.GetGenericTypeDefinition()));

	/// <remarks>
	///     Every expected key is looked up through the actual dictionary, so that its key comparer decides which keys
	///     are the same, as it does for <c>IsEqualTo</c>. The matched keys are collected with the
	///     <paramref name="actualKeyComparer" />, so two expected keys that it considers the same count once, and the
	///     second one is reported as lacking a distinct key. Without that comparer, an actual key only counts as matched
	///     when it equals a matched expected key by its own <see cref="object.Equals(object)" />. The remaining keys are
	///     therefore only named as superfluous when as many were found as the entry count asks for, because a comparer
	///     that considers more keys equal than the default one lets that scan overshoot; otherwise only the counts are
	///     reported.
	/// </remarks>
	private static async ValueTask<bool>
		CompareDictionaries(
			IDictionary actual,
			object? actualKeyComparer,
			IDictionary expected,
			StringBuilder failureBuilder,
			MemberType memberType,
			string memberPath,
			EquivalencyOptions options,
			EquivalencyTypeOptions typeOptions,
			EquivalencyContext context)
	{
		bool result = true;
		ISet<object> matchedKeys = CreateKeySet(actualKeyComparer);
		HashSet<int> collapsedKeyIndices = [];
		int index = 0;
		foreach (object? key in expected.Keys)
		{
			if (actual.Contains(key) && !matchedKeys.Add(key))
			{
				collapsedKeyIndices.Add(index);
			}

			index++;
		}

		if (actual.Count != matchedKeys.Count)
		{
			List<object> additionalKeys = [];
			foreach (object? key in actual.Keys)
			{
				if (!matchedKeys.Contains(key))
				{
					additionalKeys.Add(key);
				}
			}

			if (additionalKeys.Count == actual.Count - matchedKeys.Count)
			{
				foreach (object key in additionalKeys)
				{
					string elementMemberPath = $"{memberPath}[{key}]";
					object? actualObject = actual[key];
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
			else
			{
				AppendEntry(failureBuilder, memberType, memberPath, context);
				failureBuilder.Append(" contained ").Append(actual.Count).Append(actual.Count == 1 ? " key" : " keys")
					.Append(" and matched ").Append(matchedKeys.Count)
					.Append(matchedKeys.Count == 1 ? " expected key" : " expected keys");
				result = false;
			}
		}

		index = 0;
		foreach (object? key in expected.Keys)
		{
			bool isCollapsed = collapsedKeyIndices.Contains(index++);
			string elementMemberPath = $"{memberPath}[{key}]";
			if (!matchedKeys.Contains(key))
			{
				object? expectedObject = expected[key];
				if (typeOptions.MembersToIgnore.Any(memberToIgnore
					    => AppliesTo(memberToIgnore, MemberType.Element) &&
					       memberToIgnore.IgnoreMember(elementMemberPath, expectedObject?.GetType() ?? typeof(object))))
				{
					continue;
				}

				AppendMissingElement(failureBuilder, elementMemberPath, expectedObject, context);
				result = false;
				continue;
			}

			object? actualObject = actual[key];
			if (typeOptions.MembersToIgnore.Any(memberToIgnore
				    => AppliesTo(memberToIgnore, MemberType.Element) &&
				       memberToIgnore.IgnoreMember(elementMemberPath, actualObject?.GetType() ?? typeof(object))))
			{
				continue;
			}

			if (isCollapsed)
			{
				AppendLackedDistinctKey(failureBuilder, elementMemberPath, context);
				result = false;
			}

			if (!await Compare(actualObject, expected[key],
				    options, typeOptions,
				    failureBuilder, elementMemberPath, MemberType.Element, context))
			{
				result = false;
			}
		}

		return result;
	}
	private static async ValueTask<bool>
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

		if (typeOptions.IgnoreCollectionOrder || IsSet(actual) || IsSet(expected))
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
				    options, typeOptions,
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
	private static async ValueTask<bool>
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
					options, typeOptions,
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
		public async ValueTask
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
		public async ValueTask<(int Actual, int Expected)[]>
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

		private async ValueTask<bool>
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
		private async ValueTask<(bool IsEquivalent, int DifferenceCount)>
			GetResult(int actualIndex, int expectedIndex)
		{
			if (_results[actualIndex, expectedIndex] is { } cachedResult)
			{
				return (cachedResult, _differenceCounts[actualIndex, expectedIndex]);
			}

			object? actualObject = _actualObjects[_actualIndices[actualIndex]];
			int differenceCount = _context.DifferenceCount;
			bool isEquivalent = await Compare(actualObject, _expectedObjects[_expectedIndices[expectedIndex]],
				_options, _typeOptions,
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
