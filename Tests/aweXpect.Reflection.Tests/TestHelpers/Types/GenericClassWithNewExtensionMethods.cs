#if NET10_0_OR_GREATER
using System.Collections.Generic;

namespace aweXpect.Reflection.Tests.TestHelpers.Types;

/// <summary>
///     Uses generic C# extension blocks (<c>extension&lt;T&gt;(...)</c>) to exercise detection of generic extension
///     methods, where parameter types are generic type parameters (whose <c>FullName</c> is <see langword="null" />).
/// </summary>
public static class GenericClassWithNewExtensionMethods
{
	extension<T>(T source) where T : class
	{
		// Generic instance extension method - its public implementation carries the [Extension] attribute.
		public bool IsNotNullValue() => source is not null;
	}

	extension<T>(T[] array)
	{
		// Generic static extension method without parameters.
		public static T[] Empty() => [];

		// Generic static extension method whose parameter is a generic type parameter (FullName is null).
		public static T Identity(T value) => value;

		// Generic static extension method whose parameter is a constructed generic type (FullName is null).
		public static T First(List<T> values) => values[0];

		// Generic static extension method that declares its OWN method-level type parameter in addition to the
		// extension's type parameter. The implementation merges both into the method (extension parameter first),
		// shifting the positions relative to the grouping-type skeleton.
		public static TValue Convert<TValue>(TValue value, T element) => value;
	}
}
#endif
