namespace aweXpect.Reflection.Tests.TestHelpers.Types;

#pragma warning disable CS0414 // Field is assigned but its value is never used
public class ImmutableClass
{
	public const int ConstantField = 4;
	private static int _staticMutableField = 1;
	public readonly int ReadOnlyField = 2;
	private readonly string _privateReadOnlyField = "";
	public static int StaticSettableProperty { get; set; }
	public int GetOnlyProperty { get; }
	public string ComputedProperty => _privateReadOnlyField;
}
#pragma warning restore CS0414

public class ImmutableClassWithInitProperty
{
	public int Value { get; init; }
}

public class ImmutableDerivedClass : ImmutableClass
{
	public readonly int DerivedReadOnlyField = 3;
}

public class ClassWithMutableField
{
	public int Value = 1;
}

public class ClassWithSettableProperty
{
	public int Value { get; set; }
}

public class ClassWithMutableFieldAndSettableProperty
{
	public int Field = 1;
	public int Property { get; set; }
}

public class MutableBaseClass
{
	private int _value = 1;

	public int GetValue() => _value;

	public void SetValue(int value) => _value = value;
}

public class ClassInheritingMutableField : MutableBaseClass;

public class MutableBaseClassWithProtectedField
{
	protected int ProtectedValue = 1;
}

public class ClassInheritingProtectedMutableField : MutableBaseClassWithProtectedField;

public class ClassWithSettableIndexer
{
	private readonly int[] _values = new int[10];

	public int this[int index]
	{
		get => _values[index];
		set => _values[index] = value;
	}
}

public class ClassWithPrivateSettableProperty
{
	public int Value { get; private set; }
}

public struct MutableStruct
{
	public int Value;
}

public readonly struct ImmutableReadOnlyStruct
{
	public readonly int Value;

	public ImmutableReadOnlyStruct(int value)
	{
		Value = value;
	}
}

public record struct MutableRecordStruct(int Value);

public readonly record struct ImmutableRecordStruct(int Value);

public record PositionalRecord(int Value);

public interface IImmutableInterface
{
	int Value { get; }
}

public interface IMutableInterface
{
	int Value { get; set; }
}

public class GenericImmutableClass<T>
{
	public readonly T Value = default!;
}

public class GenericMutableClass<T>
{
	public T Value = default!;
}
