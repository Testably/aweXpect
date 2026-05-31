namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class TestClassWithReadWriteProperties
{
	private string _writeOnlyProperty = "";

	// Read-only property (getter only)
	public string ReadOnlyProperty { get; } = "read-only";

	// Write-only property (setter only)
	public string WriteOnlyProperty
	{
		set => _writeOnlyProperty = value;
	}

	// Read-write property (getter and setter)
	public string ReadWriteProperty { get; set; } = "read-write";

	public string GetWriteOnlyProperty() => _writeOnlyProperty;
}
