using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using aweXpect.Core;
using aweXpect.Results;

namespace aweXpect.Tests;

using CoreDelegate = Delegates.ThatDelegate;
using CoreGeneric = aweXpect.ThatGeneric;

public sealed class GuaranteesNotNullTests
{
	[Test]
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

	[Test]
	public async Task EveryExpectation_ShouldFailForANullSubjectWhenNegated()
	{
		List<string> deviations = Observations
			.Where(observation => observation.FailsWhenNegated == false && !IsExemptWhenNegated(observation.Name))
			.Select(observation => observation.Identifier)
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(deviations).IsEmpty()
			.Because(
				"a null subject must fail an expectation that DoesNotComplyWith negates just as it fails the expectation itself");
	}

	[Test]
	public async Task EveryExpectationThatFails_ShouldGuaranteeNotNull()
	{
		List<string> unmarked = Observations
			.Where(observation => observation.Fails && !IsExempt(observation.Name) && !observation.IsMarked)
			.Select(observation => observation.Identifier)
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(unmarked).IsEmpty()
			.Because(
				"an expectation that rules a null subject out must say so, or IsNotNullSuppressor cannot drop the CS8602 it has already answered");
	}

	[Test]
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

	[Test]
	public async Task EveryMarkedExpectation_ShouldHaveASubjectThatCanBeNull()
	{
		List<string> inert = GetMarkedExpectations().Where(method => !CanHaveANullSubjectValue(method))
			.Select(GetIdentifier)
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(inert).IsEmpty()
			.Because(
				"the attribute exists so that IsNotNullSuppressor can drop a nullability warning, and a subject that cannot be null never raises one, so marking it says nothing and makes the attribute read as documentation rather than as the suppression directive it is");
	}

	[Test]
	public async Task EveryMarkedExpectation_ShouldNarrowTheSubjectToNotNull()
	{
		List<string> nullable = GetMarkedExpectations().Where(HandsOutANullableSubject)
			.Select(GetIdentifier)
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(nullable).IsEmpty()
			.Because("an expectation that rules a null subject out must hand out the subject as not nullable when awaited");
	}

	[Test]
	public async Task EveryUnmarkedExpectation_ShouldNotNarrowTheSubjectToNotNull()
	{
		List<string> notNullable = GetAllExpectations()
			.Where(method => method.GetCustomAttribute<GuaranteesNotNullAttribute>() is null &&
			                 HandsOutANotNullableSubject(method))
			.Select(GetIdentifier)
			.Distinct().OrderBy(identifier => identifier, StringComparer.Ordinal)
			.ToList();

		await That(notNullable).IsEmpty()
			.Because(
				"an expectation that a null subject can satisfy must hand out the subject as nullable when awaited, or the compiler does not warn about dereferencing it");
	}

	[Test]
	[Arguments(nameof(HandingOutANotNullableString), true)]
	[Arguments(nameof(HandingOutANullableString), false)]
	[Arguments(nameof(HandingOutANotNullableInt), true)]
	[Arguments(nameof(HandingOutANullableIntFromItsBase), false)]
	public async Task ShouldDetectANotNullableSubject(string methodName, bool expected)
	{
		MethodInfo method = typeof(GuaranteesNotNullTests)
			.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!;

		bool result = HandsOutANotNullableSubject(method);

		await That(result).IsEqualTo(expected)
			.Because("the check for unmarked expectations must not silently degrade into one that finds nothing");
	}

	[Test]
	public async Task ShouldNegateTheExpectationsThatNeedAContinuation()
	{
		List<string> negated = Observations
			.Where(observation => observation.FailsWhenNegated is not null)
			.Select(observation => observation.Identifier)
			.ToList();

		await That(negated).Contains("ThatString.HasLength(IThat<String>)").And
			.Contains("ThatEnumerable.HasCount<TItem>(IThat<IEnumerable<TItem>>)").And
			.Contains("ThatEnumerable.All<TItem>(IThat<IEnumerable<TItem>>)")
			.Because("an expectation that only adds its constraint in a continuation must be negated as well");
	}

