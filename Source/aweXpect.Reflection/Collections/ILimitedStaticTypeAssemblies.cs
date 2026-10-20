namespace aweXpect.Reflection.Collections;

/// <summary>
///     A limited interface to allow basic filtering for types in assemblies.
/// </summary>
/// <remarks>
///     It only supports accessing <see cref="Types" /> or <see cref="Classes" />,
///     and selecting members of the filtered types.
/// </remarks>
public interface ILimitedStaticTypeAssemblies
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
	///     Get all constructors in the filtered types.
	/// </summary>
	Filtered.Constructors Constructors();

	/// <summary>
	///     Get all events in the filtered types.
	/// </summary>
	Filtered.Events Events();

	/// <summary>
	///     Get all fields in the filtered types.
	/// </summary>
	Filtered.Fields Fields();

	/// <summary>
	///     Get all methods in the filtered types.
	/// </summary>
	Filtered.Methods Methods();

	/// <summary>
	///     Get all properties in the filtered types.
	/// </summary>
	Filtered.Properties Properties();
}

/// <summary>
///     A limited interface to allow basic filtering for types in assemblies.
/// </summary>
/// <remarks>
///     In addition to the methods in <see cref="ILimitedAbstractSealedTypeAssemblies" /> it also
///     supports adding a filter for generic or nested types.
/// </remarks>
public interface ILimitedStaticTypeAssemblies<out TLimitedTypeAssemblies> : ILimitedStaticTypeAssemblies
	where TLimitedTypeAssemblies : ILimitedStaticTypeAssemblies
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
