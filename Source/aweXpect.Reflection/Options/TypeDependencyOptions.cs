using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for a specific-type dependency assertion.
/// </summary>
/// <remarks>
///     An immutable value: <c>OrOn(Type)</c> returns new options that widen the (lazily evaluated) expression. The
///     expectation shares them in an <see cref="Core.OptionsHolder{TOptions}" /> with its result and its constraint.
/// </remarks>
internal sealed class TypeDependencyOptions : IDependencyTarget
{
	private readonly Type[] _types;

	public TypeDependencyOptions(Type type)
		: this([Validate(type),])
	{
	}

	private TypeDependencyOptions(Type[] types)
	{
		_types = types;
	}

	/// <summary>
	///     The targeted types.
	/// </summary>
	public IReadOnlyList<Type> Types => _types;

	/// <summary>
	///     Returns options that widen the set of targeted types by the given <paramref name="type" />.
	/// </summary>
	public TypeDependencyOptions OrOn(Type type)
		=> new([.. _types, Validate(type),]);

	private static Type Validate(Type type)
	{
		ThrowHelper.ThrowIfNull(type, nameof(type));
		return type;
	}

	/// <summary>
	///     Checks whether the <paramref name="dependency" /> references any of the configured types.
	/// </summary>
	public bool Matches(Type dependency)
		=> _types.Any(type => TypeHelpers.MatchesType(dependency, type));

	/// <summary>
	///     Checks whether the <paramref name="type" /> references any of the configured types.
	/// </summary>
	public bool IsMatchedBy(Type type)
		=> type.ResolveDependencies().Any(Matches);

	/// <summary>
	///     Describes the configured types for an expectation message.
	/// </summary>
	/// <remarks>
	///     The constructor always sets one type, so an empty set cannot occur here.
	/// </remarks>
	public string Describe()
		=> $"type {string.Join(" or ", _types.Select(type => Formatter.Format(type)))}";
}
