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
	///     To filter for an implemented interface use <see cref="WhichImplement{TInterface}(Filtered.Types)" />,
	///     or use <see cref="WhichAreAssignableTo{TType}(Filtered.Types)" /> to cover base classes and interfaces.<br />
	///     To require <typeparamref name="TBaseType" /> to be the direct parent use
	///     <see cref="WhichInheritDirectlyFrom{TBaseType}(Filtered.Types)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static Filtered.Types WhichInheritFrom<TBaseType>(this Filtered.Types @this)
		=> @this.WhichInheritFrom(typeof(TBaseType));

	/// <summary>
	///     Filter for types that inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter for an implemented interface use <see cref="WhichImplement(Filtered.Types, Type)" />,
	///     or use <see cref="WhichAreAssignableTo(Filtered.Types, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="baseType" /> to be the direct parent use
	///     <see cref="WhichInheritDirectlyFrom(Filtered.Types, Type)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static Filtered.Types WhichInheritFrom(this Filtered.Types @this, Type baseType)
	{
		baseType.EnsureIsClass();
		return @this.Which(Filter.Suffix<Type>(type => type.InheritsFromClass(baseType),
			() => $"which inherit from {Formatter.Format(baseType)} "));
	}

	/// <summary>
	///     Filter for types that inherit directly from the base class <typeparamref name="TBaseType" />, i.e. whose
	///     direct parent is <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     To also include types with <typeparamref name="TBaseType" /> anywhere in the base-class chain use
	///     <see cref="WhichInheritFrom{TBaseType}(Filtered.Types)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static Filtered.Types WhichInheritDirectlyFrom<TBaseType>(this Filtered.Types @this)
		=> @this.WhichInheritDirectlyFrom(typeof(TBaseType));

	/// <summary>
	///     Filter for types that inherit directly from the base class <paramref name="baseType" />, i.e. whose direct
	///     parent is <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     To also include types with <paramref name="baseType" /> anywhere in the base-class chain use
	///     <see cref="WhichInheritFrom(Filtered.Types, Type)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static Filtered.Types WhichInheritDirectlyFrom(this Filtered.Types @this, Type baseType)
	{
		baseType.EnsureIsClass();
		return @this.Which(Filter.Suffix<Type>(type => type.InheritsFromClass(baseType, true),
			() => $"which inherit directly from {Formatter.Format(baseType)} "));
	}

	/// <summary>
	///     Filter for types that do not inherit from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter that an interface is not implemented use
	///     <see cref="WhichDoNotImplement{TInterface}(Filtered.Types)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static Filtered.Types WhichDoNotInheritFrom<TBaseType>(this Filtered.Types @this)
		=> @this.WhichDoNotInheritFrom(typeof(TBaseType));

	/// <summary>
	///     Filter for types that do not inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter that an interface is not implemented use
	///     <see cref="WhichDoNotImplement(Filtered.Types, Type)" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static Filtered.Types WhichDoNotInheritFrom(this Filtered.Types @this, Type baseType)
	{
		baseType.EnsureIsClass();
		return @this.Which(Filter.Suffix<Type>(type => !type.InheritsFromClass(baseType),
			() => $"which do not inherit from {Formatter.Format(baseType)} "));
	}

	/// <summary>
	///     Filter for types that do not inherit directly from the base class <typeparamref name="TBaseType" />, i.e.
	///     whose direct parent is not <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Types that inherit from <typeparamref name="TBaseType" /> only indirectly are included.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static Filtered.Types WhichDoNotInheritDirectlyFrom<TBaseType>(this Filtered.Types @this)
		=> @this.WhichDoNotInheritDirectlyFrom(typeof(TBaseType));

	/// <summary>
	///     Filter for types that do not inherit directly from the base class <paramref name="baseType" />, i.e. whose
	///     direct parent is not <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Types that inherit from <paramref name="baseType" /> only indirectly are included.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static Filtered.Types WhichDoNotInheritDirectlyFrom(this Filtered.Types @this, Type baseType)
	{
		baseType.EnsureIsClass();
		return @this.Which(Filter.Suffix<Type>(type => !type.InheritsFromClass(baseType, true),
			() => $"which do not inherit directly from {Formatter.Format(baseType)} "));
	}
}
