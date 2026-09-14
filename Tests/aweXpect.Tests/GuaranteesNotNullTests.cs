using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using aweXpect.Core;

namespace aweXpect.Tests;

using CoreDelegate = Delegates.ThatDelegate;
using CoreGeneric = aweXpect.ThatGeneric;

public sealed class GuaranteesNotNullTests
{
	[Fact]
	public async Task EveryExpectation_ShouldFailForANullSubject()
	{
		List<string> deviations = Observations
			.Where(observation => !observation.Fails && !IsExempt(observation.Name))
			.Select(observation => observation.Identifier)
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(deviations).IsEmpty()
			.Because("a null subject must fail every expectation that is not one of the exceptions in Exempt");
	}

	[Fact]
	public async Task EveryExpectation_ShouldFailForANullSubjectWhenNegated()
	{
		List<string> deviations = Observations
			.Where(observation => observation.FailsWhenNegated == false && !IsExemptWhenNegated(observation.Name) &&
			                      !NegationAwaitingACoreRelease.Contains(observation.Identifier))
			.Select(observation => observation.Identifier)
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(deviations).IsEmpty()
			.Because(
				"a null subject must fail an expectation that DoesNotComplyWith negates just as it fails the expectation itself");
	}

	[Fact]
	public async Task EveryExpectationThatFails_ShouldGuaranteeNotNull()
	{
		List<string> unmarked = Observations
			.Where(observation => observation.Fails && !IsExempt(observation.Name) && !observation.IsMarked &&
			                      !AwaitingACoreRelease.Contains(observation.Identifier))
			.Select(observation => observation.Identifier)
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(unmarked).IsEmpty()
			.Because(
				"an expectation that rules a null subject out must say so, or IsNotNullSuppressor cannot drop the CS8602 it has already answered");
	}

