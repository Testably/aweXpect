namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public static class StaticClassWithExtensionMethods
{
	public static bool IsPositive(this int value) => value > 0;

	public static string Twice(this string value) => value + value;

	public static void RegularStaticMethod() { }
}
