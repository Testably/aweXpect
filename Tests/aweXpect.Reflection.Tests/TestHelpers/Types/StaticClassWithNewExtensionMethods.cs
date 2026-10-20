#if NET10_0_OR_GREATER
using System;

namespace aweXpect.Reflection.Tests.TestHelpers.Types;

/// <summary>
///     Uses the C# extension block syntax (C# 14 / .NET 10) to exercise detection of new-syntax extension members.
/// </summary>
public static class StaticClassWithNewExtensionMethods
{
	extension(string text)
	{
		// Instance extension method (new syntax) - the public implementation carries the [Extension] attribute.
		public bool IsLongText() => text.Length > 10;

		// Instance extension property - only reachable via a (special-name) accessor.
		public bool IsEmptyText => text.Length == 0;
	}

	extension(string)
	{
		// Static extension methods (new syntax) - the public implementation has no [Extension] attribute.
		public static string Create() => "created";

		public static string Combine(int count) => count.ToString();

		// Static extension property - emitted as a (special-name) accessor in the grouping type.
		public static string DefaultValue => "default";
	}

	// Regular static method whose name matches a static extension method but with a different parameter count.
	public static string Create(int unused) => unused.ToString();

	// Regular static method whose name matches a static extension method but with a different parameter type.
	public static string Combine(string value) => value;

	// Regular static method in an extension-bearing class - not an extension method.
	public static void RegularStaticMethod() { }

	// Contains a lambda, so the class also owns a compiler-generated <>c nested type which must be
	// distinguished from the (also unspeakably named) extension grouping types.
	public static int WithClosure()
	{
		Func<int, int> increment = value => value + 1;
		return increment(41);
	}

	// Non-grouping nested type, to ensure it is skipped while scanning for grouping types.
	public sealed class NestedHelper { }
}
#endif