	[Fact]
	public async Task EveryMarkedExpectation_ShouldFailForANullSubject()
	{
		List<string> deviations = Observations
			.Where(observation => observation.IsMarked && !observation.Fails)
			.Select(observation => observation.Identifier)
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(deviations).IsEmpty()
			.Because(
				"the attribute drops a CS8602 in user code, so an expectation that does not rule a null subject out must not carry it");
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

	private static readonly Type[] TypeArgumentCandidates =
	[
		typeof(object), typeof(string), typeof(Exception), typeof(ArgumentException), typeof(EquatableSubject),
		typeof(NotifyingSubject), typeof(double), typeof(int), typeof(DayOfWeek), typeof(TimeSpan),
		typeof(DateTime), typeof(EnumerableStruct<object>), typeof(EnumerableStruct<string?>),
	];

	/// <summary>
	///     The rule: a null subject fails every expectation, except these. `IsEqualTo` and the other
	///     comparisons may be handed a null of their own to compare against, and the negated tri-state
	///     `bool?` expectations exist to cover the null case — making `IsNotTrue()` fail for it would
	///     leave it identical to `IsFalse()`, with no null-tolerant twin. `Eventually` continues a
	///     delegate expectation rather than taking a subject, so it has no null-subject behaviour of
	///     its own.
	/// </summary>
	private static readonly HashSet<string> Exempt = new(StringComparer.Ordinal)
	{
		"Eventually",
		"IsEqualTo",
		"IsEquivalentTo",
		"IsNotEqualTo",
		"IsNotEquivalentTo",
		"IsNotOneOf",
		"IsNotSameAs",
		"IsOneOf",
		"IsSameAs",
		"IsNull",
		"IsNullOrEmpty",
		"IsNullOrWhiteSpace",
		"IsNotFalse",
		"IsNotTrue",
	};

	/// <summary>
	///     `aweXpect` consumes `aweXpect.Core` as a released package outside Debug, so an attribute added to
	///     the core interface would be missing from the Release test run and fail the converse assertion
	///     below. These two qualify and are marked with the next core release instead.
	/// </summary>
	private static readonly HashSet<string> AwaitingACoreRelease = new(StringComparer.Ordinal)
	{
		"IThatSubject<T>.IsNot<TType>()",
		"IThatSubject<T>.IsNotExactly<TType>()",
	};

	/// <summary>
	///     These nest an inner expectation, so the negation is applied by <c>MappingNode</c> in `aweXpect.Core`, which
	///     flips the composite outcome without being able to tell a subject that was ruled out as <see langword="null" />
	///     from one whose expectation was merely unmet. Their outer constraints already derive from
	///     <see cref="ConstraintResult.WithNotNullValue{T}" /> and fail correctly on their own; only the composite does
	///     not. Fixing that needs the node to be told, which is a core change and therefore a core release.
	/// </summary>
	private static readonly HashSet<string> NegationAwaitingACoreRelease = new(StringComparer.Ordinal)
	{
		"ThatException.HasInner(IThat<Exception>,Type,Action<IThatSubject<Exception>>)",
		"ThatException.HasInner<TInnerException>(IThat<Exception>,Action<IThatSubject<TInnerException>>)",
		"ThatException.HasInnerException(IThat<Exception>,Action<IThatSubject<Exception>>)",
		"ThatException.HasRecursiveInnerExceptions(IThat<Exception>,Action<IThatSubject<IEnumerable<Exception>>>)",
		"ThatString.HasLines(IThat<String>,Action<IThatSubject<IEnumerable<String>>>)",
	};

	private static IReadOnlyList<Observation>? _observations;

	private static IReadOnlyList<Observation> Observations => _observations ??= Observe();

	private static bool IsExempt(string name) => Exempt.Contains(name);

	/// <summary>
	///     The negated form of an exempt expectation is exempt as well: <c>IsNotNull</c> negated is <c>IsNull</c>, which
	///     a <see langword="null" /> subject must satisfy. Deriving it keeps <see cref="Exempt" /> the single source of
	///     truth for both axes instead of maintaining a second list.
	/// </summary>
	private static bool IsExemptWhenNegated(string name) => IsExempt(name) || IsExempt(Negate(name));

	private static string Negate(string name)
		=> name.StartsWith("IsNot", StringComparison.Ordinal) ? "Is" + name.Substring(5) :
			name.StartsWith("Is", StringComparison.Ordinal) ? "IsNot" + name.Substring(2) : name;

	private sealed class Observation(
		string identifier,
		string name,
		bool fails,
		bool? failsWhenNegated,
		bool isMarked)
	{
		public string Identifier { get; } = identifier;

		public string Name { get; } = name;

		public bool Fails { get; } = fails;

		/// <summary>
		///     Whether the expectation also fails for a <see langword="null" /> subject when it is negated, or
		///     <see langword="null" /> when it cannot be negated reflectively.
		/// </summary>
		public bool? FailsWhenNegated { get; } = failsWhenNegated;

		public bool IsMarked { get; } = isMarked;
	}

	private static List<Observation> Observe()
	{
		List<Observation> observations = [];
		foreach (MethodInfo method in GetAllExpectations())
		{
			if (FailsForANullSubject(method) is { } fails)
			{
				observations.Add(new Observation(GetIdentifier(method), method.Name, fails,
					FailsWhenNegated(method),
					method.GetCustomAttribute<GuaranteesNotNullAttribute>() is not null));
			}
		}

		return observations;
	}

	private static bool? FailsForANullSubject(MethodInfo method)
	{
		MethodInfo closedMethod;
		Type subjectType;
		try
		{
			closedMethod = CloseMethod(method);
			if (!CanHaveNullSubject(closedMethod))
			{
				return null;
			}

			subjectType = GetSubjectType(closedMethod);
			Invoke(closedMethod, CreateNullSubject(subjectType));
		}
		catch (NotInvocableException)
		{
			return false;
		}
		catch (Exception exception) when (exception is not XunitException)
		{
			return false;
		}

		List<MethodInfo[]> paths =
			DiscoverCompletions(() => Invoke(closedMethod, CreateNullSubject(subjectType)), [], 0);

		// Only a failure on every path counts: an expectation that fails for one argument and passes for
		// another does not guarantee a subject is there, and must not be marked as if it did.
		bool observed = false;
		foreach (MethodInfo[] path in paths)
		{
			foreach (bool nullValues in new[]
			         {
				         false, true,
			         })
			{
				try
				{
					Await(Follow(Invoke(closedMethod, CreateNullSubject(subjectType), nullValues), path, nullValues));
					return false;
				}
				catch (XunitException)
				{
					observed = true;
				}
				catch (Exception)
				{
					return false;
				}
			}
		}

		return observed;
	}

	/// <summary>
	///     Invokes the expectation inside <c>DoesNotComplyWith</c>, which negates it.
	/// </summary>
	/// <remarks>
	///     A constraint that reports <see cref="Outcome.Failure" /> for a <see langword="null" /> subject from its
	///     <c>IsMetBy</c> has that failure turned into a success by the negation, unlike one that derives from
	///     <see cref="ConstraintResult.WithNotNullValue{T}" />, which decides before the inversion is applied. The
	///     guarantee would then only hold as long as nobody negates the expectation.
	/// </remarks>
	/// <returns>
	///     <see langword="null" /> when the expectation cannot be negated reflectively, otherwise whether the negated
	///     expectation fails for a <see langword="null" /> subject.
	/// </returns>
	private static bool? FailsWhenNegated(MethodInfo method)
	{
		MethodInfo closedMethod;
		Type subjectType;
		try
		{
			closedMethod = CloseMethod(method);
			if (!closedMethod.IsStatic || !CanHaveNullSubject(closedMethod) ||
			    GetThatSubjectType(closedMethod.GetParameters()[0].ParameterType) is not { } thatSubjectType)
			{
				return null;
			}

			subjectType = thatSubjectType;
		}
		catch (Exception)
		{
			return null;
		}

		bool observed = false;
		foreach (bool nullValues in new[]
		         {
			         false, true,
		         })
		{
			try
			{
				Await(InvokeNegated(closedMethod, subjectType, nullValues));
				return false;
			}
			catch (XunitException)
			{
				observed = true;
			}
			catch (Exception)
			{
				// The expectation cannot be invoked with these arguments, which the positive sweep already reports.
			}
		}

		return observed ? true : null;
	}

	private static object InvokeNegated(MethodInfo method, Type subjectType, bool nullValues)
	{
		Type thatSubjectType = typeof(IThatSubject<>).MakeGenericType(subjectType);
		ParameterExpression subject = Expression.Parameter(thatSubjectType, "subject");
		Expression[] arguments = method.GetParameters()
			.Select((parameter, index) => index == 0
				? (Expression)Expression.Convert(subject, parameter.ParameterType)
				: Expression.Constant(CreateArgument(parameter, nullValues), parameter.ParameterType))
			.ToArray();
		Delegate expectations = Expression
			.Lambda(typeof(Action<>).MakeGenericType(thatSubjectType),
				Expression.Call(null, method, arguments), subject)
			.Compile();

		MethodInfo doesNotComplyWith = typeof(CoreGeneric)
			.GetMethods(BindingFlags.Public | BindingFlags.Static)
			.Single(candidate => candidate.Name == nameof(CoreGeneric.DoesNotComplyWith))
			.MakeGenericMethod(subjectType);
		try
		{
			return doesNotComplyWith.Invoke(null, [CreateNullSubject(subjectType), expectations,])!;
		}
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			throw exception.InnerException;
		}
	}

