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
	///     <see cref="InheritanceScopeResult.Directly" />: an interface that a type only receives through a base class
	///     or through another implemented interface is not implemented directly. As the compiler emits all inherited
	///     interfaces into the metadata, an interface that is declared on the type and also inherited cannot be told
	///     apart and counts as not direct.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static InheritanceScopeResult WhichImplement<TInterface>(this Filtered.Types @this)
		=> @this.WhichImplement(typeof(TInterface));

	/// <summary>
	///     Filter for types that implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter for base-class inheritance use <see cref="WhichInheritFrom(Filtered.Types, Type)" />,
	///     or use <see cref="WhichAreAssignableTo(Filtered.Types, Type)" /> to cover base classes and interfaces.<br />
	///     To require <paramref name="interfaceType" /> to be implemented directly use
	///     <see cref="InheritanceScopeResult.Directly" />: an interface that a type only receives through a base class
	///     or through another implemented interface is not implemented directly. As the compiler emits all inherited
	///     interfaces into the metadata, an interface that is declared on the type and also inherited cannot be told
	///     apart and counts as not direct.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static InheritanceScopeResult WhichImplement(this Filtered.Types @this, Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		return new InheritanceScopeResult(@this, memberScope =>
		{
			bool directly = memberScope.IsDirect();
			return Filter.Suffix<Type>(type => type.Implements(interfaceType, directly),
				() => $"which {(directly ? "directly " : "")}implement {Formatter.Format(interfaceType)} ");
		});
	}

	/// <summary>
	///     Filter for types that do not implement the interface <typeparamref name="TInterface" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter that a base class is not inherited use
	///     <see cref="WhichDoNotInheritFrom{TBaseType}(Filtered.Types)" />.<br />
	///     With <see cref="InheritanceScopeResult.Directly" /> types that implement <typeparamref name="TInterface" />
	///     only through a base class or through another implemented interface are included.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <typeparamref name="TInterface" /> is not an interface.</exception>
	public static InheritanceScopeResult WhichDoNotImplement<TInterface>(this Filtered.Types @this)
		=> @this.WhichDoNotImplement(typeof(TInterface));

	/// <summary>
	///     Filter for types that do not implement the interface <paramref name="interfaceType" />.
	/// </summary>
	/// <remarks>
	///     Only implemented interfaces are considered; base-class inheritance is ignored.<br />
	///     To filter that a base class is not inherited use
	///     <see cref="WhichDoNotInheritFrom(Filtered.Types, Type)" />.<br />
	///     With <see cref="InheritanceScopeResult.Directly" /> types that implement <paramref name="interfaceType" />
	///     only through a base class or through another implemented interface are included.
	/// </remarks>
	/// <exception cref="ArgumentException">Thrown if <paramref name="interfaceType" /> is not an interface.</exception>
	public static InheritanceScopeResult WhichDoNotImplement(this Filtered.Types @this, Type interfaceType)
	{
		interfaceType.EnsureIsInterface();
		return new InheritanceScopeResult(@this, memberScope =>
		{
			bool directly = memberScope.IsDirect();
			return Filter.Suffix<Type>(type => !type.Implements(interfaceType, directly),
				() => $"which do not {(directly ? "directly " : "")}implement {Formatter.Format(interfaceType)} ");
		});
	}
}
