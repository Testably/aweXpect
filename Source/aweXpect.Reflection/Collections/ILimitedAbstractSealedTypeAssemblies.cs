namespace aweXpect.Reflection.Collections;

/// <summary>
///     A limited interface to allow basic filtering for types in assemblies.
/// </summary>
/// <remarks>
///     In addition to the methods in <see cref="ILimitedStaticTypeAssemblies" /> it
///     only supports accessing <see cref="Records" />.
/// </remarks>
public interface ILimitedAbstractSealedTypeAssemblies : ILimitedStaticTypeAssemblies
{
	/// <summary>
	///     Get all records in the filtered assemblies.
	/// </summary>
	Filtered.Types Records(AccessModifiers accessModifier = AccessModifiers.Any);
}

/// <summary>
///     A limited interface to allow basic filtering for types in assemblies.
/// </summary>
/// <remarks>
///     In addition to the methods in <see cref="ILimitedStaticTypeAssemblies" /> it also
///     supports adding a filter for generic or nested types.
/// </remarks>
public interface ILimitedAbstractSealedTypeAssemblies<out TLimitedTypeAssemblies>
	: ILimitedAbstractSealedTypeAssemblies, ILimitedStaticTypeAssemblies<TLimitedTypeAssemblies>
	where TLimitedTypeAssemblies : ILimitedAbstractSealedTypeAssemblies
{
}