	private static List<MethodInfo[]> DiscoverCompletions(Func<object> create, MethodInfo[] prefix, int depth)
	{
		object expectation;
		try
		{
			expectation = Follow(create(), prefix, false);
		}
		catch (Exception)
		{
			return [];
		}

		if (expectation.GetType().GetMethod("GetAwaiter") is not null)
		{
			return [prefix,];
		}

		if (depth >= 3)
		{
			return [];
		}

		List<MethodInfo[]> paths = [];
		foreach (MethodInfo continuation in GetContinuations(expectation))
		{
			paths.AddRange(DiscoverCompletions(create, [..prefix, continuation,], depth + 1));
		}

		return paths;
	}

	private static object Follow(object expectation, MethodInfo[] path, bool nullValues)
	{
		object current = expectation;
		foreach (MethodInfo continuation in path)
		{
			current = continuation.Invoke(current,
				continuation.GetParameters().Select(parameter => CreateContinuationArgument(parameter, nullValues)).ToArray())!;
		}

		return current;
	}

	private static IEnumerable<MethodInfo> GetAllExpectations()
		=> new[]
			{
				typeof(GuaranteesNotNullAttribute).Assembly, typeof(aweXpect.ThatString).Assembly,
			}
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsPublic || type.IsNestedPublic)
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static |
			                                    BindingFlags.Instance | BindingFlags.DeclaredOnly))
			.Where(IsExpectation);

	private static bool IsExpectation(MethodInfo method)
	{
		if (method.IsSpecialName || method.ReturnType == typeof(void) ||
		    method.GetCustomAttribute<EditorBrowsableAttribute>()?.State == EditorBrowsableState.Never)
		{
			return false;
		}

		Type? receiver = method.IsStatic
			? method.GetParameters().FirstOrDefault()?.ParameterType
			: method.DeclaringType;
		return receiver is not null && IsSupportedReceiver(receiver);
	}

	private static bool IsSupportedReceiver(Type receiver)
	{
		Type definition = receiver.IsGenericType ? receiver.GetGenericTypeDefinition() : receiver;
		return definition == typeof(IThat<>) || definition == typeof(IThatSubject<>) ||
		       definition == typeof(ThatSubject<>) || definition == typeof(CoreDelegate) ||
		       definition == typeof(CoreDelegate.WithoutValue) || definition == typeof(CoreDelegate.WithValue<>);
	}

	private static IEnumerable<MethodInfo> GetMarkedExpectations()
		=> new[]
			{
				typeof(GuaranteesNotNullAttribute).Assembly, typeof(aweXpect.ThatString).Assembly,
			}
			.SelectMany(assembly => assembly.GetTypes())
			.SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static |
			                                    BindingFlags.Instance | BindingFlags.DeclaredOnly))
			.Where(method => method.GetCustomAttribute<GuaranteesNotNullAttribute>() is not null);

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

	private static object Invoke(MethodInfo method, object subject, bool nullValues = false)
	{
		object?[] arguments = method.GetParameters()
			.Select(parameter => parameter.Position == 0 && method.IsStatic
				? subject
				: CreateArgument(parameter, nullValues))
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
			return That((object?)null);
		}

		if (definition == typeof(CoreDelegate) || definition == typeof(CoreDelegate.WithoutValue))
		{
			return That((Action)null!);
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
			.Single(candidate => candidate is { Name: nameof(That), IsGenericMethodDefinition: true, } &&
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
			throw new NotInvocableException(
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

	private static object? CreateArgument(ParameterInfo parameter, bool nullValues)
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

		if (Nullable.GetUnderlyingType(type) is { } underlyingType)
		{
			return nullValues ? null : Activator.CreateInstance(underlyingType);
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
			return CreateSingleElementArray(type.GetElementType()!);
		}

		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
		{
			return CreateSingleElementArray(type.GetGenericArguments()[0]);
		}

		if (typeof(Expression).IsAssignableFrom(type))
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>)
				? CreateMemberSelector(type.GetGenericArguments()[0])
				: throw new NotInvocableException(
					$"The test cannot invent a '{type}' for parameter '{parameter.Name}'. Extend {nameof(CreateArgument)}.");
		}

		return typeof(Delegate).IsAssignableFrom(type) ? CreateDelegate(type) : null;
	}

	private static LambdaExpression CreateMemberSelector(Type delegateType)
	{
		MethodInfo invoke = delegateType.GetMethod("Invoke")!;
		ParameterExpression subject = Expression.Parameter(invoke.GetParameters()[0].ParameterType, "subject");
		PropertyInfo? property = subject.Type
			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.FirstOrDefault(candidate
				=> candidate.CanRead && invoke.ReturnType.IsAssignableFrom(candidate.PropertyType));
		if (property is null)
		{
			throw new NotInvocableException(
				$"The test found no readable '{invoke.ReturnType}' property on '{subject.Type}' to select. Extend that type.");
		}

		return Expression.Lambda(delegateType,
			Expression.Convert(Expression.Property(subject, property), invoke.ReturnType),
			subject);
	}

	private static Delegate CreateDelegate(Type delegateType)
	{
		MethodInfo invoke = delegateType.GetMethod("Invoke")!;
		ParameterExpression[] parameters = invoke.GetParameters()
			.Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
			.ToArray();
		Expression body = invoke.ReturnType == typeof(void)
			? CreateExpectationBody(parameters)
			: Expression.Default(invoke.ReturnType);
		return Expression.Lambda(delegateType, body, parameters).Compile();
	}

	private static Expression CreateExpectationBody(ParameterExpression[] parameters)
	{
		if (parameters.Length != 1 || GetThatSubjectType(parameters[0].Type) is not { } subjectType)
		{
			return Expression.Empty();
		}

		MethodInfo satisfies = typeof(CoreGeneric)
			.GetMethods(BindingFlags.Public | BindingFlags.Static)
			.Single(method => method.Name == nameof(CoreGeneric.Satisfies) && method.GetParameters().Length == 3)
			.MakeGenericMethod(subjectType);
		LambdaExpression predicate = Expression.Lambda(
			typeof(Func<,>).MakeGenericType(subjectType, typeof(bool)),
			Expression.Constant(true),
			Expression.Parameter(subjectType, "_"));
		return Expression.Call(null, satisfies,
			Expression.Convert(parameters[0], typeof(IThat<>).MakeGenericType(subjectType)),
			predicate,
			Expression.Constant("_ => true"));
	}

	private static Type? GetThatSubjectType(Type type)
		=> new[]
			{
				type,
			}.Concat(type.GetInterfaces())
			.FirstOrDefault(candidate => candidate.IsGenericType &&
			                             candidate.GetGenericTypeDefinition() == typeof(IThat<>))
			?.GetGenericArguments()[0];

	private static Array CreateSingleElementArray(Type elementType)
	{
		Array array = Array.CreateInstance(elementType, 1);
		array.SetValue(
			elementType == typeof(string) ? "a" :
			elementType.IsValueType ? Activator.CreateInstance(elementType) : null,
			0);
		return array;
	}

	private static object Complete(object expectation, int depth = 0)
	{
		if (expectation.GetType().GetMethod("GetAwaiter") is not null)
		{
			return expectation;
		}

		if (depth < 3)
		{
			foreach (MethodInfo continuation in GetContinuations(expectation))
			{
				object? result;
				try
				{
					result = continuation.Invoke(expectation,
						continuation.GetParameters().Select(parameter => CreateContinuationArgument(parameter, false)).ToArray());
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

		throw new NotInvocableException(
			$"The test does not know how to await a '{expectation.GetType()}'. Extend {nameof(Complete)}.");
	}

	private static IEnumerable<MethodInfo> GetContinuations(object expectation)
		=> expectation.GetType()
			.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
			.Where(method => !method.IsSpecialName && !method.IsGenericMethodDefinition &&
			                 method.ReturnType != typeof(void))
			.OrderBy(method => method.Name, StringComparer.Ordinal);

	private static object? CreateContinuationArgument(ParameterInfo parameter, bool nullValues)
		=> parameter.ParameterType == typeof(TimeSpan)
			? TimeSpan.FromSeconds(1)
			: CreateArgument(parameter, nullValues);

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

	private sealed class NotInvocableException(string message) : Exception(message);

	private sealed class NotifyingSubject : INotifyPropertyChanged
	{
		/// <summary>
		///     Gives CreateMemberSelector a member to select, so that a property expression argument is a
		///     real member access rather than a constant the expectation reads no property name from.
		/// </summary>
		public string? Value { get; set; }

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
