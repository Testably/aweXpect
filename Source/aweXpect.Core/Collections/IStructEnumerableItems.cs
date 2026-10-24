using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Options;
using aweXpect.Results;

namespace aweXpect;

/// <summary>
///     The items of a struct collection of <typeparamref name="TItem" />, e.g. an <c>ImmutableArray</c>, that a
///     quantifier like <c>All()</c> or <c>AtLeast(2)</c> selected, for an extension method that adds an expectation on
///     them.
/// </summary>
/// <remarks>
///     The expectations on the items are extension methods on this interface. Only the type checks, whose type argument
///     cannot be inferred, are members of it. It is covariant in <typeparamref name="TItem" />, so that an expectation on
///     the items of a nullable type applies to the items of a not nullable type as well.
/// </remarks>
public interface IStructEnumerableItems<TCollection, out TItem>
	where TCollection : struct, IEnumerable<TItem>
{
	/// <summary>
	///     The quantifier for the items.
	/// </summary>
	EnumerableQuantifier Quantifier { get; }

	/// <summary>
	///     The subject of the expectation.
	/// </summary>
	IThat<TCollection> Subject { get; }

	/// <summary>
	///     …are of type <typeparamref name="TType" />.
	/// </summary>
	AndOrResult<TCollection, IThat<TCollection>> Are<TType>();

	/// <summary>
	///     …are of type <paramref name="type" />.
	/// </summary>
	AndOrResult<TCollection, IThat<TCollection>> Are(Type type);

	/// <summary>
	///     …are exactly of type <typeparamref name="TType" />.
	/// </summary>
	AndOrResult<TCollection, IThat<TCollection>> AreExactly<TType>();

	/// <summary>
	///     …are exactly of type <paramref name="type" />.
	/// </summary>
	AndOrResult<TCollection, IThat<TCollection>> AreExactly(Type type);
}
