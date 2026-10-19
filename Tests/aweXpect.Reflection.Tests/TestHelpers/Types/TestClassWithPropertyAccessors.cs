namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class TestClassWithPropertyAccessors
{
	private string _setterOnly = "";

	// Getter only
	public string WithGetterOnly { get; } = "";

	// Getter and a regular setter
	public string WithGetterAndSetter { get; set; } = "";

	// Getter and an init-only setter
	public string WithGetterAndInitSetter { get; init; } = "";

	// Regular setter only (no getter)
	public string WithSetterOnly
	{
		set => _setterOnly = value;
	}

	public string GetSetterOnly() => _setterOnly;
}
