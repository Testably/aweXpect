using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for an only-used-by assertion: the filtered collections of types that may use the subject, and the
///     filtered collection of types within which the users are searched.
/// </summary>
/// <remarks>
///     The collections are resolved once per assertion via <see cref="Resolve" />, which reads the dependencies of
///     every type within the scope that is not allowed, so that each subject is only a lookup.
/// </remarks>
internal sealed class TypeUsageOptions
{
	private readonly TypeSetDependencyOptions _allowed;
	private readonly Filtered.Types _within;

	/// <summary>
	///     The disallowed users, resolved when first needed.
	/// </summary>
	/// <remarks>
	///     A cache of these options, which never change, so a race only resolves the users twice.
	/// </remarks>
	private ResolvedTypeUsages? _resolved;

	public TypeUsageOptions(TypeSetDependencyOptions allowed, Filtered.Types within)
	{
		_allowed = allowed;
		_within = within;
	}

	/// <summary>
	///     Validates the allowed <paramref name="users" /> and <paramref name="additional" /> collections eagerly,
	///     before the scope is known.
	/// </summary>
	public static TypeSetDependencyOptions CreateAllowed(Filtered.Types users, Filtered.Types[] additional)
	{
		ThrowHelper.ThrowIfNull(users, nameof(users));
		return new TypeSetDependencyOptions(users, additional);
	}

	/// <summary>
	///     Resolves the allowed collections and the types within the scope once into the disallowed users of each
	///     dependency.
	/// </summary>
	public async ValueTask<ResolvedTypeUsages> Resolve(CancellationToken cancellationToken = default)
	{
		if (_resolved is not null)
		{
			return _resolved;
		}

		ResolvedTypeSet allowed = await _allowed.Resolve(cancellationToken);
		Dictionary<Type, List<Type>> usersByDependency = new();
#if NET8_0_OR_GREATER
		await foreach (Type type in _within.WithCancellation(cancellationToken))
#else
		foreach (Type type in _within)
#endif
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (!allowed.Matches(type))
			{
				AddUser(usersByDependency, type);
			}
		}

		_resolved = new ResolvedTypeUsages(usersByDependency);
		return _resolved;
	}

	/// <summary>
	///     Describes the allowed collections and the scope for an expectation message.
	/// </summary>
	public string Describe()
		=> $"{_allowed.Describe()} within {_within.GetDescription().TrimEnd()}";

	/// <summary>
	///     Registers the <paramref name="user" /> under each of its dependencies, and under the generic type
	///     definition of each constructed one, so that a generic type definition as subject finds the users of any
	///     construction of it.
	/// </summary>
	private static void AddUser(Dictionary<Type, List<Type>> usersByDependency, Type user)
	{
		foreach (Type dependency in user.ResolveDependencies())
		{
			Add(usersByDependency, dependency, user);
			if (dependency.GetGenericTypeDefinitionOfConstruction() is { } definition)
			{
				Add(usersByDependency, definition, user);
			}
		}
	}

	private static void Add(Dictionary<Type, List<Type>> usersByDependency, Type dependency, Type user)
	{
		if (!usersByDependency.TryGetValue(dependency, out List<Type>? users))
		{
			users = [];
			usersByDependency.Add(dependency, users);
		}

		users.Add(user);
	}
}

/// <summary>
///     The disallowed users of each dependency, returned by <see cref="TypeUsageOptions.Resolve" />.
/// </summary>
internal sealed class ResolvedTypeUsages
{
	private readonly Dictionary<Type, List<Type>> _usersByDependency;

	internal ResolvedTypeUsages(Dictionary<Type, List<Type>> usersByDependency)
	{
		_usersByDependency = usersByDependency;
	}

	/// <summary>
	///     Returns the names of the disallowed users of the <paramref name="type" />, except the type itself.
	/// </summary>
	/// <remarks>
	///     The dependencies are stored without array, by-ref and pointer wrappers, so the <paramref name="type" /> is
	///     unwrapped symmetrically.
	/// </remarks>
	public IReadOnlyList<string> GetDisallowedUsers(Type type)
	{
		Type used = TypeHelpers.StripElementTypes(type);
		if (!_usersByDependency.TryGetValue(used, out List<Type>? users))
		{
			return [];
		}

		return TypeHelpers.FormatTypeNames(users.Where(user => user != used).Distinct());
	}
}
