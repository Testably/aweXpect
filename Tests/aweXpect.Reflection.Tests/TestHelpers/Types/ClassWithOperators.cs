namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class ClassWithOperators
{
	public ClassWithOperators(int value)
	{
		Value = value;
	}

	public int Value { get; }

	public static ClassWithOperators operator +(ClassWithOperators left, ClassWithOperators right)
	{
		return new ClassWithOperators(left.Value + right.Value);
	}

	public static ClassWithOperators operator -(ClassWithOperators left, ClassWithOperators right)
	{
		return new ClassWithOperators(left.Value - right.Value);
	}

	public static void RegularMethod() { }
}
