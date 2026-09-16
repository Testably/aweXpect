using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

/// <summary>
///     Every scalar property expectation comes in both shapes: the continuation <c>HasX()</c> that opens the
///     comparison vocabulary, and the shorthand <c>HasX(value)</c> that spells <c>HasX().EqualTo(value)</c> in
///     one call.
/// </summary>
/// <remarks>
///     <c>HasFlag</c> is the documented exception: it asks whether a bit is set, not how two ordered values
///     compare, so it has no continuation to be the shorthand of.
/// </remarks>
public sealed class ScalarPropertyShorthandTests
{
	[Fact]
	public async Task EveryContinuation_ShouldHaveAShorthand()
	{
		List<string> missing = Continuations
			.Where(continuation => FindShorthand(continuation) is null)
			.Select(GetIdentifier)
			.OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(Continuations).IsNotEmpty()
			.Because("the reflection lookup must not silently degrade into an empty test set");
		await That(missing).IsEmpty()
			.Because("a scalar `Has…` must offer the shorthand next to the continuation");
	}

	[Fact]
	public async Task EveryShorthand_ShouldRenderLikeTheContinuation()
	{
		List<string> deviations = [];
		foreach (MethodInfo continuation in Continuations)
		{
			if (FindShorthand(continuation) is not { } shorthand)
			{
				continue;
			}

			MethodInfo closedContinuation = Close(continuation);
			MethodInfo closedShorthand = Close(shorthand);
			Type subjectType = GetSubjectType(closedContinuation);
			object expected = CreateExpected(GetPropertyType(closedContinuation.ReturnType));

			string fromContinuation = Capture(() =>
			{
				object property = closedContinuation.Invoke(null, [CreateSubject(subjectType),])!;
				Await(property.GetType().GetMethod("EqualTo")!.Invoke(property, [expected,])!);
			});
			string fromShorthand =
				Capture(() => Await(closedShorthand.Invoke(null, [CreateSubject(subjectType), expected,])!));

			if (fromContinuation.Length == 0)
			{
				// Both shapes succeeded, so comparing their messages would compare nothing.
				deviations.Add($"{GetIdentifier(continuation)}: the expected value does not make the comparison fail");
			}
			else if (fromContinuation != fromShorthand)
			{
				deviations.Add($"{GetIdentifier(continuation)}: '{fromShorthand}' instead of '{fromContinuation}'");
			}
		}

		await That(deviations).IsEmpty()
			.Because("the shorthand must be the explicit form in disguise, so that the two cannot drift apart");
	}

	/// <summary>
	///     The scalar property continuations and the type of the value their shorthand takes.
	/// </summary>
	private static readonly Dictionary<Type, Type> PropertyTypes = new()
	{
		[typeof(PropertyResult.Int<>)] = typeof(int),
		[typeof(PropertyResult.Long<>)] = typeof(long),
		[typeof(PropertyResult.DateTimeKind<>)] = typeof(DateTimeKind),
		[typeof(PropertyResult.TimeSpan<>)] = typeof(TimeSpan),
	};

	/// <summary>
	///     The type arguments to close a generic expectation with, in the order they are tried.
	/// </summary>
	private static readonly Type[] TypeArgumentCandidates =
	[
		typeof(DayOfWeek), typeof(Exception),
	];

	private static IReadOnlyList<MethodInfo>? _continuations;

	private static IReadOnlyList<MethodInfo> Continuations => _continuations ??= typeof(aweXpect.ThatString).Assembly
		.GetTypes().Where(type => type.IsPublic)
		.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
		.Where(method => method.GetParameters().Length == 1 &&
		                 GetThatSubjectType(method.GetParameters()[0].ParameterType) is not null &&
		                 method.ReturnType.IsGenericType &&
		                 PropertyTypes.ContainsKey(method.ReturnType.GetGenericTypeDefinition()))
		.ToList();

	private static MethodInfo? FindShorthand(MethodInfo continuation)
	{
		MethodInfo closedContinuation = Close(continuation);
		Type propertyType = GetPropertyType(closedContinuation.ReturnType);
		return continuation.DeclaringType!
			.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
			.Where(candidate => candidate.Name == continuation.Name &&
			                    candidate.GetParameters().Length == 2 &&
			                    candidate.GetGenericArguments().Length == continuation.GetGenericArguments().Length)
			.FirstOrDefault(candidate =>
			{
				ParameterInfo[] parameters = Close(candidate).GetParameters();
				return parameters[0].ParameterType == closedContinuation.GetParameters()[0].ParameterType &&
				       (Nullable.GetUnderlyingType(parameters[1].ParameterType) ?? parameters[1].ParameterType) ==
				       propertyType;
			});
	}

	private static Type GetPropertyType(Type propertyResultType)
		=> PropertyTypes[propertyResultType.GetGenericTypeDefinition()];

	private static object CreateExpected(Type propertyType)
	{
		if (propertyType == typeof(DateTimeKind))
		{
			return DateTimeKind.Utc;
		}

		if (propertyType == typeof(TimeSpan))
		{
			return TimeSpan.FromMinutes(42);
		}

		// A value no default subject carries, so that both shapes have a failure message to render.
		if (propertyType == typeof(long))
		{
			return 42L;
		}

		return 42;
	}

	private static MethodInfo Close(MethodInfo method)
	{
		if (!method.IsGenericMethodDefinition)
		{
			return method;
		}

		foreach (Type candidate in TypeArgumentCandidates)
		{
			try
			{
				return method.MakeGenericMethod(candidate);
			}
			catch (ArgumentException)
			{
				// The candidate violates a constraint, so the next one is tried.
			}
		}

		throw new InvalidOperationException(
			$"The test does not know which type argument closes '{method}'. Extend {nameof(TypeArgumentCandidates)}.");
	}

	private static Type GetSubjectType(MethodInfo continuation)
		=> GetThatSubjectType(continuation.GetParameters()[0].ParameterType)!;

	private static Type? GetThatSubjectType(Type type)
		=> type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IThat<>)
			? type.GetGenericArguments()[0]
			: null;

	private static object CreateSubject(Type subjectType)
	{
		MethodInfo that = typeof(Expect).GetMethods(BindingFlags.Public | BindingFlags.Static)
			.Single(candidate => candidate is
			{
				Name: nameof(That), IsGenericMethodDefinition: true,
			} && candidate.GetGenericArguments().Length == 1 && candidate.GetParameters().Length == 2 &&
			                     candidate.GetParameters()[0].ParameterType.IsGenericParameter);
		return that.MakeGenericMethod(subjectType).Invoke(null, [
			subjectType.IsValueType ? Activator.CreateInstance(subjectType) : null, "subject",
		])!;
	}

	/// <returns>The failure message of the expectation, or an empty string when it succeeded.</returns>
	private static string Capture(Action expectation)
	{
		try
		{
			expectation();
			return "";
		}
		catch (XunitException exception)
		{
			return exception.Message;
		}
	}

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

	private static string GetIdentifier(MethodInfo method)
		=> $"{method.DeclaringType!.Name}.{method.Name}({FormatType(method.GetParameters()[0].ParameterType)})";

	private static string FormatType(Type type)
	{
		string name = type.Name.Split('`')[0];
		return type.IsGenericType
			? name + "<" + string.Join(",", type.GetGenericArguments().Select(FormatType)) + ">"
			: name;
	}
}