	[Test]
	public async Task ShouldFindTheMarkedExpectations()
	{
		List<MethodInfo> marked = GetMarkedExpectations().ToList();

		await That(marked.Select(GetIdentifier)).All().AreUnique()
			.Because("each marked expectation must map to exactly one test case");
		await That(marked.Where(CanHaveNullSubject)).IsNotEmpty()
			.Because("the reflection lookup must not silently degrade into an empty test set");
		await That(marked.Select(method => method.DeclaringType!.Assembly.GetName().Name).Distinct())
			.IsEqualTo(["aweXpect", "aweXpect.Core",]).InAnyOrder()
			.Because("both assemblies declare expectations that are marked");
	}

	private static readonly Type[] TypeArgumentCandidates =
	[
		typeof(object), typeof(string), typeof(Exception), typeof(ArgumentException), typeof(EquatableSubject),
		typeof(NotifyingSubject), typeof(double), typeof(int), typeof(DayOfWeek), typeof(TimeSpan),
		typeof(DateTime), typeof(EnumerableStruct<object>), typeof(EnumerableStruct<string?>), typeof(Stream),
	];

	/// <summary>
	///     The rule: a null subject fails every expectation, except these. `IsEqualTo` and the other
	///     comparisons may be handed a null of their own to compare against, and the negated tri-state
	///     `bool?` expectations exist to cover the null case: making `IsNotTrue()` fail for it would
	///     leave it identical to `IsFalse()`, with no null-tolerant twin. `Eventually` continues a
	///     delegate expectation rather than taking a subject, so it has no null-subject behaviour of
	///     its own. `Satisfies` and `CompliesWith` hand the subject to the caller's own predicate or
	///     expectations, which state themselves how a null is to be treated, so their null-subject
	///     behaviour is not theirs to decide.
	/// </summary>
	private static readonly HashSet<string> Exempt = new(StringComparer.Ordinal)
	{
		"CompliesWith",
		"DoesNotComplyWith",
		"DoesNotSatisfy",
		"Satisfies",
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

	private static readonly Lazy<IReadOnlyList<Observation>> LazyObservations = new(Observe);

	private static IReadOnlyList<Observation> Observations => LazyObservations.Value;

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
		catch (Exception exception) when (exception is not FailException)
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
				catch (FailException)
				{
					observed = true;
				}
				catch (ArgumentNullException) when (nullValues)
				{
					// A guard that rejects the null argument lets no null subject pass, so the run with non-null
					// arguments decides.
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
		List<MethodInfo[]> paths;
		try
		{
			closedMethod = CloseMethod(method);
			if (!closedMethod.IsStatic || !CanHaveNullSubject(closedMethod) ||
			    GetThatSubjectType(closedMethod.GetParameters()[0].ParameterType) is not { } thatSubjectType)
			{
				return null;
			}

			subjectType = thatSubjectType;
			// An expectation such as HasLength() only adds its constraint in a continuation, so it is negated with each
			// continuation that completes it.
			paths = DiscoverCompletions(
				() => Invoke(closedMethod, CreateNullSubject(GetSubjectType(closedMethod))), [], 0);
		}
		catch (Exception)
		{
			return null;
		}

		// As in the positive sweep, only a failure on every path counts.
		bool observed = false;
		foreach (MethodInfo[] path in paths)
		{
			switch (FailsWhenNegated(closedMethod, subjectType, path))
			{
				case false:
					return false;
				case true:
					observed = true;
					break;
			}
		}

		return observed ? true : null;
	}

	private static bool? FailsWhenNegated(MethodInfo method, Type subjectType, MethodInfo[] path)
	{
		bool observed = false;
		foreach (bool nullValues in new[]
		         {
			         false, true,
		         })
		{
			try
			{
				Await(InvokeNegated(method, subjectType, path, nullValues));
				return false;
			}
			catch (FailException)
			{
				observed = true;
			}
			catch (ArgumentNullException) when (nullValues)
			{
				// A guard that rejects the null argument lets no null subject pass, so the run with non-null
				// arguments decides.
			}
			catch (Exception)
			{
				return false;
			}
		}

		return observed ? true : null;
	}

	private static object InvokeNegated(MethodInfo method, Type subjectType, MethodInfo[] path, bool nullValues)
	{
		Type thatSubjectType = typeof(IThatSubject<>).MakeGenericType(subjectType);
		ParameterExpression subject = Expression.Parameter(thatSubjectType, "subject");
		Expression[] arguments = method.GetParameters()
			.Select((parameter, index) => index == 0
				? (Expression)Expression.Convert(subject, parameter.ParameterType)
				: Expression.Constant(CreateArgument(parameter, nullValues), parameter.ParameterType))
			.ToArray();
		MethodInfo follow = typeof(GuaranteesNotNullTests)
			.GetMethod(nameof(Follow), BindingFlags.NonPublic | BindingFlags.Static)!;
		Delegate expectations = Expression
			.Lambda(typeof(Action<>).MakeGenericType(thatSubjectType),
				Expression.Call(null, follow,
					Expression.Convert(Expression.Call(null, method, arguments), typeof(object)),
					Expression.Constant(path),
					Expression.Constant(nullValues)),
				subject)
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
			try
			{
				current = continuation.Invoke(current,
					continuation.GetParameters().Select(parameter => CreateContinuationArgument(parameter, nullValues))
						.ToArray())!;
			}
			catch (TargetInvocationException exception) when (exception.InnerException is not null)
			{
				throw exception.InnerException;
			}
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

		// A navigation such as `Values()` on a dictionary returns a new subject instead of an expectation. It verifies
		// nothing itself; the expectations continued from it are covered in their own right.
		if (IsSupportedReceiver(method.ReturnType))
		{
			return false;
		}

		// `Get()` exposes the expectation builder to extension authors and verifies nothing itself.
		if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(IExpectThat<>))
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

	/// <summary>
	///     Whether the value the suppressor tracks (the subject of the <c>Expect.That(subject)</c> the expectation
	///     belongs to) can be <see langword="null" />, and a nullability warning can therefore exist to be dropped.
	/// </summary>
	/// <remarks>
	///     This is not <see cref="CanHaveNullSubject" />, which asks whether the harness can invoke the method on a
	///     <see langword="null" /> receiver. A receiver such as <c>ThatSubject&lt;T&gt;</c> is a struct that wraps the
	///     subject, so it can never be <see langword="null" /> itself while its subject can.
	/// </remarks>
	private static bool CanHaveANullSubjectValue(MethodInfo method)
	{
		MethodInfo closedMethod = CloseMethod(method);
		Type receiver = closedMethod.IsStatic
			? closedMethod.GetParameters()[0].ParameterType
			: closedMethod.DeclaringType!;
		Type subjectType = GetThatSubjectType(receiver) ?? receiver;
		return !subjectType.IsValueType || Nullable.GetUnderlyingType(subjectType) is not null;
	}

	/// <summary>
	///     Whether an extension on a nullable subject returns a result, or a builder for one, that still names the subject
	///     type as nullable outside the <c>IThat&lt;…&gt;</c> it continues with.
	/// </summary>
	/// <remarks>
	///     The nullable annotations of reference types are erased from runtime types, so they are read from the
	///     compiler's <c>NullableAttribute</c>, which holds one flag per reference type in the flattened signature.
	/// </remarks>
	private static bool HandsOutANullableSubject(MethodInfo method)
		=> HandsOutTheNullableSubject(method, true);

	/// <summary>
	///     Whether an extension on a nullable subject returns a result, or a builder for one, that names the subject type
	///     as not nullable outside the <c>IThat&lt;…&gt;</c> it continues with.
	/// </summary>
	/// <remarks>
	///     A result can hand out a struct type argument as nullable in its base class, like
	///     <c>NullableNumberToleranceResult&lt;TType, TThat&gt;</c>, so for a nullable struct subject only the awaited type
	///     tells.
	/// </remarks>
	private static bool HandsOutANotNullableSubject(MethodInfo method)
	{
		if (method.IsStatic &&
		    GetThatSubjectType(method.GetParameters()[0].ParameterType) is { } subjectType &&
		    Nullable.GetUnderlyingType(subjectType) is { } underlyingType)
		{
			return method.ReturnType.GetMethod("GetAwaiter")?.ReturnType.GetMethod("GetResult")?.ReturnType ==
			       underlyingType;
		}

		return HandsOutTheNullableSubject(method, false);
	}

	private static bool HandsOutTheNullableSubject(MethodInfo method, bool asNullable)
	{
		if (!method.IsStatic || GetThatSubjectType(method.GetParameters()[0].ParameterType) is not { } subjectType)
		{
			return false;
		}

		byte[] parameterFlags = GetNullableFlags(method.GetParameters()[0], method);
		bool isNullableSubject = Nullable.GetUnderlyingType(subjectType) is not null ||
		                         GetFlag(parameterFlags, 1) == 2;
		Type nonNullableSubjectType = Nullable.GetUnderlyingType(subjectType) ?? subjectType;
		return isNullableSubject &&
		       Names(method.ReturnType, GetNullableFlags(method.ReturnParameter, method), 0,
			       nonNullableSubjectType, asNullable, []);
	}

	/// <remarks>
	///     The base types are walked as well, because a result can fix the awaited type in its base class, like
	///     <c>AndOrResult&lt;TType?, TThat, TSelf&gt;</c>. A base names the result again as <c>TSelf</c>, so each base
	///     is walked only once. The flags of the declared type do not describe a base type, so only a nullable struct
	///     is detected there.
	/// </remarks>
	private static bool Names(Type type, byte[] flags, int slot, Type subjectType, bool asNullable,
		HashSet<Type> visited)
	{
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IThat<>))
		{
			return false;
		}

