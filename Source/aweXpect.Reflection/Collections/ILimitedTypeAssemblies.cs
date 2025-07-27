namespace aweXpect.Reflection.Collections;

/// <summary>
///     A limited interface to allow basic filtering for types in assemblies.
/// </summary>
/// <remarks>
///     It only supports accessing <see cref="Types" /> or <see cref="Classes" />.
/// </remarks>
public interface ILimitedTypeAssemblies
{
	/// <summary>
	///     Get all types in the filtered assemblies.
	/// </summary>
	Filtered.Types Types(AccessModifiers accessModifier = AccessModifiers.Any);

	/// <summary>
	///     Get all classes in the filtered assemblies.
	/// </summary>
	Filtered.Types Classes(AccessModifiers accessModifier = AccessModifiers.Any);
}

/// <summary>
///     A limited interface to allow basic filtering for types in assemblies.
/// </summary>
/// <remarks>
///     In addition to the methods in <see cref="ILimitedTypeAssemblies" /> it also
///     supports adding a filter for generic or nested types.
/// </remarks>
public interface ILimitedTypeAssemblies<out TLimitedTypeAssemblies> : ILimitedTypeAssemblies
	where TLimitedTypeAssemblies : ILimitedTypeAssemblies
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
