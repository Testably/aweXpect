using System;

namespace aweXpect.Helpers;

internal static class ReflectionFallbackHelpers
{
	/// <summary>
	///     The exception for <paramref name="what" /> that cannot be serialized, because reflection is switched off.
	/// </summary>
	/// <remarks>
	///     Mirrors the wording of the built-in expectations, whose helper for it is internal.
	/// </remarks>
	public static NotSupportedException SerializationNotSupported(string what)
		=> new(
			$"{what} cannot be serialized as JSON by reflection, which is switched off when publishing with trimming or Native AOT enabled. Set the runtime switch 'aweXpect.ReflectionFallback.IsSupported' to true to reflect anyway.");
}
