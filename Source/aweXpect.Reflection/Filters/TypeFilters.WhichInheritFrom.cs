using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types that inherit from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter for an implemented interface use <see cref="WhichImplement{TInterface}(Filtered.Types, bool)" />,
	///     or use <see cref="WhichAreAssignableTo{TType}(Filtered.Types)" /> to cover base classes and interfaces.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static Filtered.Types WhichInheritFrom<TBaseType>(this Filtered.Types @this, bool forceDirect = false)
		=> @this.WhichInheritFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Filter for types that inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter for an implemented interface use <see cref="WhichImplement(Filtered.Types, Type, bool)" />,
	///     or use <see cref="WhichAreAssignableTo(Filtered.Types, Type)" /> to cover base classes and interfaces.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static Filtered.Types WhichInheritFrom(this Filtered.Types @this, Type baseType, bool forceDirect = false)
	{
		baseType.EnsureIsClass();
		return @this.Which(Filter.Suffix<Type>(type => type.InheritsFromClass(baseType, forceDirect),
			$"which inherit from {Formatter.Format(baseType)} "));
	}

	/// <summary>
	///     Filter for types that do not inherit from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter that an interface is not implemented use
	///     <see cref="WhichDoNotImplement{TInterface}(Filtered.Types, bool)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static Filtered.Types WhichDoNotInheritFrom<TBaseType>(this Filtered.Types @this, bool forceDirect = false)
		=> @this.WhichDoNotInheritFrom(typeof(TBaseType), forceDirect);

	/// <summary>
	///     Filter for types that do not inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter that an interface is not implemented use
	///     <see cref="WhichDoNotImplement(Filtered.Types, Type, bool)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static Filtered.Types WhichDoNotInheritFrom(this Filtered.Types @this, Type baseType,
		bool forceDirect = false)
	{
		baseType.EnsureIsClass();
		return @this.Which(Filter.Suffix<Type>(type => !type.InheritsFromClass(baseType, forceDirect),
			$"which do not inherit from {Formatter.Format(baseType)} "));
	}
}
