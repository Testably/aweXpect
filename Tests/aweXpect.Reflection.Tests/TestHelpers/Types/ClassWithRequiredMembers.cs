namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class ClassWithRequiredMembers
{
	// ReSharper disable once UnusedMember.Global
	public string OptionalField = "";

	// ReSharper disable once UnusedMember.Global
	public required string RequiredField;

	// ReSharper disable once UnusedMember.Global
	public required string RequiredProperty { get; set; }

	// ReSharper disable once UnusedMember.Global
	public string OptionalProperty { get; set; } = "";
}

public class ClassWithOnlyRequiredMembers
{
	// ReSharper disable once UnusedMember.Global
	public required string FirstRequiredField;

	// ReSharper disable once UnusedMember.Global
	public required string SecondRequiredField;

	// ReSharper disable once UnusedMember.Global
	public required string FirstRequiredProperty { get; set; }

	// ReSharper disable once UnusedMember.Global
	public required string SecondRequiredProperty { get; set; }
}
