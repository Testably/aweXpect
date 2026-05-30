namespace aweXpect.Reflection.Collections;

/// <summary>
///     A limited interface for filtering types in assemblies after an <c>Abstract</c> or <c>Sealed</c> modifier.
/// </summary>
/// <remarks>
///     Only exposes member kinds that can be abstract or sealed (<see cref="Methods" />,
///     <see cref="Properties" />, <see cref="Events" />). <see cref="Filtered.Constructors" /> and
///     <see cref="Filtered.Fields" /> are intentionally not reachable, because constructors and fields
///     cannot be abstract or sealed.
/// </remarks>
public interface ILimitedAbstractSealedTypeAssemblies
{
	/// <summary>
	///     Get all types in the filtered assemblies.
	/// </summary>
	Filtered.Types Types(AccessModifiers accessModifier = AccessModifiers.Any);

	/// <summary>
	///     Get all classes in the filtered assemblies.
	/// </summary>
	Filtered.Types Classes(AccessModifiers accessModifier = AccessModifiers.Any);

	/// <summary>
	///     Get all records in the filtered assemblies.
	/// </summary>
	Filtered.Types Records(AccessModifiers accessModifier = AccessModifiers.Any);

	/// <summary>
	///     Get all events matching the current filter.
	/// </summary>
	Filtered.Events Events();

	/// <summary>
	///     Get all methods matching the current filter.
	/// </summary>
	Filtered.Methods Methods();

	/// <summary>
	///     Get all properties matching the current filter.
	/// </summary>
	Filtered.Properties Properties();
}

/// <summary>
///     A limited interface for filtering types in assemblies after an <c>Abstract</c> or <c>Sealed</c> modifier.
/// </summary>
/// <remarks>
///     In addition to the methods in <see cref="ILimitedAbstractSealedTypeAssemblies" /> it also
///     supports adding a filter for generic or nested types.
/// </remarks>
public interface ILimitedAbstractSealedTypeAssemblies<out TLimitedTypeAssemblies>
	: ILimitedAbstractSealedTypeAssemblies
	where TLimitedTypeAssemblies : ILimitedAbstractSealedTypeAssemblies
{
	/// <summary>
	///     Filters only for generic types.
	/// </summary>
	TLimitedTypeAssemblies Generic { get; }

	/// <summary>
	///     Filters only for nested types.
	/// </summary>
	TLimitedTypeAssemblies Nested { get; }
}
