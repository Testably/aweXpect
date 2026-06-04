using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types that are assignable to <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     To filter for base-class inheritance use <see cref="WhichInheritFrom{TBaseType}(Filtered.Types, bool)" />,
	///     for interfaces use <see cref="WhichImplement{TInterface}(Filtered.Types, bool)" />.
	/// </remarks>
	public static Filtered.Types WhichAreAssignableTo<TType>(this Filtered.Types @this)
		=> @this.WhichAreAssignableTo(typeof(TType));

	/// <summary>
	///     Filter for types that are assignable to <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     To filter for base-class inheritance use <see cref="WhichInheritFrom(Filtered.Types, Type, bool)" />,
	///     for interfaces use <see cref="WhichImplement(Filtered.Types, Type, bool)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static Filtered.Types WhichAreAssignableTo(this Filtered.Types @this, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return @this.Which(Filter.Suffix<Type>(t => type.IsAssignableFrom(t),
			$"which are assignable to {Formatter.Format(type)} "));
	}

	/// <summary>
	///     Filter for types that are not assignable to <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     To filter for base-class inheritance use <see cref="WhichInheritFrom{TBaseType}(Filtered.Types, bool)" />,
	///     for interfaces use <see cref="WhichImplement{TInterface}(Filtered.Types, bool)" />.
	/// </remarks>
	public static Filtered.Types WhichAreNotAssignableTo<TType>(this Filtered.Types @this)
		=> @this.WhichAreNotAssignableTo(typeof(TType));

	/// <summary>
	///     Filter for types that are not assignable to <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This covers base classes and interfaces in one step and considers a type itself as assignable.<br />
	///     To filter for base-class inheritance use <see cref="WhichInheritFrom(Filtered.Types, Type, bool)" />,
	///     for interfaces use <see cref="WhichImplement(Filtered.Types, Type, bool)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static Filtered.Types WhichAreNotAssignableTo(this Filtered.Types @this, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return @this.Which(Filter.Suffix<Type>(t => !type.IsAssignableFrom(t),
			$"which are not assignable to {Formatter.Format(type)} "));
	}

	/// <summary>
	///     Filter for types that are assignable from <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="WhichAreAssignableTo{TType}(Filtered.Types)" />: it keeps types to
	///     which <typeparamref name="TType" /> is assignable.
	/// </remarks>
	public static Filtered.Types WhichAreAssignableFrom<TType>(this Filtered.Types @this)
		=> @this.WhichAreAssignableFrom(typeof(TType));

	/// <summary>
	///     Filter for types that are assignable from <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="WhichAreAssignableTo(Filtered.Types, Type)" />: it keeps types to
	///     which <paramref name="type" /> is assignable.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static Filtered.Types WhichAreAssignableFrom(this Filtered.Types @this, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return @this.Which(Filter.Suffix<Type>(t => t.IsAssignableFrom(type),
			$"which are assignable from {Formatter.Format(type)} "));
	}

	/// <summary>
	///     Filter for types that are not assignable from <typeparamref name="TType" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="WhichAreNotAssignableTo{TType}(Filtered.Types)" />: it keeps types to
	///     which <typeparamref name="TType" /> is not assignable.
	/// </remarks>
	public static Filtered.Types WhichAreNotAssignableFrom<TType>(this Filtered.Types @this)
		=> @this.WhichAreNotAssignableFrom(typeof(TType));

	/// <summary>
	///     Filter for types that are not assignable from <paramref name="type" />.
	/// </summary>
	/// <remarks>
	///     This is the reverse direction of <see cref="WhichAreNotAssignableTo(Filtered.Types, Type)" />: it keeps types to
	///     which <paramref name="type" /> is not assignable.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="type" /> is an open generic type definition.</exception>
	public static Filtered.Types WhichAreNotAssignableFrom(this Filtered.Types @this, Type type)
	{
		type.EnsureIsNotOpenGeneric();
		return @this.Which(Filter.Suffix<Type>(t => !t.IsAssignableFrom(type),
			$"which are not assignable from {Formatter.Format(type)} "));
	}
}
