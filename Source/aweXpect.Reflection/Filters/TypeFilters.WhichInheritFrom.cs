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
	///     <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static InheritanceScopeResult WhichInheritFrom<TBaseType>(this Filtered.Types @this)
		=> @this.WhichInheritFrom(typeof(TBaseType));

	/// <summary>
	///     Filter for types that inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter for an implemented interface use <see cref="WhichImplement(Filtered.Types, Type)" />,
	///     or use <see cref="WhichAreAssignableTo(Filtered.Types, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="baseType" /> to be the direct parent use
	///     <see cref="InheritanceScopeResult.Directly" />.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static InheritanceScopeResult WhichInheritFrom(this Filtered.Types @this, Type baseType)
	{
		baseType.EnsureIsClass();
		return new InheritanceScopeResult(@this, memberScope =>
		{
			bool directly = memberScope.IsDirect();
			return Filter.Suffix<Type>(type => type.InheritsFromClass(baseType, directly),
				() => $"which inherit {(directly ? "directly " : "")}from {Formatter.Format(baseType)} ");
		});
	}

	/// <summary>
	///     Filter for types that do not inherit from the base class <typeparamref name="TBaseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter that an interface is not implemented use
	///     <see cref="WhichDoNotImplement{TInterface}(Filtered.Types)" />.<br />
	///     With <see cref="InheritanceScopeResult.Directly" /> only types whose direct parent is
	///     <typeparamref name="TBaseType" /> are excluded.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TBaseType" /> is an interface.</exception>
	public static InheritanceScopeResult WhichDoNotInheritFrom<TBaseType>(this Filtered.Types @this)
		=> @this.WhichDoNotInheritFrom(typeof(TBaseType));

	/// <summary>
	///     Filter for types that do not inherit from the base class <paramref name="baseType" />.
	/// </summary>
	/// <remarks>
	///     Only the base-class chain is considered; implemented interfaces are ignored.<br />
	///     To filter that an interface is not implemented use
	///     <see cref="WhichDoNotImplement(Filtered.Types, Type)" />.<br />
	///     With <see cref="InheritanceScopeResult.Directly" /> only types whose direct parent is
	///     <paramref name="baseType" /> are excluded.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="baseType" /> is an interface.</exception>
	public static InheritanceScopeResult WhichDoNotInheritFrom(this Filtered.Types @this, Type baseType)
	{
		baseType.EnsureIsClass();
		return new InheritanceScopeResult(@this, memberScope =>
		{
			bool directly = memberScope.IsDirect();
			return Filter.Suffix<Type>(type => !type.InheritsFromClass(baseType, directly),
				() => $"which do not inherit {(directly ? "directly " : "")}from {Formatter.Format(baseType)} ");
		});
	}
}
