namespace aweXpect.Reflection.Collections;

/// <summary>
///     An interface to allow filtering for types in assemblies.
/// </summary>
/// <remarks>
///     In addition to the properties and methods in
///     <see cref="ILimitedAbstractSealedTypeAssemblies{TLimitedTypeAssemblies}" /> it also
///     supports adding a filter for abstract, sealed or static types as well as accessing interfaces or enums.
/// </remarks>
public interface ITypeAssemblies
	: ILimitedAbstractSealedTypeAssemblies<ITypeAssemblies>
{
	/// <summary>
	///     Filters only for abstract types.
	/// </summary>
	ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Abstract { get; }

	/// <summary>
	///     Filters only for sealed types.
	/// </summary>
	ILimitedAbstractSealedTypeAssemblies<ILimitedAbstractSealedTypeAssemblies> Sealed { get; }

	/// <summary>
	///     Filters only for static types.
	/// </summary>
	ILimitedStaticTypeAssemblies<ILimitedStaticTypeAssemblies> Static { get; }

	/// <summary>
	///     Get all interfaces in the filtered assemblies.
	/// </summary>
	Filtered.Types Interfaces(AccessModifiers accessModifier = AccessModifiers.Any);

	/// <summary>
	///     Get all enums in the filtered assemblies.
	/// </summary>
	Filtered.Types Enums(AccessModifiers accessModifier = AccessModifiers.Any);

	/// <summary>
	///     Get all record structs in the filtered assemblies.
	/// </summary>
	Filtered.Types RecordStructs(AccessModifiers accessModifier = AccessModifiers.Any);

	/// <summary>
	///     Get all structs in the filtered assemblies.
	/// </summary>
	Filtered.Types Structs(AccessModifiers accessModifier = AccessModifiers.Any);

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

	/// <summary>
	///     An interface to allow filtering for types in assemblies.
	/// </summary>
	/// <remarks>
	///     In addition to the properties and methods in <see cref="ITypeAssemblies" /> it also
	///     allows filtering for a private protected access modifier.
	/// </remarks>
	public interface IPrivate : ITypeAssemblies
	{
		/// <summary>
		///     Filters for private protected types.
		/// </summary>
		ITypeAssemblies Protected { get; }
	}

	/// <summary>
	///     An interface to allow filtering for types in assemblies.
	/// </summary>
	/// <remarks>
	///     In addition to the properties and methods in <see cref="ITypeAssemblies" /> it also
	///     allows filtering for a protected internal access modifier.
	/// </remarks>
	public interface IProtected : ITypeAssemblies
	{
		/// <summary>
		///     Filters for protected internal types.
		/// </summary>
		ITypeAssemblies Internal { get; }
	}
}
