using System.Collections.Generic;

namespace aweXpect.Reflection.Tests.TestHelpers.Types;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
#pragma warning disable CS0067 // Event is never used
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
	public string NonNullableField = "";
	public List<string?> NonNullableGenericField = [];
	public int NonNullableValueField;
	public string NonNullableProperty { get; set; } = "";
	public List<string?> NonNullableGenericProperty { get; set; } = [];
	public int NonNullableValueProperty { get; set; }
}

public class ClassWithMixedNullableMembers
{
	public string NonNullableField = "";
	public int NonNullableValueField;
	public string? NullableField;
	public int? NullableValueField;
	public string? NullableProperty { get; set; }
	public string NonNullableProperty { get; set; } = "";
	public int? NullableValueProperty { get; set; }
	public int NonNullableValueProperty { get; set; }
}

public class ClassWithMostlyNullableMembers
{
	public string? FirstNullableField;
	public string NonNullableField = "";
	public string? SecondNullableField;
	public string? ThirdNullableField;
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

public class ClassWithNullableEvents
{
	public event EventHandler? NullableEvent;
	public event EventHandler<string>? NullableGenericEvent;
}

public class ClassWithNonNullableEvents
{
	public event EventHandler NonNullableEvent = delegate { };
	public event EventHandler<string?> NonNullableGenericEvent = delegate { };
}

public class ClassWithMixedNullableEvents
{
	public event EventHandler? NullableEvent;
	public event EventHandler NonNullableEvent = delegate { };
}

public class ClassWithSingleNullableEvent
{
	public event EventHandler? NullableEvent;
}

public class ClassWithSingleNonNullableEvent
{
	public event EventHandler NonNullableEvent = delegate { };
}

public class DerivedClassWithNullableEvent : ClassWithSingleNonNullableEvent
{
	public event EventHandler? DeclaredNullableEvent;
}

public class DerivedClassWithInheritedNullableEvent : ClassWithSingleNullableEvent
{
	public event EventHandler? DeclaredNullableEvent;
}

public class BaseClassWithPrivateNonNullableEvent
{
	// ReSharper disable once UnusedMember.Local
	private event EventHandler PrivateNonNullableEvent = delegate { };
}

public class DerivedClassWithPrivateNonNullableBaseEvent : BaseClassWithPrivateNonNullableEvent
{
	public event EventHandler? DeclaredNullableEvent;
}

// ReSharper disable ValueParameterNotUsed
public class ClassWithCustomNullableEvent
{
	public event EventHandler? NullableCustomEvent
	{
		add { }
		remove { }
	}
}

public class ClassWithCustomNonNullableEvent
{
	public event EventHandler NonNullableCustomEvent
	{
		add { }
		remove { }
	}
}
// ReSharper restore ValueParameterNotUsed

#nullable disable
public class ClassWithObliviousMembers
{
	public string ObliviousField;
	public string ObliviousProperty { get; set; }
}

public class ClassWithObliviousEvents
{
	public event EventHandler ObliviousEvent;
}
#pragma warning restore CS0067
#pragma warning restore CS0649
