namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class TestClassWithStaticMembers
{
#pragma warning disable CA2211
	// Static field
	public static string StaticField = "static";
#pragma warning restore CA2211

	// Non-static field  
	public string NonStaticField = "non-static";

	// Static constructor
	static TestClassWithStaticMembers()
	{
		StaticProperty = "initialized";
	}

	// Non-static constructor
	public TestClassWithStaticMembers()
	{
		NonStaticProperty = "initialized";
	}

	// Static property
	public static string StaticProperty { get; set; }

	// Non-static property
	public string NonStaticProperty { get; set; }

	// Static method
	public static void StaticMethod()
	{
	}

#pragma warning disable CA1822
	// Non-static method
	public void NonStaticMethod()
	{
	}
#pragma warning restore CA1822
}
