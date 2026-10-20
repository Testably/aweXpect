using System;
using aweXpect.Reflection.Collections;
using aweXpect.Reflection.Helpers;

namespace aweXpect.Reflection;

public static partial class TypeFilters
{
	/// <summary>
	///     Filter for types that implement the interface <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter for base-class inheritance use <see cref="WhichInheritFrom{TBaseType}(Filtered.Types, bool)" />,
	///     or use <see cref="WhichAreAssignableTo{TType}(Filtered.Types)" /> to cover base classes and interfaces.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static Filtered.Types WhichImplement<TInterface>(this Filtered.Types @this, bool forceDirect = false)
		=> @this.WhichImplement(typeof(TInterface), forceDirect);

	/// <summary>
	///     Filter for types that implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter for base-class inheritance use <see cref="WhichInheritFrom(Filtered.Types, Type, bool)" />,
	///     or use <see cref="WhichAreAssignableTo(Filtered.Types, Type)" /> to cover base classes and interfaces.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static Filtered.Types WhichImplement(this Filtered.Types @this, Type interfaceType, bool forceDirect = false)
	{
		interfaceType.EnsureIsInterface();
		return @this.Which(Filter.Suffix<Type>(type => type.Implements(interfaceType, forceDirect),
			$"which implement {Formatter.Format(interfaceType)} "));
	}

	/// <summary>
	///     Filter for types that do not implement the interface <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter that a base class is not inherited use
	///     <see cref="WhichDoNotInheritFrom{TBaseType}(Filtered.Types, bool)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static Filtered.Types WhichDoNotImplement<TInterface>(this Filtered.Types @this, bool forceDirect = false)
		=> @this.WhichDoNotImplement(typeof(TInterface), forceDirect);

	/// <summary>
	///     Filter for types that do not implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter that a base class is not inherited use
	///     <see cref="WhichDoNotInheritFrom(Filtered.Types, Type, bool)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static Filtered.Types WhichDoNotImplement(this Filtered.Types @this, Type interfaceType,
		bool forceDirect = false)
	{
		interfaceType.EnsureIsInterface();
		return @this.Which(Filter.Suffix<Type>(type => !type.Implements(interfaceType, forceDirect),
			$"which do not implement {Formatter.Format(interfaceType)} "));
	}
}
