using System;
using System.Linq;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace aweXpect.Core.Helpers;

internal static class GenericInterfaceExtensions
{
	/// <summary>
	///     The first interface of the <paramref name="type" /> whose generic definition matches.
	/// </summary>
	/// <remarks>
	///     The trimmer keeps the implementations of an interface it keeps, and every generic definition this is
	///     matched against is referenced by its caller, so the matched interfaces survive trimming.
	/// </remarks>
#if NET8_0_OR_GREATER
	[UnconditionalSuppressMessage("Trimming", "IL2070",
		Justification = "The matched interfaces are referenced, so they are not trimmed away.")]
#endif
	public static Type? FindGenericInterface(this Type type, Func<Type, bool> matchesDefinition)
		=> type.GetInterfaces()
			.FirstOrDefault(interfaceType => interfaceType.IsGenericType &&
			                                 matchesDefinition(interfaceType.GetGenericTypeDefinition()));
}
