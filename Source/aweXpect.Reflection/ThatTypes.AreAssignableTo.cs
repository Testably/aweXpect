using System;
using System.Collections.Generic;
using aweXpect.Core;
using aweXpect.Core.Extending;
using aweXpect.Reflection.Helpers;
using aweXpect.Results;

namespace aweXpect;

public static partial class ThatTypes
{
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable to
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />,
	///     for interfaces use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreAssignableTo<TType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.AreAssignableTo(typeof(TType));

	/// <summary>
	///     …are assignable to <typeparamref name="TType" />.
	/// </summary>
	/// <inheritdoc cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" path="/remarks" />
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreAssignableTo<TType>(
		this IEnumerableItems<Type?> items)
		=> items.AreAssignableTo(typeof(TType));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable to
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />,
	///     for interfaces use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreAssignableTo<TType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.AreAssignableTo(typeof(TType));

	/// <inheritdoc cref="AreAssignableTo{TType}(IEnumerableItems{Type})" />
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreAssignableTo<TType>(
		this IAsyncEnumerableItems<Type?> items)
		=> items.AreAssignableTo(typeof(TType));
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable to
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />,
	///     for interfaces use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreNotAssignableTo<TType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.AreNotAssignableTo(typeof(TType));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable to
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     For base-class inheritance use <see cref="InheritFrom{TBaseType}(IThat{IEnumerable{Type}})" />,
	///     for interfaces use <see cref="Implement{TInterface}(IThat{IEnumerable{Type}})" />.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreNotAssignableTo<TType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.AreNotAssignableTo(typeof(TType));
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable from
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" />: it checks that
	///     <typeparamref name="TType" /> is assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreAssignableFrom<TType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.AreAssignableFrom(typeof(TType));

	/// <summary>
	///     …are assignable from <typeparamref name="TType" />.
	/// </summary>
	/// <inheritdoc cref="AreAssignableFrom{TType}(IThat{IEnumerable{Type}})" path="/remarks" />
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreAssignableFrom<TType>(
		this IEnumerableItems<Type?> items)
		=> items.AreAssignableFrom(typeof(TType));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are assignable from
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreAssignableTo{TType}(IThat{IEnumerable{Type}})" />: it checks that
	///     <typeparamref name="TType" /> is assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreAssignableFrom<TType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.AreAssignableFrom(typeof(TType));

	/// <inheritdoc cref="AreAssignableFrom{TType}(IEnumerableItems{Type})" />
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreAssignableFrom<TType>(
		this IAsyncEnumerableItems<Type?> items)
		=> items.AreAssignableFrom(typeof(TType));
#endif

	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable from
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreNotAssignableTo{TType}(IThat{IEnumerable{Type}})" />: it checks
	///     that <typeparamref name="TType" /> is not assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	[GuaranteesNotNull]
	public static AndOrResult<IEnumerable<Type?>, IThat<IEnumerable<Type?>?>> AreNotAssignableFrom<TType>(
		this IThat<IEnumerable<Type?>?> subject)
		=> subject.AreNotAssignableFrom(typeof(TType));

#if NET8_0_OR_GREATER
	/// <summary>
	///     Verifies that all items in the filtered collection of <see cref="Type" /> are not assignable from
	///     <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="AreNotAssignableTo{TType}(IThat{IEnumerable{Type}})" />: it checks
	///     that <typeparamref name="TType" /> is not assignable to each item.
	/// </remarks>
	/// <param name="subject">The type collection subject.</param>
	[GuaranteesNotNull]
	public static AndOrResult<IAsyncEnumerable<Type?>, IThat<IAsyncEnumerable<Type?>?>> AreNotAssignableFrom<TType>(
		this IThat<IAsyncEnumerable<Type?>?> subject)
		=> subject.AreNotAssignableFrom(typeof(TType));
#endif

	[GenerateCollectionExpectation("AreAssignableTo", "is assignable to {type}", "are assignable to {type}",
		CollectionOverload = true, NegatedName = "AreNotAssignableTo", NullItemFailsBothWays = true,
		Remarks = """
		          This covers base classes and interfaces in one step and considers a type itself as assignable.
		          """)]
	private static bool IsAssignableToType(Type item, Type type)
		=> type.EnsureIsNotOpenGeneric().IsAssignableFrom(item);

	[GenerateCollectionExpectation("AreAssignableFrom", "is assignable from {type}", "are assignable from {type}",
		CollectionOverload = true, NegatedName = "AreNotAssignableFrom", NullItemFailsBothWays = true,
		Remarks = """
		          This covers base classes and interfaces in one step and considers a type itself as assignable.
		          """)]
	private static bool IsAssignableFromType(Type item, Type type)
		=> item.IsAssignableFrom(type.EnsureIsNotOpenGeneric());
}
