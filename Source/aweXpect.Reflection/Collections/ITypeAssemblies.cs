namespace aweXpect.Reflection.Collections;

/// <summary>
///     An interface to allow filtering for types
/// </summary>
public interface ITypeAssemblies
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
