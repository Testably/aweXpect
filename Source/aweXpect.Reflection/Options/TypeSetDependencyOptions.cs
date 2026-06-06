using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for a dependency assertion or filter whose targets are filtered collections of types
///     (<see cref="Filtered.Types" />).
/// </summary>
/// <remarks>
///     The instance is shared between the chainable result and the underlying constraint/filter, so that
///     <c>OrOn(…)</c> and <c>ExcludingOwnSubNamespaces(…)</c> widen or refine the (lazily evaluated) expression.
///     <para />
///     The target collections are resolved once per assertion via <see cref="Resolve" /> (the union of all
///     collections); matching happens on the returned <see cref="ResolvedTypeSet" />, so it cannot be used
///     before the resolution completed.
/// </remarks>
internal sealed class TypeSetDependencyOptions
{
	private readonly List<Filtered.Types> _targets = [];
	private bool _excludeOwnSubNamespaces;
	private ResolvedTypeSet? _resolved;

	public TypeSetDependencyOptions(Filtered.Types target, Filtered.Types[] additional)
	{
		_targets.Add(target ?? throw new ArgumentNullException(nameof(target),
			"The target collection of types must not be null."));
		if (additional is null)
		{
			throw new ArgumentNullException(nameof(additional),
				"The additional target collections of types must not be null.");
		}

		Add(additional, nameof(additional));
	}

	private TypeSetDependencyOptions(IEnumerable<Filtered.Types> targets, bool excludeOwnSubNamespaces)
	{
		_targets.AddRange(targets);
		_excludeOwnSubNamespaces = excludeOwnSubNamespaces;
	}

	/// <summary>
	///     Indicates whether sub-namespaces of the type's own namespace are still allowed (for depends-only-on
	///     and has-dependencies-outside).
	/// </summary>
	/// <remarks>
	///     The type's own namespace is always allowed; its sub-namespaces stay allowed unless the caller opted into
	///     <see cref="ExcludingOwnSubNamespaces" />.
	/// </remarks>
	public bool IncludeOwnSubNamespaces => !_excludeOwnSubNamespaces;

	/// <summary>
	///     Creates an independent copy, so that refining a (reusable) filter does not mutate the shared instance.
	/// </summary>
	public TypeSetDependencyOptions Copy()
		=> new(_targets, _excludeOwnSubNamespaces);

	/// <summary>
	///     Widens the set of targeted/allowed types by the given <paramref name="targets" />.
	/// </summary>
	public void OrOn(Filtered.Types[] targets)
	{
		if (targets is null)
		{
			throw new ArgumentNullException(nameof(targets), "The target collections of types must not be null.");
		}

		if (targets.Length == 0)
		{
			throw new ArgumentException("At least one collection of types must be specified.");
		}

		Add(targets, nameof(targets));
	}

	/// <summary>
	///     Excludes sub-namespaces of the type's own namespace from being implicitly allowed (for depends-only-on
	///     and has-dependencies-outside).
	/// </summary>
	/// <remarks>
	///     Only the exemption rule changes, not the resolved target set, so a previously resolved set stays valid.
	/// </remarks>
	public void ExcludingOwnSubNamespaces()
		=> _excludeOwnSubNamespaces = true;

	private void Add(Filtered.Types[] targets, string paramName)
	{
		// Fully validate before mutating, so that a failed widening leaves the shared instance untouched.
		if (targets.Contains(null!))
		{
			throw new ArgumentNullException(paramName, "The target collections of types must not contain null.");
		}

		_targets.AddRange(targets);
		// Widening invalidates a previously resolved set.
		_resolved = null;
	}

	/// <summary>
	///     Resolves the target collections once into their union set and returns it as a
	///     <see cref="ResolvedTypeSet" />, which performs all matching.
	/// </summary>
	/// <remarks>
	///     Each member is normalized via <see cref="TypeHelpers.StripElementTypes" />: dependencies are stored
	///     element-stripped at collection time, so array/by-ref/pointer targets must be unwrapped symmetrically
	///     (mirroring <see cref="TypeHelpers.MatchesType" /> in the specific-type overloads).
	/// </remarks>
#if NET8_0_OR_GREATER
	public async ValueTask<ResolvedTypeSet> Resolve(CancellationToken cancellationToken = default)
	{
		if (_resolved is not null)
		{
			return _resolved;
		}

		HashSet<Type> resolved = [];
		foreach (Filtered.Types target in _targets)
		{
			await foreach (Type type in target.WithCancellation(cancellationToken))
			{
				resolved.Add(TypeHelpers.StripElementTypes(type));
			}
		}

		_resolved = new ResolvedTypeSet(resolved, this);
		return _resolved;
	}
#else
	public Task<ResolvedTypeSet> Resolve(CancellationToken cancellationToken = default)
	{
		if (_resolved is null)
		{
			HashSet<Type> resolved = [];
			foreach (Filtered.Types target in _targets)
			{
				foreach (Type type in target)
				{
					cancellationToken.ThrowIfCancellationRequested();
					resolved.Add(TypeHelpers.StripElementTypes(type));
				}
			}

			_resolved = new ResolvedTypeSet(resolved, this);
		}

		return Task.FromResult(_resolved);
	}
#endif

	/// <summary>
	///     Describes the configured target collections for an expectation message.
	/// </summary>
	/// <remarks>
	///     The constructor guarantees at least one target, so an empty set cannot occur here.
	/// </remarks>
	public string Describe()
		=> string.Join(" or ", _targets.Select(target => target.GetDescription().TrimEnd()));
}

/// <summary>
///     The resolved union set of the target collections of a <see cref="TypeSetDependencyOptions" />, returned
///     by <see cref="TypeSetDependencyOptions.Resolve" />.
/// </summary>
/// <remarks>
///     All matching goes through this type, so matching against an unresolved target set is unrepresentable.
/// </remarks>
internal sealed class ResolvedTypeSet
{
	private readonly TypeSetDependencyOptions _options;
	private readonly HashSet<Type> _resolved;

	internal ResolvedTypeSet(HashSet<Type> resolved, TypeSetDependencyOptions options)
	{
		_resolved = resolved;
		_options = options;
	}

	/// <summary>
	///     Indicates whether sub-namespaces of the type's own namespace are still allowed (for depends-only-on).
	/// </summary>
	/// <remarks>
	///     Delegates to the owning options, so that a later refinement is honored even when the set was
	///     already resolved (the exemption rule is independent of the resolved set).
	/// </remarks>
	public bool IncludeOwnSubNamespaces => _options.IncludeOwnSubNamespaces;

	/// <summary>
	///     Checks whether the <paramref name="dependency" /> is a member of the resolved target set.
	/// </summary>
	/// <remarks>
	///     Membership is by <see cref="Type" /> identity. Because dependencies keep constructed generic types as
	///     written (e.g. <c>List&lt;Foo&gt;</c>) while a scanned target collection contains the generic type
	///     definition (<c>List&lt;&gt;</c>), a dependency on any construction additionally matches when its
	///     definition is a member of the set (the same rule as in the specific-type overloads, shared via
	///     <see cref="TypeHelpers.GetGenericTypeDefinitionOfConstruction" />).
	/// </remarks>
	public bool Matches(Type dependency)
		=> _resolved.Contains(dependency) ||
		   (dependency.GetGenericTypeDefinitionOfConstruction() is { } definition &&
		    _resolved.Contains(definition));

	/// <summary>
	///     Checks whether the <paramref name="type" /> has at least one dependency in the resolved target set.
	/// </summary>
	public bool IsMatchedBy(Type type)
		=> type.ResolveDependencies().Any(Matches);
}
