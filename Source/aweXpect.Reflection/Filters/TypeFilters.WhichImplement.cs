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
	///     To filter for base-class inheritance use <see cref="WhichInheritFrom{TBaseType}(Filtered.Types)" />,
	///     or use <see cref="WhichAreAssignableTo{TType}(Filtered.Types)" /> to cover base classes and interfaces.<br />
	///     To require <typeparamref name="TInterface" /> to be implemented directly use
	///     <see cref="WhichImplementDirectly{TInterface}(Filtered.Types)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static Filtered.Types WhichImplement<TInterface>(this Filtered.Types @this)
		=> @this.WhichImplement(typeof(TInterface));

	/// <summary>
	///     Filter for types that implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter for base-class inheritance use <see cref="WhichInheritFrom(Filtered.Types, Type)" />,
	///     or use <see cref="WhichAreAssignableTo(Filtered.Types, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="interfaceType" /> to be implemented directly use
	///     <see cref="WhichImplementDirectly(Filtered.Types, Type)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static Filtered.Types WhichImplement(this Filtered.Types @this, Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		return @this.Which(Filter.Suffix<Type>(type => type.Implements(interfaceType),
			() => $"which implement {Formatter.Format(interfaceType)} "));
	}

	/// <summary>
	///     Filter for types that implement the interface <typeparamref name="TInterface" /> directly.
	/// </summary>
	/// <remarks>
	///     An interface that a type only receives through a base class or through another implemented interface is
	///     not implemented directly. As the compiler emits all inherited interfaces into the metadata, an interface that
	///     is declared on the type and also inherited cannot be told apart and counts as not direct.<br />
	///     To also include types that implement <typeparamref name="TInterface" /> indirectly use
	///     <see cref="WhichImplement{TInterface}(Filtered.Types)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static Filtered.Types WhichImplementDirectly<TInterface>(this Filtered.Types @this)
		=> @this.WhichImplementDirectly(typeof(TInterface));

	/// <summary>
	///     Filter for types that implement the interface <paramref name="interfaceType" /> directly.
	/// </summary>
	/// <remarks>
	///     An interface that a type only receives through a base class or through another implemented interface is
	///     not implemented directly. As the compiler emits all inherited interfaces into the metadata, an interface that
	///     is declared on the type and also inherited cannot be told apart and counts as not direct.<br />
	///     To also include types that implement <paramref name="interfaceType" /> indirectly use
	///     <see cref="WhichImplement(Filtered.Types, Type)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static Filtered.Types WhichImplementDirectly(this Filtered.Types @this, Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		return @this.Which(Filter.Suffix<Type>(type => type.Implements(interfaceType, true),
			() => $"which directly implement {Formatter.Format(interfaceType)} "));
	}

	/// <summary>
	///     Filter for types that do not implement the interface <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter that a base class is not inherited use
	///     <see cref="WhichDoNotInheritFrom{TBaseType}(Filtered.Types)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static Filtered.Types WhichDoNotImplement<TInterface>(this Filtered.Types @this)
		=> @this.WhichDoNotImplement(typeof(TInterface));

	/// <summary>
	///     Filter for types that do not implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter that a base class is not inherited use
	///     <see cref="WhichDoNotInheritFrom(Filtered.Types, Type)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static Filtered.Types WhichDoNotImplement(this Filtered.Types @this, Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		return @this.Which(Filter.Suffix<Type>(type => !type.Implements(interfaceType),
			() => $"which do not implement {Formatter.Format(interfaceType)} "));
	}

	/// <summary>
	///     Filter for types that do not implement the interface <typeparamref name="TInterface" /> directly.
	/// </summary>
	/// <remarks>
	///     Types that implement <typeparamref name="TInterface" /> only through a base class or through another
	///     implemented interface are included.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static Filtered.Types WhichDoNotImplementDirectly<TInterface>(this Filtered.Types @this)
		=> @this.WhichDoNotImplementDirectly(typeof(TInterface));

	/// <summary>
	///     Filter for types that do not implement the interface <paramref name="interfaceType" /> directly.
	/// </summary>
	/// <remarks>
	///     Types that implement <paramref name="interfaceType" /> only through a base class or through another
	///     implemented interface are included.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static Filtered.Types WhichDoNotImplementDirectly(this Filtered.Types @this, Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		return @this.Which(Filter.Suffix<Type>(type => !type.Implements(interfaceType, true),
			() => $"which do not directly implement {Formatter.Format(interfaceType)} "));
	}
}
