using System;

namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class ClassWithObsoleteMembers
{
	[Obsolete]
	public int ObsoleteField;

	public int NonObsoleteField;

#pragma warning disable CS0067 // Event is never used
	[Obsolete]
	public event EventHandler? ObsoleteEvent;

	public event EventHandler? NonObsoleteEvent;
#pragma warning restore CS0067

	[Obsolete]
	public ClassWithObsoleteMembers()
	{
	}

	public ClassWithObsoleteMembers(int value)
	{
		NonObsoleteField = value;
	}

	[Obsolete]
	public int ObsoleteProperty { get; set; }

	public int NonObsoleteProperty { get; set; }

#pragma warning disable CA1822 // Mark members as static
	[Obsolete]
	public void ObsoleteMethod()
	{
	}

	public void NonObsoleteMethod()
	{
	}
#pragma warning restore CA1822
}
