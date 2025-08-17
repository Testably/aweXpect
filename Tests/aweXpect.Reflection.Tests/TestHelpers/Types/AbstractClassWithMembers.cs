namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public abstract class AbstractClassWithMembers
{
	public abstract string AbstractProperty { get; set; }
	public virtual string VirtualProperty { get; set; } = "";

	public abstract void AbstractMethod();
	public virtual void VirtualMethod() { }
#pragma warning disable CA1822 // Mark members as static
	public void RegularMethod() { }
#pragma warning restore CA1822

#pragma warning disable CS0067 // Event is never used
	public abstract event EventHandler AbstractEvent;
#pragma warning disable CA1070
	public virtual event EventHandler? VirtualEvent;
#pragma warning restore CA1070
#pragma warning restore CS0067
}
