using System;
using System.Collections.Generic;
using System.Linq;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection.Options;

/// <summary>
///     Options for a specific-type dependency assertion.
/// </summary>
/// <remarks>
///     The instance is shared between the chainable result and the underlying constraint, so that
///     <c>OrOn&lt;T&gt;()</c> / <c>OrOn(Type)</c> widen the (lazily evaluated) expression.
/// </remarks>
internal sealed class TypeDependencyOptions
{
	private readonly List<Type> _types = [];

	public TypeDependencyOptions(Type type)
	{
		_types.Add(type ?? throw new ArgumentNullException(nameof(type)));
	}

	/// <summary>
	///     The targeted types.
	/// </summary>
	public IReadOnlyList<Type> Types => _types;

	/// <summary>
	///     Widens the set of targeted types by the given <paramref name="type" />.
	/// </summary>
	public void OrOn(Type type)
		=> _types.Add(type ?? throw new ArgumentNullException(nameof(type)));

	/// <summary>
	///     Checks whether the <paramref name="dependency" /> references any of the configured types.
	/// </summary>
	public bool Matches(Type dependency)
		=> _types.Any(type => TypeHelpers.MatchesType(dependency, type));

	/// <summary>
	///     Describes the configured types for an expectation message.
	/// </summary>
	/// <remarks>
	///     The constructor always sets one type, so an empty set cannot occur here.
	/// </remarks>
	public string Describe()
		=> $"type {string.Join(" or ", _types.Select(type => Formatter.Format(type)))}";
}
