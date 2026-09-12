﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

using CoreDelegate = global::aweXpect.Delegates.ThatDelegate;

public sealed class GuaranteesNotNullTests
{
	private static readonly Type[] TypeArgumentCandidates =
	[
		typeof(object), typeof(string), typeof(Exception), typeof(ArgumentException), typeof(EquatableSubject),
		typeof(NotifyingSubject), typeof(double), typeof(int), typeof(DayOfWeek), typeof(TimeSpan),
		typeof(DateTime), typeof(EnumerableStruct<object>), typeof(EnumerableStruct<string?>),
	];

	// Expectations whose call shape this test cannot construct faithfully: the result needs a
	// continuation the test cannot choose, or an argument it cannot invent. Each one is covered by
	// a hand-written null-subject test instead, which ExcludedExpectations_ShouldBeCovered verifies.
	private static readonly HashSet<string> ExcludedFromInvocation =
	[
		"ThatAsyncEnumerable.All(IThat<IAsyncEnumerable<String>>)",
		"ThatAsyncEnumerable.Any(IThat<IAsyncEnumerable<String>>)",
		"ThatAsyncEnumerable.AtLeast(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.AtMost(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.Between(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.Contains<TItem>(IThat<IAsyncEnumerable<TItem>>,Func<TItem,Boolean>,String)",
		"ThatAsyncEnumerable.DoesNotContain<TItem>(IThat<IAsyncEnumerable<TItem>>,Func<TItem,Boolean>,String)",
		"ThatAsyncEnumerable.Exactly(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.HasItem<TItem>(IThat<IAsyncEnumerable<TItem>>)",
		"ThatAsyncEnumerable.HasItemThat<TItem>(IThat<IAsyncEnumerable<TItem>>,Action<IThatSubject<TItem>>)",
		"ThatAsyncEnumerable.LessThan(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.MoreThan(IThat<IAsyncEnumerable<String>>,Int32)",
		"ThatAsyncEnumerable.None(IThat<IAsyncEnumerable<String>>)",
		"ThatEnumerable.All(IThat<IEnumerable<String>>)",
		"ThatEnumerable.Any(IThat<IEnumerable<String>>)",
		"ThatEnumerable.AreAllUnique(IThat<Nullable<ImmutableArray<String>>>)",
		"ThatEnumerable.AtLeast(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.AtMost(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.Between(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.Contains(IThat<IEnumerable>,Func<Object,Boolean>,String)",
		"ThatEnumerable.Contains<TItem>(IThat<IEnumerable<TItem>>,Func<TItem,Boolean>,String)",
		"ThatEnumerable.DoesNotContain(IThat<IEnumerable>,Func<Object,Boolean>,String)",
		"ThatEnumerable.DoesNotContain<TItem>(IThat<IEnumerable<TItem>>,Func<TItem,Boolean>,String)",
		"ThatEnumerable.Exactly(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.HasItem(IThat<IEnumerable>)",
		"ThatEnumerable.HasItem<TItem>(IThat<IEnumerable<TItem>>)",
		"ThatEnumerable.HasItemThat<TItem>(IThat<IEnumerable<TItem>>,Action<IThatSubject<TItem>>)",
		"ThatEnumerable.LessThan(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.MoreThan(IThat<IEnumerable<String>>,Int32)",
		"ThatEnumerable.None(IThat<IEnumerable<String>>)",
		"ThatEventRecording.DidNotTriggerPropertyChangedFor<TSubject,TProperty>(IThat<IEventRecording<TSubject>>,Expression<Func<TSubject,TProperty>>)",
		"ThatEventRecording.TriggeredPropertyChangedFor<TSubject,TProperty>(IThat<IEventRecording<TSubject>>,Expression<Func<TSubject,TProperty>>)",
		"ThatException.HasInner(IThat<Exception>,Type,Action<IThatSubject<Exception>>)",
		"ThatException.HasInner<TInnerException>(IThat<Exception>,Action<IThatSubject<TInnerException>>)",
		"ThatException.HasInnerException(IThat<Exception>,Action<IThatSubject<Exception>>)",
		"ThatException.HasRecursiveInnerExceptions(IThat<Exception>,Action<IThatSubject<IEnumerable<Exception>>>)",
		"ThatGeneric.DoesNotSatisfy<T>(IThat<T>,Func<T,Boolean>,String)",
		"ThatGeneric.Satisfies<T>(IThat<T>,Func<T,Boolean>,String)",
		"ThatString.HasLines(IThat<String>,Action<IThatSubject<IEnumerable<String>>>)",
	];

