namespace aweXpect.Reflection.Tests.TestHelpers.Types;

public class BaseClassWithMembers
{
#pragma warning disable CS0169 // Field is never used
	// ReSharper disable once UnusedMember.Local
	private int _basePrivateField;
#pragma warning restore CS0169

#pragma warning disable CS0649 // Field is never assigned to
	// ReSharper disable once UnusedMember.Global
	public int BaseField;
#pragma warning restore CS0649
	// ReSharper disable once UnusedMember.Global
	public int BaseProperty { get; set; }

	// ReSharper disable once UnusedMember.Local
	private int BasePrivateProperty { get; set; }

#pragma warning disable CS0067 // Event is never used
	// ReSharper disable once UnusedMember.Global
	public event EventHandler? BaseEvent;
#pragma warning restore CS0067

#pragma warning disable CA1822 // Mark members as static
	// ReSharper disable once UnusedMember.Global
	public void BaseMethod() { }
#pragma warning restore CA1822

#pragma warning disable CS0067 // Event is never used
	// ReSharper disable once UnusedMember.Local
	private event EventHandler? BasePrivateEvent;
#pragma warning restore CS0067

#pragma warning disable CA1822 // Mark members as static
	// ReSharper disable once UnusedMember.Local
	private void BasePrivateMethod() { }
#pragma warning restore CA1822
}

public class DerivedClassWithMembers : BaseClassWithMembers
{
#pragma warning disable CS0649 // Field is never assigned to
	// ReSharper disable once UnusedMember.Global
	public int DerivedField;
#pragma warning restore CS0649
	// ReSharper disable once UnusedMember.Global
	public int DerivedProperty { get; set; }

#pragma warning disable CS0067 // Event is never used
	// ReSharper disable once UnusedMember.Global
	public event EventHandler? DerivedEvent;
#pragma warning restore CS0067

#pragma warning disable CA1822 // Mark members as static
	// ReSharper disable once UnusedMember.Global
	public void DerivedMethod() { }
#pragma warning restore CA1822
}

public class ClassOverridingObjectMembers
{
	public override string ToString() => nameof(ClassOverridingObjectMembers);
}

public enum EnumWithMembers
{
	First,
	Second,
}

public struct StructWithMembers
{
	// ReSharper disable once UnusedMember.Global
	public int StructProperty { get; set; }
}

public class BaseClassWithInternalMembers
{
#pragma warning disable CS0649 // Field is never assigned to
	// ReSharper disable once UnusedMember.Global
	internal int BaseInternalField;
#pragma warning restore CS0649
	// ReSharper disable once UnusedMember.Global
	internal int BaseInternalProperty { get; set; }

#pragma warning disable CS0067 // Event is never used
	// ReSharper disable once UnusedMember.Global
	internal event EventHandler? BaseInternalEvent;
#pragma warning restore CS0067

#pragma warning disable CA1822 // Mark members as static
	// ReSharper disable once UnusedMember.Global
	internal void BaseInternalMethod() { }
#pragma warning restore CA1822
}

public class DerivedClassWithInternalMembers : BaseClassWithInternalMembers;

public class ExceptionWithoutMembers : Exception;

public interface IBaseInterfaceWithMembers
{
	// ReSharper disable once UnusedMember.Global
	int BaseInterfaceProperty { get; }

	// ReSharper disable once EventNeverSubscribedTo.Global
	event EventHandler? BaseInterfaceEvent;

	// ReSharper disable once UnusedMember.Global
	void BaseInterfaceMethod();
}

public interface IDerivedInterfaceWithMembers : IBaseInterfaceWithMembers
{
	// ReSharper disable once UnusedMember.Global
	void DerivedInterfaceMethod();
}
