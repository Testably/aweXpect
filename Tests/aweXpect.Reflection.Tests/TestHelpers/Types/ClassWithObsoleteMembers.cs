namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class ClassWithObsoleteMembers
{
	public int NonObsoleteField;

	[Obsolete("This field is obsolete.")] public int ObsoleteField;

	[Obsolete("This constructor is obsolete.")]
	public ClassWithObsoleteMembers()
	{
	}

	public ClassWithObsoleteMembers(int value)
	{
		NonObsoleteField = value;
	}

	[Obsolete("This property is obsolete.")]
	public int ObsoleteProperty { get; set; }

	public int NonObsoleteProperty { get; set; }

#pragma warning disable CS0067 // Event is never used
	[Obsolete("This event is obsolete.")] public event EventHandler? ObsoleteEvent;

	public event EventHandler? NonObsoleteEvent;
#pragma warning restore CS0067

#pragma warning disable CA1822 // Mark members as static
	[Obsolete("This method is obsolete.")]
	public void ObsoleteMethod()
	{
	}

	public void NonObsoleteMethod()
	{
	}
#pragma warning restore CA1822
}