	public static TheoryData<string> MarkedExpectations
	{
		get
		{
			TheoryData<string> data = new();
			foreach (MethodInfo method in GetMarkedExpectations().Where(CanHaveNullSubject))
			{
				string identifier = GetIdentifier(method);
				if (!ExcludedFromInvocation.Contains(identifier))
				{
					data.Add(identifier);
				}
			}

			return data;
		}
	}

	[Fact]
	public async Task ShouldFindTheMarkedExpectations()
	{
		List<MethodInfo> marked = GetMarkedExpectations().ToList();

		await That(marked.Select(GetIdentifier)).AreAllUnique()
			.Because("each marked expectation must map to exactly one test case");
		await That(marked.Where(CanHaveNullSubject)).IsNotEmpty()
			.Because("the reflection lookup must not silently degrade into an empty test set");
		await That(marked.Select(method => method.DeclaringType!.Assembly.GetName().Name).Distinct())
			.IsEqualTo(["aweXpect", "aweXpect.Core",]).InAnyOrder()
			.Because("both assemblies declare expectations that are marked");
	}

#if NET8_0_OR_GREATER
	// Both assertions are statements about this repository rather than about runtime behaviour, so it is
	// enough to make them where every expectation and every test class is present: neither the
	// `IAsyncEnumerable` expectations nor their tests exist on the older target frameworks.
	[Fact]
	public async Task ExcludedExpectations_ShouldBeCovered()
	{
		List<string> identifiers = GetMarkedExpectations().Where(CanHaveNullSubject).Select(GetIdentifier).ToList();

		await That(ExcludedFromInvocation.Where(excluded => !identifiers.Contains(excluded)).ToList()).IsEmpty()
			.Because("an exclusion that no longer matches a marked expectation is stale");
		await That(ExcludedFromInvocation.Where(excluded => GetCoveringTests(excluded).Length == 0).ToList()).IsEmpty()
			.Because("every excluded expectation needs a hand-written null-subject test instead");
	}
#endif

	[Fact]
	public async Task SkippedExpectations_ShouldHaveASubjectThatCannotBeNull()
	{
		List<Type> skipped = GetMarkedExpectations().Where(method => !CanHaveNullSubject(method))
			.Select(method => GetSubjectType(CloseMethod(method)))
			.Distinct().ToList();

		await That(skipped.Where(type => type.Assembly == typeof(GuaranteesNotNullTests).Assembly).ToList()).IsEmpty()
			.Because(
				"a skipped expectation is invisible — it is neither invoked nor excluded — so a subject that only looks non-nullable because it was closed to one of this test's own helper structs must not go unnoticed");
	}

	[Theory]
	[MemberData(nameof(MarkedExpectations))]
	public async Task WhenSubjectIsNull_ShouldFail(string identifier)
	{
		MethodInfo method = GetMarkedExpectations().Single(m => GetIdentifier(m) == identifier);

		void Act() => Evaluate(method);

		await That(Act).Throws<XunitException>()
			.Because($"{identifier} is marked with [GuaranteesNotNull]");
	}

