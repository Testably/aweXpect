namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class ClassWithOperators
{
	public int Value { get; }

	public ClassWithOperators(int value) => Value = value;

	public static ClassWithOperators operator +(ClassWithOperators left, ClassWithOperators right)
		=> new(left.Value + right.Value);

	public static ClassWithOperators operator -(ClassWithOperators left, ClassWithOperators right)
		=> new(left.Value - right.Value);

	public static void RegularMethod() { }
}
