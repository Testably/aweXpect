using System;
using System.Collections;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The items of a non-generic <see cref="IEnumerable" /> that a quantifier like <c>All()</c> or
///     <c>AtLeast(2)</c> selected, for an extension method that adds an expectation on them.
/// </summary>
/// <remarks>
///     The expectations on the items are extension methods on this interface. Only the type checks, whose type argument
///     cannot be inferred, are members of it.
/// </remarks>
public interface INonGenericEnumerableItems<TCollection>
	where TCollection : IEnumerable?
{
	/// <summary>
	///     The quantifier for the items.
	/// </summary>
	EnumerableQuantifier Quantifier { get; }

	/// <summary>
	///     The subject of the expectation.
	/// </summary>
	IThat<TCollection?> Subject { get; }

	/// <summary>
	///     …are of type <typeparamref name="TType" />.
	/// </summary>
	AndOrResult<TCollection, IThat<TCollection?>> Are<TType>();

	/// <summary>
	///     …are of type <paramref name="type" />.
	/// </summary>
	AndOrResult<TCollection, IThat<TCollection?>> Are(Type type);

	/// <summary>
	///     …are exactly of type <typeparamref name="TType" />.
	/// </summary>
	AndOrResult<TCollection, IThat<TCollection?>> AreExactly<TType>();

	/// <summary>
	///     …are exactly of type <paramref name="type" />.
	/// </summary>
	AndOrResult<TCollection, IThat<TCollection?>> AreExactly(Type type);
}
