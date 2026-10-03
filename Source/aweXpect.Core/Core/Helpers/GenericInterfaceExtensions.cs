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
	///     Referencing the open generic definition does not keep an implementation under Native AOT. The AOT compiler
	///     keeps it when the program references the closed interface, like the generated registrations do for the
	///     collection and member types they see, and can drop it when it resolves every use statically, like that of
	///     the only async iterator of a program. A dropped implementation is reported as absent, so the caller degrades
	///     to the result for a type without it instead of failing.
	/// </remarks>
#if NET8_0_OR_GREATER
	[UnconditionalSuppressMessage("Trimming", "IL2070",
		Justification = "A dropped interface is reported as absent, which the callers treat like a type without it.")]
#endif
	public static Type? FindGenericInterface(this Type type, Func<Type, bool> matchesDefinition)
		=> type.GetInterfaces()
			.FirstOrDefault(interfaceType => interfaceType.IsGenericType &&
			                                 matchesDefinition(interfaceType.GetGenericTypeDefinition()));
}