		if (Nullable.GetUnderlyingType(type) is { } underlyingType)
		{
			return asNullable && underlyingType == subjectType;
		}

		if (type == subjectType && TakesASlot(type))
		{
			return GetFlag(flags, slot) == (asNullable ? 2 : 1);
		}

		if (type.IsGenericParameter)
		{
			return false;
		}

		return (type.IsGenericType && NamesTypeArgument(type, flags, slot, subjectType, asNullable, visited)) ||
		       (visited.Add(type) && type.BaseType is { } baseType &&
		        Names(baseType, [0,], 0, subjectType, asNullable, visited));
	}

	private static bool NamesTypeArgument(Type type, byte[] flags, int slot, Type subjectType, bool asNullable,
		HashSet<Type> visited)
	{
		int next = slot + (TakesASlot(type) ? 1 : 0);
		Type[] parameters = type.GetGenericTypeDefinition().GetGenericArguments();
		Type[] arguments = type.GetGenericArguments();
		for (int index = 0; index < arguments.Length; index++)
		{
			if (!IsInputValue(type, parameters[index]) &&
			    Names(arguments[index], flags, next, subjectType, asNullable, visited))
			{
				return true;
			}

			next += CountSlots(arguments[index]);
		}

		return false;
	}

	/// <summary>
	///     A type argument that stands for a value the result reads rather than hands out: the <c>TValue</c> a property
	///     builder maps from and the bounds of a <see cref="Results.BetweenResult{TTarget, TType}" />.
	/// </summary>
	private static bool IsInputValue(Type type, Type parameter)
		=> parameter.Name == "TValue" ||
		   (type.GetGenericTypeDefinition() == typeof(Results.BetweenResult<,>) && parameter.GenericParameterPosition == 1);

	private static bool TakesASlot(Type type)
		=> type.IsGenericParameter
			? !type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)
			: !type.IsValueType;

	private static int CountSlots(Type type)
	{
		if (type.IsArray)
		{
			return 1 + CountSlots(type.GetElementType()!);
		}

		int slots = TakesASlot(type) ? 1 : 0;
		return type.IsGenericType && !type.IsGenericParameter
			? slots + type.GetGenericArguments().Sum(CountSlots)
			: slots;
	}

	private static byte GetFlag(byte[] flags, int slot)
		=> flags.Length == 1 ? flags[0] : slot < flags.Length ? flags[slot] : (byte)0;

	private static byte[] GetNullableFlags(ParameterInfo parameter, MethodInfo method)
	{
		CustomAttributeData? nullable = parameter.GetCustomAttributesData().FirstOrDefault(attribute
			=> attribute.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
		if (nullable is not null)
		{
			object? value = nullable.ConstructorArguments[0].Value;
			return value is byte flag
				? [flag,]
				: ((IEnumerable<CustomAttributeTypedArgument>)value!).Select(argument => (byte)argument.Value!).ToArray();
		}

		for (MemberInfo? member = method; member is not null; member = member.DeclaringType)
		{
			CustomAttributeData? context = member.GetCustomAttributesData().FirstOrDefault(attribute
				=> attribute.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
			if (context is not null)
			{
				return [(byte)context.ConstructorArguments[0].Value!,];
			}
		}

		return [0,];
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

		if (type == typeof(Times))
		{
			return 1.Times();
		}

		if (type == typeof(int))
		{
			return 1;
		}

		if (parameter.IsOptional)
		{
			return parameter.DefaultValue;
		}

		if (Nullable.GetUnderlyingType(type) is { } underlyingType)
		{
			return nullValues ? null : Activator.CreateInstance(underlyingType);
		}

		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
		{
			Type[] arguments = type.GetGenericArguments();
			return Activator.CreateInstance(type, CreateValue(arguments[0]), CreateValue(arguments[1]));
		}

		if (type.IsValueType)
		{
			return Activator.CreateInstance(type);
		}


		if (type == typeof(string))
		{
			return "a";
		}

		if (type == typeof(object))
		{
			return new object();
		}

		if (type.IsArray)
		{
			return CreateSingleElementArray(type.GetElementType()!);
		}

		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
		{
			return CreateSingleElementArray(type.GetGenericArguments()[0]);
		}

		if (type == typeof(IEnumerable))
		{
			return CreateSingleElementArray(typeof(object));
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
		array.SetValue(CreateElement(elementType), 0);
		return array;
	}

	/// <summary>
	///     A collection of expectations or predicates rejects a <see langword="null" /> element as an invalid argument,
	///     so its element is one that can be invoked.
	/// </summary>
	private static object? CreateElement(Type type)
	{
		if (typeof(Delegate).IsAssignableFrom(type))
		{
			return CreateDelegate(type);
		}

		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>))
		{
			Type delegateType = type.GetGenericArguments()[0];
			MethodInfo invoke = delegateType.GetMethod("Invoke")!;
			return Expression.Lambda(delegateType, Expression.Default(invoke.ReturnType),
				invoke.GetParameters().Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name)));
		}

		return CreateValue(type);
	}

	private static object? CreateValue(Type type)
		=> type == typeof(string) ? "a" :
			type == typeof(object) ? new object() :
			type.IsValueType ? Activator.CreateInstance(type) : null;

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
			.GetMethods(BindingFlags.Public | BindingFlags.Instance)
			.Where(method => method.DeclaringType != typeof(object) && !method.IsSpecialName &&
			                 !method.IsGenericMethodDefinition && method.ReturnType != typeof(void))
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

	private static AndOrResult<string, IThat<string?>> HandingOutANotNullableString(IThat<string?> subject)
		=> throw new NotSupportedException();

	private static AndOrResult<string?, IThat<string?>> HandingOutANullableString(IThat<string?> subject)
		=> throw new NotSupportedException();

	private static AndOrResult<int, IThat<int?>> HandingOutANotNullableInt(IThat<int?> subject)
		=> throw new NotSupportedException();

	private static NullableNumberToleranceResult<int, IThat<int?>> HandingOutANullableIntFromItsBase(
		IThat<int?> subject)
		=> throw new NotSupportedException();

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
