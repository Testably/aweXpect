namespace aweXpect.Reflection.Tests.TestHelpers.Types;

#pragma warning disable CS0169 // Field is never used
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
public class TestClassWithFieldModifiers
{
	// Constant field (IsLiteral)
	public const int ConstantField = 42;

	// Static read-only field (IsInitOnly)
	public static readonly int StaticReadOnlyField;

	// Read-only field (IsInitOnly)
	public readonly int ReadOnlyField;

	// Mutable field (neither read-only nor constant)
	public int MutableField;
}
#pragma warning restore CS0649
#pragma warning restore CS0169
