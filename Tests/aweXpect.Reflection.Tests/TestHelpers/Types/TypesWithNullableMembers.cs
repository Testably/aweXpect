using System.Collections.Generic;

namespace aweXpect.Reflection.Tests.TestHelpers.Types;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
public class ClassWithNullableMembers
{
	public string? NullableField;
	public List<string>? NullableGenericField;
	public int? NullableValueField;
	public string? NullableProperty { get; set; }
	public List<string>? NullableGenericProperty { get; set; }
	public int? NullableValueProperty { get; set; }

	public static string? NullableWriteOnlyProperty
	{
		// ReSharper disable once ValueParameterNotUsed
		set { }
	}
}

public class ClassWithNonNullableMembers
{
	public List<string?> NonNullableGenericField = [];
	public string NonNullableField = "";
	public int NonNullableValueField;
	public string NonNullableProperty { get; set; } = "";
	public List<string?> NonNullableGenericProperty { get; set; } = [];
	public int NonNullableValueProperty { get; set; }
}

public class ClassWithMixedNullableMembers
{
	public string? NullableField;
	public string NonNullableField = "";
	public int? NullableValueField;
	public int NonNullableValueField;
	public string? NullableProperty { get; set; }
	public string NonNullableProperty { get; set; } = "";
	public int? NullableValueProperty { get; set; }
	public int NonNullableValueProperty { get; set; }
}

public class ClassWithMostlyNullableMembers
{
	public string? FirstNullableField;
	public string? SecondNullableField;
	public string? ThirdNullableField;
	public string NonNullableField = "";
	public string? FirstNullableProperty { get; set; }
	public string? SecondNullableProperty { get; set; }
	public string? ThirdNullableProperty { get; set; }
	public string NonNullableProperty { get; set; } = "";
}

public class ClassWithSingleNullableProperty
{
	public string? NullableProperty { get; set; }
}

public class ClassWithSingleNonNullableProperty
{
	public string NonNullableProperty { get; set; } = "";
}

public class DerivedClassWithNullableMembers : ClassWithNonNullableMembers
{
	public string? DeclaredNullableField;
	public string? DeclaredNullableProperty { get; set; }
}

public class ClassWithoutMembers;

#nullable disable
public class ClassWithObliviousMembers
{
	public string ObliviousField;
	public string ObliviousProperty { get; set; }
}
#nullable restore
#pragma warning restore CS0649