	private static IEnumerable<MethodInfo> GetMarkedExpectations()
		=> new[]
			{
				typeof(GuaranteesNotNullAttribute).Assembly, typeof(global::aweXpect.ThatString).Assembly,
			}
			.SelectMany(assembly => assembly.GetTypes())
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static |
			                                    BindingFlags.Instance | BindingFlags.DeclaredOnly))
			.Where(method => method.GetCustomAttribute<GuaranteesNotNullAttribute>() is not null);

	private static MethodInfo[] GetCoveringTests(string identifier)
	{
		string declaringType = identifier.Substring(0, identifier.IndexOf('.'));
		string expectation = identifier.Substring(identifier.IndexOf('.') + 1).Split('<', '(')[0];
		return typeof(GuaranteesNotNullTests).Assembly.GetTypes()
			.Where(type => type.FullName?.Contains($"{declaringType}+{expectation}+") == true)
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance |
			                                    BindingFlags.DeclaredOnly))
			.Where(method => method.Name is nameof(WhenSubjectIsNull_ShouldFail) or "WhenActualIsNull_ShouldFail")
			.ToArray();
	}

	private static bool CanHaveNullSubject(MethodInfo method)
	{
		Type subjectType = GetSubjectType(CloseMethod(method));
		return !subjectType.IsValueType || Nullable.GetUnderlyingType(subjectType) is not null;
	}

	private static string GetIdentifier(MethodInfo method)
	{
		string declaringType = FormatType(method.DeclaringType!);
		string typeArguments = method.IsGenericMethodDefinition
			? "<" + string.Join(",", method.GetGenericArguments().Select(argument => argument.Name)) + ">"
			: "";
		string parameters = string.Join(",", method.GetParameters().Select(p => FormatType(p.ParameterType)));
		return $"{declaringType}.{method.Name}{typeArguments}({parameters})";
	}

	private static string FormatType(Type type)
	{
		if (type.IsArray)
		{
			return FormatType(type.GetElementType()!) + "[]";
		}

		string name = type.Name.Split('`')[0];
		return type.IsGenericType
			? name + "<" + string.Join(",", type.GetGenericArguments().Select(FormatType)) + ">"
			: name;
	}

	private static void Evaluate(MethodInfo method)
	{
		MethodInfo closedMethod = CloseMethod(method);
		object subject = CreateNullSubject(GetSubjectType(closedMethod));
		Await(Complete(Invoke(closedMethod, subject)));
	}

	private static object Invoke(MethodInfo method, object subject)
	{
		object?[] arguments = method.GetParameters()
			.Select(parameter => parameter.Position == 0 && method.IsStatic
				? subject
				: CreateArgument(parameter))
			.ToArray();
		try
		{
			return (method.IsStatic ? method.Invoke(null, arguments) : method.Invoke(subject, arguments))!;
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			throw exception.InnerException;
		}
	}

	private static Type GetSubjectType(MethodInfo method)
	{
		Type receiver = method.IsStatic ? method.GetParameters()[0].ParameterType : method.DeclaringType!;
		return receiver.IsGenericType && receiver.GetGenericTypeDefinition() == typeof(IThat<>)
			? receiver.GetGenericArguments()[0]
			: receiver;
	}

	private static object CreateNullSubject(Type subjectType)
	{
		Type definition = subjectType.IsGenericType ? subjectType.GetGenericTypeDefinition() : subjectType;
#pragma warning disable aweXpect0001 // the expectation is awaited in Await after it was invoked reflectively
		if (definition == typeof(IThatSubject<>) || definition == typeof(ThatSubject<>))
		{
			return Expect.That((object?)null);
		}

		if (definition == typeof(CoreDelegate) || definition == typeof(CoreDelegate.WithoutValue))
		{
			return Expect.That((Action)null!);
		}
#pragma warning restore aweXpect0001

		if (definition == typeof(CoreDelegate.WithValue<>))
		{
			return InvokeThat(subjectType.GetGenericArguments()[0],
				parameter => parameter.ParameterType.IsGenericType &&
				             parameter.ParameterType.GetGenericTypeDefinition() == typeof(Func<>) &&
				             IsGenericMethodParameter(parameter.ParameterType.GetGenericArguments()[0]));
		}

		return InvokeThat(subjectType, parameter => IsGenericMethodParameter(parameter.ParameterType));
	}

	private static bool IsGenericMethodParameter(Type type)
		=> type.IsGenericParameter && type.DeclaringMethod is not null;

	private static object InvokeThat(Type typeArgument, Func<ParameterInfo, bool> subjectParameter)
	{
		MethodInfo that = typeof(Expect).GetMethods(BindingFlags.Public | BindingFlags.Static)
			.Single(candidate => candidate is { Name: nameof(Expect.That), IsGenericMethodDefinition: true, } &&
			                     candidate.GetGenericArguments().Length == 1 &&
			                     candidate.GetParameters().Length == 2 &&
			                     subjectParameter(candidate.GetParameters()[0]));
		return that.MakeGenericMethod(typeArgument).Invoke(null, [null, "subject",])!;
	}

	private static MethodInfo CloseMethod(MethodInfo method)
	{
		MethodInfo closedMethod = method;
		if (method.DeclaringType!.IsGenericTypeDefinition)
		{
			Type closedType = method.DeclaringType.MakeGenericType(
				CreateTypeArguments(method.DeclaringType.GetGenericArguments()));
			closedMethod = closedType
				.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Single(candidate => candidate.MetadataToken == method.MetadataToken);
		}

		return closedMethod.IsGenericMethodDefinition
			? closedMethod.MakeGenericMethod(CreateTypeArguments(closedMethod.GetGenericArguments()))
			: closedMethod;
	}

	private static Type[] CreateTypeArguments(Type[] genericArguments)
	{
		Dictionary<Type, Type> resolved = new();
		bool progressed = true;
		while (resolved.Count < genericArguments.Length && progressed)
		{
			progressed = false;
			foreach (Type genericArgument in genericArguments.Where(argument => !resolved.ContainsKey(argument)))
			{
				if (genericArgument.GetGenericParameterConstraints()
				    .Any(constraint => ReferencesUnresolved(constraint, genericArgument, resolved)))
				{
					continue;
				}

				if (TypeArgumentCandidates.FirstOrDefault(candidate =>
					    Satisfies(candidate, genericArgument, resolved)) is { } match)
				{
					resolved[genericArgument] = match;
					progressed = true;
				}
			}
		}

		Type? unresolved = genericArguments.FirstOrDefault(argument => !resolved.ContainsKey(argument));
		if (unresolved is not null)
		{
			throw new NotSupportedException(
				$"The test does not know which type argument satisfies '{unresolved}' of '{unresolved.DeclaringMethod?.ToString() ?? unresolved.DeclaringType?.ToString()}'. Extend {nameof(TypeArgumentCandidates)}.");
		}

		return genericArguments.Select(argument => resolved[argument]).ToArray();
	}

	private static bool ReferencesUnresolved(Type constraint, Type genericArgument, Dictionary<Type, Type> resolved)
	{
		if (constraint.IsGenericParameter)
		{
			return constraint != genericArgument && !resolved.ContainsKey(constraint);
		}

		return constraint.IsGenericType && constraint.GetGenericArguments()
			.Any(argument => ReferencesUnresolved(argument, genericArgument, resolved));
	}

	private static bool Satisfies(Type candidate, Type genericArgument, Dictionary<Type, Type> resolved)
	{
		GenericParameterAttributes attributes = genericArgument.GenericParameterAttributes;
		if (attributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint) && !candidate.IsValueType)
		{
			return false;
		}

		if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint) && candidate.IsValueType)
		{
			return false;
		}

		try
		{
			return genericArgument.GetGenericParameterConstraints()
				.Select(constraint => Substitute(constraint, candidate, genericArgument, resolved))
				.All(constraint => constraint.IsAssignableFrom(candidate));
		}
		catch (Exception exception) when (exception is ArgumentException or TypeLoadException)
		{
			return false;
		}
	}

	private static Type Substitute(Type constraint, Type candidate, Type genericArgument,
		Dictionary<Type, Type> resolved)
	{
		if (!constraint.ContainsGenericParameters || !constraint.IsGenericType)
		{
			return constraint == genericArgument ? candidate : constraint;
		}

		return constraint.GetGenericTypeDefinition().MakeGenericType(constraint.GetGenericArguments()
			.Select(argument => argument == genericArgument ? candidate :
				resolved.TryGetValue(argument, out Type? value) ? value : argument)
			.ToArray());
	}

	private static object? CreateArgument(ParameterInfo parameter)
	{
		Type type = parameter.ParameterType;
		if (type == typeof(Type))
		{
			return typeof(Exception);
		}

		if (parameter.IsOptional)
		{
			return parameter.DefaultValue;
		}

		if (type.IsValueType)
		{
			return Activator.CreateInstance(type);
		}


		if (type == typeof(string))
		{
			return "a";
		}

		if (type.IsArray)
		{
			return Array.CreateInstance(type.GetElementType()!, 0);
		}

		return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
			? Array.CreateInstance(type.GetGenericArguments()[0], 0)
			: null;
	}

	private static object Complete(object expectation, int depth = 0)
	{
		if (expectation.GetType().GetMethod("GetAwaiter") is not null)
		{
			return expectation;
		}

		if (depth < 3)
		{
			foreach (MethodInfo continuation in expectation.GetType()
				         .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				         .Where(method => !method.IsSpecialName && !method.IsGenericMethodDefinition &&
				                          method.ReturnType != typeof(void))
				         .OrderBy(method => method.Name, StringComparer.Ordinal))
			{
				object? result;
				try
				{
					result = continuation.Invoke(expectation,
						continuation.GetParameters().Select(CreateContinuationArgument).ToArray());
				}
				catch (Exception)
				{
					continue;
				}

				if (result is not null)
				{
					return Complete(result, depth + 1);
				}
			}
		}

		throw new NotSupportedException(
			$"The test does not know how to await a '{expectation.GetType()}'. Extend {nameof(Complete)}.");
	}

	private static object? CreateContinuationArgument(ParameterInfo parameter)
		=> parameter.ParameterType == typeof(TimeSpan)
			? TimeSpan.FromSeconds(1)
			: CreateArgument(parameter);

	private static void Await(object expectation)
	{
		object awaiter = expectation.GetType().GetMethod("GetAwaiter")!.Invoke(expectation, null)!;
		try
		{
			awaiter.GetType().GetMethod("GetResult")!.Invoke(awaiter, null);
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			throw exception.InnerException;
		}
	}

	private sealed class NotifyingSubject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged
		{
			add => throw new NotSupportedException();
			remove => throw new NotSupportedException();
		}
	}

	private readonly struct EnumerableStruct<T> : IEnumerable<T>
	{
		public IEnumerator<T> GetEnumerator() => Enumerable.Empty<T>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	private sealed class EquatableSubject : IEquatable<object>
	{
		public override bool Equals(object? other) => false;

		public override int GetHashCode() => 0;
	}
}
