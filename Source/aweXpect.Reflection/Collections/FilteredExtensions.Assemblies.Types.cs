using System;
using aweXpect.Reflection.Extensions;

namespace aweXpect.Reflection.Collections;

public static partial class FilteredExtensions
{
	/// <summary>
	///     Get all abstract types in the filtered assemblies with the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Static types or interfaces are not considered abstract, even though they
	///     have <see cref="Type.IsAbstract" /> set to <see langword="true" />.
	/// </remarks>
	public static Filtered.Types AbstractTypes(this Filtered.Assemblies assemblies,
		AccessModifiers accessModifier = AccessModifiers.Any)
	{
		Func<Type, bool> predicate = type => accessModifier == AccessModifiers.Any
			? type.IsReallyAbstract()
			: type.IsReallyAbstract() && type.HasAccessModifier(accessModifier);
		return assemblies.Types().Which(Filter.Prefix(predicate, $"{accessModifier.GetString()}abstract "));
	}

	/// <summary>
	///     Get all nested types in the filtered assemblies with the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Types NestedTypes(this Filtered.Assemblies assemblies,
		AccessModifiers accessModifier = AccessModifiers.Any)
	{
		Func<Type, bool> predicate = type => accessModifier == AccessModifiers.Any
			? type.IsNested
			: type.IsNested && type.HasAccessModifier(accessModifier);
		return assemblies.Types().Which(Filter.Prefix(predicate, $"{accessModifier.GetString()}nested "));
	}

	/// <summary>
	///     Get all sealed types in the filtered assemblies with the given <paramref name="accessModifier" />.
	/// </summary>
	/// <remarks>
	///     Static types are not considered sealed, even though they
	///     have <see cref="Type.IsSealed" /> set to <see langword="true" />.
	/// </remarks>
	public static Filtered.Types SealedTypes(this Filtered.Assemblies assemblies,
		AccessModifiers accessModifier = AccessModifiers.Any)
	{
		Func<Type, bool> predicate = type => accessModifier == AccessModifiers.Any
			? type.IsReallySealed()
			: type.IsReallySealed() && type.HasAccessModifier(accessModifier);
		return assemblies.Types().Which(Filter.Prefix(predicate, $"{accessModifier.GetString()}sealed "));
	}

	/// <summary>
	///     Get all static types in the filtered assemblies with the given <paramref name="accessModifier" />.
	/// </summary>
	public static Filtered.Types StaticTypes(this Filtered.Assemblies assemblies,
		AccessModifiers accessModifier = AccessModifiers.Any)
	{
		Func<Type, bool> predicate = type => accessModifier == AccessModifiers.Any
			? type.IsReallyStatic()
			: type.IsReallyStatic() && type.HasAccessModifier(accessModifier);
		return assemblies.Types().Which(Filter.Prefix(predicate, $"{accessModifier.GetString()}static "));
	}
}
