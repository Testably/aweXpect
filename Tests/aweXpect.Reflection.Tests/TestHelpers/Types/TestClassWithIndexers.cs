namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class TestClassWithIndexers
{
	// ReSharper disable once UnusedMember.Global
	public string RegularProperty { get; set; } = "";

	// ReSharper disable once UnusedMember.Global
	public string this[int index]
	{
		get => index.ToString();
		set => _ = value;
	}
}

public class TestClassWithOnlyIndexers
{
	// ReSharper disable once UnusedMember.Global
	public string this[int index]
	{
		get => index.ToString();
		set => _ = value;
	}
}
