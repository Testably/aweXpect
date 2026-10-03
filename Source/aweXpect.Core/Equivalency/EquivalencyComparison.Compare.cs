using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
	private static readonly ConcurrentDictionary<Type, TypeShape> TypeShapes = new();

	/// <remarks>
	///     When only <paramref name="expected" /> is compared by value, its <see cref="object.Equals(object)" />
	///     decides, because the type of <paramref name="actual" /> is compared by members, which ignores its
	///     <see cref="object.Equals(object)" />.<br />
	///     A type that stands for its content, such as a <see cref="StringBuilder" /> for its text, is compared as that
	///     content (see <see cref="EquivalencyContent" />).
	/// </remarks>
	private static bool CompareByValue<TActual, TExpected>(
		[DisallowNull] TActual actual,
		[DisallowNull] TExpected expected,
		bool isDecidedByExpected,
		StringBuilder failureBuilder,
		MemberPath memberPath,
		MemberType memberType,
		EquivalencyContext context)
	{
		if (EquivalencyContent.IsComparedByContent(actual.GetType()) ||
		    EquivalencyContent.IsComparedByContent(expected.GetType()))
		{
			string? thrower = GetThrower(memberPath.ToString());
			return CompareByValue(EquivalencyContent.GetContent(actual, thrower),
				EquivalencyContent.GetContent(expected, thrower), isDecidedByExpected,
				failureBuilder, memberPath, memberType, context);
		}

		if (DateTimeKindComparison.AreKindsIncompatible(actual, expected))
		{
			AppendDifference(failureBuilder, memberType, memberPath.ToString(), actual, expected, context);
			return false;
		}

		bool isEqual = UserCode.Invoke(
			static values => values.IsDecidedByExpected
				? values.Expected!.Equals(values.Actual)
				: values.Actual!.Equals(values.Expected),
			(Actual: actual, Expected: expected, IsDecidedByExpected: isDecidedByExpected),
			static values => UserCode.EqualsOf(values.IsDecidedByExpected ? values.Expected! : values.Actual!));
		if (!isEqual)
		{
			AppendDifference(failureBuilder, memberType, memberPath.ToString(), actual, expected, context);
			return false;
		}

		return true;
	}

	/// <summary>
	///     Who threw in the failure message when code of the caller fails while the value at the
	///     <paramref name="memberPath" /> is read: that member, or the subject itself at the root.
	/// </summary>
	private static string? GetThrower(string memberPath)
		=> string.IsNullOrEmpty(memberPath) ? null : memberPath;

	private static bool CompareNulls<TActual, TExpected>(TActual actual, TExpected expected,
		StringBuilder failureBuilder, MemberPath memberPath, MemberType memberType, EquivalencyContext context)
	{
		if (actual is null && expected is null)
		{
			return true;
		}

		AppendDifference(failureBuilder, memberType, memberPath.ToString(), actual, expected, context);
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

	/// <summary>
	///     Whether the entry that would be appended next is left out of the failure text.
	/// </summary>
	/// <remarks>
	///     While only counting, the entry is still counted as <see cref="AppendEntry" /> would.
	/// </remarks>
	private static bool SkipsText(EquivalencyContext context)
	{
		if (context.IsCountingOnly)
		{
			context.DifferenceCount++;
			return true;
		}

		return context.IsDecidingOnly;
	}

	private static void AppendDifference<TActual, TExpected>(StringBuilder failureBuilder,
		MemberType memberType, string memberPath, TActual actual, TExpected expected, EquivalencyContext context)
	{
		if (SkipsText(context))
		{
			return;
		}

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
		failureBuilder.Append("      Actual: ");
	}

	/// <remarks>
	///     Mirrors the wording of dictionary <c>IsEqualTo</c> for an expected key that the key comparer of the actual
	///     dictionary considers the same as another expected key, so that one entry cannot stand in for both.
	/// </remarks>
	private static void AppendLackedDistinctKey(StringBuilder failureBuilder, string memberPath,
		EquivalencyContext context)
	{
		if (SkipsText(context))
		{
			return;
		}

		AppendEntry(failureBuilder, MemberType.Element, memberPath, context);
		failureBuilder.Append(" lacked a distinct key");
	}

	private static void AppendMaxRecursionDepthExceeded(StringBuilder failureBuilder, MemberType memberType,
		string memberPath, int maxRecursionDepth, EquivalencyContext context)
	{
		if (SkipsText(context))
		{
			return;
		}

		AppendEntry(failureBuilder, memberType, memberPath, context);
		failureBuilder.Append(" exceeded the maximum recursion depth of ");
		failureBuilder.Append(maxRecursionDepth);
	}

	private static void AppendMissingElement(StringBuilder failureBuilder, string memberPath, object? expected,
		EquivalencyContext context)
	{
		if (SkipsText(context))
		{
			return;
		}

		AppendEntry(failureBuilder, MemberType.Element, memberPath, context);
		failureBuilder.Append(" was missing ");
		Formatter.Format(failureBuilder, expected, FormattingOptions.SingleLine);
	}

	private static void AppendMissingMember(StringBuilder failureBuilder, MemberType memberType, string memberPath,
		bool isAmbiguous, EquivalencyContext context)
	{
		if (SkipsText(context))
		{
			return;
		}

		AppendEntry(failureBuilder, memberType, memberPath, context);
		failureBuilder.Append(isAmbiguous
			? " was ambiguous on the actual object, which implements it explicitly for more than one interface"
			: " was missing on the actual object");
	}

	private static void AppendSuperfluousElement(StringBuilder failureBuilder, string memberPath, object? actual,
		EquivalencyContext context)
	{
		if (SkipsText(context))
		{
			return;
		}

		AppendEntry(failureBuilder, MemberType.Element, memberPath, context);
		failureBuilder.Append(" had superfluous ");
		Formatter.Format(failureBuilder, actual, FormattingOptions.SingleLine);
	}

	private static void AppendUnmatchedElement(StringBuilder failureBuilder, string memberPath,
		EquivalencyContext context)
	{
		if (SkipsText(context))
		{
			return;
		}

		AppendEntry(failureBuilder, MemberType.Element, memberPath, context);
		failureBuilder.Append(" matched no expected key");
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
	///     The <paramref name="memberPath" /> is only joined when a rule applies to the member, as most comparisons have no
	///     rule at all.
	/// </remarks>
	private static bool IsIgnored(MemberToIgnore[] membersToIgnore, MemberType memberType, MemberPath memberPath,
		Type type)
	{
		string? path = null;
#pragma warning disable S3267 // Every compared member is checked, so Any with a closure is avoided here
		foreach (MemberToIgnore memberToIgnore in membersToIgnore)
		{
			if (AppliesTo(memberToIgnore, memberType) &&
			    memberToIgnore.IgnoreMember(path ??= memberPath.ToString(), type))
			{
				return true;
			}
		}
#pragma warning restore S3267

		return false;
	}

	/// <summary>
	///     Reads a member of the <paramref name="subject" /> with the <paramref name="accessor" />, naming the member at
	///     the <paramref name="memberPath" /> as the thrower when it throws.
	/// </summary>
	private static object? ReadMember(Func<object, object?> accessor, object subject, MemberPath memberPath)
		=> UserCode.Invoke(static values => values.Accessor(values.Subject),
			(Accessor: accessor, Subject: subject, Path: memberPath),
			static values => values.Path.ToString());

	/// <summary>
	///     Checks if the stack has room for a further nested level.
	/// </summary>
	/// <remarks>
	///     The nested levels of a comparison usually complete synchronously, so each of them stays on the stack, and so
	///     do the continuations of levels that completed asynchronously on .NET Framework. When it runs low, the
	///     comparison continues on a fresh stack, as a raised <see cref="EquivalencyOptions.MaxRecursionDepth" /> or a
	///     small stack, e.g. of a thread on macOS, would otherwise overflow it.
	/// </remarks>
	private static bool HasSufficientStack()
	{
#if NET8_0_OR_GREATER
		return RuntimeHelpers.TryEnsureSufficientExecutionStack();
#else
		try
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			return true;
		}
		catch (InsufficientExecutionStackException)
		{
			return false;
		}
#endif
	}

	/// <summary>
	///     Reads the <paramref name="member" /> at the <paramref name="memberPath" /> of the <paramref name="actual" />
	///     and the <paramref name="expected" /> object, unless it is ignored or the <paramref name="actual" /> object
	///     misses it, which is added to the <paramref name="failureBuilder" /> and sets <paramref name="isMissing" />.
	/// </summary>
	/// <remarks>
	///     It does not compare the values, so that its state is no longer on the stack while the values are compared,
	///     which happens once for every nested level.
	/// </remarks>
	private static bool TryReadMember(EquivalencyMemberPlan.PlannedMember member, MemberType memberType,
		MemberPath memberPath, object actual, object expected, EquivalencyTypeOptions typeOptions,
		StringBuilder failureBuilder, EquivalencyContext context, out bool isMissing, out object? actualValue,
		out object? expectedValue)
	{
		actualValue = null;
		expectedValue = null;
		isMissing = false;
		if (IsIgnored(typeOptions.MembersToIgnore, memberType, memberPath, member.Expected.DeclaredType))
		{
			return false;
		}

		Func<object, object?>? actualAccessor = member.GetActualAccessor(out bool isAmbiguous);
		if (actualAccessor is null)
		{
			AppendMissingMember(failureBuilder, memberType, memberPath.ToString(), isAmbiguous, context);
			isMissing = true;
			return false;
		}

		actualValue = ReadMember(actualAccessor, actual, memberPath);
		expectedValue = ReadMember(member.Expected.GetValue, expected, memberPath);
		return true;
	}

	private static EquivalencyTypeOptions? GetRegisteredOptions(Type type, EquivalencyOptions equivalencyOptions,
		EquivalencyContext context)
	{
		if (!context.RegisteredOptions.TryGetValue(type, out EquivalencyTypeOptions? options))
		{
			options = equivalencyOptions.TryGetOptionsFor(type, out EquivalencyTypeOptions? registeredOptions)
				? registeredOptions
				: null;
			context.RegisteredOptions.Add(type, options);
		}

		return options;
	}

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

	/// <summary>
	///     Compares the <paramref name="actual" /> value with the expectation of an <c>It.Is…</c> in the expected
	///     object, or returns <see langword="null" /> when the expectation could not decide.
	/// </summary>
	/// <remarks>
	///     A separate method, because the comparison of a nested object is on the stack once for every nested level,
	///     and its state would otherwise also hold the evaluation of the expectation.
	/// </remarks>
	private static async ValueTask<bool?> CompareWithExpectation<TActual>(TActual actual,
		EquivalencyExpectationBuilder equivalencyExpectationBuilder, StringBuilder failureBuilder,
		MemberPath memberPath, MemberType memberType, EquivalencyContext context)
	{
		EvaluationContext evaluationContext = new();
		ConstraintResult? result;
		try
		{
			result = await equivalencyExpectationBuilder.IsMetBy(actual, evaluationContext,
				CancellationToken.None);
		}
		finally
		{
			await evaluationContext.ReleaseMaterializations();
		}

		if (result.Outcome == Outcome.Success)
		{
			return true;
		}

		if (result.Outcome != Outcome.Failure)
		{
			return null;
		}

		if (SkipsText(context))
		{
			return false;
		}

		AppendDifferenceHeader(failureBuilder, memberType, memberPath.ToString(), context);
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

#pragma warning disable S3776 // https://rules.sonarsource.com/csharp/RSPEC-3776
#pragma warning disable S107 // https://rules.sonarsource.com/csharp/RSPEC-107
	/// <remarks>
	///     Receives the options of the enclosing object instead of those of <paramref name="actual" />, because the
	///     options that apply to <paramref name="expected" /> have to be looked up from there as well. A registration
	///     for the type of <paramref name="expected" /> wins over one for the type of <paramref name="actual" />,
	///     because the members that are compared come from the expected object.
	/// </remarks>
	private static async ValueTask<bool>
		Compare<TActual, TExpected>(
			TActual actual,
			TExpected expected,
			EquivalencyOptions equivalencyOptions,
			EquivalencyTypeOptions parentTypeOptions,
			StringBuilder failureBuilder,
			MemberPath memberPath,
			MemberType memberType,
			EquivalencyContext context)
	{
		if (expected is Expectation &&
		    expected is IOptionsProvider<ExpectationBuilder>
		    {
			    Options: EquivalencyExpectationBuilder equivalencyExpectationBuilder,
		    } &&
		    await CompareWithExpectation(actual, equivalencyExpectationBuilder, failureBuilder, memberPath,
			    memberType, context) is { } isMetByExpectation)
		{
			return isMetByExpectation;
		}

		if (actual is null || expected is null)
		{
			return CompareNulls(actual, expected, failureBuilder, memberPath, memberType, context);
		}

		EquivalencyTypeOptions inheritedOptions = context.GetInheritedOptions(equivalencyOptions, parentTypeOptions);
		EquivalencyTypeOptions? actualOptions =
			GetRegisteredOptions(actual.GetType(), equivalencyOptions, context);
		EquivalencyTypeOptions? expectedOptions =
			GetRegisteredOptions(expected.GetType(), equivalencyOptions, context);
		if (IsComparedByValue(actual.GetType(), actualOptions ?? inheritedOptions, equivalencyOptions))
		{
			return CompareByValue(actual, expected, false, failureBuilder, memberPath, memberType, context);
		}

		if (IsComparedByValue(expected.GetType(), expectedOptions ?? inheritedOptions, equivalencyOptions))
		{
			return CompareByValue(actual, expected, true, failureBuilder, memberPath, memberType, context);
		}

		EquivalencyTypeOptions typeOptions = expectedOptions ?? actualOptions ?? inheritedOptions;

		ComparedPair comparedPair = new(actual, expected);
		if (!context.ComparedPairs.Add(comparedPair))
		{
			return true;
		}

		context.Depth++;
		try
		{
			string path = memberPath.ToString();
			if (context.Depth > equivalencyOptions.MaxRecursionDepth)
			{
				AppendMaxRecursionDepthExceeded(failureBuilder, memberType, path,
					equivalencyOptions.MaxRecursionDepth, context);
				return false;
			}

			if (!HasSufficientStack())
			{
				await Task.Yield();
			}

			bool isEquivalent;
			if (TryGetDictionary(actual, path, out IDictionary? actualDictionary,
				    out object? actualKeyComparer) &&
			    TryGetDictionary(expected, path, out IDictionary? expectedDictionary, out _))
			{
				isEquivalent = await CompareDictionaries(actualDictionary, actualKeyComparer, expectedDictionary,
					failureBuilder, memberType, path, equivalencyOptions, typeOptions, context);
			}
			else if (TryGetEnumerable(actual, out IEnumerable? actualEnumerable) &&
			         TryGetEnumerable(expected, out IEnumerable? expectedEnumerable))
			{
				isEquivalent = await CompareEnumerables(actualEnumerable, expectedEnumerable, failureBuilder, path,
					equivalencyOptions, typeOptions, context);
			}
			else
			{
				isEquivalent = await CompareObjects(actual, expected, failureBuilder, memberType, path,
					equivalencyOptions, typeOptions, context);
			}

			if (!HasSufficientStack())
			{
				await Task.Yield();
			}

			return isEquivalent;
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
		EquivalencyMemberPlan plan = EquivalencyMemberPlan.For(expected.GetType(), actual.GetType(),
			typeOptions.Fields, typeOptions.Properties);
		foreach (EquivalencyMemberPlan.PlannedMember field in plan.Fields)
		{
			memberCount++;
			MemberPath fieldMemberPath = MemberPath.Member(memberPath, field.Expected.Name);
			if (!TryReadMember(field, MemberType.Field, fieldMemberPath, actual, expected, typeOptions,
				    failureBuilder, context, out bool isMissing, out object? actualFieldValue,
				    out object? expectedFieldValue))
			{
				result &= !isMissing;
				continue;
			}

			if (!await Compare(actualFieldValue, expectedFieldValue,
				    options, typeOptions,
				    failureBuilder, fieldMemberPath, MemberType.Field, context))
			{
				result = false;
			}
		}

		foreach (EquivalencyMemberPlan.PlannedMember property in plan.Properties)
		{
			memberCount++;
			MemberPath propertyMemberPath = MemberPath.Member(memberPath, property.Expected.Name);
			if (!TryReadMember(property, MemberType.Property, propertyMemberPath, actual, expected, typeOptions,
				    failureBuilder, context, out bool isMissing, out object? actualPropertyValue,
				    out object? expectedPropertyValue))
			{
				result &= !isMissing;
				continue;
			}

			if (!await Compare(actualPropertyValue, expectedPropertyValue,
				    options, typeOptions,
				    failureBuilder, propertyMemberPath, MemberType.Property, context))
			{
				result = false;
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
						$"{GetMemberPath(memberType, memberPath)} has no members that could be compared on {Formatter.Format(expected.GetType())}, which would make the equivalency comparison succeed without verifying anything. Adjust the equivalency options to include the relevant members or to compare this type by value, or, when publishing with trimming or Native AOT enabled, ensure that the type is rooted, so that its members are preserved."));
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
	private static bool TryGetDictionary(object value, string memberPath,
		[NotNullWhen(true)] out IDictionary? dictionary, out object? keyComparer)
	{
		keyComparer = null;
		if (value is IDictionary nonGenericDictionary)
		{
			dictionary = nonGenericDictionary;
			keyComparer = GetKeyComparer(value);
			return true;
		}

		dictionary = null;
		if (GetTypeShape(value.GetType()).DictionaryInterface is null)
		{
			return false;
		}

		IEqualityComparer<object>? equalityComparer = GetKeyComparer(value) as IEqualityComparer<object>;
		keyComparer = equalityComparer;
		Dictionary<object, object?> entries = new(equalityComparer);
		Func<object, object?>? getKey = null;
		Func<object, object?>? getValue = null;
		foreach (object? entry in UserCode.Invoke(ToArray, (IEnumerable)value, GetThrower(memberPath)))
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
	///     The type arguments of the dictionary are out of reach here, so its comparer is read by the
	///     <see cref="DictionaryKeyComparer" /> for the generic dictionary interface it implements, the same way as for
	///     expectations that know them, through <see cref="KeyComparers" />.
	/// </remarks>
	private static object? GetKeyComparer(object dictionary)
		=> GetTypeShape(dictionary.GetType()).DictionaryInterface is { } dictionaryInterface
			? DictionaryKeyComparer.For(dictionaryInterface)?.Read(dictionary)
			: null;

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

	/// <remarks>
	///     A set has no order, so comparing two of them by position would report a difference that says nothing about
	///     their content. One side being a set is enough: the other side has nothing left to be compared against in
	///     order.
	/// </remarks>
	private static bool IsSet(object value) => GetTypeShape(value.GetType()).ImplementsSet;

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

	private static bool IsDictionaryDefinition(Type definition)
		=> definition == typeof(IReadOnlyDictionary<,>) || definition == typeof(IDictionary<,>);

	private static TypeShape GetTypeShape(Type type)
		=> TypeShapes.GetOrAdd(type, static key => new TypeShape(
			key.FindGenericInterface(IsDictionaryDefinition),
			key.FindGenericInterface(IsSetInterface) is not null));

	/// <summary>
	///     The generic dictionary interface that a type implements first, and whether it is a set.
	/// </summary>
	/// <remarks>
	///     Cached, because the interfaces of a type never change, while every object that is compared by its members is
	///     checked for a dictionary, and every sequence for a set.
	/// </remarks>
	private sealed class TypeShape(Type? dictionaryInterface, bool implementsSet)
	{
		public Type? DictionaryInterface { get; } = dictionaryInterface;
		public bool ImplementsSet { get; } = implementsSet;
	}

	/// <remarks>
	///     Every expected key is looked up through the actual dictionary, so that its key comparer decides which keys
	///     are the same, as it does for <c>IsEqualTo</c>. The matched keys are collected with the
	///     <paramref name="actualKeyComparer" />, so two expected keys that it considers the same count once, and the
	///     second one is reported as lacking a distinct key. Without that comparer, an actual key only counts as matched
	///     when it equals a matched expected key by its own <see cref="object.Equals(object)" />. The remaining keys are
	///     therefore only named as superfluous when as many were found as the entry count asks for, because a comparer
	///     that considers more keys equal than the default one lets that scan overshoot. When the entry count matches
	///     although some remain, they are reported as matching no expected key, because such a key can be a superfluous
	///     one hidden by two expected keys that the comparer considers the same, or merely spelled differently than the
	///     expected key it stands for. Otherwise only the counts are reported.
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
		HashSet<int> matchedKeyIndices = [];
		HashSet<int> collapsedKeyIndices = [];
		int index = 0;
		foreach (object? key in expected.Keys)
		{
			if (UserCode.Invoke(static entry => entry.actual.Contains(entry.key), (actual, key),
				    GetThrower(memberPath)))
			{
				matchedKeyIndices.Add(index);
				if (!matchedKeys.Add(key))
				{
					collapsedKeyIndices.Add(index);
				}
			}

			index++;
		}

		if (actual.Count != matchedKeys.Count || actualKeyComparer is null)
		{
			List<object> additionalKeys = [];
			foreach (object? key in actual.Keys)
			{
				if (!matchedKeys.Contains(key))
				{
					additionalKeys.Add(key);
				}
			}

			bool isUnmatched = additionalKeys.Count != actual.Count - matchedKeys.Count;
			if (!isUnmatched || actual.Count == matchedKeys.Count)
			{
				foreach (object key in additionalKeys)
				{
					string elementMemberPath = GetKeyPath(memberPath, key);
					object? actualObject = GetEntry(actual, key, elementMemberPath);
					if (IsIgnored(typeOptions.MembersToIgnore, MemberType.Element, elementMemberPath,
						    actualObject?.GetType() ?? typeof(object)))
					{
						continue;
					}

					if (isUnmatched)
					{
						AppendUnmatchedElement(failureBuilder, elementMemberPath, context);
					}
					else
					{
						AppendSuperfluousElement(failureBuilder, elementMemberPath, actualObject, context);
					}

					result = false;
				}
			}
			else
			{
				if (!SkipsText(context))
				{
					AppendEntry(failureBuilder, memberType, memberPath, context);
					failureBuilder.Append(" contained ").Append(actual.Count)
						.Append(actual.Count == 1 ? " key" : " keys")
						.Append(" and matched ").Append(matchedKeys.Count)
						.Append(matchedKeys.Count == 1 ? " expected key" : " expected keys");
				}

				result = false;
			}
		}

		index = 0;
		foreach (object? key in expected.Keys)
		{
			bool isMatched = matchedKeyIndices.Contains(index);
			bool isCollapsed = collapsedKeyIndices.Contains(index++);
			string elementMemberPath = GetKeyPath(memberPath, key);
			if (!isMatched)
			{
				object? expectedObject = GetEntry(expected, key, elementMemberPath);
				if (IsIgnored(typeOptions.MembersToIgnore, MemberType.Element, elementMemberPath,
					    expectedObject?.GetType() ?? typeof(object)))
				{
					continue;
				}

				AppendMissingElement(failureBuilder, elementMemberPath, expectedObject, context);
				result = false;
				continue;
			}

			object? actualObject = GetEntry(actual, key, elementMemberPath);
			if (IsIgnored(typeOptions.MembersToIgnore, MemberType.Element, elementMemberPath,
				    actualObject?.GetType() ?? typeof(object)))
			{
				continue;
			}

			if (isCollapsed)
			{
				AppendLackedDistinctKey(failureBuilder, elementMemberPath, context);
				result = false;
			}

			if (!await Compare(actualObject, GetEntry(expected, key, elementMemberPath),
				    options, typeOptions,
				    failureBuilder, elementMemberPath, MemberType.Element, context))
			{
				result = false;
			}
		}

		return result;
	}

	/// <remarks>
	///     The key is formatted without the current culture, so that a member path to ignore matches on every machine.
	/// </remarks>
	private static string GetKeyPath(string memberPath, object? key)
		=> $"{memberPath}[{(key is IFormattable formattable ? formattable.ToString(null, CultureInfo.InvariantCulture) : key)}]";

	private static object? GetEntry(IDictionary dictionary, object key, string elementMemberPath)
		=> UserCode.Invoke(static entry => entry.dictionary[entry.key], (dictionary, key), elementMemberPath);

	private static object?[] ToArray(IEnumerable enumerable) => enumerable.Cast<object?>().ToArray();

	/// <remarks>
	///     A <c>Memory&lt;T&gt;</c> or <c>ReadOnlyMemory&lt;T&gt;</c> is a sequence like an array, but implements no
	///     interface for it, and its <c>Span</c> cannot be read by reflection, so that only its length would be left to
	///     compare. Its items are therefore copied into an array, which needs reflection, as the item type is only known
	///     at runtime.
	/// </remarks>
	private static bool TryGetEnumerable(object value, [NotNullWhen(true)] out IEnumerable? enumerable)
	{
		if (value is IEnumerable valueEnumerable)
		{
			enumerable = valueEnumerable;
			return true;
		}

		Type type = value.GetType();
		if (!type.IsGenericType || !IsMemoryDefinition(type.GetGenericTypeDefinition()))
		{
			enumerable = null;
			return false;
		}

		if (!ReflectionFallback.IsSupported)
		{
			throw Tracing.WriteException(ReflectionFallback.NotSupported(
				$"The items of {Formatter.Format(type)}", "Compare the result of its ToArray() instead."));
		}

		enumerable = CopyItems(value);
		return true;
	}

	/// <remarks>
	///     netstandard2.0 has no <c>Memory&lt;T&gt;</c>, but is served to runtimes that have it, so it is matched by name
	///     there.
	/// </remarks>
	private static bool IsMemoryDefinition(Type definition)
#if NET8_0_OR_GREATER
		=> definition == typeof(Memory<>) || definition == typeof(ReadOnlyMemory<>);
#else
		=> definition.FullName is "System.Memory`1" or "System.ReadOnlyMemory`1";
#endif

#if NET8_0_OR_GREATER
	[UnconditionalSuppressMessage("Trimming", "IL2075",
		Justification = "Only called behind the reflection fallback guard.")]
#endif
	private static IEnumerable CopyItems(object memory)
		=> (IEnumerable)memory.GetType().GetMethod("ToArray", Type.EmptyTypes)!.Invoke(memory, null)!;

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
		object?[] actualObjects = UserCode.Invoke(ToArray, actual, GetThrower(memberPath));
		object?[] expectedObjects = UserCode.Invoke(ToArray, expected, GetThrower(memberPath));

		if (typeOptions.IgnoreCollectionOrder || IsSet(actual) || IsSet(expected))
		{
			return await CompareInAnyOrder(actualObjects, expectedObjects, failureBuilder, memberPath, options,
				typeOptions, context);
		}

		for (int i = 0; i < Math.Min(actualObjects.Length, expectedObjects.Length); i++)
		{
			MemberPath elementMemberPath = MemberPath.Element(memberPath, i);
			object? actualObject = actualObjects[i];
			if (IsIgnored(typeOptions.MembersToIgnore, MemberType.Element, elementMemberPath,
				    actualObject?.GetType() ?? typeof(object)))
			{
				continue;
			}

			object? expectedObject = expectedObjects[i];

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
				object? expectedObject = expectedObjects[i];
				if (IsIgnored(typeOptions.MembersToIgnore, MemberType.Element, elementMemberPath,
					    expectedObject?.GetType() ?? typeof(object)))
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
				object? actualObject = actualObjects[i];
				if (IsIgnored(typeOptions.MembersToIgnore, MemberType.Element, elementMemberPath,
					    actualObject?.GetType() ?? typeof(object)))
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
		if (context.IsDecidingOnly)
		{
			return !matcher.HasLeftovers;
		}

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
		if (typeOptions.MembersToIgnore.Length == 0)
		{
			int[] all = new int[objects.Length];
			for (int i = 0; i < all.Length; i++)
			{
				all[i] = i;
			}

			return all;
		}

		List<int> indices = new(objects.Length);
		for (int i = 0; i < objects.Length; i++)
		{
			object? element = objects[i];
			string elementMemberPath = $"{memberPath}[{i}]";
			if (!IsIgnored(typeOptions.MembersToIgnore, MemberType.Element, elementMemberPath,
				    element?.GetType() ?? typeof(object)))
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
		///     The number of differences of the pairs that were considered for the leftovers.
		/// </summary>
		/// <remarks>
		///     Only the leftovers need the count, and counting the differences means writing them, so the search for
		///     augmenting paths only decides the pairs it considers.
		/// </remarks>
		private int?[,]? _differenceCounts;

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
		private readonly PairResults _results;

		private readonly EquivalencyTypeOptions _typeOptions;

		/// <summary>
		///     The expected elements that no actual element could be matched to.
		/// </summary>
		private readonly List<int> _unmatchedExpected = [];

		/// <summary>
		///     The builder for the comparisons that only decide a pair, which never write into it.
		/// </summary>
		private readonly StringBuilder _unusedFailureBuilder = new();

		/// <summary>
		///     The search in which each actual element was last visited, so that a search does not need its own array.
		/// </summary>
		private readonly int[] _visitedInSearch;

		private int _search;

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
			_results = new PairResults(actualIndices.Length, expectedIndices.Length);
			_visitedInSearch = new int[actualIndices.Length];
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
			int previous = -1;
			for (int i = 0; i < _expectedIndices.Length; i++)
			{
				previous = await TryMatchNextTo(previous, i);
				if (previous < 0)
				{
					_search++;
					previous = await TryMatch(i);
				}

				if (previous < 0)
				{
					_unmatchedExpected.Add(i);
				}
			}
		}

		/// <summary>
		///     Whether an actual or an expected element is left without a counterpart.
		/// </summary>
		public bool HasLeftovers => _unmatchedExpected.Count > 0 || Array.IndexOf(_matchedTo, -1) >= 0;

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
					candidates.Add((await GetDifferenceCount(actualIndex, expectedIndex), actualIndex, expectedIndex));
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

		/// <summary>
		///     Matches the expected element to a free neighbour of the <paramref name="previous" /> actual element, and
		///     returns its index, or <c>-1</c> when neither neighbour is equivalent.
		/// </summary>
		/// <remarks>
		///     A collection in or against the expected order finds each match next to the previous one, so the element
		///     is not compared with every free actual element first.
		/// </remarks>
		private async ValueTask<int>
			TryMatchNextTo(int previous, int expectedIndex)
		{
			if (previous < 0)
			{
				return -1;
			}

			for (int actualIndex = previous + 1; actualIndex >= previous - 1; actualIndex -= 2)
			{
				if (actualIndex >= 0 && actualIndex < _actualIndices.Length && _matchedTo[actualIndex] < 0 &&
				    await IsEquivalent(actualIndex, expectedIndex))
				{
					_matchedTo[actualIndex] = expectedIndex;
					return actualIndex;
				}
			}

			return -1;
		}

		/// <returns>The index of the actual element the expected element is matched to, or <c>-1</c>.</returns>
		private async ValueTask<int>
			TryMatch(int expectedIndex)
		{
			for (int offset = 0; offset < _actualIndices.Length; offset++)
			{
				// Starting at the same position pairs collections that are already in order without any search.
				int actualIndex = (expectedIndex + offset) % _actualIndices.Length;
				if (_visitedInSearch[actualIndex] == _search || !await IsEquivalent(actualIndex, expectedIndex))
				{
					continue;
				}

				_visitedInSearch[actualIndex] = _search;
				if (_matchedTo[actualIndex] < 0 || await TryMatch(_matchedTo[actualIndex]) >= 0)
				{
					_matchedTo[actualIndex] = expectedIndex;
					return actualIndex;
				}
			}

			return -1;
		}

		/// <remarks>
		///     Only decides the pair without writing its differences, because only the pairs that are left over are
		///     reported. The code of the caller is still called as for a reported pair, so that it throws alike.
		/// </remarks>
		private async ValueTask<bool>
			IsEquivalent(int actualIndex, int expectedIndex)
		{
			if (_results.TryGet(actualIndex, expectedIndex, out bool cachedResult))
			{
				return cachedResult;
			}

			bool wasDecidingOnly = _context.IsDecidingOnly;
			bool wasCountingOnly = _context.IsCountingOnly;
			_context.IsDecidingOnly = true;
			_context.IsCountingOnly = false;
			bool isEquivalent;
			try
			{
				isEquivalent = await Compare(_actualObjects[_actualIndices[actualIndex]],
					_expectedObjects[_expectedIndices[expectedIndex]], _options, _typeOptions,
					_unusedFailureBuilder, GetElementPath(actualIndex), MemberType.Element, _context);
			}
			finally
			{
				_context.IsDecidingOnly = wasDecidingOnly;
				_context.IsCountingOnly = wasCountingOnly;
			}

			_results.Set(actualIndex, expectedIndex, isEquivalent);
			return isEquivalent;
		}

		/// <remarks>
		///     The differences are only counted, because only the pairs that are reported belong in the failure
		///     message. The count is taken from the context and restored afterwards, which keeps them out of the count
		///     of the message that is kept.
		/// </remarks>
		private async ValueTask<int>
			GetDifferenceCount(int actualIndex, int expectedIndex)
		{
			_differenceCounts ??= new int?[_actualIndices.Length, _expectedIndices.Length];
			if (_differenceCounts[actualIndex, expectedIndex] is { } cachedCount)
			{
				return cachedCount;
			}

			int differenceCount = _context.DifferenceCount;
			bool wasCountingOnly = _context.IsCountingOnly;
			_context.IsCountingOnly = true;
			try
			{
				await Compare(_actualObjects[_actualIndices[actualIndex]],
					_expectedObjects[_expectedIndices[expectedIndex]], _options, _typeOptions,
					_unusedFailureBuilder, GetElementPath(actualIndex), MemberType.Element, _context);
			}
			finally
			{
				_context.IsCountingOnly = wasCountingOnly;
			}

			int count = _context.DifferenceCount - differenceCount;
			_context.DifferenceCount = differenceCount;
			_differenceCounts[actualIndex, expectedIndex] = count;
			return count;
		}

		private MemberPath GetElementPath(int actualIndex)
			=> MemberPath.Element(_memberPath, _actualIndices[actualIndex]);

		/// <summary>
		///     The results of the compared pairs, kept sparse while few pairs are compared.
		/// </summary>
		/// <remarks>
		///     A collection in or near the expected order only compares about one pair per element, so a table of all
		///     pairs, which takes megabytes for a thousand elements, is only allocated once the sparse entries would
		///     take more memory than it.
		/// </remarks>
		private sealed class PairResults(int actualCount, int expectedCount)
		{
			private readonly long _denseThreshold = (long)actualCount * expectedCount / 16;
			private bool?[,]? _dense;
			private Dictionary<long, bool>? _sparse = new();

			public bool TryGet(int actualIndex, int expectedIndex, out bool result)
			{
				if (_dense is not null)
				{
					bool? value = _dense[actualIndex, expectedIndex];
					result = value.GetValueOrDefault();
					return value.HasValue;
				}

				return _sparse!.TryGetValue(GetKey(actualIndex, expectedIndex), out result);
			}

			public void Set(int actualIndex, int expectedIndex, bool result)
			{
				if (_dense is not null)
				{
					_dense[actualIndex, expectedIndex] = result;
					return;
				}

				_sparse![GetKey(actualIndex, expectedIndex)] = result;
				if (_sparse.Count > _denseThreshold)
				{
					_dense = new bool?[actualCount, expectedCount];
					foreach (KeyValuePair<long, bool> entry in _sparse)
					{
						_dense[entry.Key / expectedCount, entry.Key % expectedCount] = entry.Value;
					}

					_sparse = null;
				}
			}

			private long GetKey(int actualIndex, int expectedIndex)
				=> (long)actualIndex * expectedCount + expectedIndex;
		}
	}
#pragma warning restore S107
#pragma warning restore S3776
}
