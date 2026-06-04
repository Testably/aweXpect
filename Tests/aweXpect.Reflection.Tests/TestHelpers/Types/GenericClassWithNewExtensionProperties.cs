#if NET10_0_OR_GREATER
namespace aweXpect.Reflection.Tests.TestHelpers.Types;

/// <summary>
///     Uses a generic C# extension block (<c>extension&lt;T&gt;(...)</c>) to exercise detection of extension properties
///     whose grouping type is itself generic.
/// </summary>
public static class GenericClassWithNewExtensionProperties
{
	extension<T>(T[] array)
	{
		// Generic static extension property - declared on the generic grouping type.
		public static int Capacity => 16;
	}
}
#endif
